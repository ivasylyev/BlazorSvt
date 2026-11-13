# Документы СВТ

Карта папки `docs/`. Канон архитектуры, скоупа и статуса прода — не здесь, а в `.cursor/skills/svt-architecture/` (`SKILL.md` — статус, `roadmap.md` — план, `backlog.md` — чеклист, `catalogs-scope.md` — реестр).

## Канон

| Папка | Что это |
|---|---|
| `estimation/` | Цифры трудоёмкости. Из них собирается таблица этапов в `backlog.md` |
| `api/svt-api-v1.openapi.json` | Контракт HTTP. Сверяется тестом `OpenApiContractTests` |
| `api/svt-api-v1-developers.md` | Как вызывать API |
| `user-guide/*.ru.confluence.md` | Текст руководства пользователя. Английская версия — перевод |

Решения и риски API — `.cursor/skills/svt-architecture/reference.md` §6.6.1.

## Инженерные материалы

| Папка | Что это |
|---|---|
| `api/` | Техническое и бизнес-описание API, локальная Postman-коллекция (не контракт) |
| `tasks/` | Архив постановок уже сделанных работ. Алгоритм нового справочника — skill `add-reference-book`, не эти файлы. Состав скоупа — в `catalogs-scope.md` |
| `output-review-package/` | Процедура пакета на architecture review. Сборку делает `Build-ReviewPackage.ps1`. Копии markdown, диаграммы и zip в git не хранятся. `review-current-state.html` — снимок заключения 07.08.2026 |

## Снимки (статус прода не обновляется)

| Файл | Дата |
|---|---|
| `budget-2027/` | Позиция к бюджету 2027 |
| `docs/svt-plan-dlya-rukovoditeley.html` | План для руководства, 13.10.2026. Файл лежит во вложенной папке `docs/docs/` |
| `output-review-package/review-current-state.html` | Архитектурное заключение, 07.08.2026 |

Актуальный статус — `.cursor/skills/svt-architecture/SKILL.md`.

## Отдельные треки

| Папка | Что это |
|---|---|
| `docs/svt-v-atlas/` | Эталонный процесс (вложенная папка `docs/docs/svt-v-atlas`). Навык `atlas`, не стек СВТ 2.0 |
| `docs/svt-plan-dlya-rukovoditeley.html` | План для руководства. Вёрстка заморожена правилом `svt-exec-html` |
| `docs/sibur_style.md` | Фирменный стиль СИБУР для новых материалов |
| `docs/CX-UX.html` | CX. Не шаблон плана для руководства и не продукт СВТ |
| `meetings/` | Транскрипты и итоги встреч. Не требования и не канон архитектуры |

Индекс агента (rules, skills, commands) — `.cursor/README.md`.
