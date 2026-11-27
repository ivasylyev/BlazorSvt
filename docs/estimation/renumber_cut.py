# -*- coding: utf-8 -*-
"""Вторая перенумерация (26.11.2026): вехи 1–63 → вехи 1–34.

Карта — renumber_map_cut.json (старый номер → новый; снятые пункты — в *_deleted).
Первая перенумерация (18.11.2026: 0.x.y.z → вехи 1–63) — renumber.py и renumber_map.json.
Порядок: build_rows → renumber.py (вехи 1–63) → этот модуль (вехи 1–34).
"""
from __future__ import annotations

import json
import re
from dataclasses import replace
from pathlib import Path

import renumber as RN  # регулярки и щит от ложных срабатываний (версии, оценки, §…)

_MAP = json.loads(Path(__file__).with_name("renumber_map_cut.json").read_text(encoding="utf-8"))
TASKS: dict[str, str] = _MAP["tasks"]
PACKAGES: dict[str, str] = _MAP["packages"]
DEL_TASKS = set(_MAP["tasks_deleted"])
DEL_PACKAGES = set(_MAP["packages_deleted"])
MILESTONES: dict[int, int] = {int(k): v for k, v in _MAP["milestones"].items()}
# Веха 3 влита в 2 (пакет «Роли»); вехи 5, 6, 7 влиты в 4 (пакеты юзкейсов, прототипа и демо «Заявок»).
MS_MERGED = {3: MILESTONES[4], 5: MILESTONES[9], 6: MILESTONES[9], 7: MILESTONES[9]}
SURVIVING = sorted(MILESTONES)

_PKG_RANGE = re.compile(
    r"(?<![\w.\u2026/\-\u2013\u2014])(\d{1,2}\.\d{1,2})(\s*[\u2013\-]\s*)(\d{1,2}\.\d{1,2})(?![\d]|\.\d)"
)
_WORD = re.compile(r"(вех[а-я]*)\s+(\d+)(?:(\s*[\u2013\-]\s*)(\d+))?")


def _dot(x: str) -> str:
    return x.replace(".", "\u00b7")


def remap_text(text: str, markers: list[str] | None = None, ctx: str = "") -> str:
    """Переписывает номера пунктов, пакетов и вех в тексте. Ссылка на снятое остаётся
    видимой меткой ⟦DEL …⟧ (точки заменены на «·»), чтобы её нашёл поиск и поправил человек."""
    if not text:
        return text
    found: list[str] = []

    def mark(what: str) -> None:
        if markers is not None:
            markers.append(f"{ctx}: {what}")

    def shield(m):
        found.append(m.group(0))
        return f"\u0000{len(found) - 1}\u0000"

    s = RN._SHIELD.sub(shield, text)

    def one_task(tok: str) -> str:
        if tok in TASKS:
            return TASKS[tok]
        if tok in DEL_TASKS:
            mark(tok)
            return f"\u27e6DEL {_dot(tok)}\u27e7"
        return tok

    def rng(m):
        a, b = m.group(1), m.group(2)
        if a in DEL_TASKS or b in DEL_TASKS:
            mark(m.group(0))
            return f"\u0001\u27e6DEL {_dot(m.group(0))}\u27e7\u0001"
        if a not in TASKS or b not in TASKS:
            return m.group(0)
        pa, ia = a.rsplit(".", 1)
        pb, ib = b.rsplit(".", 1)
        olds = [f"{pa}.{i}" for i in range(int(ia), int(ib) + 1)] if pa == pb else []
        news = [TASKS[o] for o in olds if o in TASKS]
        na, nb = TASKS[a], TASKS[b]
        if (news and news == sorted(news, key=RN._key) and na.rsplit(".", 1)[0] == nb.rsplit(".", 1)[0]
                and RN._key(nb)[-1] - RN._key(na)[-1] + 1 == len(olds) == len(news)):
            return f"\u0001{na}\u2013{nb}\u0001"
        return "\u0001" + ", ".join(news) + "\u0001"

    s = RN._range.sub(rng, s)

    def one_pkg(tok: str) -> str:
        if tok in PACKAGES:
            return PACKAGES[tok]
        if tok in DEL_PACKAGES:
            mark(tok)
            return f"\u27e6DEL {_dot(tok)}\u27e7"
        return tok

    def prange(m):
        a, sep, b = m.groups()
        known = lambda x: x in PACKAGES or x in DEL_PACKAGES  # noqa: E731
        if known(a) and known(b):
            if a in DEL_PACKAGES or b in DEL_PACKAGES:
                mark(f"{a}{sep}{b}")
                return f"\u27e6DEL {_dot(a)}\u2013{_dot(b)}\u27e7"
            return f"\u0002{PACKAGES[a]}{sep}{PACKAGES[b]}\u0002"
        return m.group(0)

    parts = s.split("\u0001")
    for i in range(0, len(parts), 2):
        seg = RN._task.sub(lambda m: one_task(m.group(1)), parts[i])
        seg = _PKG_RANGE.sub(prange, seg)
        sub = seg.split("\u0002")
        for j in range(0, len(sub), 2):
            sub[j] = RN._pkg.sub(lambda m: one_pkg(m.group(1)), sub[j])
        parts[i] = "".join(sub)
    s = "".join(parts)

    def wordref(m):
        w, a, sep, b = m.group(1), int(m.group(2)), m.group(3), m.group(4)
        if b is None:
            if a in MILESTONES:
                return f"{w} {MILESTONES[a]}"
            if a in MS_MERGED:
                return f"{w} {MS_MERGED[a]}"
            mark(f"{w} {a}")
            return f"{w} \u27e6DEL {a}\u27e7"
        b = int(b)
        inner = [x for x in SURVIVING if a <= x <= int(b)]
        if not inner:
            mark(f"{w} {a}-{b}")
            return f"{w} \u27e6DEL {a}\u2013{b}\u27e7"
        lo, hi = MILESTONES[inner[0]], MILESTONES[inner[-1]]
        return f"{w} {lo}" if lo == hi else f"{w} {lo}\u2013{hi}"

    s = _WORD.sub(wordref, s)
    return re.sub(r"\u0000(\d+)\u0000", lambda m: found[int(m.group(1))], s)


# У домена «Продукты» хвост пакета (Gate A/B/C, обучение, передача, вывод) генератор собирает в 40.1.x,
# а в backlog.md до перенумерации он разложен по вехам 40 (MVP), 41 (ОПЭ), 42 (ПЭ). Сумма и оценки
# совпадают, различаются только номера. Таблица заменяет номер генератора номером backlog.md
# (одновременно, без цепочек: 40.1.11 генератора — «Замер приживаемости», 40.1.11 backlog.md — Gate A).
GEN_ALIAS: dict[str, str] = {
    "40.1.1": "40.1.11",  # Gate A
    "40.1.2": "41.1.2",   # Gate B
    "40.1.3": "42.1.1",   # Gate C
    "40.1.4": "40.1.12",  # Заключение о готовности
    "40.1.5": "41.1.1",   # Обучение и сценарий отката
    "40.1.6": "40.1.10",  # Кнопка «Сообщить об ошибке»
    "40.1.7": "40.1.13",  # Пакет передачи в поддержку
    "40.1.8": "42.1.2",   # Поддержка забрала решение
    "40.1.11": "42.1.3",  # Замер приживаемости
}
_ALIAS_TITLE = {
    "40.1.1": "Gate A, домен «Продукты»",
    "40.1.2": "Gate B, домен «Продукты»",
    "40.1.3": "Gate C (SoT Flip Decision Record), домен «Продукты»",
    "40.1.4": "Заключение о готовности до пуска пользователей, домен «Продукты»",
    "40.1.5": "Обучение пользователей и сценарий отката, домен «Продукты»",
    "40.1.6": "Подключить кнопку «Сообщить об ошибке» к экранам домена, домен «Продукты»",
    "40.1.7": "Пакет передачи в поддержку, домен «Продукты»",
    "40.1.8": "Поддержка забрала решение, домен «Продукты»",
    "40.1.11": "Замер приживаемости, домен «Продукты»",
}


def _alias(r):
    """Номер генератора → номер backlog.md; название сверяется, чтобы не подменить чужой пункт."""
    if r.item in GEN_ALIAS:
        if not r.title.startswith(_ALIAS_TITLE[r.item]):
            raise RuntimeError(f"{r.item}: название не совпало с ожидаемым для подмены номера: {r.title!r}")
        return replace(r, item=GEN_ALIAS[r.item], backlog=GEN_ALIAS[r.item].rsplit(".", 1)[0],
                       stage=GEN_ALIAS[r.item].split(".")[0])
    return r


def cut_rows(rows):
    """Строки с номерами вех 1–63 → вехи 1–34; снятые пункты убираются."""
    out = []
    for r in map(_alias, rows):
        if r.item in DEL_TASKS:
            continue
        if r.item not in TASKS:
            raise RuntimeError(f"пункт {r.item} есть в генераторе, но не в backlog.md (renumber_map_cut.json)")
        new = TASKS[r.item]
        pkg = new.rsplit(".", 1)[0]
        out.append(replace(r, stage=pkg.split(".")[0], backlog=pkg, item=new,
                           title=remap_text(r.title), note=remap_text(r.note)))
    seen = [r.item for r in out]
    if len(seen) != len(set(seen)):
        raise RuntimeError("повтор номера после второй перенумерации")
    out.sort(key=lambda r: RN._key(r.item))
    return out
