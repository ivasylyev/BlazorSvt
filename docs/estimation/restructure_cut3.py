# -*- coding: utf-8 -*-
"""Этап 2 реструктуризации бэклога СВТ (третий проход, 27.11.2026): перенос задач и пакетов, расщепление и слияние пакетов,
новые задачи, перенумерация вех/пакетов/задач.

  python docs/estimation/restructure_cut3.py                    # --dry-run (по умолчанию): карта, _stage2_cut3_preview/ и отчёт
  python docs/estimation/restructure_cut3.py --selftest         # проверки на синтетическом мини-бэклоге и на карте репозитория
  python docs/estimation/restructure_cut3.py --snapshot-report  # устаревшие номера в снимках (docs/tasks, docs/docs, atlas-methodology), только список
  python docs/estimation/restructure_cut3.py --apply            # ПЕРЕНОСИТ preview на место; нужен чистый git status по области правок

Источник решений — restructure_plan_cut3.json (26 операций; id сверяются со скриптом). Номера в реестре — ТЕКУЩИЕ номера backlog.md.
Карта «старый → новый» пишется в renumber_map_cut3.json (и в dry-run тоже). Прежние скрипты, карты и таблицы не меняются:
из renumber_cut2.py импортируются parse_backlog, remap_text2, scan_suspicious, set_map и вспомогательные регулярки; старая карта cut2 после
импорта сбрасывается и подменяется картой этого прохода.

Порядок работы: (1) разбор старого backlog.md в модель; (2) структурные операции над объектами модели (тексты не трогаются) и построение
полной карты; (3) ОДИН проход remap_text2 по всем старым текстовым полям; (4) правки и вставки новых текстов (новые заголовки, новые задачи,
переименования) — уже в новой нумерации, ремапу не подвергаются; (5) перенумерация подряд и сборка файла.
Генератор оценок и check_consistency.py скрипт не запускает.
"""
from __future__ import annotations

import argparse
import difflib
import hashlib
import json
import re
import shutil
import subprocess
import sys
from collections import Counter
from dataclasses import dataclass, field
from decimal import Decimal, ROUND_HALF_UP
from pathlib import Path, PurePosixPath
from typing import Callable

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))

import renumber as RN  # noqa: E402
import renumber_cut2 as C  # noqa: E402

# При импорте renumber_cut2 сам загрузил СТАРУЮ карту (renumber_map_cut2.json). Сбрасываем: пока не вызван set_map с картой cut3,
# remap_text2 бросит RuntimeError, а не молча применит устаревшую нумерацию.
C._S.clear()

DEFAULT_ROOT = HERE.parents[1]
PREVIEW_NAME = "_stage2_cut3_preview"
PLAN_PATH = HERE / "restructure_plan_cut3.json"
MAP_PATH = HERE / "renumber_map_cut3.json"
SNAPSHOT_REPORT_PATH = HERE / "_snapshot_report.txt"
BACKLOG_REL = ".cursor/skills/svt-architecture/backlog.md"
ROADMAP_REL = ".cursor/skills/svt-architecture/roadmap.md"

SCOPE_GLOBS = [
    ".cursor/skills/svt-architecture/*.md",
    ".cursor/skills/svt-*/**/*.md",
    ".cursor/rules/*.mdc",
    ".cursor/docs/svt/*.md",
    ".cursor/README.md",
    "docs/output-review-package/README.md",
    "docs/output-review-package/review-current-state.html",
    "docs/api/svt-api-v1-developers.md",
    "BlazorSvt/**/*.cs",
]
FORBIDDEN_PREFIXES = ("docs/tasks/", "docs/docs/", "docs/estimation/", ".cursor/skills/atlas-", ".git/", "tests/")
EXCLUDED_PARTS = {"bin", "obj", ".git", "node_modules"}
SNAPSHOT_DIRS = ["docs/tasks", "docs/docs", ".cursor/skills/atlas-methodology"]
SNAPSHOT_EXT = {".md", ".txt", ".html", ".htm", ".json", ".mdc", ".csv", ".yml", ".yaml", ".xml", ".ps1", ".py"}

EXPECTED_OPS = {
    1: "rename", 2: "move", 3: "move", 4: "check", 5: "move", 6: "rename", 7: "rename", 8: "cancelled", 9: "strip",
    10: "rename", 11: "move", 12: "delete", 13: "rename+move", 14: "split", 15: "move", 16: "delete", 17: "sign",
    18: "merge", 19: "move", 20: "move", 21: "delete", 22: "add", 23: "delete", 24: "move+edit", 25: "delete", 26: "rename",
}
OP9_SUFFIX = ": MVP, ОПЭ и ПЭ"
# Решения, принятые скриптом сверх/вне буквы постановки (попадают в отчёт, «Решения скрипта»).
STATIC_DECISIONS = [
    "remap: вместо C.remap_text2 используется копия remap_text3 — только слияние пакетов 5.9–5.10 → 5.6 даёт «5.6», а не «5.6, 5.6»; renumber_cut2.py не менялся",
    "ссылки «backlog 4.4.8 и 4.4.9» в docs/api/svt-api-v1-developers.md экранированы и не заменяются: они устарели до прохода, замена по текущей карте была бы неверной по смыслу",
    "токены, которых нет в карте (удалённые пакеты, расщеплённый 5.4, устаревшие ссылки), в тексте не заменяются и идут в markers; рядом со слитыми номерами в одной строке возможны двусмысленные пары («6.17 и 6.17», «5.6, 5.6») — строки перечислены в разделе 4",
    "названия вех в roadmap.md берутся из backlog.md; колонки «Результат/Работы/Шаги» roadmap.md и вводные абзацы вех не переписывались (ручные места, раздел 6)",
    "в html правятся только текстовые узлы, в .cs — только комментарии; docs/tasks, docs/docs, docs/estimation, tests и atlas-* не затрагиваются (отчёт `--snapshot-report` — справочный)",
]
FILE_ROW_RE = re.compile(r"^\| \*\*\d+\*\* \|")


class PlanError(Exception):
    pass


class OpError(Exception):
    pass


def in_scope(rel: str) -> bool:
    """Независимая от glob-ов проверка области правок (проверка g)."""
    parts = set(PurePosixPath(rel).parts)
    if rel.startswith(FORBIDDEN_PREFIXES) or parts & EXCLUDED_PARTS:
        return False
    if rel in (BACKLOG_REL, ROADMAP_REL, ".cursor/README.md", "docs/output-review-package/README.md",
               "docs/output-review-package/review-current-state.html", "docs/api/svt-api-v1-developers.md"):
        return True
    if rel.startswith(".cursor/skills/svt-") and rel.endswith(".md"):
        return True
    if rel.startswith(".cursor/rules/") and rel.count("/") == 2 and rel.endswith(".mdc"):
        return True
    if rel.startswith(".cursor/docs/svt/") and rel.count("/") == 3 and rel.endswith(".md"):
        return True
    if rel.startswith("BlazorSvt/") and rel.endswith(".cs"):
        return True
    return False


def enumerate_scope(root: Path) -> list[str]:
    out: set[str] = set()
    for pat in SCOPE_GLOBS:
        for p in root.glob(pat):
            if p.is_file():
                rel = p.relative_to(root).as_posix()
                if in_scope(rel):
                    out.add(rel)
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
    mixed: bool = False


def read_src(root: Path, rel: str, strict: bool = True) -> Src:
    raw = (root / rel).read_bytes()
    bom = raw.startswith(b"\xef\xbb\xbf")
    s = (raw[3:] if bom else raw).decode("utf-8")
    n_crlf, n_lf = s.count("\r\n"), s.count("\n")
    stray_cr = s.replace("\r\n", "").count("\r")
    mixed = bool(stray_cr or (n_crlf and n_crlf != n_lf))
    if mixed and strict:
        raise RuntimeError(f"{rel}: смешанные окончания строк (CRLF={n_crlf}, LF={n_lf}, CR={stray_cr}) — файл не обработан")
    text = s.replace("\r\n", "\n").replace("\r", "\n") if mixed else s.replace("\r\n", "\n")
    return Src(rel, raw, text, bom, bool(n_crlf) and not mixed, mixed)


def encode_like(src: Src, text: str) -> bytes:
    if src.crlf:
        text = text.replace("\n", "\r\n")
    data = text.encode("utf-8")
    return (b"\xef\xbb\xbf" + data) if src.bom else data


# ---------------------------------------------------------------------------------------------
# Реестр
# ---------------------------------------------------------------------------------------------
def load_plan(path: Path = PLAN_PATH) -> dict:
    plan = json.loads(path.read_text(encoding="utf-8"))
    ids = [o["id"] for o in plan["ops"]]
    if ids != list(range(1, 27)):
        raise PlanError(f"id операций реестра {ids} ≠ 1–26")
    for o in plan["ops"]:
        if EXPECTED_OPS[o["id"]] != o["op"]:
            raise PlanError(f"операция {o['id']}: в реестре «{o['op']}», в скрипте «{EXPECTED_OPS[o['id']]}»")
    return plan


# ---------------------------------------------------------------------------------------------
# Модель backlog.md
# ---------------------------------------------------------------------------------------------
@dataclass
class Line:
    text: str
    ln: int | None = None          # номер строки в старом backlog.md; None — новый текст (ремапу не подвергается)

    @property
    def fresh(self) -> bool:
        return self.ln is None


@dataclass
class Task:
    old: str | None
    status: str
    rest: str                      # всё после «- [ ] N.M.K. »
    ln: int | None
    edits: list[Callable[[str], str]] = field(default_factory=list)
    new: str = ""

    @property
    def fresh(self) -> bool:
        return self.ln is None


@dataclass
class Pkg:
    olds: list[str]                # прежние номера пакетов, вошедшие сюда (у новых — пусто)
    head: str                      # текст заголовка после «## N.M. »
    head_ln: int | None
    est: str                       # строка «**Оценка:** …»
    est_old: tuple[Decimal, Decimal] | None
    est_third: str
    old_task_sum: tuple[Decimal, Decimal] | None
    intro: list[Line]
    tasks: list[Task]
    crits: list[Line]
    fresh: bool = False
    head_edits: list[Callable[[str], str]] = field(default_factory=list)
    new: str = ""
    est_val: tuple[Decimal, Decimal] = (Decimal(0), Decimal(0))


@dataclass
class Ms:
    num: int
    name: str
    head_ln: int
    est: str
    est_old: tuple[Decimal, Decimal]
    intro: list[Line]
    pkgs: list[Pkg]
    est_val: tuple[Decimal, Decimal] = (Decimal(0), Decimal(0))


@dataclass
class Model:
    preamble: list[Line]
    mss: list[Ms]
    tail: list[Line]


def triple(rest: str) -> tuple[Decimal, Decimal]:
    m = C.TRIPLE_RE.search(rest)
    return (Decimal(m.group(1)), Decimal(m.group(2))) if m else (Decimal(0), Decimal(0))


def task_title(rest: str) -> str:
    m = C.TRIPLE_RE.search(rest)
    return (rest[: m.start()] if m else rest).strip()


def parse_model(text: str) -> Model:
    bl = C.parse_backlog(text)
    if C.render_parsed(bl) != text:
        raise RuntimeError("разбор backlog.md не воспроизводит исходный текст (обратная сборка расходится)")
    mss: list[Ms] = []
    for m in bl.mss:
        if m.steps is not None:
            raise C.StructureError(f"веха {m.num}: в заголовке подпись шагов")
        ms = Ms(m.num, m.name, m.head[0], m.est_line[1], m.est, [Line(t, n) for n, t in m.intro], [])
        for p in m.pkgs:
            L = p.lines
            hm = C.PKG_RE.match(L[0][1])
            if len(L) < 5 or L[1][1] != "" or not C.EST_RE.match(L[2][1]) or L[3][1] != "":
                raise C.StructureError(f"пакет {p.key}: нестандартная шапка")
            idx = p.task_indexes()
            if not idx or idx != list(range(idx[0], idx[0] + len(idx))):
                raise C.StructureError(f"пакет {p.key}: задачи не образуют сплошной блок")
            tasks = []
            for i in idx:
                tm = C.TASK_RE.match(L[i][1])
                tid = f"{tm.group(2)}.{tm.group(3)}.{tm.group(4)}"
                tasks.append(Task(tid, tm.group(1), L[i][1][tm.end():], L[i][0]))
            tail = L[idx[-1] + 1:]
            crits: list[Line] = []
            if tail:
                if tail[0][1] != "" or any(t != "" and not t.startswith("**Критерий:**") for _, t in tail):
                    raise C.StructureError(f"пакет {p.key}: нестандартный хвост после задач")
                crits = [Line(t, n) for n, t in tail if t != ""]
            em = C.EST_RE.match(L[2][1])
            ms.pkgs.append(Pkg([p.key], L[0][1][hm.end():], L[0][0], L[2][1], p.est(), em.group(3), p.task_sum(),
                               [Line(t, n) for n, t in C._strip_blank(L[4: idx[0]])], tasks, crits))
        mss.append(ms)
    return Model([Line(t, n) for n, t in bl.preamble], mss, [Line(t, n) for n, t in bl.tail])


def render_model(model: Model) -> str:
    blocks = []
    for k, ms in enumerate(model.mss, 1):
        pkgs = []
        for j, p in enumerate(ms.pkgs, 1):
            L = [f"## {k}.{j}. {p.head}", "", p.est, ""]
            if p.intro:
                L += [l.text for l in p.intro] + [""]
            for n, t in enumerate(p.tasks, 1):
                L.append(f"- [{t.status}] {k}.{j}.{n}. {t.rest}")
            for c in p.crits:
                L += ["", c.text]
            pkgs.append(L)
        blocks.append(C.ms_lines(f"# Веха {k} — {ms.name}", ms.est, [l.text for l in ms.intro], pkgs))
    return C.render_backlog([l.text for l in model.preamble], blocks, [l.text for l in model.tail])


def fmt_num(d: Decimal) -> str:
    return C.fmt_num(d)


# ---------------------------------------------------------------------------------------------
# Операции над моделью
# ---------------------------------------------------------------------------------------------
class Ctx:
    def __init__(self, model: Model):
        self.model = model

    def ms(self, num: int) -> Ms:
        for m in self.model.mss:
            if m.num == num:
                return m
        raise OpError(f"веха {num} не найдена")

    def pkg(self, key: str) -> tuple[Ms, Pkg]:
        for m in self.model.mss:
            for p in m.pkgs:
                if key in p.olds:
                    return m, p
        raise OpError(f"пакет {key} не найден")

    def task(self, tid: str) -> tuple[Pkg, int, Task]:
        for m in self.model.mss:
            for p in m.pkgs:
                for i, t in enumerate(p.tasks):
                    if t.old == tid:
                        return p, i, t
        raise OpError(f"задача {tid} не найдена")

    def pop_task(self, tid: str) -> Task:
        p, i, t = self.task(tid)
        p.tasks.pop(i)
        return t

    def pop_pkg(self, key: str) -> Pkg:
        m, p = self.pkg(key)
        m.pkgs.remove(p)
        return p

    def pkg_index(self, ms: Ms, key: str) -> int:
        for i, p in enumerate(ms.pkgs):
            if key in p.olds:
                return i
        raise OpError(f"пакет {key} не лежит в вехе {ms.num}")

    def remove_empty_pkg(self, key: str, allow_tasks: tuple[str, ...] = (), allow_crits: bool = False) -> Pkg:
        """Удаление пакета: допустимо, только если задач нет (кроме явно названных) и критерий перенесён (или назван)."""
        _, p = self.pkg(key)
        left = [t.old for t in p.tasks]
        if left != list(allow_tasks):
            raise OpError(f"пакет {key}: перед удалением остались задачи {left}, ожидались {list(allow_tasks)}")
        if p.crits and not allow_crits:
            raise OpError(f"пакет {key}: перед удалением остался критерий — его нужно перенести")
        return self.pop_pkg(key)

    def split_pkg(self, key: str, specs: list[dict]) -> list[Pkg]:
        """Расщепление: пакет убирается, из его задач собираются новые пакеты. spec: head, tasks, intro, crits, ms_pos (не здесь)."""
        _, p = self.pkg(key)
        have = [t.old for t in p.tasks]
        want = [x for s in specs for x in s["tasks"]]
        if sorted(have) != sorted(want):
            raise OpError(f"расщепление {key}: задачи пакета {have} ≠ задачам частей {want}")
        by = {t.old: t for t in p.tasks}
        out = []
        for s in specs:
            out.append(Pkg([], s["head"], None, "", None, "—", None, list(s["intro"]), [by[x] for x in s["tasks"]],
                           list(s["crits"]), fresh=True))
        self.pop_pkg(key)
        return out

    def merge_pkgs(self, keys: list[str], head: str, order: list[str], crits_order: list[str]) -> Pkg:
        pk = [self.pkg(k)[1] for k in keys]
        have = sorted(t.old for p in pk for t in p.tasks)
        if have != sorted(order):
            raise OpError(f"слияние {keys}: задачи {have} ≠ порядку {order}")
        for p in pk:
            if p.intro:
                raise OpError(f"слияние: у пакета {p.olds} есть вводный текст — правила его переноса нет")
        by = {t.old: t for p in pk for t in p.tasks}
        crits = []
        for k in crits_order:
            crits += self.pkg(k)[1].crits
        merged = Pkg(list(keys), head, None, "", None, "—", None, [], [by[x] for x in order], crits, fresh=True)
        return merged

    def add_task(self, pkg: Pkg, rest: str, status: str = " ", index: int | None = None) -> Task:
        t = Task(None, status, rest, None)
        pkg.tasks.insert(len(pkg.tasks) if index is None else index, t)
        return t


def rename_edit(frm: str, to: str, what: str) -> Callable[[str], str]:
    def f(s: str) -> str:
        if not s.startswith(frm):
            raise OpError(f"{what}: текст не начинается с «{frm}»: {s[:80]}")
        return to + s[len(frm):]
    return f


def head_replace_edit(frm: str, to: str, what: str) -> Callable[[str], str]:
    def f(s: str) -> str:
        if s.count(frm) != 1:
            raise OpError(f"{what}: в заголовке «{s}» фрагмент «{frm}» найден {s.count(frm)} раз")
        return s.replace(frm, to)
    return f


@dataclass
class OpsInfo:
    deleted_pkgs: dict[str, str] = field(default_factory=dict)
    deleted_tasks: dict[str, str] = field(default_factory=dict)
    ambiguous: dict[str, dict[str, str]] = field(default_factory=dict)
    ambiguous_keys: dict[str, list[Pkg]] = field(default_factory=dict)
    new_task_count: int = 0
    op9_names: list[tuple[int, str]] = field(default_factory=list)
    decisions: list[str] = field(default_factory=list)
    renames: list[tuple[str, str]] = field(default_factory=list)       # (было, стало) — заголовки задач (до тройки)
    task_adds: list[str] = field(default_factory=list)                  # заголовки новых задач
    removed_task_titles: list[str] = field(default_factory=list)
    stat_deleted_pkg_tasks: int = 0


def _ids(s: str, n: int | None = None, kind: str = "task") -> list[str]:
    rx = r"\d+\.\d+\.\d+" if kind == "task" else r"\d+\.\d+"
    out = re.findall(rf"(?<![\d.])({rx})(?![\d]|\.\d)", s)
    if n is not None and len(out) != n:
        raise OpError(f"в строке реестра «{s}» ожидалось {n} номеров, найдено {out}")
    return out


def apply_ops(model: Model, plan: dict) -> OpsInfo:
    ops = {o["id"]: o for o in plan["ops"]}
    X = Ctx(model)
    info = OpsInfo()
    info.stat_deleted_pkg_tasks = len(X.pkg("6.17")[1].tasks)

    # op 1: название вехи 3
    o = ops[1]
    ms3 = X.ms(3)
    if ms3.name != o["from"]:
        raise OpError(f"op1: название вехи 3 «{ms3.name}» ≠ «{o['from']}»")
    ms3.name = o["to"]

    # op 9: суффикс в названиях вех
    o = ops[9]
    suffix = re.search(r"«([^»]+)»", o["what"]).group(1)
    if suffix != OP9_SUFFIX:
        raise OpError(f"op9: суффикс реестра «{suffix}» ≠ «{OP9_SUFFIX}»")
    nums = [int(x) for x in re.findall(r"\d+", o["what"].split("вех", 1)[1])]
    for m in model.mss:
        has = m.name.endswith(suffix)
        if (m.num in nums) != has:
            raise OpError(f"op9: у вехи {m.num} суффикс {'есть' if has else 'отсутствует'}, по реестру {'должен быть' if m.num in nums else 'не должен быть'}")
        if has:
            m.name = m.name[: -len(suffix)]
            info.op9_names.append((m.num, m.name))

    # op 2 / op 14 (MVP API): состав вехи 3
    o2 = ops[2]
    a, b = _ids(o2["what"], 2, "pkg")
    keys = [f"4.{i}" for i in range(int(a.split(".")[1]), int(b.split(".")[1]) + 1)]
    ms4 = X.ms(4)
    moved = [X.pop_pkg(k) for k in keys]
    i31 = X.pkg_index(ms3, "3.1")
    ms3.pkgs[i31:i31] = moved

    # op 3: 3.2 → веха 4 перед 4.11
    o = ops[3]
    k_from = _ids(o["what"], 1, "pkg")[0]
    k_before = _ids(o["to"], 1, "pkg")
    pk32 = X.pop_pkg(k_from)
    ms4.pkgs.insert(X.pkg_index(ms4, k_before[0]), pk32)

    # op 4: 2.5.1 → [x]
    tid = _ids(ops[4]["what"], 1)[0]
    _, _, t = X.task(tid)
    if t.status != " ":
        raise OpError(f"op4: задача {tid} уже не «[ ]»")
    t.status = "x"

    # op 5: группа задач в 4.12 после 4.12.5
    o = ops[5]
    group = _ids(o["what"], 6)
    after_id = _ids(o["to"], 1)[0]
    taken = [X.pop_task(x) for x in group]
    p412, ia, _ = X.task(after_id)
    pos = ia + 1
    for t in taken:
        p412.tasks.insert(pos, t)
        pos += 1

    # op 13: 5.4.13 → после группы, переименование
    o = ops[13]
    tid13 = _ids(o["what"], 1)[0]
    t13 = X.pop_task(tid13)
    if not t13.rest.startswith(o["from"]):
        raise OpError(f"op13: «{t13.rest[:70]}» не начинается с реестрового from")
    p412.tasks.insert(pos, t13)
    t13.edits.append(rename_edit(o["from"], o["to"], "op13"))
    info.renames.append((o["from"], o["to"]))

    # op 6, 7: переименование 4.5.5 и 4.5.6
    for oid in (6, 7):
        o = ops[oid]
        tid = _ids(o["what"], 1)[0]
        _, _, t = X.task(tid)
        if not t.rest.startswith(o["from"]):
            raise OpError(f"op{oid}: задача {tid} «{t.rest[:70]}» не начинается с «{o['from']}»")
        t.edits.append(rename_edit(o["from"], o["to"], f"op{oid}"))
        info.renames.append((o["from"], o["to"]))

    # op 10: пакет 4.14
    o = ops[10]
    _, p = X.pkg(_ids(o["what"], 1, "pkg")[0])
    p.head_edits.append(head_replace_edit(o["from"], o["to"], "op10"))

    # op 11: 5.5.1 → в конец 3.2; op 12: удалить 5.5
    o = ops[11]
    t11 = X.pop_task(_ids(o["what"], 1)[0])
    _, p32 = X.pkg("3.2")
    p32.tasks.append(t11)
    X.remove_empty_pkg(_ids(ops[12]["what"], 1, "pkg")[0])
    info.deleted_pkgs["5.5"] = "op12: пакет пуст после переноса 5.5.1 (теперь в бывшем 3.2)"

    # op 14: расщепление 5.4
    o = ops[14]
    pk54_ms, pk54 = X.pkg("5.4")
    parts = o["parts"]
    want0 = [f"5.4.{i}" for i in range(1, 8)]
    if parts[0]["tasks"] != want0:
        raise OpError(f"op14: задачи части 1 {parts[0]['tasks']} ≠ {want0}")
    intro2 = parts[1]["intro"]
    intro2_text = intro2[intro2.index("«") + 1: intro2.rindex("»")]
    sign0, sign1 = parts[0]["sign"], parts[1]["sign"]
    old_intro, old_crits = list(pk54.intro), list(pk54.crits)
    p_mvp, p_atlas = X.split_pkg("5.4", [
        {"head": f"{parts[0]['name']} {sign0}", "tasks": parts[0]["tasks"], "intro": old_intro, "crits": []},
        {"head": f"{parts[1]['name']} {sign1}", "tasks": parts[1]["tasks"], "intro": [Line(intro2_text)], "crits": old_crits},
    ])
    i31 = X.pkg_index(ms3, "3.1")
    ms3.pkgs.insert(i31, p_mvp)
    ms10 = X.ms(10)
    ms10.pkgs.insert(X.pkg_index(ms10, "10.4") + 1, p_atlas)
    info.ambiguous["5.4"] = {}  # значения — после нумерации
    info.ambiguous_keys["5.4"] = [p_mvp, p_atlas]

    # op 15, 16: 5.7.1 → начало 5.8; удалить 5.7
    t15 = X.pop_task(_ids(ops[15]["what"], 1)[0])
    _, p58 = X.pkg("5.8")
    p58.tasks.insert(0, t15)
    X.remove_empty_pkg(_ids(ops[16]["what"], 1, "pkg")[0])
    info.deleted_pkgs["5.7"] = "op16: пакет пуст после переноса 5.7.1 (теперь начало бывшего 5.8); вводный абзац «Доля прежних сумм…» не переносится"

    # op 17: подпись шагов 5.8
    o = ops[17]
    add_step = re.search(r"добавить «([^»]+)»", o["what"]).group(1)

    def sign_edit(s: str) -> str:
        m = re.match(r"^(.*\(Шаги: )([^)]*)(\))$", s)
        if not m:
            raise OpError(f"op17: в заголовке 5.8 нет подписи «(Шаги: …)»: {s}")
        if add_step in m.group(2):
            raise OpError(f"op17: шаг «{add_step}» уже есть в подписи 5.8")
        return f"{m.group(1)}{m.group(2)}, {add_step}{m.group(3)}"
    p58.head_edits.append(sign_edit)

    # op 18: слияние 5.9 и 5.10
    o = ops[18]
    name18 = re.search(r"«(.+)»", o["target"]).group(1)
    keys18 = o["from"]
    pm = X.merge_pkgs(keys18, f"{name18} {o['sign']}", o["tasks_order"], keys18)
    ms5 = X.ms(5)
    i59 = X.pkg_index(ms5, keys18[0])
    for k in keys18:
        X.pop_pkg(k)
    ms5.pkgs.insert(i59, pm)

    # op 19, 20, 21: 6.10 → 6.9
    t19 = X.pop_task(_ids(ops[19]["what"], 1)[0])
    _, p69 = X.pkg("6.9")
    after19 = _ids(ops[19]["to"], 1)[0]
    if p69.tasks[-1].old != after19:
        raise OpError(f"op19: последняя задача 6.9 — {p69.tasks[-1].old}, по реестру {after19}")
    p69.tasks.append(t19)
    _, p610 = X.pkg("6.10")
    if not p610.crits:
        raise OpError("op20: у 6.10 нет критерия")
    p69.crits += p610.crits
    p610.crits = []
    X.remove_empty_pkg(_ids(ops[21]["what"], 1, "pkg")[0])
    info.deleted_pkgs["6.10"] = "op21: пакет пуст после переноса 6.10.1 и критерия в 6.9"

    # op 22: руководства
    o = ops[22]
    sign22 = re.search(r"«(\(Шаг: [^)]*\))»", o["what"]).group(1)
    items = []
    for s in o["tasks"]:
        mm = re.match(r"^(.*?) (\(\d+(?:\.\d+)? / \d+(?:\.\d+)? / [^)]*\))$", s)
        if not mm:
            raise OpError(f"op22: не разобрана задача реестра «{s}»")
        items.append((mm.group(1), mm.group(2)))
    for tg in o["targets"]:
        _, p = X.pkg(tg["package_old"])
        aft = re.match(r"^\d+\.\d+\.\d+", tg["after"])
        if aft and p.tasks[-1].old != aft.group(0):
            raise OpError(f"op22: пакет {tg['package_old']}: «после {aft.group(0)}», но последняя задача — {p.tasks[-1].old}")
        if "Внедрение" not in p.head:
            info.decisions.append(f"op22: в заголовке пакета {tg['package_old']} нет шага «Внедрение» — задачи с «(Шаг: Внедрение)» добавлены по реестру")
        for title, trp in items:
            rest = f"{title} {sign22} {trp}"
            X.add_task(p, rest)
            info.task_adds.append(f"{title} {sign22}")
            info.new_task_count += 1
    if info.new_task_count != o["new_tasks"]:
        raise OpError(f"op22: добавлено {info.new_task_count}, в реестре new_tasks={o['new_tasks']}")

    # op 23: удалить 6.17 с задачей и критерием
    o = ops[23]
    k23 = _ids(o["what"], 1, "pkg")[0]
    t23 = _ids(o["what"], 1)[0]
    _, p617 = X.pkg(k23)
    if [t.old for t in p617.tasks] != [t23]:
        raise OpError(f"op23: задачи пакета {k23} — {[t.old for t in p617.tasks]}, по реестру только {t23}")
    for t in p617.tasks:
        info.deleted_tasks[t.old] = f"op23: удалена вместе с пакетом {k23}"
        info.removed_task_titles.append(task_title(t.rest))
    X.remove_empty_pkg(k23, allow_tasks=(t23,), allow_crits=True)
    info.deleted_pkgs[k23] = f"op23: удалён целиком с задачей {t23} и Критерием (named_by_owner)"

    # op 24, 25, 26
    o = ops[24]
    tid24 = _ids(o["what"], 1)[0]
    t24 = X.pop_task(tid24)
    p31, i, _ = X.task(_ids(o["to"], 1)[0])
    ed = o["edit"]
    chg = next(c for c in plan["estimates"]["changed"] if c["item"] == tid24)
    old_a, old_b = [Decimal(x.strip()) for x in chg["from"].split("/")]
    if triple(t24.rest) != (old_a, old_b):
        raise OpError(f"op24: тройка {tid24} {triple(t24.rest)} ≠ {chg['from']}")
    if t24.status != " ":
        raise OpError("op24: задача 6.23.1 уже [x]")
    t24.status = ed["status"].strip("[]") or " "
    new_triple = ed["triple"]

    def edit24(s: str) -> str:
        mm = re.match(r"^(?P<title>.+?) \((?P<a>\d+(?:\.\d+)?) / (?P<b>\d+(?:\.\d+)?) / (?P<c>[^)]*)\)(?P<note> — .*)?$", s)
        if not mm:
            raise OpError(f"op24: не разобран текст задачи: {s}")
        note = mm.group("note") or ""
        dropped = ed["drop_note"]
        need = re.search(r"«([^»]+)»", dropped).group(1)
        if need not in note:
            raise OpError(f"op24: примечание задачи «{note}» не содержит «{need}»")
        return f"{ed['title_mark']} {mm.group('title')} {new_triple}"
    t24.edits.append(edit24)
    p31.tasks.insert(i + 1, t24)
    info.renames.append((task_title(t24.rest), f"{ed['title_mark']} {task_title(t24.rest)}"))
    X.remove_empty_pkg(_ids(ops[25]["what"], 1, "pkg")[0])
    info.deleted_pkgs["6.23"] = "op25: пакет пуст после переноса 6.23.1 (теперь в бывшем 3.1)"

    o = ops[26]
    tid26 = _ids(o["what"], 1)[0]
    _, _, t26 = X.task(tid26)
    if not t26.rest.startswith(o["from"]):
        raise OpError(f"op26: {tid26} «{t26.rest[:70]}» не начинается с «{o['from']}»")
    t26.edits.append(rename_edit(o["from"], o["to"], "op26"))
    info.renames.append((o["from"], o["to"]))

    return info


# ---------------------------------------------------------------------------------------------
# Нумерация и карта
# ---------------------------------------------------------------------------------------------
def number_model(model: Model, old_pkgs: list[str], old_tasks: list[str], info: OpsInfo) -> dict:
    t_map: dict[str, str] = {}
    p_map: dict[str, str] = {}
    merged: dict[str, list[str]] = {}
    new_tasks: dict[str, str] = {}
    moved: dict[str, str] = {}
    for k, ms in enumerate(model.mss, 1):
        if ms.num != k:
            raise OpError(f"веха {ms.num} оказалась на позиции {k}")
        for j, p in enumerate(ms.pkgs, 1):
            p.new = f"{k}.{j}"
            for o in p.olds:
                if o in p_map:
                    raise OpError(f"пакет {o} встречается дважды")
                p_map[o] = p.new
            if len(p.olds) > 1:
                merged[p.new] = list(p.olds)
            for n, t in enumerate(p.tasks, 1):
                t.new = f"{k}.{j}.{n}"
                if t.old:
                    if t.old in t_map:
                        raise OpError(f"задача {t.old} встречается дважды")
                    t_map[t.old] = t.new
                else:
                    new_tasks[t.new] = task_title(t.rest)
    for key, parts in info.ambiguous_keys.items():
        info.ambiguous[key] = {task_title_of_pkg(p.head): p.new for p in parts}
        for o in (key,):
            if o in p_map:
                raise OpError(f"пакет {o} расщеплён, но попал в карту")
    deleted_p, deleted_t = info.deleted_pkgs, info.deleted_tasks
    if set(p_map) & set(deleted_p) or set(p_map) & set(info.ambiguous):
        raise OpError("карта пакетов пересекается с удалёнными/неоднозначными")
    rest_p = set(old_pkgs) - set(p_map) - set(deleted_p) - set(info.ambiguous)
    if rest_p:
        raise OpError(f"пакеты без судьбы: {sorted(rest_p, key=C._key)}")
    rest_t = set(old_tasks) - set(t_map) - set(deleted_t)
    if rest_t or (set(t_map) | set(deleted_t)) - set(old_tasks):
        raise OpError(f"задачи без судьбы: {sorted(rest_t, key=C._key)}")
    pkg_of_task_old = {t: ".".join(t.split(".")[:2]) for t in old_tasks}
    for o, n in t_map.items():
        po = pkg_of_task_old[o]
        if p_map.get(po) != ".".join(n.split(".")[:2]):
            moved[o] = n
    return {
        "_about": "Четвёртая перенумерация (этап 2, 27.11.2026): перенос задач и пакетов, расщепление 5.4, слияние 5.9/5.10, новые задачи. "
                  "Генерирует restructure_cut3.py по backlog.md и restructure_plan_cut3.json; номера «старых» ключей — ТЕКУЩИЕ номера backlog.md.",
        "milestones": {str(m.num): m.num for m in model.mss},
        "milestone_names": {str(k): ms.name for k, ms in enumerate(model.mss, 1)},
        "packages": {k: p_map[k] for k in sorted(p_map, key=C._key)},
        "packages_merged": {k: merged[k] for k in sorted(merged, key=C._key)},
        "tasks": {k: t_map[k] for k in sorted(t_map, key=C._key)},
        "deleted_packages": {k: deleted_p[k] for k in sorted(deleted_p, key=C._key)},
        "deleted_tasks": {k: deleted_t[k] for k in sorted(deleted_t, key=C._key)},
        "new_tasks": {k: new_tasks[k] for k in sorted(new_tasks, key=C._key)},
        "moved_tasks": {k: moved[k] for k in sorted(moved, key=C._key)},
        "ambiguous_packages": {k: info.ambiguous[k] for k in sorted(info.ambiguous, key=C._key)},
    }


def task_title_of_pkg(head: str) -> str:
    m = C.STEPS_TAIL_RE.match(head)
    return (m.group(1) if m else head).strip()


# ---------------------------------------------------------------------------------------------
# Ремап текста (локальные щиты от ложных срабатываний)
# ---------------------------------------------------------------------------------------------
_LOCAL_SHIELD = re.compile(
    r"\bh[1-6]\.\s+\d+(?:\.\d+)+\.?"
    r"|(?i:openapi)[^\n\d]{0,12}\d+\.\d+\.\d+"
    r"|(?<![\d.])2\.3\.50(?![\d.])"
    r"|«5\.5»"
    r"|\bbacklog\s+0\.3\.2\.15"
)
# Известные ссылки, устаревшие ДО этого прохода (номера докатегории cut2): механическая замена дала бы неверный результат.
FILE_SHIELDS: dict[str, list[re.Pattern]] = {
    "docs/api/svt-api-v1-developers.md": [re.compile(r"backlog 4\.4\.8 и 4\.4\.9")],
}
_NUM_HEADING = re.compile(r"^(#{1,6}\s+\d+(?:\.\d+)+\.?)(?=\s|$)(.*)$")
_SNIP_W = 38


def _shield(text: str, rel: str) -> tuple[str, list[str]]:
    stash: list[str] = []

    def h(m):
        stash.append(m.group(0))
        return f"\ue000{len(stash) - 1}\ue001"

    s = _LOCAL_SHIELD.sub(h, text)
    for rx in FILE_SHIELDS.get(rel, ()):
        s = rx.sub(h, s)
    return s, stash


def _unshield(s: str, stash: list[str]) -> str:
    return re.sub(r"\ue000(\d+)\ue001", lambda m: stash[int(m.group(1))], s)


def _snip(text: str, tok: str, width: int = _SNIP_W) -> str:
    i = text.find(tok)
    if i < 0:
        i = 0
    return text[max(0, i - width): i + len(tok) + width].replace("\n", " ").strip()


def remap_text3(text: str, markers: list[str] | None = None, ctx: str = "", notes: list[str] | None = None) -> str:
    """Копия C.remap_text2 с единственным отличием: при слиянии пакетов/задач (op18) диапазон, у которого после
    переноса несколько старых номеров дали ОДИН новый, не раскрывается в «5.6, 5.6», а схлопывается («5.6»).
    Импортируемый renumber_cut2.py не менялся (по условию задачи — копия в новом файле)."""
    if not text:
        return text
    if not C.map_loaded():
        raise RuntimeError("карта cut3 не загружена (set_map)")
    S = C._S
    M, T, P = S["MS"], S["TASKS"], S["PKGS"]
    stash: list[str] = []

    def mark(tok: str) -> None:
        if markers is not None:
            markers.append(f"{ctx}: {tok}")

    def note(msg: str) -> None:
        if notes is not None:
            notes.append(f"{ctx}: {msg}")

    def hold(val: str) -> str:
        stash.append(val)
        return f"\u0000{len(stash) - 1}\u0000"

    s = C._SHIELD_EXTRA.sub(lambda m: hold(m.group(0)), text)
    s = RN._SHIELD.sub(lambda m: hold(m.group(0)), s)

    def wordref(m):
        w, a, sep, b = m.group(1), int(m.group(2)), m.group(3), m.group(4)
        if b is None:
            if a not in M:
                return m.group(0)
            return hold(f"{w} {M[a]}")
        b = int(b)
        if a > b or any(x not in M for x in range(a, b + 1)):
            return m.group(0)
        new = [M[x] for x in range(a, b + 1)]
        lo, hi = min(new), max(new)
        if lo == hi:
            w2 = w
            lw = w.lower()
            if lw in C._COLLAPSE:
                prev = re.search(r"([А-Яа-яЁё]+)\s*\(?$", s[: m.start()])
                keep_gen = lw == "вехи" and prev is not None and prev.group(1).lower() in C._ALLOWED_PREV_FOR_KEEP
                if not keep_gen:
                    w2 = C._cap_like(w, C._COLLAPSE[lw])
            note(f"«{m.group(0)}» → «{w2} {lo}» (слитые вехи схлопнуты)")
            return hold(f"{w2} {lo}")
        return hold(f"{w} {lo}\u2013{hi}")

    s = C._WORD.sub(wordref, s)

    def rng(m):
        a, b = m.group(1), m.group(2)
        if a not in T or b not in T:
            return m.group(0)
        ia, ib = S["T_OLD_I"][a], S["T_OLD_I"][b]
        if ia > ib:
            return m.group(0)
        news = [T[o] for o in S["T_OLD"][ia: ib + 1]]
        idx = sorted({S["T_NEW_I"][n] for n in news})
        lo, hi = S["T_NEW"][idx[0]], S["T_NEW"][idx[-1]]
        if idx == list(range(idx[0], idx[0] + len(idx))):
            return hold(lo if lo == hi else f"{lo}\u2013{hi}")
        note(f"диапазон «{m.group(0)}» стал непрерывным не в одном родителе — раскрыт в перечень ({len(idx)} пунктов)")
        return hold(", ".join(S["T_NEW"][i] for i in idx))

    s = RN._range.sub(rng, s)

    def one_task(m):
        tok = m.group(1)
        if tok in T:
            return hold(T[tok])
        mark(tok)
        return tok

    s = RN._task.sub(one_task, s)

    def prange(m):
        a, sep, b = m.groups()
        if a in P and b in P:
            ia, ib = S["P_OLD_I"][a], S["P_OLD_I"][b]
            if ia > ib:
                return m.group(0)
            news = [P[o] for o in S["P_OLD"][ia: ib + 1]]
            idx = sorted({S["P_NEW_I"][n] for n in news})
            lo, hi = S["P_NEW"][idx[0]], S["P_NEW"][idx[-1]]
            if idx == list(range(idx[0], idx[0] + len(idx))):
                return hold(lo if lo == hi else f"{lo}{sep}{hi}")
            note(f"диапазон пакетов «{m.group(0)}» стал непрерывным не в одном родителе — раскрыт в перечень ({len(idx)} пакетов)")
            return hold(", ".join(S["P_NEW"][i] for i in idx))
        return m.group(0)

    s = C._PKG_RANGE.sub(prange, s)

    def one_pkg(m):
        tok = m.group(1)
        if tok in P:
            return hold(P[tok])
        mark(tok)
        return tok

    s = C._PKG.sub(one_pkg, s)
    return re.sub(r"\u0000(\d+)\u0000", lambda m: stash[int(m.group(1))], s)


@dataclass
class Collector:
    unknown: list[tuple[str, int, str, str]] = field(default_factory=list)      # (файл, строка, токен, фрагмент)
    suspicious: list[tuple[str, int, str, str]] = field(default_factory=list)
    notes: list[str] = field(default_factory=list)
    decisions: list[str] = field(default_factory=list)
    manual: list[str] = field(default_factory=list)
    op9: list[tuple[str, int, str, int, str]] = field(default_factory=list)     # (файл, строка, где, число, фрагмент)
    headings_kept: int = 0


def remap_line(col: Collector, rel: str, no: int, text: str, where: str | None = None) -> str:
    if not text:
        return text
    s, stash = _shield(text, rel)
    mk: list[str] = []
    nt: list[str] = []
    out = remap_text3(s, mk, f"{rel}:{no}", nt)
    for x in mk:
        tok = x.rsplit(": ", 1)[1]
        col.unknown.append((rel, no, tok, _snip(text, tok)))
    col.notes.extend(nt)
    for kind, frag in C.scan_suspicious(s):
        col.suspicious.append((rel, no, kind, _unshield(frag, stash).replace("\n", " ").strip()))
    out = _unshield(out, stash)
    if OP9_SUFFIX in out:
        n = out.count(OP9_SUFFIX)
        w = where or ("heading" if re.match(r"^# Веха \d+ — ", out) else "other")
        col.op9.append((rel, no, w, n, _snip(out, OP9_SUFFIX)))
        out = out.replace(OP9_SUFFIX, "")
    return out


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


_HTML_SKIP = re.compile(r"(<style\b.*?</style\s*>|<script\b.*?</script\s*>|<!--.*?-->|<[^>]+>)", re.S | re.I)


def remap_html(col: Collector, rel: str, text: str) -> str:
    """Только текстовые узлы: теги, атрибуты, style/script и комментарии не трогаются."""
    parts = _HTML_SKIP.split(text)
    out, line = [], 1
    for i, p in enumerate(parts):
        if i % 2 == 0:
            out.append("\n".join(remap_line(col, rel, line + k, x) for k, x in enumerate(p.split("\n"))))
        else:
            out.append(p)
        line += p.count("\n")
    return "".join(out)


def cs_comment_spans(src: str) -> list[tuple[int, int]]:
    """Диапазоны текста комментариев C# (после // или /*): простой лексер, строки и символы пропускаются."""
    spans: list[tuple[int, int]] = []
    i, n = 0, len(src)
    while i < n:
        c = src[i]
        if c == "/" and i + 1 < n and src[i + 1] == "/":
            j = src.find("\n", i)
            j = n if j < 0 else j
            k = i + 2
            while k < j and src[k] == "/":
                k += 1
            spans.append((k, j))
            i = j
            continue
        if c == "/" and i + 1 < n and src[i + 1] == "*":
            j = src.find("*/", i + 2)
            spans.append((i + 2, n if j < 0 else j))
            i = n if j < 0 else j + 2
            continue
        if c in '$@"':
            j, verbatim = i, False
            while j < n and src[j] in "$@":
                verbatim = verbatim or src[j] == "@"
                j += 1
            if j < n and src[j] == '"':
                q = j
                while q < n and src[q] == '"':
                    q += 1
                if q - j >= 3 and not verbatim:
                    end = src.find('"' * (q - j), q)
                    i = n if end < 0 else end + (q - j)
                    continue
                k = j + 1
                if verbatim:
                    while k < n:
                        if src[k] == '"':
                            if k + 1 < n and src[k + 1] == '"':
                                k += 2
                                continue
                            break
                        k += 1
                else:
                    while k < n:
                        if src[k] == "\\":
                            k += 2
                            continue
                        if src[k] in '"\n':
                            break
                        k += 1
                i = k + 1
                continue
            i += 1
            continue
        if c == "'":
            k = i + 1
            while k < n and src[k] != "'":
                if src[k] == "\\":
                    k += 2
                    continue
                if src[k] == "\n":
                    break
                k += 1
            i = k + 1
            continue
        i += 1
    return spans


def remap_cs(col: Collector, rel: str, text: str) -> str:
    out, pos, last_a, line = [], 0, 0, 1
    for a, b in cs_comment_spans(text):
        out.append(text[pos:a])
        line += text.count("\n", last_a, a)
        last_a = a
        seg = text[a:b]
        out.append("\n".join(remap_line(col, rel, line + k, x, where="comment") for k, x in enumerate(seg.split("\n"))))
        pos = b
    out.append(text[pos:])
    return "".join(out)


def remap_file(col: Collector, rel: str, text: str) -> str:
    if rel.endswith(".cs"):
        return remap_cs(col, rel, text)
    if rel.endswith(".html"):
        return remap_html(col, rel, text)
    return remap_whole(col, rel, text)


# ---------------------------------------------------------------------------------------------
# backlog.md: ремап модели, правки, оценки
# ---------------------------------------------------------------------------------------------
def remap_model(col: Collector, model: Model, rel: str = BACKLOG_REL) -> None:
    """Один проход по всем СТАРЫМ текстам модели (новые строки с ln=None пропускаются)."""
    for l in model.preamble + model.tail:
        l.text = remap_line(col, rel, l.ln, l.text)
    for ms in model.mss:
        ms.name = remap_line(col, rel, ms.head_ln, ms.name)
        for l in ms.intro:
            l.text = remap_line(col, rel, l.ln, l.text)
        for p in ms.pkgs:
            if p.head_ln is not None:
                p.head = remap_line(col, rel, p.head_ln, p.head)
            for l in p.intro + p.crits:
                if not l.fresh:
                    l.text = remap_line(col, rel, l.ln, l.text)
            for t in p.tasks:
                if not t.fresh:
                    t.rest = remap_line(col, rel, t.ln, t.rest)


def apply_edits(model: Model) -> None:
    for ms in model.mss:
        for p in ms.pkgs:
            for f in p.head_edits:
                p.head = f(p.head)
            for t in p.tasks:
                for f in t.edits:
                    t.rest = f(t.rest)


def recompute_estimates(model: Model, decisions: list[str]) -> list[tuple[str, str, str, str, str]]:
    """Оценки пакетов и вех. Возвращает строки таблицы (ключ, название, было, стало, способ)."""
    rows = []
    for k, ms in enumerate(model.mss, 1):
        for j, p in enumerate(ms.pkgs, 1):
            sa = sb = Decimal(0)
            for t in p.tasks:
                a, b = triple(t.rest)
                sa += a
                sb += b
            if p.fresh or p.est_old is None:
                p.est_val = (sa, sb)
                p.est = f"**Оценка:** {fmt_num(sa)} / {fmt_num(sb)} / —"
                rows.append((p.new, task_title_of_pkg(p.head), "—", f"{fmt_num(sa)} / {fmt_num(sb)}", "новый: сумма троек"))
            else:
                da, db = sa - p.old_task_sum[0], sb - p.old_task_sum[1]
                p.est_val = (p.est_old[0] + da, p.est_old[1] + db)
                if da != 0 or db != 0:
                    p.est = f"**Оценка:** {fmt_num(p.est_val[0])} / {fmt_num(p.est_val[1])} / {p.est_third}"
                    rows.append((p.new, task_title_of_pkg(p.head), f"{fmt_num(p.est_old[0])} / {fmt_num(p.est_old[1])}",
                                 f"{fmt_num(p.est_val[0])} / {fmt_num(p.est_val[1])}", f"прежнее {'+' if da >= 0 else '−'}{fmt_num(abs(da))} / {'+' if db >= 0 else '−'}{fmt_num(abs(db))}"))
        a = sum((p.est_val[0] for p in ms.pkgs), Decimal(0))
        b = sum((p.est_val[1] for p in ms.pkgs), Decimal(0))
        ms.est_val = (a, b)
        if ms.est_old != (a, b):
            m = C.EST_RE.match(ms.est)
            if m.group(3) != "—":
                decisions.append(f"веха {ms.num}: третий слот «Оценки» не «—» — в новой строке сохранён «{m.group(3)}»")
            ms.est = f"**Оценка:** {fmt_num(a)} / {fmt_num(b)} / {m.group(3)}"
            rows.append((f"веха {k}", ms.name, f"{fmt_num(ms.est_old[0])} / {fmt_num(ms.est_old[1])}", f"{fmt_num(a)} / {fmt_num(b)}", "сумма пакетов"))
    return rows


# ---------------------------------------------------------------------------------------------
# roadmap.md
# ---------------------------------------------------------------------------------------------
_TABLE_HEAD = "| Веха | Название |"


def _cells(line: str) -> list[str]:
    return [c.strip() for c in line.strip().strip("|").split("|")]


def _pkg_range(k: int, n: int) -> str:
    if n == 1:
        return f"{k}.1"
    if n == 2:
        return f"{k}.1, {k}.2"
    return f"{k}.1–{k}.{n}"


def _years(d: Decimal) -> str:
    v = (d / Decimal(220)).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
    return "~" + f"{v:f}".replace(".", ",")


@dataclass
class RoadmapResult:
    text: str
    old_rows: dict[int, list[str]]
    new_rows: dict[int, list[str]]
    old_totals: tuple[Decimal, Decimal]
    new_totals: tuple[Decimal, Decimal]
    old_sum: tuple[Decimal, Decimal]
    new_sum: tuple[Decimal, Decimal]
    years_old_ok: bool


def build_roadmap(col: Collector, text: str, model: Model) -> RoadmapResult:
    lines = text.split("\n")
    hi = [i for i, l in enumerate(lines) if l.startswith(_TABLE_HEAD)]
    if len(hi) != 1:
        raise C.StructureError(f"roadmap.md: таблица «{_TABLE_HEAD}» найдена {len(hi)} раз(а)")
    h = hi[0]
    rows_i = [i for i in range(h + 2, len(lines)) if FILE_ROW_RE.match(lines[i])]
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
            raise C.StructureError(f"roadmap.md:{i + 1}: в строке {len(cs)} колонок, нужно 9")
        if "| " + " | ".join(cs) + " |" != lines[i]:
            raise C.StructureError(f"roadmap.md:{i + 1}: строка не восстанавливается из ячеек")
        k = int(cs[0].strip("*"))
        old_rows[k], row_line[k] = cs, i + 1
    if sorted(old_rows) != list(range(1, 17)):
        raise C.StructureError(f"roadmap.md: вехи в таблице: {sorted(old_rows)}")
    if len(model.mss) != 16:
        raise C.StructureError(f"вех в модели {len(model.mss)}, ожидалось 16")
    new_rows: dict[int, list[str]] = {}
    osa = osb = nsa = nsb = Decimal(0)
    for k, ms in enumerate(model.mss, 1):
        old = old_rows[k]
        osa += Decimal(old[3])
        osb += Decimal(old[4])
        a, b = ms.est_val
        nsa += a
        nsb += b
        name = ms.name
        old_name = old[1].replace(OP9_SUFFIX, "")
        if OP9_SUFFIX in old[1]:
            col.op9.append((ROADMAP_REL, row_line[k], "table", old[1].count(OP9_SUFFIX), old[1]))
        if k != 3 and old_name != name:
            col.decisions.append(f"roadmap: название вехи {k} в таблице «{old_name}» ≠ backlog «{name}» — оставлено из backlog")
        cells = [f"**{k}**", name, _pkg_range(k, len(ms.pkgs)), fmt_num(a), fmt_num(b), old[5]]
        for idx in (6, 7, 8):
            cells.append(remap_line(col, ROADMAP_REL, row_line[k], old[idx], where="table"))
        new_rows[k] = cells
        if k not in (3, 4, 5, 6, 10) and old[2] != cells[2]:
            col.decisions.append(f"roadmap: колонка «Пакеты» вехи {k}: было {old[2]!r}, по факту {cells[2]!r}")
    tot_old = _cells(lines[tot_i[0]])
    yr_old = _cells(lines[tot_i[1]])
    old_t = (Decimal(tot_old[3].strip("*")), Decimal(tot_old[4].strip("*")))
    years_ok = yr_old[3].strip("*") == _years(old_t[0]) and yr_old[4].strip("*") == _years(old_t[1])
    t_line = lines[tot_i[0]]
    for old_v, new_v in ((tot_old[3], f"**{fmt_num(nsa)}**"), (tot_old[4], f"**{fmt_num(nsb)}**")):
        if t_line.count(old_v) != 1:
            raise C.StructureError(f"roadmap.md: значение {old_v} в строке «Итого» найдено {t_line.count(old_v)} раз")
        t_line = t_line.replace(old_v, new_v)
    y_line = lines[tot_i[1]]
    for old_v, new_v in ((yr_old[3], f"**{_years(nsa)}**"), (yr_old[4], f"**{_years(nsb)}**")):
        if y_line.count(old_v) != 1:
            raise C.StructureError(f"roadmap.md: значение {old_v} в строке лет найдено {y_line.count(old_v)} раз")
        y_line = y_line.replace(old_v, new_v)
    before = [remap_line(col, ROADMAP_REL, i + 1, lines[i]) for i in range(0, h)]
    after = [remap_line(col, ROADMAP_REL, i + 1, lines[i]) for i in range(tot_i[-1] + 1, len(lines))]
    out = before + [lines[h], lines[h + 1]]
    out += ["| " + " | ".join(new_rows[k]) + " |" for k in sorted(new_rows)]
    out += [t_line, y_line]
    out += after
    return RoadmapResult("\n".join(out), old_rows, new_rows, old_t, (nsa, nsb), (osa, osb), (nsa, nsb), years_ok)


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


def est_report(bl: C.Backlog, tol: Decimal = Decimal("0.1")) -> dict:
    pk_bad, notask = {}, {}
    for ms in bl.mss:
        for p in ms.pkgs:
            ts, st = p.task_sum(), p.est()
            if st is None or abs(ts[0] - st[0]) > tol or abs(ts[1] - st[1]) > tol:
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


def order_violations(bl: C.Backlog) -> list[str]:
    """Пакеты, где после невыполненной задачи стоит выполненная (правило шапки backlog.md)."""
    out = []
    for ms in bl.mss:
        for p in ms.pkgs:
            st = [C.TASK_RE.match(p.lines[i][1]).group(1) for i in p.task_indexes()]
            seen_open = False
            for s in st:
                if s == " ":
                    seen_open = True
                elif seen_open:
                    out.append(p.key)
                    break
    return out


def _titles(bl: C.Backlog) -> Counter:
    c: Counter = Counter()
    for ms in bl.mss:
        for p in ms.pkgs:
            for i in p.task_indexes():
                rest = p.lines[i][1][C.TASK_RE.match(p.lines[i][1]).end():]
                c[task_title(rest)] += 1
    return c


def _pk_of(bl: C.Backlog) -> list[str]:
    return [p.key for m in bl.mss for p in m.pkgs]


@dataclass
class Computation:
    results: dict[str, bytes]
    srcs: dict[str, Src]
    scanned: list[str]
    new_texts: dict[str, str]
    old_texts: dict[str, str]
    col: Collector
    mapd: dict
    map_text: str
    old_text: str
    new_text: str
    bl_old: C.Backlog
    model: Model
    info: OpsInfo
    rr: RoadmapResult
    est_rows: list
    plan: dict
    fp: list[tuple[str, int, int, list[str]]]
    mixed_skipped: list[str]
    undecodable: list[str]
    layout_lines: list[str]
    layout_ok: bool
    roadmap_old_text: str


FALSE_POSITIVES = [
    ("раздел документа «### N.M»", re.compile(r"^#{2,6}\s+\d+(?:\.\d+)+\.?(?=\s|$)")),
    ("«§N.M»", re.compile(r"§\s?\d+(?:\.\d+)*")),
    ("«h4. N.N.N.»", re.compile(r"\bh[1-6]\.\s+\d+(?:\.\d+)+\.?")),
    ("версия «2.3.50»", re.compile(r"(?<![\d.])2\.3\.50(?![\d.])")),
    ("«openapi 3.0.1»", re.compile(r"(?i)openapi[^\n\d]{0,12}\d+\.\d+\.\d+")),
    ("число «5.5»", re.compile(r"«5\.5»")),
    ("«backlog 0.3.2.15»", re.compile(r"\bbacklog\s+0\.3\.2\.15")),
    ("«grok-4.7-high-fast»", re.compile(r"grok-4\.7-high-fast")),
    ("даты ДД.ММ.ГГГГ", re.compile(r"(?<![\d.])\d{1,2}\.\d{1,2}\.\d{4}(?![\d.])")),
]


def check_layout(mapd: dict, model: Model) -> tuple[bool, list[str]]:
    """Ожидаемая раскладка из постановки — проверяется вычислением по построенной карте."""
    P, T = mapd["packages"], mapd["tasks"]
    exp_p = {f"4.{i}": f"3.{i}" for i in range(1, 8)}
    exp_p.update({"4.8": "4.1", "4.9": "4.2", "4.10": "4.3", "3.2": "4.4", "3.1": "3.9"})
    exp_p.update({f"4.{i}": f"4.{i - 6}" for i in range(11, 16)})
    exp_p.update({"4.12": "4.6", "4.14": "4.8", "5.6": "5.4", "5.8": "5.5", "5.9": "5.6", "5.10": "5.6", "6.9": "6.9",
                  "6.11": "6.10", "6.25": "6.22", "10.4": "10.4", "10.5": "10.6"})
    exp_t = {f"5.4.{i}": f"3.8.{i}" for i in range(1, 8)}
    exp_t.update({"4.1.5": "4.6.6", "4.2.5": "4.6.7", "4.3.4": "4.6.8", "4.3.5": "4.6.9", "4.3.6": "4.6.10", "4.3.7": "4.6.11",
                  "5.4.13": "4.6.12", "4.12.6": "4.6.13", "6.23.1": "3.9.7", "3.1.6": "3.9.6", "3.1.7": "3.9.8", "5.5.1": "4.4.5",
                  "5.7.1": "5.5.1", "5.8.1": "5.5.2", "5.9.1": "5.6.1", "5.9.2": "5.6.2", "5.10.1": "5.6.3", "5.10.2": "5.6.4",
                  "6.10.1": "6.9.14", "5.4.8": "10.5.1", "5.4.17": "10.5.9"})
    bad = []
    for k, v in exp_p.items():
        if P.get(k) != v:
            bad.append(f"пакет {k}: карта {P.get(k)!r}, ожидалось {v}")
    for k, v in exp_t.items():
        if T.get(k) != v:
            bad.append(f"задача {k}: карта {T.get(k)!r}, ожидалось {v}")
    amb = mapd["ambiguous_packages"].get("5.4", {})
    if amb != {"MVP API": "3.8", "API для Атласа": "10.5"}:
        bad.append(f"неоднозначный 5.4: {amb}")
    if sorted(mapd["deleted_packages"], key=C._key) != sorted(["5.5", "5.7", "6.10", "6.17", "6.23"], key=C._key):
        bad.append(f"удалённые пакеты: {sorted(mapd['deleted_packages'])}")
    if sorted(mapd["deleted_tasks"]) != ["6.17.1"]:
        bad.append(f"удалённые задачи: {sorted(mapd['deleted_tasks'])}")
    counts = [len(m.pkgs) for m in model.mss]
    if counts[2:6] != [9, 9, 6, 23] or counts[9] != 6:
        bad.append(f"число пакетов вех 3, 4, 5, 6, 10: {counts[2]}, {counts[3]}, {counts[4]}, {counts[5]}, {counts[9]}; ожидалось 9, 9, 6, 23, 6")
    lines = [f"проверено позиций раскладки: {len(exp_p)} пакетов + {len(exp_t)} задач + 5.4 + удалённые; пакетов в вехах 3/4/5/6/10: {counts[2]}/{counts[3]}/{counts[4]}/{counts[5]}/{counts[9]}"]
    return (not bad), lines + bad


def run_checks(comp: Computation, bl_new: C.Backlog, determinism: tuple[bool, str]) -> list[CheckResult]:
    out: list[CheckResult] = []
    old_text, new_text, plan, mapd = comp.old_text, comp.new_text, comp.plan, comp.mapd
    bl_old = comp.bl_old
    # a) [SVT-…] в области правок
    ca, cb, ca_all, cb_all = Counter(), Counter(), Counter(), Counter()
    per_file = []
    for rel in comp.scanned:
        o, n = comp.old_texts[rel], comp.new_texts[rel]
        xa, xb = Counter(C.SVT_RE.findall(o)), Counter(C.SVT_RE.findall(n))
        ca.update(xa)
        cb.update(xb)
        ca_all.update(re.findall(r"SVT-\d+", o))
        cb_all.update(re.findall(r"SVT-\d+", n))
        if xa != xb:
            per_file.append(f"{rel}: {dict((xa - xb) + (xb - xa))}")
    d = (ca - cb) + (cb - ca)
    out.append(CheckResult("a) множество и кратности [SVT-…] до/после", ca == cb and ca_all == cb_all and not per_file,
                           f"файлов {len(comp.scanned)}; уникальных {len(ca)}, вхождений {sum(ca.values())}; все «SVT-N»: {sum(ca_all.values())} → {sum(cb_all.values())}"
                           + (f"; расхождения: {dict(d)}" if d else ""), per_file))
    # b) счёты (формулой)
    t_old, t_new = _count(r"^- \[[ x]\] \d+\.\d+\.\d+\. ", old_text), _count(r"^- \[[ x]\] \d+\.\d+\.\d+\. ", new_text)
    p_old, p_new = _count(r"^## \d+\.\d+\. ", old_text), _count(r"^## \d+\.\d+\. ", new_text)
    m_old, m_new = _count(r"^# Веха \d+ — ", old_text), _count(r"^# Веха \d+ — ", new_text)
    del_t = comp.info.stat_deleted_pkg_tasks
    add_t = plan["ops"][21]["new_tasks"]
    exp_t = t_old - del_t + add_t
    del_p = len([o for o in plan["ops"] if o["op"] == "delete"])
    merge_n = len(plan["ops"][17]["from"])
    exp_p = p_old - del_p - (merge_n - 1) + 1 + 0
    lines_b = [
        f"задач:  было {t_old}; − {del_t} (задачи удаляемого пакета 6.17: 6.17.1); + {add_t} (новые руководства) → ожидается {exp_t}; стало {t_new}",
        f"пакетов: было {p_old}; − {del_p} (удалены 5.5, 5.7, 6.10, 6.17, 6.23); − {merge_n - 1} (слияние {merge_n} → 1); + 1 («MVP API»); + 0 (расщепление: остаток = бывший 5.4) → ожидается {exp_p}; стало {p_new}",
        f"вех:    было {m_old}; стало {m_new}; ожидается 16",
    ]
    out.append(CheckResult("b) число задач/пакетов/вех (формулой)", t_new == exp_t and p_new == exp_p and m_new == 16 and m_old == 16,
                           f"задач {t_old}→{t_new} (ожид. {exp_t}); пакетов {p_old}→{p_new} (ожид. {exp_p}); вех {m_old}→{m_new}", lines_b))
    # c) суммы
    eo, en = est_report(bl_old), est_report(bl_new)
    lines = [f"исходный backlog: пакетов с расхождением сумма задач ≠ «Оценка» (>0.1): {len(eo['pk_bad'])}; вех с расхождением: {len(eo['ms_bad'])}"]
    inv: dict[str, list[str]] = {}
    for o, n in mapd["packages"].items():
        inv.setdefault(n, []).append(o)
    unexpl_p = [k for k in en["pk_bad"] if not any(o in eo["pk_bad"] for o in inv.get(k, []))]
    unexpl_m = [k for k in en["ms_bad"] if k not in eo["ms_bad"]]
    for k in unexpl_p:
        ts, st = en["pk_bad"][k]
        lines.append(f"  [ОШИБКА] новый пакет {k}: сумма задач {fmt_num(ts[0])} / {fmt_num(ts[1])}; «Оценка» {st and (fmt_num(st[0]) + ' / ' + fmt_num(st[1]))}")
    for k in unexpl_m:
        sp, st = en["ms_bad"][k]
        lines.append(f"  [ОШИБКА] новая веха {k}: пакеты {fmt_num(sp[0])} / {fmt_num(sp[1])}; «Оценка» вехи {fmt_num(st[0])} / {fmt_num(st[1])}")
    inherited_p = [k for k in en["pk_bad"] if k not in unexpl_p]
    if inherited_p:
        lines.append(f"  унаследованные расхождения пакетов (как в исходном): {inherited_p}")
    rr = comp.rr
    old_a = sum((m.est[0] for m in bl_old.mss), Decimal(0))
    old_b = sum((m.est[1] for m in bl_old.mss), Decimal(0))
    new_a = sum((m.est[0] for m in bl_new.mss), Decimal(0))
    new_b = sum((m.est[1] for m in bl_new.mss), Decimal(0))
    tol = Decimal("0.1")
    ops = {o["id"]: o for o in plan["ops"]}
    chg = plan["estimates"]["changed"][0]
    f0 = [Decimal(x.strip()) for x in chg["from"].split("/")]
    t1 = [Decimal(x) for x in re.findall(r"\d+(?:\.\d+)?", ops[24]["edit"]["triple"])[:2]]
    d_623 = (t1[0] - f0[0], t1[1] - f0[1])
    old617 = next(p for m in bl_old.mss for p in m.pkgs if p.key == "6.17")
    d_617 = (-old617.est()[0], -old617.est()[1])
    # новые задачи: каждая тройка реестра × число целевых пакетов
    d_new = (sum((Decimal(re.findall(r"\((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) /", s)[0][0]) for s in ops[22]["tasks"]), Decimal(0)) * len(ops[22]["targets"]),
             sum((Decimal(re.findall(r"\((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) /", s)[0][1]) for s in ops[22]["tasks"]), Decimal(0)) * len(ops[22]["targets"]))
    exp_d = (d_623[0] + d_617[0] + d_new[0], d_623[1] + d_617[1] + d_new[1])
    act_d = (new_a - old_a, new_b - old_b)
    reg_d = (Decimal(28), Decimal(14))
    lines.append(f"дельта итога: факт {fmt_num(act_d[0])} / {fmt_num(act_d[1])}; из компонентов (6.23.1 {fmt_num(d_623[0])}/{fmt_num(d_623[1])}, 6.17 {fmt_num(d_617[0])}/{fmt_num(d_617[1])}, "
                 f"руководства +{fmt_num(d_new[0])}/+{fmt_num(d_new[1])}) {fmt_num(exp_d[0])} / {fmt_num(exp_d[1])}; по реестру +28 / +14")
    delta_ok = (abs(act_d[0] - exp_d[0]) <= tol and abs(act_d[1] - exp_d[1]) <= tol and abs(act_d[0] - reg_d[0]) <= tol and abs(act_d[1] - reg_d[1]) <= tol)
    if not delta_ok:
        lines.append("  [РАСХОЖДЕНИЕ] фактическая дельта не равна ожидаемой — не подгонялось")
    old_tot_ok = abs(old_a - rr.old_totals[0]) <= tol and abs(old_b - rr.old_totals[1]) <= tol
    lines.append(f"итог backlog до: {fmt_num(old_a)} / {fmt_num(old_b)}; итог roadmap до: {fmt_num(rr.old_totals[0])} / {fmt_num(rr.old_totals[1])}; совпадают: {old_tot_ok}")
    lines.append(f"итог backlog после: {fmt_num(new_a)} / {fmt_num(new_b)}; итог roadmap после: {fmt_num(rr.new_totals[0])} / {fmt_num(rr.new_totals[1])}")
    rm_ok = (abs(rr.new_sum[0] - new_a) <= tol and abs(rr.new_sum[1] - new_b) <= tol
             and abs(rr.new_totals[0] - new_a) <= tol and abs(rr.new_totals[1] - new_b) <= tol)
    rows_ok = True
    for m in bl_new.mss:
        cs = rr.new_rows[m.num]
        if abs(Decimal(cs[3]) - m.est[0]) > tol or abs(Decimal(cs[4]) - m.est[1]) > tol:
            rows_ok = False
            lines.append(f"  [ОШИБКА] roadmap веха {m.num}: {cs[3]} / {cs[4]} ≠ backlog {fmt_num(m.est[0])} / {fmt_num(m.est[1])}")
    for o, cs in rr.old_rows.items():
        bm = next(m for m in bl_old.mss if m.num == o)
        if abs(Decimal(cs[3]) - bm.est[0]) > tol or abs(Decimal(cs[4]) - bm.est[1]) > tol:
            lines.append(f"  [было] веха {o}: roadmap {cs[3]} / {cs[4]} ≠ backlog {fmt_num(bm.est[0])} / {fmt_num(bm.est[1])}")
    if not rr.years_old_ok:
        lines.append("  [было] строка «чел.-годы» не воспроизводится формулой итог/220 — формат лет оставлен по формуле")
    if eo["notask"] or en["notask"]:
        lines.append(f"  задачи без тройки: было {sum(len(v) for v in eo['notask'].values())}, стало {sum(len(v) for v in en['notask'].values())}")
    lines += [f"  {r[0]} «{r[1]}»: {r[2]} → {r[3]} ({r[4]})" for r in comp.est_rows]
    out.append(CheckResult("c) суммы троек/пакетов/вех/roadmap и дельта итога",
                           not unexpl_p and not unexpl_m and delta_ok and rm_ok and rows_ok and old_tot_ok,
                           f"неожиданных расхождений пакетов {len(unexpl_p)}, вех {len(unexpl_m)}; дельта факт {fmt_num(act_d[0])}/{fmt_num(act_d[1])} "
                           f"vs ожид. +28/+14: {delta_ok}; roadmap сходится: {rm_ok and rows_ok}", lines))
    # d) метки DEL
    bad_marks = [rel for rel, t in comp.new_texts.items() if "\u27e6DEL" in t or "\u0000" in t or "\ue000" in t or "\ue001" in t]
    out.append(CheckResult("d) нет меток ⟦DEL и служебных символов", not bad_marks, f"найдено в: {bad_marks}" if bad_marks else "0 вхождений"))
    # e) структура
    errs: list[str] = []
    if [m.num for m in bl_new.mss] != list(range(1, 17)):
        errs.append(f"вехи: {[m.num for m in bl_new.mss]}")
    for m in bl_new.mss:
        if re.search(r"\((Шаг|Шаги)\b", m.head[1]):
            errs.append(f"веха {m.num}: в заголовке подпись шагов")
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
    if C.render_parsed(bl_new) != new_text:
        errs.append("повторный разбор нового backlog.md не восстанавливает текст")
    if render_model(parse_model(new_text)) != new_text:
        errs.append("модель нового backlog.md без операций не воспроизводит текст")
    out.append(CheckResult("e) структура нового backlog.md", not errs, "16 вех, пакеты и задачи подряд, разбор восстанавливает текст" if not errs else "; ".join(errs[:10]), errs))
    # f) детерминизм
    out.append(CheckResult("f) двойной прогон даёт тот же результат", determinism[0], determinism[1]))
    # g) область правок
    bad = [r for r in list(comp.results) if not in_scope(r)] + [r for r in comp.scanned if not in_scope(r)]
    out.append(CheckResult("g) изменённые файлы только в области правок", not bad,
                           f"к записи {len(comp.results)} файлов из {len(comp.scanned)} просмотренных; вне области: {bad}" if bad
                           else f"к записи {len(comp.results)} файлов из {len(comp.scanned)} просмотренных, все в области"))
    # h) выполненные задачи первыми
    vo, vn = order_violations(bl_old), order_violations(bl_new)
    new_viol = [k for k in vn if not any(o in vo for o in inv.get(k, []))]
    inh = [k for k in vn if k not in new_viol]
    out.append(CheckResult("h) в пакетах [x] стоят первыми", not new_viol,
                           f"нарушений в исходном: {len(vo)} {vo}; в новом: {len(vn)}; новых (ошибка): {new_viol}; наследуемых: {inh}"))
    # i) ни одна задача не потеряна и не продублирована
    to, tn_ = _titles(bl_old), _titles(bl_new)
    exp = Counter()
    ren = {frm: to_ for frm, to_ in comp.info.renames}
    for title, n in to.items():
        # переименования реестра: заменяются по префиксу один раз
        hit = [f for f in ren if title.startswith(f)]
        key = title
        if hit:
            f = max(hit, key=len)
            key = ren[f] + title[len(f):]
        exp[key] += n
    for t in comp.info.removed_task_titles:
        exp[t] -= 1
    for t in comp.info.task_adds:
        exp[t] += 1
    exp = +exp
    # заголовки с цифрами (ремап) — сравниваем после ремапа старых заголовков
    def rm(t: str) -> str:
        s, st = _shield(t, "")
        return _unshield(remap_text3(s), st)
    exp2 = Counter()
    for k, v in exp.items():
        exp2[rm(k)] += v
    diff_missing, diff_extra = exp2 - tn_, tn_ - exp2
    out.append(CheckResult("i) множество названий задач (до/после, с переименованиями реестра и 18 новыми)",
                           not diff_missing and not diff_extra,
                           f"названий до {sum(to.values())}, ожидается {sum(exp2.values())}, после {sum(tn_.values())}",
                           [f"потеряно/не хватает: {dict(diff_missing)}", f"лишнее: {dict(diff_extra)}"] if (diff_missing or diff_extra) else []))
    # j) раскладка и полнота карты
    out.append(CheckResult("j) раскладка из постановки (вычислением) и полнота карты", comp.layout_ok,
                           f"карта: пакетов {len(mapd['packages'])}, задач {len(mapd['tasks'])}; удалено пакетов {len(mapd['deleted_packages'])}, задач {len(mapd['deleted_tasks'])}; новых задач {len(mapd['new_tasks'])}",
                           comp.layout_lines))
    return out


# ---------------------------------------------------------------------------------------------
# Основной расчёт (чистая функция от содержимого файлов)
# ---------------------------------------------------------------------------------------------
def compute(root: Path) -> Computation:
    plan = load_plan()
    col = Collector()
    col.decisions.extend(STATIC_DECISIONS)
    bsrc = read_src(root, BACKLOG_REL)
    bl_old = C.parse_backlog(bsrc.text)
    if C.render_parsed(bl_old) != bsrc.text:
        raise RuntimeError("разбор backlog.md не воспроизводит исходный текст (обратная сборка расходится)")
    base = parse_model(bsrc.text)
    if render_model(base) != bsrc.text:
        raise RuntimeError("модель backlog.md без операций не воспроизводит исходный текст")
    old_pkgs = [p.olds[0] for m in base.mss for p in m.pkgs]
    old_tasks = [t.old for m in base.mss for p in m.pkgs for t in p.tasks]
    model = parse_model(bsrc.text)
    info = apply_ops(model, plan)                                     # (2) структура
    mapd = number_model(model, old_pkgs, old_tasks, info)
    layout_ok, layout_lines = check_layout(mapd, model)
    C.set_map(mapd)                                                   # подмена карты cut2 своей — до любого remap_text2
    map_text = C.dump_map(mapd)
    remap_model(col, model)                                           # (3) один проход по старым текстам
    apply_edits(model)                                                # (4) переименования и новые тексты
    est_rows = recompute_estimates(model, col.decisions)
    new_text = render_model(model)                                    # (5)
    col.decisions.extend(info.decisions)
    rsrc = read_src(root, ROADMAP_REL)
    rr = build_roadmap(col, rsrc.text, model)
    srcs: dict[str, Src] = {BACKLOG_REL: bsrc, ROADMAP_REL: rsrc}
    results: dict[str, bytes] = {}
    new_texts: dict[str, str] = {BACKLOG_REL: new_text, ROADMAP_REL: rr.text}
    old_texts: dict[str, str] = {BACKLOG_REL: bsrc.text, ROADMAP_REL: rsrc.text}
    for rel in (BACKLOG_REL, ROADMAP_REL):
        b = encode_like(srcs[rel], new_texts[rel])
        if b != srcs[rel].raw:
            results[rel] = b
    scanned = [BACKLOG_REL, ROADMAP_REL]
    mixed_skipped: list[str] = []
    undecodable: list[str] = []
    fp: list[tuple[str, int, int, list[str]]] = []
    fp_tot = {name: [0, 0, []] for name, _ in FALSE_POSITIVES}
    for rel in enumerate_scope(root):
        if rel in (BACKLOG_REL, ROADMAP_REL):
            continue
        try:
            s = read_src(root, rel, strict=False)
        except UnicodeDecodeError:
            undecodable.append(rel)
            continue
        srcs[rel] = s
        scanned.append(rel)
        nt = remap_file(col, rel, s.text)
        old_texts[rel], new_texts[rel] = s.text, nt
        if nt != s.text:
            if s.mixed:
                raise RuntimeError(f"{rel}: смешанные окончания строк, а файл меняется — остановка")
            results[rel] = encode_like(s, nt)
        elif s.mixed:
            mixed_skipped.append(rel)
        ol, nl = s.text.split("\n"), nt.split("\n")
        if len(ol) != len(nl):
            raise RuntimeError(f"{rel}: число строк изменилось после ремапа ({len(ol)} → {len(nl)})")
        for name, rx in FALSE_POSITIVES:
            for i, (a, b) in enumerate(zip(ol, nl)):
                for m in rx.finditer(a):
                    fp_tot[name][0] += 1
                    if m.group(0) in b:
                        fp_tot[name][1] += 1
                    else:
                        fp_tot[name][2].append(f"{rel}:{i + 1}: {m.group(0)}")
    fp = [(name, v[0], v[1], v[2]) for name, v in fp_tot.items()]
    # ручные места из реестра и найденные скриптом
    for r in plan["text_rewrites"]:
        col.manual.append(f"[реестр] {r['where']} — {r['status']}")
    for rel in sorted(FILE_SHIELDS):
        if rel in old_texts:
            for i, t in enumerate(old_texts[rel].split("\n"), 1):
                for rx in FILE_SHIELDS[rel]:
                    if rx.search(t):
                        t5, t6 = mapd["tasks"].get("5.4.8"), mapd["tasks"].get("5.4.9")
                        col.manual.append(
                            f"[скрипт] {rel}:{i}: «{rx.search(t).group(0)}» — ссылка устарела ДО этого прохода (в текущем бэклоге 4.4.8–4.4.9 — тесты шаблона RO-фреймворка, "
                            f"а по смыслу «блокирует выход на прод» — 5.4.8/5.4.9 → в новой нумерации {t5}/{t6}); не заменялось, решение за владельцем")
    return Computation(results, srcs, scanned, new_texts, old_texts, col, mapd, map_text, bsrc.text, new_text, bl_old, model, info, rr,
                       est_rows, plan, fp, mixed_skipped, undecodable, layout_lines, layout_ok, rsrc.text)


def digest(results: dict[str, bytes], map_text: str) -> str:
    h = hashlib.sha256()
    for rel in sorted(results):
        h.update(rel.encode("utf-8") + b"\0" + results[rel] + b"\0")
    h.update(map_text.encode("utf-8"))
    return h.hexdigest()


def changed_lines(src: Src, new_bytes: bytes) -> tuple[int, int]:
    nb = new_bytes[3:] if src.bom else new_bytes
    nt = nb.decode("utf-8").replace("\r\n", "\n")
    add = rem = 0
    for l in difflib.unified_diff(src.text.split("\n"), nt.split("\n"), n=0, lineterm=""):
        if l.startswith("+") and not l.startswith("+++"):
            add += 1
        elif l.startswith("-") and not l.startswith("---"):
            rem += 1
    return add, rem


# ---------------------------------------------------------------------------------------------
# Отчёт
# ---------------------------------------------------------------------------------------------
LEFTOVER_PATTERNS = [
    ("(4.1–4.8)", r"\(4\.1\s*[–-]\s*4\.8\)"),
    ("4.1–4.7", r"(?<![\d.])4\.1\s*[–-]\s*4\.7(?![\d]|\.\d)"),
    ("4.5–4.7", r"(?<![\d.])4\.5\s*[–-]\s*4\.7(?![\d]|\.\d)"),
    ("6.3–6.10", r"(?<![\d.])6\.3\s*[–-]\s*6\.10(?![\d]|\.\d)"),
    ("6.11–6.17", r"(?<![\d.])6\.11\s*[–-]\s*6\.17(?![\d]|\.\d)"),
    ("вехи 4–9", r"вехи 4\s*[–-]\s*9(?!\d)"),
    ("вехи 6–13", r"вехи 6\s*[–-]\s*13(?!\d)"),
    ("веха 3", r"[Вв]еха 3(?!\d)"),
    ("Среды", r"Среды"),
    ("5.4", r"(?<![\d.])5\.4(?![\d]|\.\d)"),
    ("6.10", r"(?<![\d.])6\.10(?![\d]|\.\d)"),
    ("6.17", r"(?<![\d.])6\.17(?![\d]|\.\d)"),
    ("6.23", r"(?<![\d.])6\.23(?![\d]|\.\d)"),
    ("5.5", r"(?<![\d.])5\.5(?![\d]|\.\d)"),
    ("5.7", r"(?<![\d.])5\.7(?![\d]|\.\d)"),
]
MANUAL_PATTERNS = [
    ("старая «веха 3 = Среды»", r"[Вв]еха 3\b.{0,6}Среды|Среды подготовлены|веха 3 «Среды»"),
    ("«Порядок вех» / «Принцип порядка» / «Критический путь»", r"Порядок вех|Принцип порядка|Критический путь"),
    ("пункт 2.5.1 (закрыт, op4)", r"(?<![\d.])2\.5\.1(?![\d]|\.\d)"),
    ("«Без шага» (список; 6.17 уходит)", r"Без шага"),
    ("D-ADM / D-SEC-SCA (В2)", r"D-ADM|D-SEC-SCA"),
    ("API Атласа 5.4 «входит в MVP заявок»", r"Пилот read-only API для Атласа|API выложен на прод"),
]


def _grep_new(texts: dict[str, str], patterns: list[tuple[str, str]]) -> dict[str, list[str]]:
    out: dict[str, list[str]] = {name: [] for name, _ in patterns}
    for name, rx in patterns:
        r = re.compile(rx)
        for rel in sorted(texts):
            for i, t in enumerate(texts[rel].split("\n"), 1):
                m = r.search(t)
                if m:
                    out[name].append(f"{rel}:{i}: …{_snip(t, m.group(0), 30)}…")
    return out


def _nonshielded_text_scan(texts: dict[str, str]) -> dict[str, list[str]]:
    return _grep_new({k: v for k, v in texts.items() if not k.endswith(".cs")}, LEFTOVER_PATTERNS)


def build_report(comp: Computation, checks: list[CheckResult], dig: str, full: bool) -> list[str]:
    col, mapd = comp.col, comp.mapd
    r: list[str] = []
    r.append("== 1. Свод ==")
    r.append(f"  просмотрено файлов области правок: {len(comp.scanned)}; будет изменено: {len(comp.results)}; "
             f"не декодируются как UTF-8 (пропущены): {len(comp.undecodable)} {comp.undecodable}; смешанные окончания, файл не менялся: {len(comp.mixed_skipped)} {comp.mixed_skipped}")
    r.append("  замены по файлам (строк добавлено / удалено):")
    for rel in sorted(comp.results):
        add, rem = changed_lines(comp.srcs[rel], comp.results[rel])
        r.append(f"    {rel}: +{add} / -{rem}")
    r.append("  ложные срабатывания из инвентаря (старые строки → в новом файле фрагмент на месте):")
    for name, total, same, bad in comp.fp:
        r.append(f"    {name}: найдено {total}, не изменено {same}, изменено {total - same}" + (f" — {bad[:5]}" if bad else ""))
    r.append(f"  номер в начале нумерованного заголовка («### 4.1 …») не менялся — это номер раздела документа: {col.headings_kept} строк")
    r.append("")
    r.append("== 2. Проверки ==")
    for c in checks:
        r.append(f"  {'ok  ' if c.ok else 'FAIL'} {c.name}: {c.detail}")
        r.extend("      " + x for x in c.lines)
    r.append(f"  sha256 предпросмотра: {dig}")
    r.append("")
    r.append("== 3. Старая веха/пакет → новая ==")
    r.append("  вехи: номера не меняются (1–16 → 1–16); веха 3: «Среды подготовлены» → «Proof of concept и прототип»; суффикс «: MVP, ОПЭ и ПЭ» снят у вех "
             + ", ".join(str(n) for n, _ in comp.info.op9_names))
    same = 0
    names_new = {p.new: task_title_of_pkg(p.head) for m in comp.model.mss for p in m.pkgs}
    for o, n in mapd["packages"].items():
        if o == n:
            same += 1
            continue
        r.append(f"  пакет {o} → {n}  «{names_new[n]}»")
    r.append(f"  пакетов без смены номера: {same}")
    for k, v in mapd["ambiguous_packages"].items():
        r.append(f"  пакет {k} → НЕОДНОЗНАЧНО (расщеплён): " + "; ".join(f"«{a}» → {b}" for a, b in v.items()))
    for k, v in mapd["deleted_packages"].items():
        r.append(f"  пакет {k} → удалён: {v}")
    for k, v in mapd["deleted_tasks"].items():
        r.append(f"  задача {k} → удалена: {v}")
    r.append(f"  перенесённых задач (не вместе со своим пакетом): {len(mapd['moved_tasks'])}")
    for o, n in mapd["moved_tasks"].items():
        r.append(f"    {o} → {n}")
    r.append(f"  новые задачи: {len(mapd['new_tasks'])}: " + ", ".join(mapd["new_tasks"]))
    r.append("")
    resolvable = {"5.4": "расщеплён (неоднозначно)", "5.5": "пакет удалён", "5.7": "пакет удалён", "6.10": "пакет удалён",
                  "6.17": "пакет удалён", "6.23": "пакет удалён", "6.17.1": "задача удалена"}
    imp = [u for u in col.unknown if u[2] in resolvable]
    other = [u for u in col.unknown if u[2] not in resolvable]
    r.append(f"== 4. Markers (токены, которых нет в карте; в тексте НЕ заменены): всего {len(col.unknown)}; ручное решение (удалённые/расщеплённые) {len(imp)}; прочие {len(other)} ==")
    r.append(f"  -- ручное решение (файл:строка[токен]: фрагмент; строки backlog.md/roadmap.md — по СТАРОМУ файлу) --")
    for rel, no, tok, frag in imp:
        r.append(f"  {rel}:{no}: [{tok} — {resolvable[tok]}] …{frag}…")
    r.append("  -- прочие (даты без года, версии, номера не из бэклога и т.п.) --")
    for rel, no, tok, frag in (other if full else other[:200]):
        r.append(f"  {rel}:{no}: [{tok}] …{frag}…")
    if not full and len(other) > 200:
        r.append(f"  … ещё {len(other) - 200}")
    coll = [u for u in other if u[2] in mapd["new_tasks"]]
    r.append(f"  -- коллизии: токен не из текущего бэклога (устаревшая ссылка), но после прохода совпадёт с id НОВОЙ задачи: {len(coll)} --")
    for rel, no, tok, frag in coll:
        r.append(f"  {rel}:{no}: [{tok}] → теперь «{mapd['new_tasks'][tok]}» …{frag}…")
    r.append("")
    r.append(f"== 5. Подозрительные места (scan_suspicious, после ремапа остались как есть): {len(col.suspicious)} ==")
    for rel, no, kind, frag in col.suspicious:
        r.append(f"  {rel}:{no}: ({kind}) …{frag}…")
    r.append("  -- поиск по паттернам старых голых диапазонов в НОВЫХ текстах (файл:строка по новому файлу; .cs не сканировались) --")
    left = _nonshielded_text_scan(comp.new_texts)
    for name, _ in LEFTOVER_PATTERNS:
        hits = left[name]
        r.append(f"  [{name}]: {len(hits)}")
        for h in (hits if full else hits[:40]):
            r.append(f"    {h}")
        if not full and len(hits) > 40:
            r.append(f"    … ещё {len(hits) - 40}")
    r.append("")
    r.append("== 6. Ручные места ==")
    r.extend("  " + x for x in col.manual)
    found = _grep_new({k: v for k, v in comp.new_texts.items() if k in (BACKLOG_REL, ROADMAP_REL) or k.endswith(".md") or k.endswith(".mdc")}, MANUAL_PATTERNS)
    for name, _ in MANUAL_PATTERNS:
        r.append(f"  [найдено скриптом] {name}: {len(found[name])}")
        for h in found[name][:60]:
            r.append(f"    {h}")
    r.append("  [найдено скриптом] roadmap.md, колонки «Результат/Работы/Шаги» не переписывались (ручная правка); строка вехи 3 сохранила прежние тексты колонок")
    r.append("  [найдено скриптом] roadmap.md, строка 12: перечень карт перенумерации — упоминание renumber_map_cut3.json не добавлено (ручная правка)")
    r.append("")
    r.append("== 7. Суффикс «: MVP, ОПЭ и ПЭ» (op9) ==")
    heads = [x for x in col.op9 if x[2] == "heading"]
    tab = [x for x in col.op9 if x[2] == "table"]
    oth = [x for x in col.op9 if x[2] not in ("heading", "table")]
    r.append(f"  заголовки вех (backlog.md): {len(comp.info.op9_names)}; строки таблицы roadmap.md: {len(tab)}; вне заголовков и таблицы: {len(oth)}")
    for rel, no, w, n, frag in oth:
        r.append(f"    {rel}:{no} ×{n} ({w}): …{frag}…")
    r.append("")
    r.append("== 8. Изменённые файлы (имя, число строк с заменами) ==")
    for rel in sorted(comp.results):
        add, _ = changed_lines(comp.srcs[rel], comp.results[rel])
        r.append(f"  {rel}: {add}")
    r.append(f"  всего: {len(comp.results)}")
    r.append("")
    r.append(f"== Заметки remap (схлопнутые диапазоны, раскрытые перечни): {len(col.notes)} ==")
    r.extend("  " + n for n in col.notes)
    r.append("")
    r.append(f"== Решения скрипта: {len(col.decisions)} ==")
    r.extend("  " + x for x in col.decisions)
    return r


# ---------------------------------------------------------------------------------------------
# Снимки
# ---------------------------------------------------------------------------------------------
def snapshot_report(root: Path) -> tuple[list[str], int]:
    plan = load_plan()
    base = parse_model((root / BACKLOG_REL).read_bytes().decode("utf-8-sig").replace("\r\n", "\n"))
    old_pkgs = [p.olds[0] for m in base.mss for p in m.pkgs]
    old_tasks = [t.old for m in base.mss for p in m.pkgs for t in p.tasks]
    model = parse_model((root / BACKLOG_REL).read_bytes().decode("utf-8-sig").replace("\r\n", "\n"))
    info = apply_ops(model, plan)
    mapd = number_model(model, old_pkgs, old_tasks, info)
    changed_t = {k for k, v in mapd["tasks"].items() if k != v} | set(mapd["deleted_tasks"])
    changed_p = {k for k, v in mapd["packages"].items() if k != v} | set(mapd["deleted_packages"]) | set(mapd["ambiguous_packages"])
    out: list[str] = []
    files = 0
    for d in SNAPSHOT_DIRS:
        base_dir = root / d
        if not base_dir.exists():
            continue
        for p in sorted(base_dir.rglob("*")):
            if not p.is_file() or p.suffix.lower() not in SNAPSHOT_EXT or set(p.relative_to(root).parts) & EXCLUDED_PARTS:
                continue
            try:
                text = p.read_bytes().decode("utf-8-sig")
            except UnicodeDecodeError:
                continue
            files += 1
            rel = p.relative_to(root).as_posix()
            for i, line in enumerate(text.replace("\r\n", "\n").replace("\r", "\n").split("\n"), 1):
                s, _ = _shield(line, rel)
                s = RN._SHIELD.sub(lambda m: "·" * len(m.group(0)), C._SHIELD_EXTRA.sub(lambda m: "·" * len(m.group(0)), s))
                hit = None
                for m in RN._task.finditer(s):
                    if m.group(1) in changed_t:
                        hit = m
                        break
                if hit is None:
                    for m in C._PKG.finditer(RN._task.sub(lambda m: "·" * len(m.group(0)), s)):
                        if m.group(1) in changed_p:
                            hit = m
                            break
                if hit is not None:
                    a = max(0, hit.start() - 45)
                    out.append(f"{rel}:{i}: {line[a:a + 100].strip()}")
    return out, files


# ---------------------------------------------------------------------------------------------
# Запись, apply
# ---------------------------------------------------------------------------------------------
def write_preview(preview: Path, comp: Computation, report: list[str]) -> None:
    if preview.exists():
        if preview.name != PREVIEW_NAME or preview.resolve().parent != HERE:
            raise RuntimeError(f"предпросмотр {preview} вне ожидаемой папки — не очищаю")
        shutil.rmtree(preview)
    preview.mkdir(parents=True)
    for rel, b in comp.results.items():
        dst = preview / PurePosixPath(rel)
        dst.parent.mkdir(parents=True, exist_ok=True)
        dst.write_bytes(b)
    (preview / "_report.txt").write_text("\n".join(report) + "\n", encoding="utf-8")


def git_dirty(root: Path, rels: list[str]) -> list[str]:
    dirty: list[str] = []
    for i in range(0, len(rels), 100):
        chunk = rels[i: i + 100]
        p = subprocess.run(["git", "-C", str(root), "status", "--porcelain", "--"] + chunk, capture_output=True, text=True, encoding="utf-8")
        if p.returncode != 0:
            raise RuntimeError(f"git status не выполнен: {p.stderr.strip()}")
        dirty += [l for l in p.stdout.splitlines() if l.strip()]
    return dirty


# ---------------------------------------------------------------------------------------------
# Selftest
# ---------------------------------------------------------------------------------------------
MINI = """# Мини-бэклог

---

# Веха 1 — Альфа

**Оценка:** 3 / 2 / —

## 1.1. Первый (Шаг: Разработка)

**Оценка:** 3 / 2 / —

- [x] 1.1.1. Задача А (1 / 1 / —)
- [ ] 1.1.2. Задача Б (2 / 1 / —)

**Критерий:** Первый готов.

---

# Веха 2 — Бета: MVP, ОПЭ и ПЭ

**Оценка:** 13 / 6.5 / —

## 2.1. Второй (Шаг: Разработка)

**Оценка:** 6 / 3 / —

- [ ] 2.1.1. В1 (1 / 0.5 / —)
- [ ] 2.1.2. В2 (2 / 1 / —) — см. 2.1.3 и 2.3.1
- [ ] 2.1.3. В3 (3 / 1.5 / —)

**Критерий:** Второй готов.

## 2.2. Третий (Шаг: Разработка)

**Оценка:** 4 / 2 / —

- [ ] 2.2.1. Г1 (4 / 2 / —)

**Критерий:** Третий готов.

## 2.3. Четвёртый (Шаг: Разработка)

**Оценка:** 2 / 1 / —

- [ ] 2.3.1. Д1 (2 / 1 / —)

**Критерий:** Четвёртый готов.

## 2.4. Пятый (Шаг: Разработка)

**Оценка:** 1 / 0.5 / —

- [ ] 2.4.1. Е1 (1 / 0.5 / —)

**Критерий:** Пятый готов.

---

## Трассировка задач (docs/tasks)

Хвост: пакеты 2.1–2.3.
"""


def selftest() -> int:
    fails: list[str] = []

    def ok(name: str, cond: bool, info: str = "") -> None:
        print(f"  {'ok  ' if cond else 'FAIL'} {name}" + (f" — {info}" if info and not cond else ""))
        if not cond:
            fails.append(name)

    print("selftest: синтетический мини-бэклог")
    model = parse_model(MINI)
    ok("модель без операций воспроизводит текст", render_model(model) == MINI)
    X = Ctx(model)
    old_pkgs = [p.olds[0] for m in model.mss for p in m.pkgs]
    old_tasks = [t.old for m in model.mss for p in m.pkgs for t in p.tasks]
    info = OpsInfo()
    # 1. перенос задачи: 2.1.2 → конец 1.1
    t = X.pop_task("2.1.2")
    X.pkg("1.1")[1].tasks.append(t)
    # 2. расщепление 2.1 на две части; критерий остаётся во второй
    crit21 = list(X.pkg("2.1")[1].crits)
    parts = X.split_pkg("2.1", [
        {"head": "Часть А (Шаг: Разработка)", "tasks": ["2.1.1"], "intro": [Line("Вводная части А")], "crits": []},
        {"head": "Часть Б (Шаг: Разработка)", "tasks": ["2.1.3"], "intro": [], "crits": crit21},
    ])
    ms2 = X.ms(2)
    ms2.pkgs[0:0] = parts
    # 3. слияние 2.2 и 2.3 с объединением критериев
    merged = X.merge_pkgs(["2.2", "2.3"], "Слитый (Шаги: А, Б)", ["2.2.1", "2.3.1"], ["2.2", "2.3"])
    i = X.pkg_index(ms2, "2.2")
    X.pop_pkg("2.2")
    X.pop_pkg("2.3")
    ms2.pkgs.insert(i, merged)
    # 4. удаление пакета с переносом критерия: 2.4
    p24 = X.pkg("2.4")[1]
    X.pkg("1.1")[1].tasks.append(X.pop_task("2.4.1"))
    X.pkg("1.1")[1].crits += p24.crits
    p24.crits = []
    try:
        X.remove_empty_pkg("2.4")
        ok("удаление пустого пакета с перенесённым критерием", True)
    except OpError as e:
        ok("удаление пустого пакета с перенесённым критерием", False, str(e))
    info.deleted_pkgs["2.4"] = "тест"
    p = X.pkg("1.1")[1]
    # 5. добавление задачи
    X.add_task(p, "Новая (Шаг: Внедрение) (2 / 1 / —)")
    # удаление непустого пакета запрещено
    try:
        Ctx(parse_model(MINI)).remove_empty_pkg("2.1")
        ok("удаление пакета с задачами запрещено", False)
    except OpError:
        ok("удаление пакета с задачами запрещено", True)
    info.ambiguous_keys["2.1"] = parts
    mapd = number_model(model, old_pkgs, old_tasks, info)
    C.set_map(mapd)
    col = Collector()
    remap_model(col, model)
    apply_edits(model)
    rows = recompute_estimates(model, col.decisions)
    text = render_model(model)
    bl = C.parse_backlog(text)
    ok("задачи 1.1: А, Б, В2, Е1, новая → 1.1.1–1.1.5", [t.rest.split(" (")[0] for t in model.mss[0].pkgs[0].tasks] == ["Задача А", "Задача Б", "В2", "Е1", "Новая"])
    ok("карта: 2.1.2 → 1.1.3, 2.4.1 → 1.1.4", mapd["tasks"]["2.1.2"] == "1.1.3" and mapd["tasks"]["2.4.1"] == "1.1.4", str(mapd["tasks"]))
    ok("карта: 2.1.3 → 2.2.1; 2.2.1, 2.3.1 → 2.3.1, 2.3.2 (слияние)", mapd["tasks"]["2.1.3"] == "2.2.1" and mapd["tasks"]["2.2.1"] == "2.3.1" and mapd["tasks"]["2.3.1"] == "2.3.2")
    ok("карта: пакеты 2.2, 2.3 → 2.3; 2.1 неоднозначен; 2.4 удалён", mapd["packages"].get("2.2") == "2.3" and mapd["packages"].get("2.3") == "2.3"
       and "2.1" in mapd["ambiguous_packages"] and "2.4" in mapd["deleted_packages"])
    ok("критерии слитого пакета — подряд", "**Критерий:** Третий готов.\n\n**Критерий:** Четвёртый готов." in text)
    ok("критерий удалённого пакета перенесён в 1.1", text.index("**Критерий:** Пятый готов.") < text.index("# Веха 2"))
    ok("повторный разбор восстанавливает текст", C.render_parsed(bl) == text)
    en = est_report(bl)
    ok("суммы пакетов = «Оценка» после операций", not en["pk_bad"] and not en["ms_bad"], str(en))
    ok("ремап «см. 2.1.3 и 2.3.1» один раз", "см. 2.2.1 и 2.3.2" in text, text[text.find("В2"): text.find("В2") + 60])
    ok("op9 снимает суффикс только в тексте вех", "# Веха 2 — Бета" in text and OP9_SUFFIX not in text)
    ok("в хвосте диапазон пакетов «2.1–2.3» (2.1 неоднозначен → без замены 2.1, 2.3→2.3)", "пакеты 2.1–2.3." in text or "пакеты 2.1, 2.3." in text, text[-60:])
    ok("новая задача не ремапилась и имеет номер 1.1.5", "- [ ] 1.1.5. Новая (Шаг: Внедрение) (2 / 1 / —)" in text)
    print("selftest: ремап на карте репозитория")
    root = DEFAULT_ROOT
    if (root / BACKLOG_REL).exists() and PLAN_PATH.exists():
        bs = (root / BACKLOG_REL).read_bytes().decode("utf-8-sig").replace("\r\n", "\n")
        base = parse_model(bs)
        mdl = parse_model(bs)
        inf = apply_ops(mdl, load_plan())
        md = number_model(mdl, [p.olds[0] for m in base.mss for p in m.pkgs], [t.old for m in base.mss for p in m.pkgs for t in p.tasks], inf)
        C.set_map(md)
        mk: list[str] = []
        cases = [
            ("пакеты 4.1–4.7 готовы", "3.1–3.7"),
            ("задачи 4.5.5–4.5.6", "3.5.5–3.5.6"),
            ("задачи 5.4.10–5.4.11", "10.5.3–10.5.4"),
            ("задачи 5.4.1–5.4.7", "3.8.1–3.8.7"),
            ("после 5.8.1 и 6.10.1", "5.5.2 и 6.9.14"),
            ("пилот 4.8–4.10", "4.1–4.3"),
        ]
        for src, want in cases:
            got = remap_text3(src, mk, "t", [])
            ok(f"remap «{src}» → …{want}…", want in got, got)
        mk = []
        got = remap_text3("пакет 5.4 и задача 6.17.1, пакет 6.17", mk, "t", [])
        ok("пакет «5.4», 6.17.1 и 6.17 не заменяются, идут в markers", got == "пакет 5.4 и задача 6.17.1, пакет 6.17" and len(mk) == 3, f"{got} {mk}")
        ok("5.4 → неоднозначный в карте", "5.4" in md["ambiguous_packages"] and "5.4" not in md["packages"])
        ok("раскладка из постановки совпадает с вычислением", check_layout(md, mdl)[0], "\n".join(check_layout(md, mdl)[1]))
    else:
        print("  пропущено: нет backlog.md или реестра")
    C._S.clear()
    print("selftest: " + ("ОШИБКИ: " + "; ".join(fails) if fails else "все проверки пройдены"))
    return 1 if fails else 0


# ---------------------------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------------------------
def main(argv: list[str] | None = None) -> int:
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except Exception:
        pass
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    g = ap.add_mutually_exclusive_group()
    g.add_argument("--dry-run", action="store_true", help="по умолчанию: карта, предпросмотр и отчёт; файлы на месте не меняются")
    g.add_argument("--apply", action="store_true", help="перенести preview на место (нужен чистый git status по области правок; идемпотентность не гарантируется)")
    ap.add_argument("--root", type=Path, default=DEFAULT_ROOT, help="корень репозитория")
    ap.add_argument("--selftest", action="store_true", help="тесты на синтетическом мини-бэклоге и на карте репозитория")
    ap.add_argument("--snapshot-report", action="store_true", help="устаревшие номера в снимках: только список файл:строка:фрагмент")
    ap.add_argument("--full", action="store_true", help="печатать отчёт полностью")
    args = ap.parse_args(argv)
    root = args.root.resolve()
    if args.selftest:
        return selftest()
    if args.snapshot_report:
        lines, files = snapshot_report(root)
        SNAPSHOT_REPORT_PATH.write_text("\n".join(lines) + ("\n" if lines else ""), encoding="utf-8")
        print(f"снимки: просмотрено файлов {files}; строк с изменившимися номерами {len(lines)}; список — {SNAPSHOT_REPORT_PATH}")
        return 0
    preview = HERE / PREVIEW_NAME
    comp = compute(root)
    C._S.clear()
    comp2 = compute(root)
    d1, d2 = digest(comp.results, comp.map_text), digest(comp2.results, comp2.map_text)
    determinism = (d1 == d2 and comp.results == comp2.results and comp.new_text == comp2.new_text,
                   f"два расчёта подряд совпадают: хэши {'равны' if d1 == d2 else 'РАЗНЫЕ'}")
    bl_new = C.parse_backlog(comp.new_text)
    checks = run_checks(comp, bl_new, determinism)
    ok = all(c.ok for c in checks)
    MAP_PATH.write_bytes(comp.map_text.encode("utf-8"))      # карта — создаваемый артефакт этапа, пишется всегда
    report = build_report(comp, checks, d1, full=True)
    write_preview(preview, comp, report)
    for rel, b in comp.results.items():
        if (preview / PurePosixPath(rel)).read_bytes() != b:
            raise RuntimeError(f"предпросмотр {rel} не совпал с расчётом")
    print("\n".join(build_report(comp, checks, d1, full=args.full)))
    print(f"\nКарта: {MAP_PATH}\nПредпросмотр: {preview}\nПолный отчёт: {preview / '_report.txt'}")
    if not ok:
        print("\nПРОВЕРКИ НЕ ПРОЙДЕНЫ — файлы на место не переносятся.", file=sys.stderr)
        return 1
    if args.apply:
        print("\nВНИМАНИЕ: --apply не идемпотентен; повторный запуск по уже изменённым файлам даст неверные номера.")
        dirty = git_dirty(root, comp.scanned)
        if dirty:
            print("ОТКАЗ: в области правок есть незафиксированные изменения:\n  " + "\n  ".join(dirty[:30]), file=sys.stderr)
            return 1
        for rel, b in comp.results.items():
            (root / rel).write_bytes(b)
        print(f"--apply: перенесено файлов: {len(comp.results)}")
    else:
        print("\nDRY-RUN: файлы канона не менялись.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
