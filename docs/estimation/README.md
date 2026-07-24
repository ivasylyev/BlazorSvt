# Оценка трудоёмкости трансформации СВТ

Источник истины для **ч. III** страницы плана в Confluence
([pageId=567442215](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215)).

## Перегенерация

```bash
python docs/estimation/generate_effort_estimate.py
```

Пишет TSV, Excel-ориентированные таблицы и Wiki Markup:

| Файл | Назначение |
|------|------------|
| `00_summary_by_stage.tsv` | Сводка по этапам MVP |
| `01_detail_items.tsv` | Пункты бэклога |
| `02_by_backlog_task.tsv` | Суммы по задачам |
| `03_assumptions_and_norms.tsv` | Допущения и нормы |
| `04_done_vs_remaining.tsv` | Сделано / остаток |
| `part-iii-effort.confluence.md` | Текст для вставки в Confluence (Wiki Markup) |
| `SVT_transformation_effort_estimate.xlsx` | (если создаётся отдельно) |

## Как обновить Confluence ч. III

1. Открыть [страницу плана](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215) → Edit.
2. В редакторе переключиться на *Wiki Markup* (или вставить через Insert → Markup).
3. Заменить содержимое раздела **III. Оценка трудоёмкости** (сейчас TBD) телом из `part-iii-effort.confluence.md`, начиная с строки `h1. III. Оценка трудоёмкости`.
4. Сохранить страницу.

Прямой REST-публикации из CI/агента нет (корпоративный Confluence недоступен без VPN/учётки автора).
