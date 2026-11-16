# Страницы скилов

Тексты для человека. Процедура, которую читает агент, — `../skills/<имя>/SKILL.md`. Договор страницы — [writing-pages.md](writing-pages.md). Слои правил и slash-команд — [`../README.md`](../README.md).

Имя файла совпадает с именем скила. Папка — трек.

## Цепочка

```text
grill-with-docs
  → to-spec
svt-architecture
  → add-reference-book
      → create-programmability   (накат на localhost)
      → publish-all-sql          (копия в каталог деплоя)
atlas
  → plan-grafik-md-html
      → svt-vehi-html   (вехи СВТ в разделе «г», скрипт их не собирает)
```

## СВТ

| Страница | Кто вызывает | О чём |
|---|---|---|
| [svt-architecture](svt/svt-architecture.md) | агент | Канон СВТ 2.0: на каждый вопрос один файл |
| [add-reference-book](svt/add-reference-book.md) | агент | Read-only справочник из легаси. Редактор, загрузчик и заявки — вне скила |
| [create-programmability](svt/create-programmability.md) | человек `/create_programmability` и агент | Накат Programmability на localhost/`mdm` |
| [publish-all-sql](svt/publish-all-sql.md) | человек `/publish_all_sql` и агент | Копия SQL в каталог деплоя `C:\publish\v2` |
| [grill-with-docs](svt/grill-with-docs.md) | агент | Интервью по ещё не записанному плану; термин — в `glossary.md` |
| [to-spec](svt/to-spec.md) | человек | Снимок постановки в `docs/tasks` по уже закрытому разговору |
| [handoff](svt/handoff.md) | человек | Передача разговора в другой чат. В цепочку выше не входит |
| [skill-review](svt/skill-review.md) | агент | Ревью навыков и документов агента: неверное действие и лишнее чтение. В цепочку выше не входит |

## Атлас

| Страница | Кто вызывает | О чём |
|---|---|---|
| [atlas](atlas/atlas.md) | агент | Кольцо эталонного процесса и документы эталона СВТ |
| [plan-grafik-md-html](atlas/plan-grafik-md-html.md) | агент | Текст и таблицы разделов 0, 1а и 1б между Markdown и HTML |
| [svt-vehi-html](atlas/svt-vehi-html.md) | агент | Полоска и карточки вех СВТ в разделе «г» план-графика из черновика |
