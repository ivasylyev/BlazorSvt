# Документы СВТ

Карта папки `docs/`. Пути в таблицах — от корня репозитория. Канон архитектуры, скоупа и статуса прода — не здесь, а в `.cursor/skills/svt-architecture/` (`status.md` — статус, `architecture.md` — обоснование, `roadmap.md` — порядок и сводка вех, `cutover.md` — критерии Gate, `backlog.md` — чеклист, `catalogs-scope.md` — реестр).

## Канон

| Папка | Что это |
|---|---|
| `docs/estimation/` | Цифры трудоёмкости. Из них собирается таблица вех в `roadmap.md` |
| `docs/api/svt-api-v1.openapi.json` | Контракт HTTP. Сверяется тестом `OpenApiContractTests` |
| `docs/api/svt-api-v1-developers.md` | Как вызывать API |
| `docs/user-guide/*.ru.confluence.md` | Текст руководства пользователя. Английская версия — перевод |

Решения и риски API — `.cursor/skills/svt-architecture/architecture.md` §6.6.1.

## Инженерные материалы

| Папка | Что это |
|---|---|
| `docs/api/` | Техническое и бизнес-описание API. Контракт — `svt-api-v1.openapi.json`. Локальная Postman-коллекция контрактом не является |
| `docs/tasks/` | Снимки постановок, в том числе ещё не сделанных. Не исходные требования и не процедура. Состав скоупа — `catalogs-scope.md`, алгоритм нового справочника — skill `svt-migrate-catalog`. Новый снимок пишет skill `to-spec` |
| `docs/output-review-package/` | Таблица файлов пакета на architecture review и снимок `review-current-state.html` (07.08.2026). Процедура сборки — skill `svt-create-review-package` |

## Снимки (статус прода не обновляется)

| Файл | Дата |
|---|---|
| `docs/docs/budget-2027/` | Позиция к бюджету 2027 |
| `docs/docs/svt-plan-dlya-rukovoditeley.html` | План для руководства, 13.10.2026 |
| `docs/output-review-package/review-current-state.html` | Архитектурное заключение, 07.08.2026 |

Актуальный статус — `.cursor/skills/svt-architecture/status.md`. Указатель «вопрос → файл» — `SKILL.md` того же навыка.

## Отдельные треки

| Папка | Что это |
|---|---|
| `docs/docs/svt-v-atlas/` | Эталонный процесс. Навык `atlas-methodology`, не стек СВТ 2.0 |
| `docs/docs/План-график Атлас/atlas-milestones-draft.md` | Черновик вех стрима СВТ. Не канон. Порядок вех — `roadmap.md`. Процедура его не открывает |
| `docs/docs/План-график Атлас/atlas-milestones-draft2.md` | Снимок текста того же стрима. Не канон. Процедура его не открывает |
| `docs/docs/План-график Атлас/atlas-milestones-mapping-without-SVT.md` | Вариант сверки без фронта СВТ. Не канон. Процедура его не открывает |
| `docs/docs/svt-plan-dlya-rukovoditeley.html` | План для руководства. Вёрстка заморожена правилом `svt-exec-html` |
| `docs/docs/sibur_style.md` | Фирменный стиль СИБУР для новых материалов |
| `docs/docs/CX-UX.html` | CX. Не шаблон плана для руководства и не продукт СВТ |
| `docs/meetings/` | Транскрипты и итоги встреч. Не требования и не канон архитектуры |

Индекс агента (rules, skills, commands) — `.cursor/README.md`.
