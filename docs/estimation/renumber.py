# -*- coding: utf-8 -*-
"""Последний шаг раскладки: номера вех 1–14 → вехи 1–63 по очереди исполнения.

Таблица — renumber_map.json: задача → новый номер, пакет → новые пакеты, снятые задачи.
Порядок вех и пакетов — roadmap.md и backlog.md.
"""
from __future__ import annotations

import json
import re
from dataclasses import replace
from pathlib import Path

_MAP = json.loads((Path(__file__).with_name("renumber_map.json")).read_text(encoding="utf-8"))
TASKS: dict[str, str] = _MAP["tasks"]
PACKAGES: dict[str, list[str]] = _MAP["packages"]
DELETED: dict[str, str] = _MAP["deleted"]

# Пакет, разнесённый по нескольким вехам: ссылка в тексте ведёт на эту часть.
SPLIT_CHOICE = {
    "1.10": "6.1", "2.4": "8.2", "2.7": "15.6", "4.1": "9.3", "4.2": "11.2",
    "9.2": "21.21", "9.3": "27.1", "9.4": "33.1", "9.5": "40.1", "9.6": "46.1", "9.7": "52.1", "9.8": "58.1",
}

_TID = r"\d{1,2}\.\d{1,2}\.\d{1,2}"
_task = re.compile(rf"(?<![\w.§\-])({_TID})(?![\d]|\.\d)")
_range = re.compile(rf"(?<![\w.§\-])({_TID})\s*[–-]\s*({_TID})(?![\d]|\.\d)")
_pkg = re.compile(r"(?<![\w.§/\-×])(\d{1,2}\.\d{1,2})(?![\d]|\.\d)")
_SHIELD = re.compile(
    r"СВТ 2\.0|эталон\w*\s+2\.7|этап\w*\s+2\.7|сервис\w*\s+2\.5|релиз\w*\s+\d+\.\d+\.\d+|SVT-\d+"
    r"|§\s?[\d.]+[а-яё]?(?:–[\d.]+[а-яё]?)?|документ\w*\s+2\.0|v\d+(?:\.\d+)*"
    r"|\(\d+(?:\.\d+)? / \d+(?:\.\d+)? / [^)]*\)|×\s?\d+(?:\.\d+)?|(?<![\d.])\d+(?:\.\d+)? / \d+(?:\.\d+)?(?![\d.])"
)


def _key(t: str) -> tuple[int, ...]:
    return tuple(int(x) for x in t.split("."))


def remap_text(text: str) -> str:
    if not text:
        return text
    found: list[str] = []

    def shield(m: re.Match) -> str:
        found.append(m.group(0))
        return f"\u0000{len(found) - 1}\u0000"

    text = _SHIELD.sub(shield, text)

    def rng(m: re.Match) -> str:
        a, b = m.group(1), m.group(2)
        if a not in TASKS or b not in TASKS:
            return m.group(0)
        pa, ia = a.rsplit(".", 1)
        pb, ib = b.rsplit(".", 1)
        olds = [f"{pa}.{i}" for i in range(int(ia), int(ib) + 1)] if pa == pb else []
        news = sorted((TASKS[o] for o in olds if o in TASKS), key=_key)
        na, nb = TASKS[a], TASKS[b]
        if news and news[0] == na and news[-1] == nb and na.rsplit(".", 1)[0] == nb.rsplit(".", 1)[0] \
                and _key(nb)[-1] - _key(na)[-1] + 1 == len(olds):
            return f"\u0001{na}\u2013{nb}\u0001"
        return "\u0001" + ", ".join(TASKS.get(o, o) for o in olds) + "\u0001"

    text = _range.sub(rng, text)

    def one(m: re.Match) -> str:
        return TASKS.get(m.group(1), m.group(1))

    def onepkg(m: re.Match) -> str:
        p = m.group(1)
        if p not in PACKAGES:
            return p
        return PACKAGES[p][0] if len(PACKAGES[p]) == 1 else SPLIT_CHOICE.get(p, p)

    parts = text.split("\u0001")
    for i in range(0, len(parts), 2):
        parts[i] = _pkg.sub(onepkg, _task.sub(one, parts[i]))
    text = "".join(parts)
    return re.sub(r"\u0000(\d+)\u0000", lambda m: found[int(m.group(1))], text)


def renumber_rows(rows, extra_rows):
    """Строки с номерами вех 1–14 → номера вех 1–63; снятые задачи убираются, новые добавляются."""
    out = []
    for r in rows:
        if r.item in DELETED:
            continue
        new = TASKS[r.item]
        pkg = new.rsplit(".", 1)[0]
        out.append(replace(r, stage=pkg.split(".")[0], backlog=pkg, item=new,
                           title=remap_text(r.title), note=remap_text(r.note)))
    out.extend(extra_rows)
    seen = [r.item for r in out]
    if len(seen) != len(set(seen)):
        raise RuntimeError("повтор номера после перенумерации")
    out.sort(key=lambda r: _key(r.item))
    return out
