# svt-architecture

## Что делает

`svt-architecture` держит канон СВТ 2.0: на каждый вопрос один файл. Постановки `docs/tasks`, транскрипты, бюджет и план для руководства — снимки. При расхождении берётся строка таблицы ниже. Та же таблица в [`SKILL.md`](../../skills/svt-architecture/SKILL.md) — канон для агента.

## Когда брать

Агент берёт скил сам, когда задача совпадает с `description`: разработка, рефакторинг, выбор решения, функция, которой ещё нет в реестре и бэклоге.

| Ситуация | Куда |
|---|---|
| Статус, скоуп, веха, Gate, риск, термин, чеклист, функция вне реестра | Строка таблицы «Канон» |
| Новый read-only справочник | [`svt-migrate-catalog`](svt-migrate-catalog.md) |
| Накат SP на localhost | [`svt-create-programmability`](svt-create-programmability.md) |
| Собрать пакет версии в `C:\publish` | [`svt-publish`](svt-publish.md) |
| Собрать zip пакета архитектурного ревью | [`svt-create-review-package`](svt-create-review-package.md) |
| Эталон, кольцо, роли | [`atlas-methodology`](../atlas/atlas-methodology.md) |
| Разделы 0 / 1а / 1б план-графика | [`atlas-plan-grafik-md-html`](../atlas/atlas-plan-grafik-md-html.md) |
| Как писать текущий код модуля | правило `svt-development-patterns` |
| Обязательные ограничения архитектуры | правило `svt-architecture-principles` |

## Канон

Таблица «вопрос → файл» — [`SKILL.md`](../../skills/svt-architecture/SKILL.md), раздел «Канон». Оглавление пакета на ревью — [`README.md`](../../skills/svt-architecture/README.md) папки скила. Таблица файлов и ссылок Confluence — `docs/output-review-package/README.md`. Сборка zip — [`svt-create-review-package`](svt-create-review-package.md).

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

Контекст перед работой по треку СВТ. [`svt-migrate-catalog`](svt-migrate-catalog.md) сверяет справочник с `catalogs-scope.md`. Правила `svt-architecture-principles` и `svt-development-patterns` держат ограничения и паттерны кода. Цепочка — [`docs/README.md`](../README.md).
