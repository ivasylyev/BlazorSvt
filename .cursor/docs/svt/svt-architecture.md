# svt-architecture

## Что делает

`svt-architecture` держит канон СВТ 2.0: на каждый вопрос один файл. Постановки `docs/tasks`, транскрипты, бюджет и план для руководства — снимки. При расхождении берётся строка таблицы ниже. Та же таблица в [`SKILL.md`](../../skills/svt-architecture/SKILL.md) — канон для агента.

## Когда брать

Агент берёт скил сам, когда задача совпадает с `description`: разработка, рефакторинг, выбор решения, функция, которой ещё нет в реестре и бэклоге.

| Ситуация | Куда |
|---|---|
| Статус, скоуп, веха, Gate, риск, термин, чеклист, функция вне реестра | Строка таблицы «Канон» |
| Новый read-only справочник | [`add-reference-book`](add-reference-book.md) |
| Накат SP на localhost | [`create-programmability`](create-programmability.md) |
| Публикация SQL в каталог деплоя | [`publish-all-sql`](publish-all-sql.md) |
| Эталон, кольцо, роли | [`atlas`](../atlas/atlas.md) |
| Разделы 0 / 1а / 1б план-графика | [`plan-grafik-md-html`](../atlas/plan-grafik-md-html.md) |
| Полоска и карточки вех СВТ в разделе «г» план-графика | [`svt-vehi-html`](../atlas/svt-vehi-html.md) |
| Как писать текущий код модуля | правило `svt-development-patterns` |
| Обязательные ограничения архитектуры | правило `svt-architecture-principles` |

## Канон

| Вопрос | Канон |
|---|---|
| Что на проде, API, MCP, Import | [status.md](../../skills/svt-architecture/status.md) |
| Обоснование и целевое состояние | [architecture.md](../../skills/svt-architecture/architecture.md) |
| Состав, домены, RO/E/L, меню, приоритеты | [catalogs-scope.md](../../skills/svt-architecture/catalogs-scope.md) |
| Порядок доменов и вех, критический путь, оценки вех | [roadmap.md](../../skills/svt-architecture/roadmap.md) |
| Правила перехода и критерии Gate | [cutover.md](../../skills/svt-architecture/cutover.md) |
| Риски и меры | [risks.md](../../skills/svt-architecture/risks.md) |
| Условия одобрения | [review-conditions.md](../../skills/svt-architecture/review-conditions.md) |
| Приложения D-* | [mitigations.md](../../skills/svt-architecture/mitigations.md) |
| Шаги поставки | [delivery-steps.md](../../skills/svt-architecture/delivery-steps.md); подписи в [backlog.md](../../skills/svt-architecture/backlog.md) |
| Термины | [glossary.md](../../skills/svt-architecture/glossary.md) |
| Чеклист задач | [backlog.md](../../skills/svt-architecture/backlog.md) |
| Функция, которой нет в реестре | [outside-registry.md](../../skills/svt-architecture/outside-registry.md) |
| Чеклист перед кодом | [pre-implementation-checklist.md](../../skills/svt-architecture/pre-implementation-checklist.md) |
| Обязательные ограничения | правило `svt-architecture-principles` |
| Как писать текущий код | правило `svt-development-patterns` |
| Новый RO-справочник | skill [`add-reference-book`](../../skills/add-reference-book/SKILL.md) |
| Контракт HTTP | `docs/api/svt-api-v1.openapi.json` |
| Как вызывать API | `docs/api/svt-api-v1-developers.md` |
| Решения и риски API | [architecture.md](../../skills/svt-architecture/architecture.md) §6.6.1 |

Канон скоупа при изменении — [catalogs-scope.md](../../skills/svt-architecture/catalogs-scope.md) в репозитории. Excel на Confluence — исходный дамп легаси.

Порядок первого чтения пакета на ревью и три диаграммы — [`README.md`](../../skills/svt-architecture/README.md) папки скила. Пакет для передачи собирает `docs/output-review-package/Build-ReviewPackage.ps1`. Канон — файлы навыка. Zip в git не коммитится.

## Частые вопросы

**Постановка, транскрипт или план для руководства говорит иначе.**

Это снимок. Берётся строка таблицы «Канон».

**Excel на Confluence и `catalogs-scope.md` разошлись.**

Канон скоупа — `catalogs-scope.md` в репозитории. Excel — исходный дамп легаси.

**Какая из трёх диаграмм целевая для текущей программы.**

Порядок и смысл схем — в [`README.md`](../../skills/svt-architecture/README.md) папки скила. Диаграмма контейнеров в текущую программу не входит.

## Сработало, если

- Ответ ссылается на файл из таблицы «Канон».
- Функция, которой нет в реестре, разбирается по `outside-registry.md`.
- Перед кодом открыт `pre-implementation-checklist.md`.

## Куда встаёт

Контекст перед работой по треку СВТ. [`add-reference-book`](add-reference-book.md) сверяет справочник с `catalogs-scope.md`. Правила `svt-architecture-principles` и `svt-development-patterns` держат ограничения и паттерны кода. Цепочка — [`docs/README.md`](../README.md).
