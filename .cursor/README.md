# Документация агента Cursor (`.cursor`)

Карта правил, навыков и slash-команд для СВТ 2.0. Канон при расхождении — **skill**, не command и не краткий alwaysApply-абзац.

Маршрут «ситуация → скил» — [`docs/README.md`](docs/README.md). Процедура, которую читает агент, — `skills/<имя>/SKILL.md`. Договор страницы скила — [`docs/writing-pages.md`](docs/writing-pages.md).

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
| `svt-development-patterns.mdc` | `BlazorSvt/**/*` | Паттерны модулей, grid, API, sync, тесты |
| `svt-add-reference-book.mdc` | Modules / SqlScripts Modules | Указатель на skill `add-reference-book` |
| `svt-atlas.mdc` | `docs/docs/svt-v-atlas/**` | Указатель на skill `atlas` (эталонные документы) |
| `svt-exec-html.mdc` | план для руководства | Вёрстка HTML для Капли/Аветисова |

## Commands → Skills

| Command | Канон | Страница |
|---------|--------|----------|
| `/publish_all_sql` | skill `publish-all-sql` | [publish-all-sql.md](docs/svt/publish-all-sql.md) |
| `/create_programmability` | skill `create-programmability` | [create-programmability.md](docs/svt/create-programmability.md) |

Перед выполнением slash-команды агент читает соответствующий skill. Текст command не должен расходиться со skill; при сомнении — skill.

## Что куда не класть

- Длинный статус прода и полный план — в skill `svt-architecture`, таблица «Канон» в `SKILL.md`. AlwaysApply эту таблицу не дублирует
- Страницы скилов для человека — `.cursor/docs/`. Каталог — `.cursor/docs/README.md`. Индекс документов продукта — `docs/README.md` в корне репозитория. Пакет architecture review собирает `docs/output-review-package/Build-ReviewPackage.ps1`: zip рядом с папкой, сгенерированные копии из папки скрипт удаляет. Zip в git не коммитится
- Алгоритм нового справочника — в `add-reference-book`, rule только указывает
- Методология эталона / кольцо / промпты Атласа — в `atlas`. Синхронизация разделов 0 / 1а / 1б план-графика — в `plan-grafik-md-html`. Полоска и карточки вех СВТ в разделе «г» — в `svt-vehi-html`
