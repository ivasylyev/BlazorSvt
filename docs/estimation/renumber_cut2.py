# -*- coding: utf-8 -*-
"""Третья перенумерация (этап 2, 27.11.2026): вехи 1–34 → вехи 1–16.

Раскладка вех — restructure_plan.json (реестр этапа 1). Карта — renumber_map_cut2.json
(старый номер → новый). Слитые вехи: пакеты новой вехи идут подряд по порядку старых вех из реестра,
номер задачи внутри пакета сохраняется; исключение — пакет старого 7.10 (задачи переставляются по группам).
Снятых пунктов нет: «неизвестный» токен не меняется и попадает в список markers.

Предыдущие слои: renumber.py (0.x.y.z → вехи 1–63) и renumber_cut.py (вехи 1–63 → 1–34).
Модуль только строит карту и переписывает текст; файлы канона он не меняет.
"""
from __future__ import annotations

import json
import re
from dataclasses import dataclass, field
from decimal import Decimal
from pathlib import Path

import renumber as RN  # регулярки и щит от ложных срабатываний (версии, оценки, §…)

HERE = Path(__file__).resolve().parent
MAP_PATH = HERE / "renumber_map_cut2.json"
PLAN_PATH = HERE / "restructure_plan.json"

# ---------------------------------------------------------------------------------------------
# Разбор backlog.md
# ---------------------------------------------------------------------------------------------
Ln = tuple[int, str]  # (номер строки в исходном файле, 1-based; текст без \n)

MS_RE = re.compile(r"^# Веха (\d+) — (.+)$")
PKG_RE = re.compile(r"^## (\d+)\.(\d+)\. ")
TASK_RE = re.compile(r"^- \[( |x)\] (\d+)\.(\d+)\.(\d+)\. ")
EST_RE = re.compile(r"^\*\*Оценка:\*\* (\S+) / (\S+) / (\S+)\s*$")
STEPS_TAIL_RE = re.compile(r"^(.*?)\s*\((Шаг|Шаги): ([^)]*)\)\s*$")
TRIPLE_RE = re.compile(r"\((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) / [^)]*\)")
SVT_RE = re.compile(r"\[SVT-\d+\]")
TAIL_HEAD = "## Трассировка задач"


class StructureError(Exception):
    pass


@dataclass
class Pkg:
    ms: int
    no: int
    lines: list[Ln]

    @property
    def key(self) -> str:
        return f"{self.ms}.{self.no}"

    def task_indexes(self) -> list[int]:
        return [i for i, (_, t) in enumerate(self.lines) if TASK_RE.match(t)]

    def est(self) -> tuple[Decimal, Decimal] | None:
        for _, t in self.lines:
            m = EST_RE.match(t)
            if m:
                return Decimal(m.group(1)), Decimal(m.group(2))
        return None

    def task_sum(self) -> tuple[Decimal, Decimal]:
        a = b = Decimal(0)
        for i in self.task_indexes():
            m = TRIPLE_RE.search(self.lines[i][1])
            if m:
                a += Decimal(m.group(1))
                b += Decimal(m.group(2))
        return a, b

    def tasks_without_triple(self) -> list[str]:
        out = []
        for i in self.task_indexes():
            if not TRIPLE_RE.search(self.lines[i][1]):
                out.append(TASK_RE.match(self.lines[i][1]).group(0).strip())
        return out


@dataclass
class Milestone:
    num: int
    head: Ln
    name: str                  # без «(Шаги: …)»
    steps: str | None          # текст после «Шаги: »
    est_line: Ln
    est: tuple[Decimal, Decimal]
    intro: list[Ln]
    pkgs: list[Pkg] = field(default_factory=list)


@dataclass
class Backlog:
    preamble: list[Ln]
    mss: list[Milestone]
    tail: list[Ln]


def _strip_blank(lines: list[Ln]) -> list[Ln]:
    a, b = 0, len(lines)
    while a < b and lines[a][1].strip() == "":
        a += 1
    while b > a and lines[b - 1][1].strip() == "":
        b -= 1
    return lines[a:b]


def parse_backlog(text: str) -> Backlog:
    """Разбор по заголовкам `# Веха`, `## N.M.`; хвост — со строки `## Трассировка задач`."""
    lines: list[Ln] = [(i + 1, t) for i, t in enumerate(text.split("\n"))]
    ms_idx = [i for i, (_, t) in enumerate(lines) if MS_RE.match(t)]
    if not ms_idx:
        raise StructureError("в файле нет заголовков `# Веха`")
    tails = [i for i, (_, t) in enumerate(lines) if t.startswith(TAIL_HEAD)]
    if len(tails) != 1:
        raise StructureError(f"заголовок «{TAIL_HEAD}» найден {len(tails)} раз(а), нужен один")
    tail_i = tails[0]
    if tail_i < ms_idx[-1]:
        raise StructureError("«Трассировка задач» стоит до последней вехи")
    preamble = lines[: ms_idx[0]]
    tail = lines[tail_i:]
    mss: list[Milestone] = []
    bounds = ms_idx + [tail_i]
    for j in range(len(ms_idx)):
        block = lines[bounds[j]: bounds[j + 1]]
        # хвост блока: пустые строки, `---`, пустые строки
        blk = list(block)
        while blk and blk[-1][1].strip() == "":
            blk.pop()
        if not blk or blk[-1][1] != "---":
            raise StructureError(f"веха на строке {block[0][0]}: нет разделителя `---` в конце блока")
        blk.pop()
        while blk and blk[-1][1].strip() == "":
            blk.pop()
        mh = MS_RE.match(blk[0][1])
        num = int(mh.group(1))
        title = mh.group(2)
        sm = STEPS_TAIL_RE.match(title)
        name, steps = (sm.group(1), sm.group(3)) if sm else (title, None)
        if len(blk) < 4 or blk[1][1] != "" or not EST_RE.match(blk[2][1]):
            raise StructureError(f"веха {num} (строка {blk[0][0]}): после заголовка ожидаются пустая строка и `**Оценка:**`")
        em = EST_RE.match(blk[2][1])
        est = (Decimal(em.group(1)), Decimal(em.group(2)))
        pk_idx = [i for i, (_, t) in enumerate(blk) if t.startswith("## ")]
        first_pk = pk_idx[0] if pk_idx else len(blk)
        intro = _strip_blank(blk[3:first_pk])
        ms = Milestone(num, blk[0], name, steps, blk[2], est, intro)
        for k, pi in enumerate(pk_idx):
            end = pk_idx[k + 1] if k + 1 < len(pk_idx) else len(blk)
            plines = blk[pi:end]
            while plines and plines[-1][1].strip() == "":
                plines.pop()
            pm = PKG_RE.match(plines[0][1])
            if not pm:
                raise StructureError(f"строка {plines[0][0]}: заголовок пакета не по шаблону `## N.M. `")
            if int(pm.group(1)) != num:
                raise StructureError(f"строка {plines[0][0]}: пакет {pm.group(1)}.{pm.group(2)} внутри вехи {num}")
            ms.pkgs.append(Pkg(num, int(pm.group(2)), plines))
        mss.append(ms)
    return Backlog(preamble, mss, tail)


def ms_lines(head: str, est: str, intro: list[str], pkgs: list[list[str]]) -> list[str]:
    out = [head, "", est, ""]
    if intro:
        out += intro + [""]
    for p in pkgs:
        out += p + [""]
    while out and out[-1] == "":
        out.pop()
    return out


def render_backlog(preamble: list[str], blocks: list[list[str]], tail: list[str]) -> str:
    out = list(preamble)
    for b in blocks:
        out += b + ["", "---", ""]
    out += tail
    return "\n".join(out)


def render_parsed(bl: Backlog) -> str:
    """Обратная сборка без изменений — проверка, что разбор не теряет текст."""
    blocks = [
        ms_lines(m.head[1], m.est_line[1], [t for _, t in m.intro], [[t for _, t in p.lines] for p in m.pkgs])
        for m in bl.mss
    ]
    return render_backlog([t for _, t in bl.preamble], blocks, [t for _, t in bl.tail])


def fmt_num(d: Decimal) -> str:
    """198.7, 103.76, 475 — без хвостовых нулей, 2 знака."""
    s = f"{d.quantize(Decimal('0.01')):f}"
    if "." in s:
        s = s.rstrip("0").rstrip(".")
    return s


# ---------------------------------------------------------------------------------------------
# Пакет старого 7.10 (новый 4.10): группы задач
# ---------------------------------------------------------------------------------------------
_RE_TAGGED = re.compile(r"^- \[( |x)\] \d+\.\d+\.\d+\. \[(Overtime|SVT-\d+)\]")
_RE_ANY = re.compile(r"^- \[( |x)\] \d+\.\d+\.\d+\. ")

# Ожидание из постановки — только проверка результата классификации по содержимому.
EXPECTED_7_10 = {
    1: list(range(1, 6)),
    2: list(range(6, 21)),
    3: list(range(21, 37)) + list(range(57, 67)),
    4: list(range(37, 57)),
}


def classify_7_10(text: str) -> int:
    m = _RE_TAGGED.match(text)
    if m:
        done = m.group(1) == "x"
        tag = m.group(2)
        if done and tag == "Overtime":
            return 1
        if done and tag.startswith("SVT-"):
            return 2
        if not done and tag.startswith("SVT-"):
            return 3
        raise StructureError(f"7.10: не сделан с [Overtime] — нет группы: {text[:80]}")
    m = _RE_ANY.match(text)
    if m and m.group(1) == " ":
        return 4
    raise StructureError(f"7.10: сделан без [Overtime]/[SVT-…] — нет группы: {text[:80]}")


def reorder_7_10(pkg: Pkg) -> tuple[list[Ln], dict[int, int]]:
    """Возвращает строки пакета с переставленными задачами и {старый номер задачи: новый}."""
    idx = pkg.task_indexes()
    if not idx or idx != list(range(idx[0], idx[0] + len(idx))):
        raise StructureError("7.10: строки задач не образуют сплошной блок")
    tasks = [pkg.lines[i] for i in idx]
    groups: dict[int, list[int]] = {1: [], 2: [], 3: [], 4: []}
    by_old: dict[int, Ln] = {}
    for ln in tasks:
        no = int(TASK_RE.match(ln[1]).group(4))
        groups[classify_7_10(ln[1])].append(no)
        by_old[no] = ln
    bad = []
    for g in (1, 2, 3, 4):
        if groups[g] != EXPECTED_7_10[g]:
            bad.append(f"группа {g}: найдено {groups[g]}, ожидалось {EXPECTED_7_10[g]}")
    if bad:
        raise StructureError("7.10: группы не совпали с ожидаемыми старыми номерами:\n  " + "\n  ".join(bad))
    order = groups[1] + groups[2] + groups[3] + groups[4]
    new_of_old = {old: i + 1 for i, old in enumerate(order)}
    lines = pkg.lines[: idx[0]] + [by_old[o] for o in order] + pkg.lines[idx[-1] + 1:]
    return lines, new_of_old


# ---------------------------------------------------------------------------------------------
# Карта
# ---------------------------------------------------------------------------------------------
def _key(t: str) -> tuple[int, ...]:
    return RN._key(t)


def build_map(bl: Backlog, plan: dict) -> dict:
    old_nums = [m.num for m in bl.mss]
    if old_nums != list(range(1, len(old_nums) + 1)):
        raise StructureError(f"старые вехи идут не подряд: {old_nums}")
    layout = plan["milestones"]
    news = [m["new"] for m in layout]
    if news != list(range(1, len(layout) + 1)):
        raise StructureError(f"новые вехи в реестре идут не подряд: {news}")
    all_old = [o for m in layout for o in m["old"]]
    if sorted(all_old) != old_nums:
        raise StructureError(f"реестр вехи не покрывает старые вехи 1–{len(old_nums)} ровно по разу: {sorted(all_old)}")
    ms_map: dict[int, int] = {}
    merged: dict[int, list[int]] = {}
    for m in layout:
        for o in m["old"]:
            ms_map[o] = m["new"]
        if len(m["old"]) > 1:
            merged[m["new"]] = list(m["old"])
    pkg_map: dict[str, str] = {}
    task_map: dict[str, str] = {}
    by_num = {m.num: m for m in bl.mss}
    for m in layout:
        k = m["new"]
        seq = 0
        for o in m["old"]:
            pkgs = by_num[o].pkgs
            nums = [p.no for p in pkgs]
            if nums != list(range(1, len(nums) + 1)):
                raise StructureError(f"веха {o}: пакеты идут не подряд: {nums}")
            for p in pkgs:
                seq += 1
                newpkg = f"{k}.{seq}"
                pkg_map[p.key] = newpkg
                tidx = p.task_indexes()
                tnums = [int(TASK_RE.match(p.lines[i][1]).group(4)) for i in tidx]
                tpk = {(int(TASK_RE.match(p.lines[i][1]).group(2)), int(TASK_RE.match(p.lines[i][1]).group(3))) for i in tidx}
                if tpk and tpk != {(p.ms, p.no)}:
                    raise StructureError(f"пакет {p.key}: в нём задачи другого пакета: {sorted(tpk)}")
                if tnums != list(range(1, len(tnums) + 1)):
                    raise StructureError(f"пакет {p.key}: задачи идут не подряд: {tnums}")
                if p.key == plan["task_reorder"]["package_old"]:
                    _, new_of_old = reorder_7_10(p)
                    for old_n in tnums:
                        task_map[f"{p.key}.{old_n}"] = f"{newpkg}.{new_of_old[old_n]}"
                else:
                    for old_n in tnums:
                        task_map[f"{p.key}.{old_n}"] = f"{newpkg}.{old_n}"
    if pkg_map.get(plan["task_reorder"]["package_old"]) != plan["task_reorder"]["package_new"]:
        raise StructureError(
            f"пакет {plan['task_reorder']['package_old']} получил номер {pkg_map.get(plan['task_reorder']['package_old'])}, "
            f"в реестре — {plan['task_reorder']['package_new']}"
        )
    return {
        "_about": "Третья перенумерация (этап 2, 27.11.2026): вехи 1–34 → 1–16. Генерирует restructure_apply.py по backlog.md и restructure_plan.json.",
        "milestones": {str(o): ms_map[o] for o in sorted(ms_map)},
        "milestones_merged": {str(k): v for k, v in sorted(merged.items())},
        "milestone_names": {str(m["new"]): m["name"] for m in layout},
        "milestones_deleted": [],
        "packages": {k: pkg_map[k] for k in sorted(pkg_map, key=_key)},
        "packages_deleted": [],
        "tasks": {k: task_map[k] for k in sorted(task_map, key=_key)},
        "tasks_deleted": [],
    }


def dump_map(m: dict) -> str:
    """JSON в стиле renumber_map_cut.json: отступ в 1 пробел, CRLF."""
    return json.dumps(m, ensure_ascii=False, indent=1).replace("\n", "\r\n") + "\r\n"


# ---------------------------------------------------------------------------------------------
# Состояние карты (задаётся load_map/set_map)
# ---------------------------------------------------------------------------------------------
_S: dict = {}


def set_map(m: dict) -> None:
    _S["MS"] = {int(k): v for k, v in m["milestones"].items()}
    _S["TASKS"] = dict(m["tasks"])
    _S["PKGS"] = dict(m["packages"])
    _S["T_OLD"] = sorted(_S["TASKS"], key=_key)
    _S["P_OLD"] = sorted(_S["PKGS"], key=_key)
    _S["T_NEW"] = sorted(set(_S["TASKS"].values()), key=_key)
    _S["P_NEW"] = sorted(set(_S["PKGS"].values()), key=_key)
    _S["T_OLD_I"] = {k: i for i, k in enumerate(_S["T_OLD"])}
    _S["P_OLD_I"] = {k: i for i, k in enumerate(_S["P_OLD"])}
    _S["T_NEW_I"] = {k: i for i, k in enumerate(_S["T_NEW"])}
    _S["P_NEW_I"] = {k: i for i, k in enumerate(_S["P_NEW"])}
    _S["DEL_T"] = set(m.get("tasks_deleted", []))
    _S["DEL_P"] = set(m.get("packages_deleted", []))


def load_map(path: Path = MAP_PATH) -> dict:
    m = json.loads(Path(path).read_text(encoding="utf-8"))
    set_map(m)
    return m


def map_loaded() -> bool:
    return "TASKS" in _S


if MAP_PATH.exists():
    load_map()

# ---------------------------------------------------------------------------------------------
# remap_text2
# ---------------------------------------------------------------------------------------------
_PKG_RANGE = re.compile(
    r"(?<![\w.\u2026/\-\u2013\u2014])(\d{1,2}\.\d{1,2})(\s*[\u2013\-]\s*)(\d{1,2}\.\d{1,2})(?![\d]|\.\d)"
)
# Дополнение к RN._SHIELD: версии моделей («Grok 4.7») — не пакеты бэклога.
_SHIELD_EXTRA = re.compile(r"(?i)(?<![\w])(?:grok|gpt|claude|gemini|opus|sonnet|haiku)[\s\-]*\d+(?:\.\d+)+")
# Как RN._pkg, но «/» блокирует только если перед ним не цифра: «10.7/7.6» — два пакета, «docs/2.7» — путь.
_PKG = re.compile(r"(?<![\w.§\-×])(?<!\D/)(\d{1,2}\.\d{1,2})(?![\d]|\.\d)")
_WORD = re.compile(
    r"(?<![А-Яа-яЁёA-Za-z])([Вв]ех[а-я]*)\s+(\d+)(?:(\s*[\u2013\u2014\-]\s*)(\d+))?(?!\d|\.\d)"
)
# Слитый диапазон «вехи 4–6» → «веха 5»: форма слова после схлопывания.
_COLLAPSE = {"вехи": "веха", "вех": "вехи", "вехам": "вехе", "вехами": "вехой", "вехах": "вехе"}
# После этих предлогов «вехи» — родительный падеж единственного числа («до вехи 5»), форму не меняем.
_ALLOWED_PREV_FOR_KEEP = {"до", "после", "от", "из", "без", "для", "у", "вне", "около", "кроме", "среди",
                          "внутри", "вместо", "ради", "с", "со"}


def _cap_like(src: str, dst: str) -> str:
    return dst[0].upper() + dst[1:] if src[:1].isupper() else dst


def remap_text2(text: str, markers: list[str] | None = None, ctx: str = "", notes: list[str] | None = None) -> str:
    """Переписывает номера пунктов, пакетов и вех (вехи 1–34 → 1–16).

    markers — «неизвестные токены»: похожи на номер задачи/пакета, но их нет в карте; текст не меняется.
    notes — сведения для отчёта (схлопнутые диапазоны, раскрытые перечни).
    """
    if not text:
        return text
    if not map_loaded():
        raise RuntimeError("карта renumber_map_cut2.json не загружена (load_map/set_map)")
    M, T, P = _S["MS"], _S["TASKS"], _S["PKGS"]
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

    s = _SHIELD_EXTRA.sub(lambda m: hold(m.group(0)), text)
    s = RN._SHIELD.sub(lambda m: hold(m.group(0)), s)

    # --- вехи -----------------------------------------------------------------------------
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
            if lw in _COLLAPSE:
                prev = re.search(r"([А-Яа-яЁё]+)\s*\(?$", s[: m.start()])
                keep_gen = lw == "вехи" and prev is not None and prev.group(1).lower() in _ALLOWED_PREV_FOR_KEEP
                if not keep_gen:
                    w2 = _cap_like(w, _COLLAPSE[lw])
            note(f"«{m.group(0)}» → «{w2} {lo}» (слитые вехи схлопнуты)")
            return hold(f"{w2} {lo}")
        return hold(f"{w} {lo}\u2013{hi}")

    s = _WORD.sub(wordref, s)

    # --- диапазоны задач -------------------------------------------------------------------
    def rng(m):
        a, b = m.group(1), m.group(2)
        if a not in T or b not in T:
            return m.group(0)
        ia, ib = _S["T_OLD_I"][a], _S["T_OLD_I"][b]
        if ia > ib:
            return m.group(0)
        news = [T[o] for o in _S["T_OLD"][ia: ib + 1]]
        idx = sorted(_S["T_NEW_I"][n] for n in news)
        lo, hi = _S["T_NEW"][idx[0]], _S["T_NEW"][idx[-1]]
        if idx == list(range(idx[0], idx[0] + len(idx))):
            return hold(lo if lo == hi else f"{lo}\u2013{hi}")
        note(f"диапазон «{m.group(0)}» стал непрерывным не в одном родителе — раскрыт в перечень ({len(idx)} пунктов)")
        return hold(", ".join(_S["T_NEW"][i] for i in idx))

    s = RN._range.sub(rng, s)

    def one_task(m):
        tok = m.group(1)
        if tok in T:
            return hold(T[tok])
        mark(tok)
        return tok

    s = RN._task.sub(one_task, s)

    # --- диапазоны пакетов -----------------------------------------------------------------
    def prange(m):
        a, sep, b = m.groups()
        if a in P and b in P:
            ia, ib = _S["P_OLD_I"][a], _S["P_OLD_I"][b]
            if ia > ib:
                return m.group(0)
            news = [P[o] for o in _S["P_OLD"][ia: ib + 1]]
            idx = sorted(_S["P_NEW_I"][n] for n in news)
            lo, hi = _S["P_NEW"][idx[0]], _S["P_NEW"][idx[-1]]
            if idx == list(range(idx[0], idx[0] + len(idx))):
                return hold(lo if lo == hi else f"{lo}{sep}{hi}")
            note(f"диапазон пакетов «{m.group(0)}» стал непрерывным не в одном родителе — раскрыт в перечень ({len(idx)} пакетов)")
            return hold(", ".join(_S["P_NEW"][i] for i in idx))
        return m.group(0)

    s = _PKG_RANGE.sub(prange, s)

    def one_pkg(m):
        tok = m.group(1)
        if tok in P:
            return hold(P[tok])
        mark(tok)
        return tok

    s = _PKG.sub(one_pkg, s)
    # возвращаем отложенное (значения не содержат меток, вложенности нет)
    return re.sub(r"\u0000(\d+)\u0000", lambda m: stash[int(m.group(1))], s)


# ---------------------------------------------------------------------------------------------
# Подозрительные места: то, что remap_text2 оставил как есть, но что выглядит как старая нумерация
# ---------------------------------------------------------------------------------------------
_SUS_WORD = re.compile(
    r"(?<![А-Яа-яЁёA-Za-z])[Вв]ех[а-я]*\s+(\d+)(?:\s*[\u2013\u2014\-]\s*(\d+))?(?!\d|\.\d)"
)
_SUS_LIST = re.compile(
    r"(?<![А-Яа-яЁёA-Za-z])[Вв]ех[а-я]*\s+\d+(?:\s*[\u2013\u2014\-]\s*\d+)?\s*(?:,|и|или|/)\s*\d+"
)
_SUS_4PART = re.compile(r"(?<![\w.])\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}(?![\d]|\.\d)")
_SUS_ARROW = re.compile(r"(?:…|\.\.\.)\s*(?:→|->)\s*\d{1,2}(?![\d.])")
# Места, которые regex пакетов/пунктов пропускает по lookbehind: перед пакетом — «×» или «-», перед пунктом — «-».
_SUS_EXCLUDED = re.compile(r"(?<=[\-\u00d7])(\d{1,2}\.\d{1,2}(?:\.\d{1,2})?)(?![\d]|\.\d)(?!\.[A-Za-z])")


def scan_suspicious(text: str) -> list[tuple[str, str]]:
    """(вид, фрагмент) по старому тексту строки. Не меняет текст."""
    out: list[tuple[str, str]] = []
    if not map_loaded() or not text:
        return out
    stash: list[str] = []
    def _h(m):
        stash.append(m.group(0))
        return f"\u0000{len(stash) - 1}\u0000"

    s = RN._SHIELD.sub(_h, _SHIELD_EXTRA.sub(_h, text))
    for m in _SUS_WORD.finditer(s):
        a = int(m.group(1))
        b = int(m.group(2)) if m.group(2) else a
        if a not in _S["MS"] or b not in _S["MS"] or a > b:
            out.append(("номер вехи вне 1–34", m.group(0)))
    for m in _SUS_LIST.finditer(s):
        out.append(("список вех (замена только первого номера)", m.group(0)))
    for m in _SUS_4PART.finditer(s):
        out.append(("формат 0.x.y.z / N.N.N.N", m.group(0)))
    for m in _SUS_ARROW.finditer(s):
        out.append(("цепочка номеров вех со стрелкой", m.group(0)))
    for m in _SUS_EXCLUDED.finditer(s):
        tok = m.group(1)
        before = s[m.start() - 2: m.start()]
        if before[:1].isalpha():  # «grok-4.7», «process-2.7» — часть слова/имени файла
            continue
        if tok in _S["TASKS"] or tok in _S["PKGS"]:
            out.append(("номер после «-» или «×» — remap его не трогает", s[max(0, m.start() - 14): m.end() + 8]))
    return out


# ---------------------------------------------------------------------------------------------
# Строки генератора (этап 3, 27.11.2026): вехи 1–34 → вехи 1–16
# ---------------------------------------------------------------------------------------------
def cut2_rows(rows, markers: list[str] | None = None, notes: list[str] | None = None):
    """Строки генератора (item, backlog, stage, title, note) в нумерации вех 1–34 → вехи 1–16.

    Образец — renumber_cut.cut_rows. Снятых пунктов нет: строка, которой нет в карте, — RuntimeError;
    дубль номера после перенумерации — RuntimeError; результат отсортирован по новому ключу
    (порядок троек пакета 4.10 следует backlog.md, его задаёт карта задач).
    markers/notes — как у remap_text2 (неизвестные токены и схлопнутые диапазоны для отчёта).
    """
    from dataclasses import replace

    if not map_loaded():
        raise RuntimeError("карта renumber_map_cut2.json не загружена (load_map/set_map)")
    tasks = _S["TASKS"]
    out = []
    for r in rows:
        if r.item not in tasks:
            raise RuntimeError(f"пункт {r.item} есть в генераторе, но не в backlog.md (renumber_map_cut2.json)")
        new = tasks[r.item]
        pkg = new.rsplit(".", 1)[0]
        out.append(replace(
            r, stage=pkg.split(".")[0], backlog=pkg, item=new,
            title=remap_text2(r.title, markers, f"{r.item} title", notes),
            note=remap_text2(r.note, markers, f"{r.item} note", notes),
        ))
    seen = [r.item for r in out]
    if len(seen) != len(set(seen)):
        dup = sorted({x for x in seen if seen.count(x) > 1}, key=_key)
        raise RuntimeError(f"повтор номера после третьей перенумерации: {dup}")
    out.sort(key=lambda r: _key(r.item))
    return out
