# -*- coding: utf-8 -*-
"""Четвёртая перенумерация строк генератора (этап 3, 27.11.2026): слой `cut3_rows` после `cut2_rows`.

Цепочка: build_rows → wbs_layout → renumber → renumber_cut → renumber_cut2 → renumber_cut3.
Вход — строки в нумерации вех 1–16 ДО этапа 2 (то есть «текущие номера» реестра restructure_plan_cut3.json);
выход — те же строки в нумерации канона backlog.md на 27.11.2026 (вехи 1–16, пакетов 85, задач 619).

Источники решений (слой не придумывает ничего сверх них):
  * renumber_map_cut3.json — карта «старый → новый» (tasks, deleted_tasks, new_tasks, packages);
  * restructure_plan_cut3.json — реестр из 26 операций; здесь реализованы ТОЛЬКО операции, которые касаются строк
    генератора (номера задач, названия, тройки, статусы). Операции над заголовками пакетов, вводными абзацами и
    критериями (op 1, 9, 10, 17, 18 в части подписи, 20) в строках генератора не отражаются: пакетов и вех как
    отдельных сущностей в Row нет, есть только колонки stage и backlog (номер пакета), их переводит карта.
Скрипт этапа 2 `restructure_cut3.py` здесь НЕ запускается; из него импортируются только функции ремапа текста
(remap_text3, _shield, _unshield) — логика ремапа не дублируется (см. README).

Если канон правят позже, слой не подгоняет числа: он реализует операции реестра и падает (RuntimeError), если строка
генератора не совпала с тем, что реестр ожидает (название «from», тройка, статус).
"""
from __future__ import annotations

import json
import re
from dataclasses import replace
from decimal import Decimal
from pathlib import Path

HERE = Path(__file__).resolve().parent
MAP_PATH = HERE / "renumber_map_cut3.json"
PLAN_PATH = HERE / "restructure_plan_cut3.json"

_TASK_ID = re.compile(r"(?<![\d.])(\d+\.\d+\.\d+)(?![\d]|\.\d)")
_DOUBLE_LABEL = re.compile(r"^\[(?:Overtime|SVT-\d+)\] \[(?:Overtime|SVT-\d+)\]")
_TRIPLE = re.compile(r"\((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) / [^)]*\)")


def _key(t: str) -> tuple[int, ...]:
    return tuple(int(x) for x in t.split("."))


def _ids(s: str, n: int | None = None) -> list[str]:
    out = _TASK_ID.findall(s)
    if n is not None and len(out) != n:
        raise RuntimeError(f"в строке реестра «{s}» ожидалось {n} номеров задач, найдено {out}")
    return out


def load_inputs() -> tuple[dict, dict]:
    m = json.loads(MAP_PATH.read_text(encoding="utf-8"))
    p = json.loads(PLAN_PATH.read_text(encoding="utf-8"))
    ids = sorted(o["id"] for o in p["ops"])
    if ids != list(range(1, 27)):
        raise RuntimeError(f"id операций реестра {ids} ≠ 1–26")
    return m, p


# ---------------------------------------------------------------------------------------------
# Ремап текста (title, note): импорт из restructure_cut3.py, состояние renumber_cut2 сохраняется и восстанавливается
# ---------------------------------------------------------------------------------------------
class _TextRemap:
    """Контекст: на время работы подставляет карту cut3 в renumber_cut2._S и возвращает прежнюю карту обратно.

    Импорт restructure_cut3 при первом обращении очищает renumber_cut2._S (так задумано в том скрипте), поэтому слой
    импортирует его лениво и после того, как cut2_rows уже отработал; прежнее состояние сохраняется и возвращается.
    """

    def __init__(self, mapd: dict) -> None:
        self.mapd = mapd

    def __enter__(self):
        import renumber_cut2 as C
        self.C = C
        self.saved = dict(C._S)
        import restructure_cut3 as R3  # noqa: E402  (ленивый импорт, см. docstring)
        self.R3 = R3
        C._S.clear()
        C.set_map(self.mapd)
        return self

    def __exit__(self, *exc) -> None:
        self.C._S.clear()
        self.C._S.update(self.saved)

    def text(self, s: str, markers: list[str], ctx: str, notes: list[str]) -> str:
        if not s:
            return s
        shielded, stash = self.R3._shield(s, "")
        out = self.R3.remap_text3(shielded, markers, ctx, notes)
        return self.R3._unshield(out, stash)


# ---------------------------------------------------------------------------------------------
# Правила слоя: каждое — отдельная функция «реестр op N»
# ---------------------------------------------------------------------------------------------
def _rename_prefix(title: str, frm: str, to: str, what: str) -> str:
    if not title.startswith(frm):
        raise RuntimeError(f"{what}: название «{title}» не начинается с «{frm}» (реестр)")
    return to + title[len(frm):]


def _mark_done(r):
    """Статус Done: сделано = полная оценка (как у прочих Done-строк генератора)."""
    return replace(r, status="Done", done_no_ai=r.no_ai, done_with_ai=r.with_ai)


def op4_check_251(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 4: задача 2.5.1 → [x]. Тройка не меняется."""
    o = next(x for x in plan["ops"] if x["id"] == 4)
    old = _ids(o["what"], 1)[0]
    new = M["tasks"][old]
    r = rows[new]
    if r.status == "Done":
        raise RuntimeError(f"op4: {new} уже Done")
    rows[new] = _mark_done(r)
    report.append(f"op4: {new} Todo → Done (тройка {r.no_ai} / {r.with_ai} не менялась)")


def op6_op7_rename_labels(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 6, 7: метки [SVT-5097] и [Overtime] у бывших 4.5.5 и 4.5.6 (теперь 3.5.5, 3.5.6).

    Ставятся здесь, а не в OVERTIME_ITEMS/JIRA_ITEMS: те правила остаются в нумерации 1–63 (15.5.5 → JIRA, 15.5.6 → Overtime)."""
    for oid in (6, 7):
        o = next(x for x in plan["ops"] if x["id"] == oid)
        new = M["tasks"][_ids(o["what"], 1)[0]]
        r = rows[new]
        rows[new] = replace(r, title=_rename_prefix(r.title, o["from"], o["to"], f"op{oid}"))
        report.append(f"op{oid}: {new} «{r.title}» → «{rows[new].title}»")


def op13_rename_5322(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 13: задача [SVT-5322] (бывшая 5.4.13) — название без «— грид и API»; тройка и примечание сохраняются."""
    o = next(x for x in plan["ops"] if x["id"] == 13)
    new = M["tasks"][_ids(o["what"], 1)[0]]
    r = rows[new]
    rows[new] = replace(r, title=_rename_prefix(r.title, o["from"], o["to"], "op13"))
    report.append(f"op13: {new} → «{rows[new].title}»")


def op24_edit_iis(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 24: бывшая 6.23.1 (теперь 3.9.7): метка [Overtime], тройка 1 / 1, статус [x], примечание снимается."""
    o = next(x for x in plan["ops"] if x["id"] == 24)
    old = _ids(o["what"], 1)[0]
    new = M["tasks"][old]
    r = rows[new]
    ed = o["edit"]
    chg = next(c for c in plan["estimates"]["changed"] if c["item"] == old)
    a, b = (Decimal(x.strip()) for x in chg["from"].split("/"))
    if (Decimal(str(r.no_ai)), Decimal(str(r.with_ai))) != (a, b):
        raise RuntimeError(f"op24: тройка {new} {r.no_ai} / {r.with_ai} ≠ {chg['from']} (реестр)")
    if r.status == "Done":
        raise RuntimeError(f"op24: {new} уже Done")
    need = re.search(r"«([^»]+)»", ed["drop_note"]).group(1)
    if need not in r.note:
        raise RuntimeError(f"op24: примечание «{r.note}» не содержит «{need}»")
    m = _TRIPLE.fullmatch(ed["triple"])
    if not m:
        raise RuntimeError(f"op24: не разобрана тройка «{ed['triple']}»")
    if r.title.startswith("["):
        raise RuntimeError(f"op24: у {new} уже есть метка: «{r.title}»")
    r2 = replace(r, title=f"{ed['title_mark']} {r.title}", no_ai=float(m.group(1)), with_ai=float(m.group(2)), note="")
    rows[new] = _mark_done(r2)
    report.append(f"op24: {new} «{r.title}» {r.no_ai} / {r.with_ai} Todo → «{r2.title}» {r2.no_ai} / {r2.with_ai} Done; "
                  f"примечание «{r.note}» снято")


def op26_rename_drafts(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 26: бывшая 3.1.7 (теперь 3.9.8) — новое название целиком; тройка и примечание сохраняются.

    В названии генератора после «from» стоит хвост « §2б» (в backlog.md его не было). Название задаётся реестром
    полностью, поэтому хвост не переносится — об этом строка в отчёте."""
    o = next(x for x in plan["ops"] if x["id"] == 26)
    new = M["tasks"][_ids(o["what"], 1)[0]]
    r = rows[new]
    tail = r.title[len(o["from"]):] if r.title.startswith(o["from"]) else None
    if tail is None:
        raise RuntimeError(f"op26: название «{r.title}» не начинается с «{o['from']}» (реестр)")
    rows[new] = replace(r, title=o["to"])
    report.append(f"op26: {new} → «{o['to']}»" + (f"; хвост «{tail}» из названия генератора не перенесён" if tail else ""))


def op22_add_guides(rows: dict, plan: dict, M: dict, report: list[str]) -> None:
    """реестр op 22: 18 новых задач — «Руководство администратора» и «Руководство пользователя» (2 / 1 / —), Todo.

    Номера и названия — new_tasks карты; тройка — из реестра. В Row, кроме названия, тройки и статуса, колонки только
    «примечание» — у аналогичных строк ОПЭ-пакетов оно разное по смыслу, поэтому у новых задач пусто (в каноне примечания нет)."""
    o = next(x for x in plan["ops"] if x["id"] == 22)
    trip = {}
    for s in o["tasks"]:
        mm = re.match(r"^(.*?) \((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) / [^)]*\)$", s)
        if not mm:
            raise RuntimeError(f"op22: не разобрана задача реестра «{s}»")
        trip[mm.group(1)] = (float(mm.group(2)), float(mm.group(3)))
    new_tasks = M["new_tasks"]
    if len(new_tasks) != o["new_tasks"]:
        raise RuntimeError(f"op22: в карте {len(new_tasks)} новых задач, в реестре {o['new_tasks']}")
    for item, title in new_tasks.items():
        base = title.split(" (Шаг:")[0]
        if base not in trip:
            raise RuntimeError(f"op22: у новой задачи {item} «{title}» нет тройки в реестре")
        if item in rows:
            raise RuntimeError(f"op22: номер {item} уже занят")
        pkg = item.rsplit(".", 1)[0]
        no_ai, with_ai = trip[base]
        rows[item] = replace(next(iter(rows.values())), stage=pkg.split(".")[0], backlog=pkg, item=item, title=title,
                             status="Todo", no_ai=no_ai, with_ai=with_ai, done_no_ai=0.0, done_with_ai=0.0, note="")
    report.append(f"op22: добавлено {len(new_tasks)} задач (2 / 1, Todo); колонка «примечание» пуста — в каноне примечания нет")


# ---------------------------------------------------------------------------------------------
# Слой
# ---------------------------------------------------------------------------------------------
def cut3_rows(rows, markers: list[str] | None = None, notes: list[str] | None = None, report: list[str] | None = None):
    """Строки генератора (вехи 1–16 до этапа 2) → строки в нумерации канона 27.11.2026.

    Шаги: (1) удалить строки deleted_tasks (op 23); (2) перевести item/backlog/stage по карте tasks и прогнать
    title/note через remap_text3; (3) правки op 4, 6, 7, 13, 24, 26; (4) добавить 18 строк op 22; (5) проверки и сортировка.
    Строка, которой нет в карте, и дубль номера — RuntimeError (как у прежних слоёв)."""
    M, P = load_inputs()
    markers = markers if markers is not None else []
    notes = notes if notes is not None else []
    report = report if report is not None else []
    tasks, deleted = M["tasks"], M["deleted_tasks"]

    # op 23: удаление (id берутся из реестра и сверяются с картой)
    op23 = next(x for x in P["ops"] if x["id"] == 23)
    if set(_ids(op23["what"])) != set(deleted):
        raise RuntimeError(f"op23: задачи реестра {_ids(op23['what'])} ≠ deleted_tasks карты {sorted(deleted)}")

    out: dict[str, object] = {}
    with _TextRemap(M) as tr:
        for r in rows:
            if r.item in deleted:
                report.append(f"op23: удалена {r.item} «{r.title}» ({r.no_ai} / {r.with_ai})")
                continue
            if r.item not in tasks:
                raise RuntimeError(f"пункт {r.item} есть в генераторе, но не в карте renumber_map_cut3.json")
            new = tasks[r.item]
            if new in out:
                raise RuntimeError(f"повтор номера после четвёртой перенумерации: {new}")
            pkg = new.rsplit(".", 1)[0]
            out[new] = replace(
                r, stage=pkg.split(".")[0], backlog=pkg, item=new,
                title=tr.text(r.title, markers, f"{r.item} title", notes),
                note=tr.text(r.note, markers, f"{r.item} note", notes),
            )
    missing = sorted(set(tasks) - {x.item for x in rows}, key=_key)
    if missing:
        raise RuntimeError(f"в карте есть пункты, которых нет в генераторе: {missing}")

    for rule in (op4_check_251, op6_op7_rename_labels, op13_rename_5322, op24_edit_iis, op26_rename_drafts, op22_add_guides):
        rule(out, P, M, report)

    bad = [k for k, r in out.items() if _DOUBLE_LABEL.match(r.title)]
    if bad:
        raise RuntimeError(f"две метки подряд в названии: {bad}")
    return sorted(out.values(), key=lambda r: _key(r.item))
