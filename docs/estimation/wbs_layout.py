# -*- coding: utf-8 -*-
"""Очередь вех СВТ: прежние id MVP → веха.пакет.задача.

build_rows() по-прежнему собирает строки со старыми id.
apply_new_wbs() назначает номера вех 1–14 и раскладывает комплекты вывода.
"""
from __future__ import annotations

import re
from dataclasses import dataclass, replace
from decimal import Decimal, ROUND_DOWN

DOMAINS = (
    "Ставки",
    "Паритеты",
    "Маршруты",
    "Продукты",
    "Склады и ограничения",
    "Матрица каналов",
    "Интеграция с ЦБД",
    "Вспомогательные",
)
WRITE_DOMAINS = DOMAINS[:7]

# Прежняя веха → новая. 1.0/1.1/1.2/2.0 в тексте дорожной карты бывают номерами разделов.
MILESTONE = {
    "0.0": "1",
    "0.1": "2",
    "0.2": "3",
    "0.9": "4",
    "0.3": "5",
    "0.4": "6",
    "0.5": "7",
    "0.6": "8",
    "0.7": "9",
    "0.8": "10",
    "1.0": "11",
    "1.1": "12",
    "1.2": "13",
    "2.0": "14",
}

REQ_ORDER = [
    "0.9.1.1", "0.9.1.2", "0.9.1.3", "0.9.1.4", "0.9.1.5", "0.9.1.6", "0.9.1.7",
    "0.9.1.15", "0.9.1.16", "0.9.1.17", "0.9.1.18",
    "0.9.1.12",
    "0.9.1.8", "0.9.1.9",
    "0.9.1.13",
    "0.9.1.10",
    "0.9.1.14",
    "0.9.1.11",
]

KEEP_STAGE1 = [
    "0.0.9", "0.0.10", "0.0.1", "0.0.2", "0.0.3", "0.0.4",
    "0.0.5", "0.0.6", "0.0.7", "0.0.8", "0.0.11",
]


@dataclass
class Item:
    old_id: str
    title: str
    note: str
    no_ai: float
    with_ai: float
    status: str
    done_no_ai: float = 0.0
    done_with_ai: float = 0.0
    mark: str = "[ ]"
    new_id: str = ""
    package: str = ""
    stage: str = ""
    src_pkg: str = ""
    pkg_title: str = ""


def fmt_days(n: float) -> str:
    q = Decimal(str(n)).quantize(Decimal("0.1"))
    if q == q.to_integral():
        return str(int(q))
    return f"{q:.1f}"


def split_even(total: float, n: int) -> list[float]:
    t = Decimal(str(total))
    q = (t / Decimal(n)).quantize(Decimal("0.1"), rounding=ROUND_DOWN)
    vals = [q] * (n - 1)
    last = (t - q * (n - 1)).quantize(Decimal("0.1"))
    if sum(vals, Decimal("0")) + last != t:
        raise RuntimeError(f"split {total} / {n} -> {vals} + {last}")
    vals.append(last)
    return [float(v) for v in vals]


def pkg_of(old_id: str) -> str:
    return ".".join(old_id.split(".")[:-1])


def id_tuple(old_id: str) -> tuple[int, ...]:
    return tuple(int(p) for p in old_id.split(".") if p.isdigit())


def domain_rank(it: Item) -> int:
    blob = f"{it.note} {it.title}"
    if "Доработка фреймворка" in it.title or it.title.startswith("Перенос purge"):
        return 99
    if "волна записи Ставок" in blob:
        return 0
    for i, name in enumerate(DOMAINS):
        if it.note.startswith(name) or f"— {name};" in it.title or f"— {name};" in it.note:
            return i
    return 50


def _stamp(it: Item, new_id: str, src_pkg: str = "", pkg_title: str = "") -> Item:
    package = pkg_of(new_id)
    stage = package.split(".")[0]
    return replace(
        it,
        new_id=new_id,
        package=package,
        stage=stage,
        src_pkg=src_pkg or it.src_pkg,
        pkg_title=pkg_title or it.pkg_title,
    )


def _seq(items: list[Item], new_pkg: str, src_pkg: str = "", pkg_title: str = "") -> list[Item]:
    out = []
    for i, it in enumerate(items, 1):
        out.append(_stamp(it, f"{new_pkg}.{i}", src_pkg=src_pkg, pkg_title=pkg_title))
    return out


def _share_note(i: int, n: int, a: float, b: float, last_name: str, extra: str = "") -> str:
    note = f"доля 1/{n} прежней суммы {fmt_days(a)} / {fmt_days(b)}"
    if i == n - 1:
        note += f"; остаток округления до одной десятой на этом комплекте ({last_name})"
    if extra:
        note += "; " + extra
    return note


def _step_of(title: str) -> str:
    m = re.search(r"\(Шаг(?:и)?: [^)]+\)", title)
    return m.group(0) if m else ""


def _synth(
    old_key: str,
    title: str,
    step: str,
    no_ai: float,
    with_ai: float,
    note: str,
    new_id: str,
    pkg_title: str,
) -> Item:
    full = title if not step else f"{title} {step}"
    it = Item(
        old_id=old_key,
        title=full.strip(),
        note=note,
        no_ai=no_ai,
        with_ai=with_ai,
        status="Todo",
        mark="[ ]",
        pkg_title=pkg_title,
    )
    if not new_id:
        return it
    return _stamp(it, new_id, pkg_title=pkg_title)


def new_layout(items: list[Item]) -> list[Item]:
    by_id = {it.old_id: it for it in items}
    index = {it.old_id: i for i, it in enumerate(items)}

    def children(pkg: str, *, domain_sort: bool = False) -> list[Item]:
        xs = [it for it in items if pkg_of(it.old_id) == pkg]
        if domain_sort:
            xs.sort(key=lambda it: (domain_rank(it), id_tuple(it.old_id)))
        else:
            xs.sort(key=lambda it: index[it.old_id])
        return xs

    def keep(old_pkg: str, new_pkg: str) -> list[Item]:
        return _seq(children(old_pkg), new_pkg, src_pkg=old_pkg)

    out: list[Item] = []

    for i, old in enumerate(KEEP_STAGE1, 1):
        out += keep(old, f"1.{i}")
    for i, old in enumerate(
        ["0.1.1", "0.1.2", "0.1.3", "0.1.4", "0.1.5", "0.1.6", "0.1.7", "0.1.8"], 1
    ):
        out += keep(old, f"2.{i}")

    out += keep("0.2.1", "3.1")
    out += keep("0.2.2", "3.2")
    out += _seq(children("0.2.3", domain_sort=True), "3.3", src_pkg="0.2.3")

    channel_title = "Канал обратной связи (Шаг: Наблюдение)"
    out += _seq(
        [replace(by_id["0.7.5.1"], pkg_title=channel_title)],
        "3.4",
        pkg_title=channel_title,
    )

    hand = by_id["2.0.3.1"]
    acc = by_id["2.0.3.3"]
    hab = by_id["0.7.5.2"]
    # Order of the 10 shares: RO, заявки, 7 доменов записи, вспомогательные.
    # Remainder of rounding sits on the last share (вспомогательные).
    ten_labels = (
        ["волна чтения", "домен «Заявки»"]
        + [f"домен «{name}»" for name in WRITE_DOMAINS]
        + ["домен «Вспомогательные»"]
    )
    assert len(ten_labels) == 10
    ha, hb = split_even(hand.no_ai, 10), split_even(hand.with_ai, 10)
    aa, ab = split_even(acc.no_ai, 10), split_even(acc.with_ai, 10)
    pa, pb = split_even(hab.no_ai, 10), split_even(hab.with_ai, 10)

    def kit_items(label: str, idx: int, new_pkg: str, pkg_title: str) -> list[Item]:
        last = "Вспомогательные"
        specs = [
            (
                f"2.0.3.1#{idx}",
                "Пакет передачи в поддержку",
                "(Шаг: Закрытие и изменения)",
                ha[idx], hb[idx],
                _share_note(idx, 10, hand.no_ai, hand.with_ai, last, "регламент, обучение второй линии, логи и мониторинг"),
            ),
            (
                f"2.0.3.3#{idx}",
                "Поддержка забрала решение",
                "(Шаг: Закрытие и изменения)",
                aa[idx], ab[idx],
                _share_note(idx, 10, acc.no_ai, acc.with_ai, last, "типовой инцидент по логам без разработчика"),
            ),
            (
                f"0.7.5.2#{idx}",
                "Замер приживаемости",
                "(Шаг: Наблюдение)",
                pa[idx], pb[idx],
                _share_note(idx, 10, hab.no_ai, hab.with_ai, last, "контроль метрик перехода; цифру без источника не ставить"),
            ),
        ]
        made = []
        # number them after whatever is already in the package — caller passes start
        return [
            _synth(key, f"{title}, {label}", step, n, w, note, "", pkg_title)
            for key, title, step, n, w, note in specs
        ]

    # RO kit is its own package 3.5; stamp ids here.
    ro_bits = kit_items("волна чтения", 0, "3.5", "Вывод волны чтения (Шаги: Закрытие и изменения, Наблюдение)")
    ro_title = "Вывод волны чтения (Шаги: Закрытие и изменения, Наблюдение)"
    out += _seq(ro_bits, "3.5", pkg_title=ro_title)
    out += keep("0.2.4", "3.6")

    req = []
    for oid in REQ_ORDER:
        it = by_id[oid]
        if oid == "0.9.1.15":
            it = replace(
                it,
                note=re.sub(r"Номер пункта не значит, что работа после релиза:\s*", "", it.note),
            )
        req.append(it)
    out += _seq(req, "4.1", src_pkg="0.9.1")
    req_bits = kit_items("домен «Заявки»", 1, "4.2", "Вывод домена «Заявки» (Шаги: Закрытие и изменения, Наблюдение)")
    out += _seq(req_bits, "4.2", pkg_title="Вывод домена «Заявки» (Шаги: Закрытие и изменения, Наблюдение)")

    for i, old in enumerate(["0.3.1", "0.3.2"], 1):
        out += keep(old, f"5.{i}")
    for i, old in enumerate(["0.4.1", "0.4.2"], 1):
        out += keep(old, f"6.{i}")
    for i, old in enumerate(
        ["0.5.1", "0.5.2", "0.5.3", "0.5.4", "0.5.5", "0.5.6", "0.5.7", "0.5.8", "0.5.9"], 1
    ):
        out += keep(old, f"7.{i}")
    for i, old in enumerate(["0.6.1", "0.6.2", "0.6.3", "0.6.4", "0.6.5", "0.6.6"], 1):
        out += keep(old, f"8.{i}")

    out += keep("0.7.1", "9.1")
    write_rows = children("0.7.2", domain_sort=True)
    by_domain: dict[int, list[Item]] = {i: [] for i in range(7)}
    framework: list[Item] = []
    for it in write_rows:
        rank = domain_rank(it)
        if rank == 99:
            framework.append(it)
        elif rank < 7:
            by_domain[rank].append(it)
        else:
            raise RuntimeError(f"нет домена у {it.old_id}: {it.title} / {it.note}")

    gates = [by_id[f"0.7.3.{n}"] for n in (1, 2, 3, 5, 6, 7)]
    splits = [split_even(g.no_ai, 7) for g in gates]
    splits_w = [split_even(g.with_ai, 7) for g in gates]
    gate_titles = [
        ("Gate A", "(Шаг: Качество и приёмка)", "не календарь ОПЭ; чел.-дни не пересчитаны"),
        ("Gate B", "(Шаг: Внедрение)", "серийно по календарю; чел.-дни не пересчитаны"),
        ("Gate C (SoT Flip Decision Record)", "(Шаг: Внедрение)", "чел.-дни не пересчитаны"),
        ("Заключение о готовности до пуска пользователей", "(Шаг: Качество и приёмка)", "до Gate B"),
        ("Обучение пользователей и сценарий отката", "(Шаг: Внедрение)", "репетиция отката — только на первом флипе «Ставок»"),
        ("Подключить кнопку «Сообщить об ошибке» к экранам домена", "(Шаг: Разработка)", "фреймворк кнопки — в вехе заявок"),
    ]
    for d, name in enumerate(WRITE_DOMAINS):
        label = f"домен «{name}»"
        pkg = f"9.{d + 2}"
        title = f"Домен «{name}»: редакторы, загрузчики и вывод"
        body = list(by_domain[d])
        if name == "Ставки":
            body.append(by_id["0.7.3.4"])
        for g, (gt, step, extra), col, colw in zip(gates, gate_titles, splits, splits_w):
            body.append(
                _synth(
                    f"{g.old_id}#{d}",
                    f"{gt}, {label}",
                    step,
                    col[d],
                    colw[d],
                    _share_note(d, 7, g.no_ai, g.with_ai, "Интеграция с ЦБД", extra),
                    "",
                    title,
                )
            )
        ten_idx = d + 2
        body += kit_items(label, ten_idx, pkg, title)
        out += _seq(body, pkg, pkg_title=title)

    fw_title = "Доработка фреймворка и purge на тираже записи (Шаг: Разработка)"
    out += _seq(framework, "9.9", pkg_title=fw_title)
    out += keep("0.7.4", "9.10")

    out += keep("0.8.1", "10.1")
    out += keep("0.8.2", "10.2")
    aux_title = (
        "Gate A/B/C и вывод вспомогательных "
        "(Шаги: Качество и приёмка, Внедрение, Разработка, Закрытие и изменения, Наблюдение)"
    )
    aux = children("0.8.3")
    aux += kit_items("домен «Вспомогательные»", 9, "10.3", aux_title)
    out += _seq(aux, "10.3", src_pkg="0.8.3", pkg_title=aux_title)
    out += keep("0.8.4", "10.4")
    out += keep("0.8.5", "10.5")

    out += keep("1.0.1", "11.1")
    out += keep("1.1.1", "12.1")
    out += keep("1.2.1", "13.1")
    out += keep("2.0.1", "14.1")
    out += keep("2.0.2", "14.2")
    lessons_title = "Уроки программы (Шаг: Закрытие и изменения)"
    out += _seq([replace(by_id["2.0.3.2"], pkg_title=lessons_title)], "14.3", pkg_title=lessons_title)

    seen = [it.old_id for it in out if "#" not in it.old_id and it.old_id in by_id]
    missing = set(by_id) - set(seen)
    # aggregates are consumed, not copied
    consumed = {
        "2.0.3.1", "2.0.3.3", "0.7.5.2",
        "0.7.3.1", "0.7.3.2", "0.7.3.3", "0.7.3.5", "0.7.3.6", "0.7.3.7",
    }
    missing -= consumed
    if missing:
        raise RuntimeError(f"не разложены: {sorted(missing)[:12]}")
    return _remap_notes(out)


def id_maps(laid: list[Item]) -> tuple[dict[str, str], dict[str, str]]:
    """task/package map, and milestone map (0.x plus 1.0/1.1/1.2/2.0)."""
    task: dict[str, str] = {}
    pkg: dict[str, str] = {}
    for it in laid:
        if it.old_id in task or "#" in it.old_id:
            continue
        task[it.old_id] = it.new_id
        if it.src_pkg:
            pkg[it.src_pkg] = it.package
    # dissolved packages: point at the first place the work now lives
    pkg.setdefault("0.7.2", "9.2")
    pkg.setdefault("0.7.3", "9.2")
    pkg.setdefault("0.7.5", "3.4")
    pkg.setdefault("2.0.3", "3.5")
    # split tasks: first kit (волна чтения or Ставки)
    first_of: dict[str, str] = {}
    for it in laid:
        base = it.old_id.split("#", 1)[0]
        if "#" in it.old_id and base not in first_of:
            first_of[base] = it.new_id
    task.update(first_of)
    return {**pkg, **task}, dict(MILESTONE)


def _remap_notes(laid: list[Item]) -> list[Item]:
    task, mile = id_maps(laid)
    out = []
    for it in laid:
        out.append(
            replace(
                it,
                title=replace_ids(it.title, task, mile, late=True),
                note=replace_ids(it.note, task, mile, late=True),
            )
        )
    return out


_SHIELD = re.compile(
    r"СВТ 2\.0|эталон(?:а|е|у|ом|ы)?\s+2\.7|релиз\s+2\.3\.\d+|SVT-\d+"
    r"|§[\d.]+[а-яё]?(?:–[\d.]+[а-яё]?)?"
    r"|документ(?:а|е|у|ом)?\s+2\.0"
)


def _shield(text: str) -> tuple[str, list[str]]:
    found: list[str] = []

    def sub(m: re.Match) -> str:
        found.append(m.group(0))
        return f"\u0000{len(found) - 1}\u0000"

    return _SHIELD.sub(sub, text), found


def _unshield(text: str, found: list[str]) -> str:
    return re.sub(r"\u0000(\d+)\u0000", lambda m: found[int(m.group(1))], text)


def _etap_word(suffix: str) -> str:
    return {
        "": "веха",
        "а": "вехи",
        "е": "вехе",
        "у": "вехе",
        "ом": "вехой",
        "ы": "вехи",
        "ов": "вех",
        "ами": "вехами",
    }[suffix]


def replace_ids(text: str, task: dict[str, str], mile: dict[str, str], *, late: bool) -> str:
    if not text:
        return text
    text, shielded = _shield(text)
    text = text.replace("MVP 0.5+", "вехи 7")
    text = text.replace("MVP 0.4+", "вехи 6")
    text = text.replace("итог MVP 2.0", "итог вехи 14")

    def rng(m: re.Match) -> str:
        a, b = m.group(1), m.group(2)
        if a in mile and b in mile:
            return f"{mile[a]}–{mile[b]}"
        return m.group(0)

    def sl(m: re.Match) -> str:
        a, b = m.group(1), m.group(2)
        if a in mile and b in mile:
            return f"{mile[a]}/{mile[b]}"
        return m.group(0)

    # Не захватывать хвост длинного id: в «0.0.1–0.0.4» фрагмент «0.1–0.0» не диапазон вех.
    text = re.sub(
        r"(?<![\d.])(0\.\d|1\.\d|2\.0)–(0\.\d|1\.\d|2\.0)(?![\d.])",
        rng,
        text,
    )
    text = re.sub(
        r"(?<![\d.])(0\.\d|1\.\d|2\.0)/(0\.\d|1\.\d|2\.0)(?![\d.])",
        sl,
        text,
    )

    keys = set(task)
    for k, v in mile.items():
        if k.startswith("0."):
            keys.add(k)
        elif late:
            keys.add(k)
    # longest first so 0.2.3.61 wins over 0.2.3 and 0.2
    ordered = sorted(keys, key=len, reverse=True)
    if ordered:
        pat = re.compile(r"\b(?:" + "|".join(re.escape(k) for k in ordered) + r")\b")

        def one(m: re.Match) -> str:
            s = m.group(0)
            if s in task:
                return task[s]
            return mile.get(s, s)

        text = pat.sub(one, text)

    late_ids = {"1.0": "11", "1.1": "12", "1.2": "13", "2.0": "14"}

    def late_pref(m: re.Match) -> str:
        word, num = m.group(1), m.group(2)
        if word == "MVP":
            return "веха " + late_ids[num]
        if word.startswith("этап"):
            suf = word[len("этап"):]
            return _etap_word(suf) + " " + late_ids[num]
        if word.startswith("вех"):
            return word + " " + late_ids[num]
        return word + " " + late_ids[num]

    text = re.sub(
        r"(MVP|этап(?:а|е|у|ом|ы|ов|ами)?|вех[аеиоуы]?|backlog|пакет(?:а|е|у|ом|ы|ов)?|пункт(?:а|е|у|ом|ы|ов)?)\s+(1\.0|1\.1|1\.2|2\.0)(?![\d.])",
        late_pref,
        text,
    )
    text = re.sub(r"MVP\s+(\d+)", r"веха \1", text)
    def etap_sub(m: re.Match) -> str:
        word = _etap_word(m.group(2) or "")
        if m.group(1).isupper():
            return word[:1].upper() + word[1:]
        return word

    text = re.sub(
        r"(?<![А-Яа-яA-Za-z])([Ээ])тап(а|е|у|ом|ы|ов|ами)?(?![А-Яа-яA-Za-z])",
        etap_sub,
        text,
    )
    text = re.sub(r"\bв веха (\d+)", r"в вехе \1", text)
    text = re.sub(r"\bконец веха (\d+)", r"конец вехи \1", text)
    for src, dst in (
        ("этого вехи", "этой вехи"),
        ("этому вехе", "этой вехе"),
        ("этом вехе", "этой вехе"),
        ("этот веха", "эта веха"),
        ("этим вехой", "этой вехой"),
        ("Этого вехи", "Этой вехи"),
        ("Этому вехе", "Этой вехе"),
        ("Этом вехе", "Этой вехе"),
        ("Этот веха", "Эта веха"),
    ):
        text = text.replace(src, dst)
    return _unshield(text, shielded)


def apply_new_wbs(rows):
    """Row from generate_effort_estimate → rows with new ids. Import is local to avoid a cycle."""
    items = []
    for r in rows:
        title = r.title.replace("(Веха:", "(Шаг:").replace("(Вехи:", "(Шаги:")
        mark = "[x]" if r.status == "Done" else "[ ]"
        items.append(
            Item(
                old_id=r.item,
                title=title,
                note=r.note,
                no_ai=r.no_ai,
                with_ai=r.with_ai,
                status=r.status,
                done_no_ai=r.done_no_ai,
                done_with_ai=r.done_with_ai,
                mark=mark,
            )
        )
    laid = new_layout(items)
    # local import of Row would cycle; caller passes a factory via attribute
    from generate_effort_estimate import Row

    out = []
    for it in laid:
        out.append(
            Row(
                it.stage,
                it.package,
                it.new_id,
                it.title,
                it.status,
                it.no_ai,
                it.with_ai,
                it.done_no_ai,
                it.done_with_ai,
                it.note,
            )
        )
    return out
