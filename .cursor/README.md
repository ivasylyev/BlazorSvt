# Документация агента Cursor (`.cursor`)

Карта правил, навыков и slash-команд для СВТ 2.0. Канон при расхождении — **skill**, не command и не краткий alwaysApply-абзац.

## Слои

| Слой | Путь | Когда |
|------|------|--------|
| **Rules** | `rules/*.mdc` | Постоянный или glob-контекст |
| **Skills** | `skills/*/SKILL.md` | Процедуры и канон по задаче |
| **Commands** | `commands/*.md` | Slash-команды; тонкие обёртки над skills |

## Rules

| Файл | Режим | Назначение |
|------|-------|------------|
| `svt-project-context.mdc` | alwaysApply | Краткий контекст + указатели на skill |
| `svt-architecture-principles.mdc` | alwaysApply | Обязательные ограничения архитектуры |
| `svt-development-patterns.mdc` | `BlazorSvt/**/*` | Паттерны модулей, grid, API, sync, тесты |
| `svt-add-reference-book.mdc` | Modules / SqlScripts Modules | Указатель на skill `add-reference-book` |
| `svt-atlas.mdc` | `docs/docs/svt-v-atlas/**` | Указатель на skill `atlas` (эталонные документы) |
| `svt-exec-html.mdc` | план для руководства | Вёрстка HTML для Капли/Аветисова |

## Skills (канон процедур)

| Skill | Когда |
|-------|--------|
| `svt-architecture` | Архитектура, скоуп, roadmap, backlog, требования вне реестра |
| `add-reference-book` | Добавление read-only справочника из легаси (`SKILL.md` + `discovery.md` / `sql.md` / `csharp.md` / `grid-column-glossary.md`) |
| `create-programmability` | Накат Programmability на localhost/`mdm` |
| `publish-all-sql` | Публикация SQL в `C:\publish\v2` (`-Mode …`) |
| `atlas` | Эталонный процесс / документы Атласа — **отдельный трек**, не стек и не SQL СВТ 2.0 |

Подробности архитектуры: `skills/svt-architecture/` (`SKILL.md`, `reference.md`, `roadmap.md`, `backlog.md`, `catalogs-scope.md`).

## Commands → Skills

| Command | Канон |
|---------|--------|
| `/publish_all_sql` | skill `publish-all-sql` |
| `/create_programmability` | skill `create-programmability` |

Перед выполнением slash-команды агент читает соответствующий skill. Текст command не должен расходиться со skill; при сомнении — skill.

## Что куда не класть

- Длинный статус прода и полный MVP — в `svt-architecture` (`SKILL.md` — статус, `roadmap.md` — план), не раздувать alwaysApply
- Человеческий индекс документов — `docs/README.md`. Пакет architecture review собирает `docs/output-review-package/Build-ReviewPackage.ps1`: zip рядом с папкой, сгенерированные копии из папки скрипт удаляет. Zip в git не коммитится
- Алгоритм нового справочника — в `add-reference-book`, rule только указывает
- Методология эталона / кольцо / промпты Атласа — в `atlas`, не смешивать с разработкой кода
