# Документация агента Cursor (`.cursor`)

Карта правил, навыков и slash-команд для СВТ 2.0. Канон при расхождении — **skill**, не command и не краткий alwaysApply-абзац.

Маршрут «ситуация → скил» — [`.cursor/docs/README.md`](docs/README.md). Процедура, которую читает агент, — `skills/<имя>/SKILL.md`. Договор страницы скила — [`docs/writing-pages.md`](docs/writing-pages.md).

## Слои

| Слой | Путь | Когда |
|------|------|--------|
| **Rules** | `rules/*.mdc` | Постоянный или glob-контекст |
| **Skills** | `skills/*/SKILL.md` | Процедуры и канон по задаче. Агент берёт сам по `description` |
| **Commands** | `commands/*.md` | Slash-команды. Вызывает человек; тонкие обёртки над skills |
| **Docs** | `docs/<трек>/*.md` | Страница скила для человека. Каталог — `docs/README.md` |

## Rules

| Файл | Режим | Назначение |
|------|-------|------------|
| `svt-project-context.mdc` | alwaysApply | Краткий контекст + указатель на таблицу «Канон» |
| `svt-architecture-principles.mdc` | alwaysApply | Обязательные ограничения архитектуры |
| `ne-udalyat-sami.mdc` | alwaysApply | Не удалять без явного названия, не придумывать факты |
| `svt-development-patterns.mdc` | `BlazorSvt/**/*` | Паттерны модулей, grid, API, sync, тесты |
| `svt-add-reference-book.mdc` | Modules / SqlScripts Modules | Указатель на skill `svt-migrate-catalog` |
| `svt-atlas.mdc` | `docs/docs/svt-v-atlas/**` | Указатель на skill `atlas-methodology` (эталонные документы) |
| `svt-exec-html.mdc` | план для руководства | Вёрстка HTML для Капли/Аветисова |

## Commands → Skills

| Command | Канон | Страница |
|---------|--------|----------|
| `/publish` | skill `svt-publish` | [svt-publish.md](docs/svt/svt-publish.md) |
| `/create_programmability` | skill `svt-create-programmability` | [svt-create-programmability.md](docs/svt/svt-create-programmability.md) |

Перед выполнением slash-команды агент читает соответствующий skill. Текст command не должен расходиться со skill; при сомнении — skill.

## Что куда не класть

- Длинный статус прода и полный план — в skill `svt-architecture`, таблица «Канон» в `SKILL.md`. AlwaysApply эту таблицу не дублирует
- Страницы скилов для человека — `.cursor/docs/`. Каталог — `.cursor/docs/README.md`. Индекс документов продукта — `docs/README.md` в корне репозитория. Пакет architecture review — `docs/output-review-package/README.md`
- Алгоритм нового справочника — в `svt-migrate-catalog`, rule только указывает
- Методология эталона / кольцо / промпты Атласа — в `atlas-methodology`. Синхронизация разделов 0 / 1а / 1б план-графика — в `atlas-plan-grafik-md-html`
