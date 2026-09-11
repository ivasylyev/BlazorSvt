# Оценка трудоёмкости трансформации СВТ

Источник цифр для таблицы этапов в [backlog.md](../../.cursor/skills/svt-architecture/backlog.md)
и поэлементных троек в том же файле.
Confluence ч. III: [pageId=567442215](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215).

WBS ids = `{MVP}.{пакет}.{пункт}` (не путать с `reference.md` §6.9).
Скоуп RO/E/L — `catalogs-scope.md`. Третья цифра (человек с ИИ) только на MVP 0.0=25 и 0.1=60.

## Перегенерация

```bash
python docs/estimation/generate_effort_estimate.py
python docs/estimation/generate_backlog_md.py
```

Первый скрипт пишет TSV и Wiki Markup. Второй пересобирает `backlog.md` (нарратив пакетов в скрипте + цифры из `build_rows()`).

| Файл | Назначение |
|------|------------|
| `00_summary_by_stage.tsv` | Сводка по этапам MVP (включая слот «человек») |
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
