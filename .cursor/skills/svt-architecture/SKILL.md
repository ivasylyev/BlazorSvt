---
name: svt-architecture
description: >-
  Архитектура и контекст проекта СВТ (Система Ведения Тарифов). Используй при
  разработке функционала, рефакторинге, проектировании справочников, batch-загрузки,
  синхронизации, интеграций, принятии архитектурных решений или фиксации требований
  к функции, которой ещё нет в реестре и бэклоге.
---

# Контекст СВТ

## Канон

При расхождении берётся файл из этой таблицы. Постановки `docs/tasks`, транскрипты, бюджет и план для руководства — снимки, не процедуры.

| Вопрос | Канон |
|---|---|
| Что на проде, API, MCP, Import | [status.md](status.md) |
| Обоснование и целевое состояние | [architecture.md](architecture.md) |
| Состав, домены, RO/E/L, меню, приоритеты | [catalogs-scope.md](catalogs-scope.md) |
| Порядок доменов и вех, критический путь, оценки вех | [roadmap.md](roadmap.md) |
| Правила перехода и критерии Gate | [cutover.md](cutover.md) |
| Риски и меры | [risks.md](risks.md) |
| Условия одобрения | [review-conditions.md](review-conditions.md) |
| Приложения D-* | [mitigations.md](mitigations.md) |
| Шаги поставки | [delivery-steps.md](delivery-steps.md); подписи в [backlog.md](backlog.md) |
| Термины | [glossary.md](glossary.md) |
| Чеклист задач | [backlog.md](backlog.md) |
| Функция, которой нет в реестре | [outside-registry.md](outside-registry.md) |
| Чеклист перед кодом | [pre-implementation-checklist.md](pre-implementation-checklist.md) |
| Обязательные ограничения | правило `svt-architecture-principles` |
| Как писать текущий код | правило `svt-development-patterns` |
| Новый RO-справочник | skill `add-reference-book` |
| Контракт HTTP | `docs/api/svt-api-v1.openapi.json` |
| Как вызывать API | `docs/api/svt-api-v1-developers.md` |
| Решения и риски API | [architecture.md](architecture.md) §6.6.1 |

## Когда применять

- Новый справочник, редактор, импорт, интеграция
- Рефакторинг существующего кода
- Выбор между альтернативами (кеш, микросервисы, логика в SQL и т.д.)
- Оценка соответствия решения целевой архитектуре
- Функция, которой нет в реестре справочников и в бэклоге

## Краткий контекст

Текст раздела — [status.md](status.md).

## Чеклист перед реализацией

Текст раздела — [pre-implementation-checklist.md](pre-implementation-checklist.md).

## Требования, которых ещё нет в реестре

Текст раздела — [outside-registry.md](outside-registry.md).

## Ключевые антипаттерны легаси (не повторять)

Текст раздела — [architecture.md](architecture.md), после §3.

## Подробная документация

- Обоснование замещения, сравнение альтернатив и целевое состояние — [architecture.md](architecture.md)
  (условное архитектурное одобрение — [review-conditions.md](review-conditions.md))
- Дорожная карта: порядок доменов, сводка вех и критический путь — [roadmap.md](roadmap.md)
- Правила перехода и критерии Gate A/B/C/D — [cutover.md](cutover.md)
- Риски и меры — [risks.md](risks.md)
- Условия одобрения — [review-conditions.md](review-conditions.md)
- Приложения threat model / SoD / SLI/SLO / … — [mitigations.md](mitigations.md)
- Двенадцать шагов поставки — [delivery-steps.md](delivery-steps.md)
- Термины — [glossary.md](glossary.md)
- Чеклист работ — [backlog.md](backlog.md). Таблица вех и оценка трудоёмкости — [roadmap.md](roadmap.md)
- Реестр справочников (SoT скоупа: состав доменов, RO/E/L, меню, приоритеты) — [catalogs-scope.md](catalogs-scope.md). Канон при изменении — этот файл в репозитории; Excel на Confluence — только исходный дамп легаси
- Текущий статус контура — [status.md](status.md)
- Функции вне реестра — [outside-registry.md](outside-registry.md)
- Чеклист перед кодом — [pre-implementation-checklist.md](pre-implementation-checklist.md)
- Оглавление пакета и диаграммы —
  [README.md](README.md) (файлы `01-as-is-legacy.png`, `02-to-be-current-program.png`, `03-future-containers-out-of-scope.png` лежат в этой же папке)
- Пакет для передачи на ревью собирает [Build-ReviewPackage.ps1](../../../docs/output-review-package/Build-ReviewPackage.ps1) по [README](../../../docs/output-review-package/README.md): markdown этого навыка, диаграммы из этой папки и `SVT-idealnyy-process-2.7.html` попадают в zip, затем скрипт удаляет эти копии из папки пакета. Канон — файлы навыка. Zip в git не коммитится
