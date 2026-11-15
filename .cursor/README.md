# Документация агента Cursor (`.cursor`)

Карта правил, навыков и slash-команд для СВТ 2.0. Канон при расхождении — **skill**, не command и не краткий alwaysApply-абзац.

Страницы скилов для человека — [`docs/`](docs/README.md). Процедура, которую читает агент, — `skills/<имя>/SKILL.md`.

## Куда идти

| Ситуация | Куда |
|---|---|
| Скоуп, веха, Gate, риск, функция вне реестра | `svt-architecture` |
| Новый read-only справочник из легаси | `add-reference-book` |
| Накатить SP/view на localhost | `/create_programmability` → `create-programmability` |
| Собрать SQL в `C:\publish\v2` | `/publish_all_sql` → `publish-all-sql` |
| Кольцо, роли, эталон, промпты Атласа | `atlas` |
| Разделы 0 / 1а / 1б план-графика, MD ↔ HTML | `plan-grafik-md-html` |

## Слои

| Слой | Путь | Когда |
|------|------|--------|
| **Rules** | `rules/*.mdc` | Постоянный или glob-контекст |
| **Skills** | `skills/*/SKILL.md` | Процедуры и канон по задаче. Агент берёт сам по `description` |
| **Commands** | `commands/*.md` | Slash-команды. Вызывает человек; тонкие обёртки над skills |
| **Docs** | `docs/<трек>/*.md` | Страница скила для человека |

## Rules

| Файл | Режим | Назначение |
|------|-------|------------|
| `svt-project-context.mdc` | alwaysApply | Краткий контекст + указатель на таблицу «Канон» |
| `svt-architecture-principles.mdc` | alwaysApply | Обязательные ограничения архитектуры |
| `svt-development-patterns.mdc` | `BlazorSvt/**/*` | Паттерны модулей, grid, API, sync, тесты |
| `svt-add-reference-book.mdc` | Modules / SqlScripts Modules | Указатель на skill `add-reference-book` |
| `svt-atlas.mdc` | `docs/docs/svt-v-atlas/**` | Указатель на skill `atlas` (эталонные документы) |
| `svt-exec-html.mdc` | план для руководства | Вёрстка HTML для Капли/Аветисова |

## СВТ

Агент берёт скил сам, когда задача совпадает с `description`.

- **[svt-architecture](skills/svt-architecture/SKILL.md)** — канон архитектуры, скоупа, вех, Gate, рисков и чеклиста. Страница: [svt-architecture.md](docs/svt/svt-architecture.md).
- **[add-reference-book](skills/add-reference-book/SKILL.md)** — read-only справочник из легаси: discovery, SQL, C#, грид. Страница: [add-reference-book.md](docs/svt/add-reference-book.md).
- **[create-programmability](skills/create-programmability/SKILL.md)** — накат Programmability на localhost/`mdm`. Человек: `/create_programmability`. Страница: [create-programmability.md](docs/svt/create-programmability.md).
- **[publish-all-sql](skills/publish-all-sql/SKILL.md)** — публикация SQL в `C:\publish\v2`. Человек: `/publish_all_sql`. Страница: [publish-all-sql.md](docs/svt/publish-all-sql.md).

## Атлас

Отдельный трек от кода и SQL СВТ 2.0. Агент берёт скил сам по `description`.

- **[atlas](skills/atlas/SKILL.md)** — кольцо, эталон, роли, промпты и шаблоны. Страница: [atlas.md](docs/atlas/atlas.md).
- **[plan-grafik-md-html](skills/plan-grafik-md-html/SKILL.md)** — разделы 0 / 1а / 1б план-графика, MD ↔ HTML. Страница: [plan-grafik-md-html.md](docs/atlas/plan-grafik-md-html.md).

## Commands → Skills

| Command | Канон | Страница |
|---------|--------|----------|
| `/publish_all_sql` | skill `publish-all-sql` | [publish-all-sql.md](docs/svt/publish-all-sql.md) |
| `/create_programmability` | skill `create-programmability` | [create-programmability.md](docs/svt/create-programmability.md) |

Перед выполнением slash-команды агент читает соответствующий skill. Текст command не должен расходиться со skill; при сомнении — skill.

## Что куда не класть

- Длинный статус прода и полный план — в skill `svt-architecture`, таблица «Канон» в `SKILL.md`. AlwaysApply эту таблицу не дублирует
- Страницы скилов для человека — `.cursor/docs/`. Индекс документов продукта — `docs/README.md` в корне репозитория. Пакет architecture review собирает `docs/output-review-package/Build-ReviewPackage.ps1`: zip рядом с папкой, сгенерированные копии из папки скрипт удаляет. Zip в git не коммитится
- Алгоритм нового справочника — в `add-reference-book`, rule только указывает
- Методология эталона / кольцо / промпты Атласа — в `atlas`. Синхронизация разделов 0 / 1а / 1б план-графика — в `plan-grafik-md-html`
