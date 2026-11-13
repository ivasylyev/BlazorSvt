# Документы СВТ

Карта папки `docs/`. Пути в таблицах — от корня репозитория. Канон архитектуры, скоупа и статуса прода — не здесь, а в `.cursor/skills/svt-architecture/` (`SKILL.md` — статус, `roadmap.md` — план, `backlog.md` — чеклист, `catalogs-scope.md` — реестр).

## Канон

| Папка | Что это |
|---|---|
| `docs/estimation/` | Цифры трудоёмкости. Из них собирается таблица этапов в `backlog.md` |
| `docs/api/svt-api-v1.openapi.json` | Контракт HTTP. Сверяется тестом `OpenApiContractTests` |
| `docs/api/svt-api-v1-developers.md` | Как вызывать API |
| `docs/user-guide/*.ru.confluence.md` | Текст руководства пользователя. Английская версия — перевод |

Решения и риски API — `.cursor/skills/svt-architecture/reference.md` §6.6.1.

## Инженерные материалы

| Папка | Что это |
|---|---|
| `docs/api/` | Техническое и бизнес-описание API. Контракт — `svt-api-v1.openapi.json`. Локальная Postman-коллекция контрактом не является |
| `docs/tasks/` | Архив уже сделанного. Не исходные требования. Состав скоупа — `catalogs-scope.md`, алгоритм нового справочника — skill `add-reference-book` |
| `docs/output-review-package/` | Процедура пакета на architecture review. `Build-ReviewPackage.ps1` собирает zip и удаляет сгенерированные копии из папки. Zip в git не коммитится. `review-current-state.html` — снимок заключения 07.08.2026 |

## Снимки (статус прода не обновляется)

| Файл | Дата |
|---|---|
| `docs/budget-2027/` | Позиция к бюджету 2027 |
| `docs/docs/svt-plan-dlya-rukovoditeley.html` | План для руководства, 13.10.2026 |
| `docs/output-review-package/review-current-state.html` | Архитектурное заключение, 07.08.2026 |

Актуальный статус — `.cursor/skills/svt-architecture/SKILL.md`.

## Отдельные треки

| Папка | Что это |
|---|---|
| `docs/docs/svt-v-atlas/` | Эталонный процесс. Навык `atlas`, не стек СВТ 2.0 |
| `docs/docs/svt-plan-dlya-rukovoditeley.html` | План для руководства. Вёрстка заморожена правилом `svt-exec-html` |
| `docs/docs/sibur_style.md` | Фирменный стиль СИБУР для новых материалов |
| `docs/docs/CX-UX.html` | CX. Не шаблон плана для руководства и не продукт СВТ |
| `docs/meetings/` | Транскрипты и итоги встреч. Не требования и не канон архитектуры |

Индекс агента (rules, skills, commands) — `.cursor/README.md`.
