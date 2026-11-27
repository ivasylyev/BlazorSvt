# Оценка трудоёмкости трансформации СВТ

Источник цифр для таблицы вех в [roadmap.md](../../.cursor/skills/svt-architecture/roadmap.md)
и поэлементных троек в [backlog.md](../../.cursor/skills/svt-architecture/backlog.md).
Confluence ч. III: [pageId=567442215](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215).

WBS ids = `{веха}.{пакет}.{задача}` (не путать с [review-conditions.md](../../.cursor/skills/svt-architecture/review-conditions.md)).
Скоуп RO/E/L — `catalogs-scope.md`. Третья цифра (человек с ИИ) только на прежних вехах 1=25 и 2=60 (до перенумераций вех 18.11.2026, 26.11.2026 и 27.11.2026; на вехи 1–16 не раскладывается).

## Перегенерация

```bash
python docs/estimation/generate_effort_estimate.py
```

Скрипт пишет TSV и Wiki Markup. `backlog.md` и `roadmap.md` он не пишет: они ведутся вручную.

Номера в выгрузке — вехи 1–16. Цепочка: `build_rows` (старые id) → `wbs_layout.py` → `renumber.py` (вехи 1–63, карта `renumber_map.json`) → `renumber_cut.py` (вехи 1–34, карта `renumber_map_cut.json`; снятые пункты убираются) → `renumber_cut2.py` (`cut2_rows`: вехи 1–34 → 1–16, карта `renumber_map_cut2.json`, раскладка вех — реестр `restructure_plan.json`; порядок задач пакета 4.10 следует `backlog.md`) → `renumber_cut3.py` (`cut3_rows`: вехи 1–16 до этапа 2 → вехи 1–16 канона на 27.11.2026, карта `renumber_map_cut3.json`, реестр решений `restructure_plan_cut3.json`; ремап текстов `title` и `note` — функция `remap_text3` из `restructure_cut3.py`, импортируется, не копируется). Правила генератора (`FRAMEWORK_PACKAGES`, `OVERTIME_ITEMS`, `JIRA_ITEMS` и др.) записаны в нумерации 1–63 и действуют до второго шага; метки `[Overtime]` и `[SVT-…]` у 3.5.5, 3.5.6 и 3.9.7 ставит `renumber_cut3.py` (в нумерации 1–63 это 15.5.5 → `JIRA_ITEMS`, 15.5.6 → `OVERTIME_ITEMS`; 3.9.7 в них не входила). Пункт генератора, которого нет в карте, останавливает запуск (`RuntimeError`); дубль номера после перенумерации — тоже.

`restructure_apply.py --apply` уже применён к `backlog.md` и `roadmap.md` (27.11.2026); повторно к канону его применять нельзя — он перенумерует уже перенумерованное.

`restructure_cut3.py` (этап 2: перенос пакетов и задач, расщепление и слияние пакетов, 18 новых задач; карта `renumber_map_cut3.json`, реестр `restructure_plan_cut3.json`) — применён 27.11.2026, повторно не запускать (`--apply` не идемпотентен). Слой `renumber_cut3.py` реализует из реестра только операции над строками генератора: op 23 (удаление 6.17.1), op 22 (18 задач «Руководство администратора» и «Руководство пользователя», 2 / 1, Todo), op 4 (2.5.1 → Done), op 6–7 (метки 3.5.5, 3.5.6), op 13 (название 4.6.12), op 24 (3.9.7: метка `[Overtime]`, 1 / 1, Done, примечание снято), op 26 (название 3.9.8); перенос пакетов и задач — через карту. Названий вех и пакетов в строках генератора нет, поэтому их переименования (op 1, 9, 10, 17, 18) в выгрузку не попадают. Генератор печатает применённые операции в конце вывода.

## Сверка с каноном

```bash
python docs/estimation/generate_effort_estimate.py
python docs/estimation/check_consistency.py
```

`check_consistency.py` (только стандартная библиотека; код возврата 1 при расхождении) сверяет TSV выгрузки с `backlog.md` и `roadmap.md`: (a) множество номеров задач; (b) тройки «без ИИ / с ИИ» по задачам; (c) суммы по пакетам; (d) суммы по вехам и итог 2172.8 / 1012.91; (e) статусы `[x]`/`[ ]` и Done/Todo. Расхождения, возникшие не из-за перенумерации, записаны в словаре `KNOWN` скрипта с причиной и печатаются отдельным разделом. Запускать после перегенерации выгрузки.

| Файл | Назначение |
|------|------------|
| `00_summary_by_stage.tsv` | Сводка по вехам (включая слот «человек») |
| `01_detail_items.tsv` | Пункты бэклога |
| `02_by_backlog_task.tsv` | Суммы по пакетам |
| `03_assumptions_and_norms.tsv` | Допущения и нормы |
| `04_done_vs_remaining.tsv` | Сделано / остаток |
| `05_item_triples.tsv` | Пункт → без ИИ / с ИИ |
| `part-iii-effort.confluence.md` | Текст для вставки в Confluence (Wiki Markup) |
| `SVT_transformation_effort_estimate.xlsx` | (если создаётся отдельно) |

## Как обновить Confluence ч. III

1. Открыть [страницу плана](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215) → Edit.
2. В редакторе переключиться на *Wiki Markup* (или вставить через Insert → Markup).
3. Заменить содержимое раздела **III. Оценка трудоёмкости** телом из `part-iii-effort.confluence.md`, начиная с строки `h1. III. Оценка трудоёмкости`.
4. Сохранить страницу.

Прямой REST-публикации из CI/агента нет (корпоративный Confluence недоступен без VPN/учётки автора).
