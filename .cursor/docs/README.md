# Страницы скилов

Тексты для человека. Процедура, которую читает агент, — `../skills/<имя>/SKILL.md`. Договор страницы — [writing-pages.md](writing-pages.md). Слои правил и slash-команд — [`../README.md`](../README.md).

Имя файла совпадает с именем скила. Папка — трек: `svt`, `atlas` или `common`. Скилы СВТ начинаются с `svt-`, скилы Атласа — с `atlas-`. Общие скилы (`common`) префикса не имеют.

## Цепочка

```text
svt-architecture
  → svt-migrate-catalog
      → svt-create-programmability   (накат на localhost)
      → svt-publish                  (пакет версии в C:\publish)
atlas-methodology
  → atlas-plan-grafik-md-html
grill-with-docs
  → to-spec
```

## СВТ

| Страница | Кто вызывает | О чём |
|---|---|---|
| [svt-architecture](svt/svt-architecture.md) | агент | Канон СВТ 2.0: на каждый вопрос один файл |
| [svt-migrate-catalog](svt/svt-migrate-catalog.md) | агент | Read-only справочник из легаси. Редактор, загрузчик и заявки — вне скила |
| [svt-migrate-catalog-orchestrator](svt/svt-migrate-catalog-orchestrator.md) | человек (явный вызов) | Конвейер миграции RO-справочников; канон правил — svt-migrate-catalog |
| [svt-create-programmability](svt/svt-create-programmability.md) | человек `/create_programmability` и агент | Накат Programmability на localhost/`mdm` |
| [svt-publish](svt/svt-publish.md) | человек `/publish` и агент | Пакет версии в `C:\publish\{версия}` и zip |

## Атлас

| Страница | Кто вызывает | О чём |
|---|---|---|
| [atlas-methodology](atlas/atlas-methodology.md) | агент | Кольцо эталонного процесса и документы эталона СВТ |
| [atlas-plan-grafik-md-html](atlas/atlas-plan-grafik-md-html.md) | агент | Текст и таблицы разделов 0, 1а и 1б между Markdown и HTML |

## Общие

| Страница | Кто вызывает | О чём |
|---|---|---|
| [grill-with-docs](common/grill-with-docs.md) | агент | Интервью по ещё не записанному плану; термин — в `glossary.md` |
| [to-spec](common/to-spec.md) | человек | Снимок постановки в `docs/tasks` по уже закрытому разговору |
| [handoff](common/handoff.md) | человек | Передача разговора в другой чат. В цепочку выше не входит |
| [skill-review](common/skill-review.md) | агент по просьбе | Ревью навыков и документов агента: противоречия, дубли, пробелы, путь к факту, лишнее чтение. В цепочку выше не входит |
