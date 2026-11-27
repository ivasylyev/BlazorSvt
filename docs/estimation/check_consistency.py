# -*- coding: utf-8 -*-
"""Сверка выгрузки генератора оценок с каноном: backlog.md и roadmap.md (вехи 1–16).

Запуск (после `python docs/estimation/generate_effort_estimate.py`):
    python docs/estimation/check_consistency.py
Код возврата: 0 — расхождений нет; 1 — есть расхождения, которых нет в списке KNOWN ниже.

Источник со стороны генератора — его TSV: 05_item_triples.tsv (пункт, пакет, веха, Без ИИ, С ИИ, статус).
Канон: .cursor/skills/svt-architecture/backlog.md и roadmap.md. Только стандартная библиотека.

Проверки:
  a) множество номеров задач N.M.K совпадает;
  b) тройка «(без ИИ / с ИИ / —)» в названии задачи backlog.md = 05_item_triples.tsv (допуск 0.05);
  c) сумма по пакету = строка `**Оценка:**` пакета (допуск 0.1);
  d) сумма по вехе = `**Оценка:**` вехи и колонки «Без ИИ»/«С ИИ» таблицы вех roadmap.md (допуск 0.1);
     итог = TOTAL_NO_AI / TOTAL_WITH_AI;
  e) статус `[x]/[ ]` в backlog.md = Done/Todo в выгрузке (Partial допустим только при `[ ]`).
Данные не подгоняются: известные расхождения, не связанные с перенумерацией, выведены отдельным разделом.
"""
from __future__ import annotations

import csv
import io
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parent.parent
BACKLOG = ROOT / ".cursor" / "skills" / "svt-architecture" / "backlog.md"
ROADMAP = ROOT / ".cursor" / "skills" / "svt-architecture" / "roadmap.md"
TRIPLES = HERE / "05_item_triples.tsv"
SUMMARY = HERE / "00_summary_by_stage.tsv"

TOL_ITEM = 0.05
TOL_SUM = 0.1
TOTAL_NO_AI = 2144.8
TOTAL_WITH_AI = 998.91
MILESTONES = list(range(1, 17))

# Известные расхождения, возникшие НЕ из-за перенумерации. Ключ — (проверка, номер); значение — почему.
# Ошибкой скрипта они не считаются, но печатаются отдельным разделом. Пополнять только с обоснованием.
# На 27.11.2026 список пуст: оба прежних исключения закрыты решением владельца (статусы 4.10.10–4.10.20 — по
# backlog.md; оценки 9.2.3, 11.2.3, 12.2.3, 13.2.3 — как у 7.2.3).
KNOWN: dict[tuple[str, str], str] = {}

TAIL_HEAD = "## Трассировка задач"
MS_RE = re.compile(r"^# Веха (\d+) — ")
PKG_RE = re.compile(r"^## (\d+)\.(\d+)\. ")
TASK_RE = re.compile(r"^- \[( |x)\] (\d+\.\d+\.\d+)\. (.*)$")
EST_RE = re.compile(r"^\*\*Оценка:\*\* (\S+) / (\S+) / (\S+)\s*$")
TRIPLE_RE = re.compile(r"\((\d+(?:\.\d+)?) / (\d+(?:\.\d+)?) / [^)]*\)")
ROAD_ROW_RE = re.compile(r"^\|\s*\*\*(\d+)\*\*\s*\|")


def key(t: str) -> tuple[int, ...]:
    return tuple(int(x) for x in t.split("."))


def num(s: str) -> float:
    return float(s.replace(",", "."))


def read_text(p: Path) -> str:
    return p.read_text(encoding="utf-8-sig")


# ----------------------------------------------------------------------------------------------
# Разбор канона
# ----------------------------------------------------------------------------------------------
def parse_backlog() -> dict:
    tasks: dict[str, dict] = {}      # N.M.K -> {done, triple|None, line}
    pkg_est: dict[str, tuple[float, float]] = {}
    ms_est: dict[int, tuple[float, float]] = {}
    cur_ms: int | None = None
    cur_pkg: str | None = None
    for i, line in enumerate(read_text(BACKLOG).split("\n"), 1):
        line = line.rstrip("\r")
        if line.startswith(TAIL_HEAD):
            break
        m = MS_RE.match(line)
        if m:
            cur_ms, cur_pkg = int(m.group(1)), None
            continue
        m = PKG_RE.match(line)
        if m:
            cur_pkg = f"{m.group(1)}.{m.group(2)}"
            continue
        m = EST_RE.match(line)
        if m:
            val = (num(m.group(1)), num(m.group(2)))
            if cur_pkg is not None:
                pkg_est[cur_pkg] = val
            elif cur_ms is not None:
                ms_est[cur_ms] = val
            continue
        m = TASK_RE.match(line)
        if m:
            tr = TRIPLE_RE.findall(m.group(3))
            triple = (float(tr[-1][0]), float(tr[-1][1])) if tr else None
            tasks[m.group(2)] = {"done": m.group(1) == "x", "triple": triple, "line": i}
    return {"tasks": tasks, "pkg_est": pkg_est, "ms_est": ms_est}


def parse_roadmap() -> dict:
    rows: dict[int, tuple[float, float]] = {}
    total: tuple[float, float] | None = None
    for line in read_text(ROADMAP).split("\n"):
        m = ROAD_ROW_RE.match(line)
        if m:
            cells = [c.strip().strip("*").strip() for c in line.strip().strip("|").split("|")]
            rows[int(m.group(1))] = (num(cells[3]), num(cells[4]))
        elif "Итого (чел.-дни)" in line:
            cells = [c.strip().strip("*").strip() for c in line.strip().strip("|").split("|")]
            total = (num(cells[3]), num(cells[4]))
    return {"rows": rows, "total": total}


def parse_export() -> dict:
    items: dict[str, dict] = {}
    with TRIPLES.open(encoding="utf-8-sig", newline="") as f:
        for r in csv.DictReader(f, delimiter="\t"):
            items[r["item"]] = {
                "backlog": r["backlog"], "stage": r["stage"], "no_ai": float(r["no_ai"]),
                "with_ai": float(r["with_ai"]), "status": r["status"],
            }
    return items


# ----------------------------------------------------------------------------------------------
# Проверки
# ----------------------------------------------------------------------------------------------
class Report:
    def __init__(self) -> None:
        self.results: list[tuple[str, bool, str]] = []
        self.known: list[tuple[str, str, str, str]] = []
        self.warnings: list[str] = []
        self.used_known: set[tuple[str, str]] = set()

    def add(self, check: str, errors: list[str], info: str) -> None:
        self.results.append((check, not errors, info))
        for e in errors:
            print(f"  [{check}] {e}")

    def split(self, check: str, ident: str, msg: str, errors: list[str]) -> None:
        k = (check, ident)
        if k in KNOWN:
            self.used_known.add(k)
            self.known.append((check, ident, msg, KNOWN[k]))
        else:
            errors.append(f"{ident}: {msg}")


def main() -> int:
    for p in (BACKLOG, ROADMAP, TRIPLES):
        if not p.exists():
            print(f"нет файла: {p}")
            return 1
    bl = parse_backlog()
    rd = parse_roadmap()
    ex = parse_export()
    rep = Report()

    # Выгрузка устарела относительно генератора/карты?
    for src in ("generate_effort_estimate.py", "renumber_cut2.py", "renumber_map_cut2.json"):
        if (HERE / src).stat().st_mtime > TRIPLES.stat().st_mtime + 1:
            rep.warnings.append(f"{src} новее {TRIPLES.name}: возможно, выгрузка не перегенерирована")

    tasks = bl["tasks"]
    print(f"backlog.md: задач {len(tasks)}, пакетов с оценкой {len(bl['pkg_est'])}, вех {len(bl['ms_est'])}; "
          f"выгрузка: пунктов {len(ex)}; roadmap.md: вех {len(rd['rows'])}")

    # a) множества
    errs: list[str] = []
    only_gen = sorted(set(ex) - set(tasks), key=key)
    only_bl = sorted(set(tasks) - set(ex), key=key)
    for t in only_gen:
        rep.split("a", t, "только в генераторе", errs)
    for t in only_bl:
        rep.split("a", t, "только в backlog.md", errs)
    if only_gen:
        print(f"  только в генераторе ({len(only_gen)}): {', '.join(only_gen)}")
    if only_bl:
        print(f"  только в backlog.md ({len(only_bl)}): {', '.join(only_bl)}")
    rep.add("a", errs, f"генератор {len(ex)}, backlog.md {len(tasks)}, общих {len(set(ex) & set(tasks))}")

    common = sorted(set(ex) & set(tasks), key=key)

    # b) тройки
    errs, no_triple, n_ok = [], [], 0
    for t in common:
        tr = tasks[t]["triple"]
        e = ex[t]
        if tr is None:
            no_triple.append(t)
            msg = f"в названии backlog.md нет тройки (выгрузка: {e['no_ai']} / {e['with_ai']})"
            if e["no_ai"] == 0 and e["with_ai"] == 0:
                rep.split("b", t, msg, errs)
            else:  # тройки нет, а в генераторе числа не нулевые — известным исключением не считается
                errs.append(f"{t}: {msg}")
            continue
        if abs(tr[0] - e["no_ai"]) > TOL_ITEM or abs(tr[1] - e["with_ai"]) > TOL_ITEM:
            rep.split("b", t, f"backlog.md {tr[0]} / {tr[1]}, выгрузка {e['no_ai']} / {e['with_ai']}"
                      f" (backlog.md:{tasks[t]['line']})", errs)
        else:
            n_ok += 1
    rep.add("b", errs, f"совпало {n_ok} из {len(common)}; без тройки в backlog.md: {len(no_triple)}")

    # c) пакеты
    errs = []
    pk_sum: dict[str, list[float]] = {}
    for t, e in ex.items():
        pkg = t.rsplit(".", 1)[0]
        s = pk_sum.setdefault(pkg, [0.0, 0.0])
        s[0] += e["no_ai"]
        s[1] += e["with_ai"]
        if e["backlog"] != pkg:
            rep.split("c", t, f"колонка backlog выгрузки {e['backlog']} не совпадает с номером пункта", errs)
    pkgs = sorted(set(pk_sum) | set(bl["pkg_est"]), key=key)
    for p in pkgs:
        if p not in bl["pkg_est"]:
            rep.split("c", p, "пакета нет в backlog.md (`**Оценка:**`)", errs)
        elif p not in pk_sum:
            rep.split("c", p, "пакета нет в выгрузке", errs)
        else:
            b, g = bl["pkg_est"][p], pk_sum[p]
            if abs(b[0] - g[0]) > TOL_SUM or abs(b[1] - g[1]) > TOL_SUM:
                rep.split("c", p, f"backlog.md {b[0]} / {b[1]}, выгрузка {g[0]:.2f} / {g[1]:.2f}", errs)
    rep.add("c", errs, f"пакетов {len(pkgs)}")

    # d) вехи
    errs = []
    ms_sum: dict[int, list[float]] = {}
    for t, e in ex.items():
        s = ms_sum.setdefault(int(t.split(".")[0]), [0.0, 0.0])
        s[0] += e["no_ai"]
        s[1] += e["with_ai"]
        if int(e["stage"]) != int(t.split(".")[0]):
            rep.split("d", t, f"колонка stage выгрузки {e['stage']} не совпадает с номером пункта", errs)
    for m in MILESTONES:
        g = ms_sum.get(m)
        b = bl["ms_est"].get(m)
        r = rd["rows"].get(m)
        if g is None or b is None or r is None:
            rep.split("d", str(m), f"нет данных: выгрузка={g}, backlog.md={b}, roadmap.md={r}", errs)
            continue
        for name, ref in (("backlog.md", b), ("roadmap.md", r)):
            if abs(ref[0] - g[0]) > TOL_SUM or abs(ref[1] - g[1]) > TOL_SUM:
                rep.split("d", f"{m} ({name})", f"{name} {ref[0]} / {ref[1]}, выгрузка {g[0]:.2f} / {g[1]:.2f}", errs)
    extra_ms = sorted((set(ms_sum) | set(bl["ms_est"]) | set(rd["rows"])) - set(MILESTONES))
    for m in extra_ms:
        rep.split("d", str(m), "лишняя веха вне 1–16", errs)
    tot = (sum(v[0] for v in ms_sum.values()), sum(v[1] for v in ms_sum.values()))
    if abs(tot[0] - TOTAL_NO_AI) > TOL_SUM or abs(tot[1] - TOTAL_WITH_AI) > TOL_SUM:
        rep.split("d", "итог (выгрузка)", f"{tot[0]:.2f} / {tot[1]:.2f}, ожидалось {TOTAL_NO_AI} / {TOTAL_WITH_AI}", errs)
    if rd["total"] is None or abs(rd["total"][0] - TOTAL_NO_AI) > TOL_SUM or abs(rd["total"][1] - TOTAL_WITH_AI) > TOL_SUM:
        rep.split("d", "итог (roadmap.md)", f"{rd['total']}, ожидалось {TOTAL_NO_AI} / {TOTAL_WITH_AI}", errs)
    rep.add("d", errs, f"вех {len(MILESTONES)}; итог выгрузки {tot[0]:.2f} / {tot[1]:.2f}")

    # 00_summary_by_stage.tsv — того же состава вех, что и выгрузка
    errs = []
    if SUMMARY.exists():
        with SUMMARY.open(encoding="utf-8-sig", newline="") as f:
            stages = [r[0] for r in csv.reader(f, delimiter="\t")][1:]
        expected = [str(m) for m in MILESTONES] + [stages[-1]]
        if stages != expected:
            errs.append(f"00_summary_by_stage.tsv: вехи {stages}, ожидалось 1–16 и итоговая строка")
    rep.add("d2", errs, "00_summary_by_stage.tsv: 16 вех + итог")

    # e) статусы
    errs, n_ok, partial = [], 0, 0
    for t in common:
        done, st = tasks[t]["done"], ex[t]["status"]
        ok = (done and st == "Done") or (not done and st in ("Todo", "Partial"))
        if not ok:
            rep.split("e", t, f"backlog.md {'[x]' if done else '[ ]'}, выгрузка {st} (backlog.md:{tasks[t]['line']})", errs)
        else:
            n_ok += 1
            partial += st == "Partial"
    rep.add("e", errs, f"совпало {n_ok} из {len(common)}; из них Partial при `[ ]`: {partial}")

    # --- итог --------------------------------------------------------------------------------
    print()
    for check, ok, info in rep.results:
        print(f"{check}: {'OK' if ok else 'FAIL'} — {info}")
    for w in rep.warnings:
        print(f"ВНИМАНИЕ: {w}")
    stale = sorted(set(KNOWN) - rep.used_known)
    for k in stale:
        print(f"ВНИМАНИЕ: известное исключение {k} больше не встречается — удалите из KNOWN")
    if rep.known:
        print("\nРасхождения, не связанные с перенумерацией (KNOWN):")
        groups: dict[tuple[str, str], list[str]] = {}
        for check, ident, _msg, why in rep.known:
            groups.setdefault((check, why), []).append(ident)
        for (check, why), idents in groups.items():
            print(f"  [{check}] {', '.join(idents)}\n      {why}")
    failed = [c for c, ok, _ in rep.results if not ok]
    ok_msg = f"новых расхождений нет (известных исключений KNOWN: {len(rep.known)})"
    print("\nРЕЗУЛЬТАТ:", ok_msg if not failed else "РАСХОЖДЕНИЯ в проверках " + ", ".join(failed))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", line_buffering=True)
    sys.exit(main())
