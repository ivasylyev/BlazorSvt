# -*- coding: utf-8 -*-
"""Этап 2 перестройки бэклога СВТ: вехи 1–34 → 1–16 (слияние вех, перестановка пунктов пакета 7.10).

  python docs/estimation/restructure_apply.py            # --dry-run (по умолчанию): только _stage2_preview/ и отчёт
  python docs/estimation/restructure_apply.py --apply    # переносит файлы из предпросмотра на место

Источник — restructure_plan.json. Карта пишется в renumber_map_cut2.json. Что именно меняется — п.1–6 постановки:
backlog.md и roadmap.md перестраиваются, остальные файлы области получают только замену номеров (renumber_cut2.remap_text2).
Генератор оценок (этап 3) этот скрипт не трогает.
"""
from __future__ import annotations

import argparse
import difflib
import hashlib
import json
import re
import shutil
import sys
from collections import Counter
from dataclasses import dataclass, field
from decimal import Decimal
from pathlib import Path, PurePosixPath

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))

import renumber_cut2 as C  # noqa: E402

DEFAULT_ROOT = HERE.parents[1]
PREVIEW_NAME = "_stage2_preview"
BACKLOG_REL = ".cursor/skills/svt-architecture/backlog.md"
ROADMAP_REL = ".cursor/skills/svt-architecture/roadmap.md"

# Постановка п.3: если у старой (не первой) вехи нет вводного текста — строку «Фаза» не добавляем («пустые не добавлять»).
# True — добавлять строку «Фаза» и для вех без вводного текста.
PHASE_LINE_FOR_EMPTY_INTRO = False

SCOPE_GLOBS = [
    ".cursor/skills/svt-architecture/*.md",
    ".cursor/skills/svt-*/**/*.md",
    ".cursor/rules/*.mdc",
    ".cursor/docs/**/*.md",
    ".cursor/README.md",
    "docs/output-review-package/README.md",
]
FORBIDDEN_PREFIXES = ("docs/tasks/", "docs/docs/", "docs/estimation/", "BlazorSvt/", ".cursor/skills/atlas-")


def in_scope(rel: str) -> bool:
    """Независимая от glob-ов проверка п.5 / п.g."""
    if rel.startswith(FORBIDDEN_PREFIXES):
        return False
    if rel.startswith("docs/output-review-package/") and rel.endswith(".html"):
        return False
    if rel.lower().endswith(".png"):
        return False
    if rel.startswith(".cursor/skills/svt-") and rel.endswith(".md"):
        return True
    if rel.startswith(".cursor/rules/") and rel.count("/") == 2 and rel.endswith(".mdc"):
        return True
    if rel.startswith(".cursor/docs/") and rel.endswith(".md"):
        return True
    return rel in (".cursor/README.md", "docs/output-review-package/README.md")


def enumerate_scope(root: Path) -> list[str]:
    out: set[str] = set()
    for pat in SCOPE_GLOBS:
        for p in root.glob(pat):
            if p.is_file():
                out.add(p.relative_to(root).as_posix())
    return sorted(out)


# ---------------------------------------------------------------------------------------------
# Файлы: UTF-8, BOM и окончания строк сохраняются
# ---------------------------------------------------------------------------------------------
@dataclass
class Src:
    rel: str
    raw: bytes
    text: str          # окончания приведены к \n, без BOM
    bom: bool
    crlf: bool


def read_src(root: Path, rel: str) -> Src:
    raw = (root / rel).read_bytes()
    bom = raw.startswith(b"\xef\xbb\xbf")
    body = raw[3:] if bom else raw
    s = body.decode("utf-8")
    n_crlf, n_lf = s.count("\r\n"), s.count("\n")
    stray_cr = s.replace("\r\n", "").count("\r")
    if stray_cr or (n_crlf and n_crlf != n_lf):
        raise RuntimeError(f"{rel}: смешанные окончания строк (CRLF={n_crlf}, LF={n_lf}, CR={stray_cr}) — файл не обработан")
    return Src(rel, raw, s.replace("\r\n", "\n"), bom, bool(n_crlf))


def encode_like(src: Src, text: str) -> bytes:
    if src.crlf:
        text = text.replace("\n", "\r\n")
    data = text.encode("utf-8")
    return (b"\xef\xbb\xbf" + data) if src.bom else data


# ---------------------------------------------------------------------------------------------
# Сбор отчёта
# ---------------------------------------------------------------------------------------------
@dataclass
class Collector:
    unknown: list[tuple[str, int, str, str]] = field(default_factory=list)      # (файл, строка, токен, фрагмент)
    suspicious: list[tuple[str, int, str, str]] = field(default_factory=list)   # (файл, строка, вид, фрагмент)
    notes: list[str] = field(default_factory=list)
    decisions: list[str] = field(default_factory=list)
    manual: list[str] = field(default_factory=list)
    headings_kept: int = 0


def _snip(text: str, tok: str, width: int = 38) -> str:
    i = text.find(tok)
    if i < 0:
        i = 0
    return text[max(0, i - width): i + len(tok) + width].replace("\n", " ").strip()


def remap_line(col: Collector, rel: str, no: int, text: str) -> str:
    mk: list[str] = []
    nt: list[str] = []
    out = C.remap_text2(text, mk, f"{rel}:{no}", nt)
    for x in mk:
        tok = x.rsplit(": ", 1)[1]
        col.unknown.append((rel, no, tok, _snip(text, tok)))
    col.notes.extend(nt)
    for kind, frag in C.scan_suspicious(text):
        col.suspicious.append((rel, no, kind, frag.replace("\n", " ").strip()))
    return out


def remap_lines(col: Collector, rel: str, lines: list[C.Ln]) -> list[str]:
    return [remap_line(col, rel, no, t) for no, t in lines]


# Нумерованный заголовок документа («### 4.1 Стратегия», «### 6.9 …») — номер раздела этого документа, не пакет бэклога.
_NUM_HEADING = re.compile(r"^(#{1,6}\s+\d+(?:\.\d+)+\.?)(?=\s|$)(.*)$")


def remap_whole(col: Collector, rel: str, text: str) -> str:
    out = []
    for i, t in enumerate(text.split("\n")):
        m = _NUM_HEADING.match(t)
        if m:
            col.headings_kept += 1
            out.append(m.group(1) + remap_line(col, rel, i + 1, m.group(2)))
        else:
            out.append(remap_line(col, rel, i + 1, t))
    return "\n".join(out)


# ---------------------------------------------------------------------------------------------
# backlog.md
# ---------------------------------------------------------------------------------------------
@dataclass
class NewStruct:
    text: str
    pkg_counts: dict[int, int]            # новая веха → число пакетов
    ms_est: dict[int, tuple[Decimal, Decimal]]
    phase_omitted: list[str]


def restructure_backlog(col: Collector, bl: C.Backlog, plan: dict) -> NewStruct:
    by_num = {m.num: m for m in bl.mss}
    blocks: list[list[str]] = []
    pkg_counts: dict[int, int] = {}
    ms_est: dict[int, tuple[Decimal, Decimal]] = {}
    omitted: list[str] = []
    for m in plan["milestones"]:
        k, olds = m["new"], m["old"]
        a = b = Decimal(0)
        for o in olds:
            a += by_num[o].est[0]
            b += by_num[o].est[1]
            if by_num[o].est_line[1].split(" / ")[-1].strip() != "—":
                col.decisions.append(f"веха {o}: третий слот «Оценки» не «—» — в новой вехе поставлено «—»")
        ms_est[k] = (a, b)
        est_line = f"**Оценка:** {C.fmt_num(a)} / {C.fmt_num(b)} / —"
        segs: list[list[str]] = []
        for i, o in enumerate(olds):
            seg = remap_lines(col, BACKLOG_REL, by_num[o].intro)
            if i == 0:
                if seg:
                    segs.append(seg)
                continue
            ph_name = remap_line(col, BACKLOG_REL, by_num[o].head[0], by_num[o].name).rstrip().rstrip(".")
            if seg or PHASE_LINE_FOR_EMPTY_INTRO:
                segs.append([f"**Фаза: {ph_name}.**"] + ([""] + seg if seg else []))
            else:
                omitted.append(f"старая веха {o} «{by_num[o].name}» → новая {k}")
        intro: list[str] = []
        for j, sg in enumerate(segs):
            if j:
                intro.append("")
            intro += sg
        pkgs: list[list[str]] = []
        for o in olds:
            for p in by_num[o].pkgs:
                lines = p.lines
                if p.key == plan["task_reorder"]["package_old"]:
                    lines, _ = C.reorder_7_10(p)
                pkgs.append(remap_lines(col, BACKLOG_REL, lines))
        pkg_counts[k] = len(pkgs)
        blocks.append(C.ms_lines(f"# Веха {k} — {m['name']}", est_line, intro, pkgs))
    preamble = remap_lines(col, BACKLOG_REL, bl.preamble)
    tail = remap_lines(col, BACKLOG_REL, bl.tail)
    return NewStruct(C.render_backlog(preamble, blocks, tail), pkg_counts, ms_est, omitted)


# ---------------------------------------------------------------------------------------------
# roadmap.md
# ---------------------------------------------------------------------------------------------
_TABLE_HEAD = "| Веха | Название |"


def _cells(line: str) -> list[str]:
    return [c.strip() for c in line.strip().strip("|").split("|")]


def _phase_of(name: str) -> str:
    if re.search(r"(?<![А-Яа-яЁё])ОПЭ", name):
        return "ОПЭ"
    if re.search(r"(?<![А-Яа-яЁё])ПЭ(?![А-Яа-яЁё])", name):
        return "ПЭ"
    if "MVP" in name:
        return "MVP"
    raise C.StructureError(f"не удалось определить фазу по названию вехи: {name!r}")


def _glue(parts: list[tuple[str, str]]) -> str:
    return " ".join(f"{ph}: {txt.strip().rstrip('.').strip()}." for ph, txt in parts)


def _pkg_range(k: int, n: int) -> str:
    if n == 1:
        return f"{k}.1"
    if n == 2:
        return f"{k}.1, {k}.2"
    return f"{k}.1–{k}.{n}"


@dataclass
class RoadmapResult:
    text: str
    old_rows: dict[int, list[str]]
    new_rows: dict[int, list[str]]
    totals_stated: tuple[Decimal, Decimal]
    old_sum: tuple[Decimal, Decimal]
    new_sum: tuple[Decimal, Decimal]


def build_roadmap(col: Collector, text: str, plan: dict, pkg_counts: dict[int, int]) -> RoadmapResult:
    lines = text.split("\n")
    hi = [i for i, l in enumerate(lines) if l.startswith(_TABLE_HEAD)]
    if len(hi) != 1:
        raise C.StructureError(f"roadmap.md: таблица «{_TABLE_HEAD}» найдена {len(hi)} раз(а)")
    h = hi[0]
    rows_i = [i for i in range(h + 2, len(lines)) if re.match(r"^\| \*\*\d+\*\* \|", lines[i])]
    if not rows_i or rows_i != list(range(rows_i[0], rows_i[0] + len(rows_i))):
        raise C.StructureError("roadmap.md: строки вех не идут подряд")
    tot_i = [i for i in range(rows_i[-1] + 1, len(lines)) if lines[i].startswith("| | **Итого")]
    if len(tot_i) != 2 or tot_i != list(range(rows_i[-1] + 1, rows_i[-1] + 3)):
        raise C.StructureError("roadmap.md: ожидались две строки «Итого» сразу после вех")
    old_rows: dict[int, list[str]] = {}
    row_line: dict[int, int] = {}
    for i in rows_i:
        cs = _cells(lines[i])
        if len(cs) != 9:
            raise C.StructureError(f"roadmap.md:{i+1}: в строке {len(cs)} колонок, нужно 9")
        old_rows[int(cs[0].strip("*"))] = cs
        row_line[int(cs[0].strip("*"))] = i + 1
    if sorted(old_rows) != list(range(1, 35)):
        raise C.StructureError(f"roadmap.md: старые вехи в таблице: {sorted(old_rows)}")

    # ячейки старых строк пропускаем через remap_text2 (номера пакетов/пунктов внутри текста)
    def rmcell(o: int, idx: int) -> str:
        return remap_line(col, ROADMAP_REL, row_line[o], old_rows[o][idx])

    new_rows: dict[int, list[str]] = {}
    sa = sb = Decimal(0)
    osa = osb = Decimal(0)
    for o in old_rows:
        osa += Decimal(old_rows[o][3])
        osb += Decimal(old_rows[o][4])
    for m in plan["milestones"]:
        k, olds = m["new"], m["old"]
        a = sum((Decimal(old_rows[o][3]) for o in olds), Decimal(0))
        b = sum((Decimal(old_rows[o][4]) for o in olds), Decimal(0))
        sa += a
        sb += b
        if len(olds) == 1:
            res, works = rmcell(olds[0], 6), rmcell(olds[0], 7)
        else:
            phases = [(_phase_of(old_rows[o][1]), o) for o in olds]
            if [p for p, _ in phases] != ["MVP", "ОПЭ", "ПЭ"]:
                raise C.StructureError(f"веха {k}: фазы старых вех {olds}: {[p for p, _ in phases]} (ожидалось MVP, ОПЭ, ПЭ)")
            res = _glue([(p, rmcell(o, 6)) for p, o in phases])
            works = _glue([(p, rmcell(o, 7)) for p, o in phases])
        steps: list[str] = []
        for o in olds:
            for s in old_rows[o][8].split(","):
                s = s.strip()
                if s and s not in steps:
                    steps.append(s)
        step_cell = ", ".join(steps) if steps else old_rows[olds[0]][8]
        pk = _pkg_range(k, pkg_counts[k])
        new_rows[k] = [f"**{k}**", m["name"], pk, C.fmt_num(a), C.fmt_num(b), "—", res, works, step_cell]
        if len(olds) == 1 and old_rows[olds[0]][5] != "—":
            col.decisions.append(f"roadmap: веха {olds[0]} колонка «Человек» = {old_rows[olds[0]][5]!r} — в новой строке «—»")
    # ключевая проверка: пакеты для вех 1–3 «как были»
    for k in (1, 2, 3):
        if new_rows[k][2] != old_rows[k][2]:
            col.decisions.append(f"roadmap: колонка «Пакеты» вехи {k}: было {old_rows[k][2]!r}, по факту {new_rows[k][2]!r}")
    totals = _cells(lines[tot_i[0]])
    ts = (Decimal(totals[3].strip("*")), Decimal(totals[4].strip("*")))
    before = [remap_line(col, ROADMAP_REL, i + 1, lines[i]) for i in range(0, h)]
    after_start = tot_i[-1] + 1
    after = [remap_line(col, ROADMAP_REL, i + 1, lines[i]) for i in range(after_start, len(lines))]
    out = before + [lines[h], lines[h + 1]]
    out += ["| " + " | ".join(new_rows[k]) + " |" for k in sorted(new_rows)]
    out += [lines[i] for i in tot_i]
    out += after
    return RoadmapResult("\n".join(out), old_rows, new_rows, ts, (osa, osb), (sa, sb))


# ---------------------------------------------------------------------------------------------
# Проверки
# ---------------------------------------------------------------------------------------------
@dataclass
class CheckResult:
    name: str
    ok: bool
    detail: str = ""
    lines: list[str] = field(default_factory=list)


def _count(pattern: str, text: str) -> int:
    return len(re.findall(pattern, text, flags=re.M))


def _fmt_d(d: Decimal) -> str:
    return C.fmt_num(d)


def est_report(bl: C.Backlog, tol: Decimal = Decimal("0.1")) -> dict:
    pk_bad = {}
    notask = {}
    for ms in bl.mss:
        for p in ms.pkgs:
            ts = p.task_sum()
            st = p.est()
            if st is None:
                pk_bad[p.key] = (ts, None)
            elif abs(ts[0] - st[0]) > tol or abs(ts[1] - st[1]) > tol:
                pk_bad[p.key] = (ts, st)
            nt = p.tasks_without_triple()
            if nt:
                notask[p.key] = nt
    ms_bad = {}
    for ms in bl.mss:
        sp = [p.est() for p in ms.pkgs]
        sa = sum((x[0] for x in sp if x), Decimal(0))
        sb = sum((x[1] for x in sp if x), Decimal(0))
        if abs(sa - ms.est[0]) > tol or abs(sb - ms.est[1]) > tol:
            ms_bad[ms.num] = ((sa, sb), ms.est)
    return {"pk_bad": pk_bad, "ms_bad": ms_bad, "notask": notask}


def run_checks(old_text: str, new: NewStruct, bl_old: C.Backlog, mapd: dict, plan: dict,
               rr: RoadmapResult, results: dict[str, bytes], determinism: tuple[bool, str],
               scope_files: list[str]) -> list[CheckResult]:
    out: list[CheckResult] = []
    new_text = new.text
    # a) множество и кратности [SVT-NNNN]
    ca, cb = Counter(C.SVT_RE.findall(old_text)), Counter(C.SVT_RE.findall(new_text))
    ca_all, cb_all = Counter(re.findall(r"SVT-\d+", old_text)), Counter(re.findall(r"SVT-\d+", new_text))
    d = (ca - cb) + (cb - ca)
    out.append(CheckResult("a) множество [SVT-…] до/после", ca == cb and ca_all == cb_all,
                           f"уникальных {len(ca)}, всего вхождений {sum(ca.values())}; все «SVT-N»: {sum(ca_all.values())} → {sum(cb_all.values())}"
                           + (f"; расхождения: {dict(d)}" if d else "")))
    # b) счётчики
    t_old, t_new = _count(r"^- \[[ x]\] \d+\.\d+\.\d+\. ", old_text), _count(r"^- \[[ x]\] \d+\.\d+\.\d+\. ", new_text)
    p_old, p_new = _count(r"^## \d+\.\d+\. ", old_text), _count(r"^## \d+\.\d+\. ", new_text)
    m_old, m_new = _count(r"^# Веха \d+ — ", old_text), _count(r"^# Веха \d+ — ", new_text)
    out.append(CheckResult("b) число задач/пакетов/вех", t_old == t_new and p_old == p_new and m_new == 16,
                           f"задач {t_old}→{t_new}; пакетов {p_old}→{p_new}; вех {m_old}→{m_new}"))
    # c) суммы
    bl_new = C.parse_backlog(new_text)
    eo, en = est_report(bl_old), est_report(bl_new)
    lines: list[str] = []
    lines.append(f"исходный файл: пакетов с расхождением сумма задач ≠ «Оценка» (>0.1): {len(eo['pk_bad'])}")
    for key, (ts, st) in eo["pk_bad"].items():
        lines.append(f"  [было] пакет {key}→{mapd['packages'][key]}: задачи {_fmt_d(ts[0])} / {_fmt_d(ts[1])}; "
                     + (f"оценка пакета {_fmt_d(st[0])} / {_fmt_d(st[1])}" if st else "нет строки «Оценка»"))
    lines.append(f"исходный файл: вех с расхождением сумма пакетов ≠ «Оценка» вехи (>0.1): {len(eo['ms_bad'])}")
    for num, (sp, st) in eo["ms_bad"].items():
        lines.append(f"  [было] веха {num}: пакеты {_fmt_d(sp[0])} / {_fmt_d(sp[1])}; оценка вехи {_fmt_d(st[0])} / {_fmt_d(st[1])}")
    if eo["notask"]:
        lines.append(f"  задачи без тройки в названии: {sum(len(v) for v in eo['notask'].values())} в {len(eo['notask'])} пакетах")
    inv = {v: k for k, v in mapd["packages"].items()}
    new_bad_back = {inv[k]: v for k, v in en["pk_bad"].items()}
    pk_same = set(new_bad_back) == set(eo["pk_bad"]) and all(
        new_bad_back[k][0] == eo["pk_bad"][k][0] and new_bad_back[k][1] == eo["pk_bad"][k][1] for k in new_bad_back)
    # веха: оценка новой = сумма старых; расхождение новой = сумма расхождений старых
    ms_ok = True
    eps = Decimal("0.001")
    old_by = {m.num: m for m in bl_old.mss}

    def pk_sum(ms: C.Milestone) -> tuple[Decimal, Decimal]:
        sp = [p.est() for p in ms.pkgs]
        return sum((x[0] for x in sp if x), Decimal(0)), sum((x[1] for x in sp if x), Decimal(0))

    for k, (a, b) in new.ms_est.items():
        nm = next(m for m in bl_new.mss if m.num == k)
        sa, sb = pk_sum(nm)
        olds = next(m["old"] for m in plan["milestones"] if m["new"] == k)
        exp_a = sum((old_by[o].est[0] - pk_sum(old_by[o])[0] for o in olds), Decimal(0))
        exp_b = sum((old_by[o].est[1] - pk_sum(old_by[o])[1] for o in olds), Decimal(0))
        if abs(nm.est[0] - a) > eps or abs(nm.est[1] - b) > eps:
            ms_ok = False
            lines.append(f"  [ОШИБКА] строка «Оценка» новой вехи {k} ({_fmt_d(nm.est[0])} / {_fmt_d(nm.est[1])}) ≠ сумме старых вех ({_fmt_d(a)} / {_fmt_d(b)})")
        if abs((nm.est[0] - sa) - exp_a) > eps or abs((nm.est[1] - sb) - exp_b) > eps:
            ms_ok = False
            lines.append(f"  [ОШИБКА] новая веха {k}: «Оценка» − сумма пакетов ≠ сумме расхождений старых вех")
    for num, (sp, st) in en["ms_bad"].items():
        lines.append(f"  [стало] веха {num}: пакеты {_fmt_d(sp[0])} / {_fmt_d(sp[1])}; оценка вехи {_fmt_d(st[0])} / {_fmt_d(st[1])} (унаследовано от слитых вех)")
    tot_a = sum((a for a, _ in new.ms_est.values()), Decimal(0))
    tot_b = sum((b for _, b in new.ms_est.values()), Decimal(0))
    old_a = sum((m.est[0] for m in bl_old.mss), Decimal(0))
    old_b = sum((m.est[1] for m in bl_old.mss), Decimal(0))
    tot_ok = abs(tot_a - rr.totals_stated[0]) <= Decimal("0.1") and abs(tot_b - rr.totals_stated[1]) <= Decimal("0.1")
    roadmap_ok = (abs(rr.new_sum[0] - rr.old_sum[0]) <= Decimal("0.001") and abs(rr.new_sum[1] - rr.old_sum[1]) <= Decimal("0.001")
                  and abs(rr.new_sum[0] - rr.totals_stated[0]) <= Decimal("0.1")
                  and abs(rr.new_sum[1] - rr.totals_stated[1]) <= Decimal("0.05"))
    lines.append(f"сумма вех backlog: до {_fmt_d(old_a)} / {_fmt_d(old_b)}, после {_fmt_d(tot_a)} / {_fmt_d(tot_b)}; итог roadmap {_fmt_d(rr.totals_stated[0])} / {_fmt_d(rr.totals_stated[1])}")
    lines.append(f"сумма строк roadmap: до {_fmt_d(rr.old_sum[0])} / {_fmt_d(rr.old_sum[1])}, после {_fmt_d(rr.new_sum[0])} / {_fmt_d(rr.new_sum[1])}")
    # расхождения «Оценки» вех backlog и строк roadmap (исходные)
    for o, cs in rr.old_rows.items():
        bm = next(m for m in bl_old.mss if m.num == o)
        if abs(Decimal(cs[3]) - bm.est[0]) > Decimal("0.1") or abs(Decimal(cs[4]) - bm.est[1]) > Decimal("0.1"):
            lines.append(f"  [было] веха {o}: roadmap {cs[3]} / {cs[4]} ≠ backlog {_fmt_d(bm.est[0])} / {_fmt_d(bm.est[1])}")
    out.append(CheckResult("c) суммы троек/пакетов/вех/roadmap", pk_same and ms_ok and tot_ok and roadmap_ok,
                           f"расхождений пакетов было {len(eo['pk_bad'])}, стало {len(en['pk_bad'])} (совпадают с исходными: {pk_same}); "
                           f"вехи backlog {tot_a}/{tot_b} vs roadmap {rr.totals_stated[0]}/{rr.totals_stated[1]}: {tot_ok}; roadmap строк {roadmap_ok}", lines))
    # d) метки DEL
    dels = [rel for rel, b in results.items() if "\u27e6DEL" in b.decode("utf-8")]
    if "\u27e6DEL" in new_text:
        dels.append("backlog(new_text)")
    out.append(CheckResult("d) нет меток ⟦DEL", not dels, f"найдено в: {dels}" if dels else "0 вхождений"))
    # e) структура
    errs: list[str] = []
    heads = [(m.num, m.head[1]) for m in bl_new.mss]
    if [n for n, _ in heads] != list(range(1, 17)):
        errs.append(f"вехи: {[n for n, _ in heads]}")
    for n, hline in heads:
        if re.search(r"\((Шаг|Шаги)\b", hline):
            errs.append(f"веха {n}: в заголовке подпись шагов")
    for m in bl_new.mss:
        nums = [p.no for p in m.pkgs]
        if nums != list(range(1, len(nums) + 1)):
            errs.append(f"веха {m.num}: пакеты {nums}")
        for p in m.pkgs:
            tn = []
            for i in p.task_indexes():
                tm = C.TASK_RE.match(p.lines[i][1])
                if (int(tm.group(2)), int(tm.group(3))) != (p.ms, p.no):
                    errs.append(f"пакет {p.key}: задача {tm.group(2)}.{tm.group(3)}.{tm.group(4)} в чужом пакете")
                tn.append(int(tm.group(4)))
            if tn != list(range(1, len(tn) + 1)):
                errs.append(f"пакет {p.key}: задачи {tn[:8]}…")
            if new.pkg_counts[m.num] != len(m.pkgs):
                errs.append(f"веха {m.num}: число пакетов {len(m.pkgs)} ≠ {new.pkg_counts[m.num]}")
    if C.render_parsed(bl_new) != new_text:
        errs.append("повторный разбор нового backlog.md не восстанавливает текст")
    out.append(CheckResult("e) структура нового backlog.md", not errs, "16 вех 1..16, пакеты и задачи подряд" if not errs else "; ".join(errs[:10]), errs))
    # f) детерминированность
    out.append(CheckResult("f) идемпотентность предпросмотра", determinism[0], determinism[1]))
    # g) область правок
    bad = [r for r in list(results) if not in_scope(r)]
    bad += [r for r in scope_files if not in_scope(r)]
    out.append(CheckResult("g) область правок", not bad, f"к записи {len(results)} файлов; вне области: {bad}" if bad else f"к записи {len(results)} файлов, все в области п.5"))
    return out


# ---------------------------------------------------------------------------------------------
# Основной расчёт (чистая функция от содержимого файлов)
# ---------------------------------------------------------------------------------------------
@dataclass
class Computation:
    results: dict[str, bytes]
    srcs: dict[str, Src]
    scanned: list[str]
    col: Collector
    mapd: dict
    map_text: str
    old_struct: tuple[int, int, int]
    new_struct: tuple[int, int, int]
    new: NewStruct
    rr: RoadmapResult
    bl_old: C.Backlog
    old_backlog_text: str
    plan: dict


def compute(root: Path) -> Computation:
    plan = json.loads((HERE / "restructure_plan.json").read_text(encoding="utf-8"))
    bsrc = read_src(root, BACKLOG_REL)
    bl_old = C.parse_backlog(bsrc.text)
    if C.render_parsed(bl_old) != bsrc.text:
        raise RuntimeError("разбор backlog.md не воспроизводит исходный текст (обратная сборка расходится)")
    mapd = C.build_map(bl_old, plan)
    C.set_map(mapd)
    map_text = C.dump_map(mapd)
    col = Collector()
    new = restructure_backlog(col, bl_old, plan)
    rsrc = read_src(root, ROADMAP_REL)
    rr = build_roadmap(col, rsrc.text, plan, new.pkg_counts)
    srcs: dict[str, Src] = {BACKLOG_REL: bsrc, ROADMAP_REL: rsrc}
    results: dict[str, bytes] = {}
    for rel, new_text in ((BACKLOG_REL, new.text), (ROADMAP_REL, rr.text)):
        b = encode_like(srcs[rel], new_text)
        if b != srcs[rel].raw:
            results[rel] = b
    # сканирование старого текста backlog/roadmap на «подозрительные» уже сделано в remap_line (по тем же строкам);
    # строки, которые в backlog не попали в remap (заголовки вех, строки «Оценка») номеров пакетов не содержат.
    scope = enumerate_scope(root)
    scanned = [BACKLOG_REL, ROADMAP_REL]
    for rel in scope:
        if rel in (BACKLOG_REL, ROADMAP_REL):
            continue
        s = read_src(root, rel)
        srcs[rel] = s
        scanned.append(rel)
        nt = remap_whole(col, rel, s.text)
        b = encode_like(s, nt)
        if b != s.raw:
            results[rel] = b
    # строки, требующие решения человека (постановка, text_rewrites)
    dev = ".cursor/rules/svt-development-patterns.mdc"
    if dev in srcs:
        for i, t in enumerate(srcs[dev].text.split("\n"), 1):
            if "write-through, редактор и загрузчик" in t:
                col.manual.append(f"{dev}:{i}: «{t.strip()[:110]}» — по постановке править по бэклогу на веху «Ставки» (новая 6), механически скрипт даст веху 5 («Заявки»)")
    old_struct = (len(bl_old.mss), sum(len(m.pkgs) for m in bl_old.mss),
                  sum(len(p.task_indexes()) for m in bl_old.mss for p in m.pkgs))
    bl_new = C.parse_backlog(new.text)
    new_struct = (len(bl_new.mss), sum(len(m.pkgs) for m in bl_new.mss),
                  sum(len(p.task_indexes()) for m in bl_new.mss for p in m.pkgs))
    return Computation(results, srcs, scanned, col, mapd, map_text, old_struct, new_struct, new, rr, bl_old, bsrc.text, plan)


def digest(results: dict[str, bytes], map_text: str) -> str:
    h = hashlib.sha256()
    for rel in sorted(results):
        h.update(rel.encode("utf-8") + b"\0" + results[rel] + b"\0")
    h.update(map_text.encode("utf-8"))
    return h.hexdigest()


def changed_lines(src: Src, new_bytes: bytes) -> tuple[int, int]:
    nb = new_bytes[3:] if src.bom else new_bytes
    nt = nb.decode("utf-8").replace("\r\n", "\n")
    a, b = src.text.split("\n"), nt.split("\n")
    add = rem = 0
    for l in difflib.unified_diff(a, b, n=0, lineterm=""):
        if l.startswith("+") and not l.startswith("+++"):
            add += 1
        elif l.startswith("-") and not l.startswith("---"):
            rem += 1
    return add, rem


# ---------------------------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------------------------
def build_report(comp: Computation, checks: list[CheckResult], dig: str, full: bool) -> list[str]:
    col = comp.col
    r: list[str] = []
    r.append("== Файлы к изменению (добавлено/удалено строк) ==")
    for rel in sorted(comp.results):
        add, rem = changed_lines(comp.srcs[rel], comp.results[rel])
        r.append(f"  {rel}: +{add} / -{rem}")
    r.append(f"  всего файлов: {len(comp.results)} из {len(comp.scanned)} просмотренных")
    r.append("")
    r.append("== Структура до → после (backlog.md) ==")
    r.append(f"  вех {comp.old_struct[0]} → {comp.new_struct[0]}; пакетов {comp.old_struct[1]} → {comp.new_struct[1]}; задач {comp.old_struct[2]} → {comp.new_struct[2]}")
    r.append(f"  карта: пакетов {len(comp.mapd['packages'])}, задач {len(comp.mapd['tasks'])}, слитых вех {len(comp.mapd['milestones_merged'])}")
    r.append("")
    uk_t = [u for u in col.unknown if u[2].count(".") == 2]
    uk_p = [u for u in col.unknown if u[2].count(".") == 1]
    r.append(f"== Неизвестные токены (не изменены): всего {len(col.unknown)}; вид N.M.K — {len(uk_t)}, вид N.M — {len(uk_p)} ==")
    for rel, no, tok, frag in (uk_t if full else uk_t[:60]):
        r.append(f"  {rel}:{no}: [{tok}] …{frag}…")
    r.append("  — вид N.M —")
    for rel, no, tok, frag in (uk_p if full else uk_p[:60]):
        r.append(f"  {rel}:{no}: [{tok}] …{frag}…")
    r.append("")
    r.append(f"== Подозрительные старые номера, оставленные как есть: {len(col.suspicious)} ==")
    for rel, no, kind, frag in col.suspicious:
        r.append(f"  {rel}:{no}: ({kind}) …{frag}…")
    r.append("")
    r.append(f"== Заметки remap (схлопнутые диапазоны, раскрытые перечни): {len(col.notes)} ==")
    for n in col.notes:
        r.append(f"  {n}")
    r.append("")
    r.append(f"== Требуют решения человека: {len(col.manual)} ==")
    r.extend("  " + x for x in col.manual)
    r.append("")
    r.append(f"== Решения скрипта: {len(col.decisions) + len(comp.new.phase_omitted) + 1} ==")
    r.append(f"  номер в начале нумерованного заголовка (`### 4.1 …`) не менялся — это номер раздела документа: {col.headings_kept} строк")
    r.extend("  " + x for x in col.decisions)
    r.extend("  строка «Фаза» не добавлена (нет вводного текста): " + x for x in comp.new.phase_omitted)
    r.append("")
    r.append("== Проверки ==")
    for c in checks:
        r.append(f"  {'OK  ' if c.ok else 'FAIL'} {c.name}: {c.detail}")
        r.extend("      " + x for x in c.lines)
    r.append(f"  sha256 предпросмотра: {dig}")
    return r


def write_preview(root: Path, preview: Path, comp: Computation) -> None:
    if preview.exists():
        if preview.name != PREVIEW_NAME or preview.resolve().parent != HERE:
            raise RuntimeError(f"предпросмотр {preview} вне ожидаемой папки — не очищаю")
        shutil.rmtree(preview)
    preview.mkdir(parents=True)
    for rel, b in comp.results.items():
        dst = preview / PurePosixPath(rel)
        dst.parent.mkdir(parents=True, exist_ok=True)
        dst.write_bytes(b)


def main(argv: list[str] | None = None) -> int:
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except Exception:
        pass
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    g = ap.add_mutually_exclusive_group()
    g.add_argument("--dry-run", action="store_true", help="по умолчанию: предпросмотр и отчёт, файлы на месте не меняются")
    g.add_argument("--apply", action="store_true", help="перенести результат из предпросмотра на место")
    ap.add_argument("--root", type=Path, default=DEFAULT_ROOT, help="корень репозитория (по умолчанию — по расположению скрипта)")
    ap.add_argument("--full", action="store_true", help="печатать отчёт полностью")
    args = ap.parse_args(argv)
    root = args.root.resolve()
    preview = HERE / PREVIEW_NAME

    comp = compute(root)
    comp2 = compute(root)
    d1, d2 = digest(comp.results, comp.map_text), digest(comp2.results, comp2.map_text)
    prev_note = ""
    determinism = (d1 == d2 and comp.results == comp2.results, f"два расчёта подряд совпадают: {d1 == d2}")
    checks = run_checks(comp.old_backlog_text, comp.new, comp.bl_old, comp.mapd, comp.plan, comp.rr,
                        comp.results, determinism, comp.scanned)
    ok = all(c.ok for c in checks)

    # карта — создаваемый файл этапа, пишется всегда
    map_path = C.MAP_PATH
    if not map_path.exists() or map_path.read_bytes() != comp.map_text.encode("utf-8"):
        map_path.write_bytes(comp.map_text.encode("utf-8"))
    write_preview(root, preview, comp)
    # сверка записанного предпросмотра с расчётом
    for rel, b in comp.results.items():
        if (preview / PurePosixPath(rel)).read_bytes() != b:
            raise RuntimeError(f"предпросмотр {rel} не совпал с расчётом")
    report = build_report(comp, checks, d1, full=True)
    (preview / "_report.txt").write_text("\n".join(report) + "\n", encoding="utf-8")
    shown = build_report(comp, checks, d1, full=args.full)
    print("\n".join(shown))
    print(f"\nПредпросмотр: {preview}\nПолный отчёт: {preview / '_report.txt'}")
    if not ok:
        print("\nПРОВЕРКИ НЕ ПРОЙДЕНЫ — файлы на место не переносятся.", file=sys.stderr)
        return 1
    if args.apply:
        for rel, b in comp.results.items():
            (root / rel).write_bytes(b)
        print(f"\n--apply: перенесено файлов: {len(comp.results)}")
    else:
        print("\nDRY-RUN: файлы канона не менялись.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
