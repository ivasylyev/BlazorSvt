# Оценка трудоёмкости трансформации СВТ

Источник цифр для таблицы вех в [roadmap.md](../../.cursor/skills/svt-architecture/roadmap.md)
и поэлементных троек в [backlog.md](../../.cursor/skills/svt-architecture/backlog.md).
Confluence ч. III: [pageId=567442215](https://confluence.sibur.local/pages/viewpage.action?pageId=567442215).

WBS ids = `{веха}.{пакет}.{задача}` (не путать с [review-conditions.md](../../.cursor/skills/svt-architecture/review-conditions.md)).
Скоуп RO/E/L — `catalogs-scope.md`. Третья цифра (человек с ИИ) только на вехе 1=25 и вехе 2=60.

## Перегенерация

```bash
python docs/estimation/generate_effort_estimate.py
python docs/estimation/generate_backlog_md.py
```

Первый скрипт пишет TSV и Wiki Markup. Второй собирает черновик чеклиста `backlog.md` и не воспроизводит ручной нарратив (очередь Атласа, примечания пунктов). Сводку вех и критический путь он не пишет: они вручную в `roadmap.md`. Перед запуском сверить вывод и не затирать ручные формулировки.

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
