# Роль: C#

Сначала прочитай `{AgentsDir}\common.md`, `{WorkDir}\spec.md` и `{WorkDir}\sql-report.md`.

Задача — фазы 4–5 `add-reference-book/csharp.md` для своего `{Entity}`: модуль, ресурсы, регистрация, API и тесты. SQL уже накатан: колонки снимка — из `sql-report.md`, имена и заголовки — из spec.

Параллельно работают csharp-агенты других справочников батча и один docs-агент.

## Твои файлы

Как в фазах 4–5 `csharp.md` и чеклисте `add-reference-book`:

- `BlazorSvt/Modules/{Entity}/**`
- `BlazorSvt/Platform/Domain/IdsEnum/*` — только новые enum из раздела 5; существующие не перегенерировать
- `BlazorSvt/Host/Program.cs`, `Platform/Resources/Platform.resx`, `Platform.ru-RU.resx` — свои строки
- `.cursor/skills/add-reference-book/grid-column-glossary.md` — только новые строки
- `tests/**` — свои тесты и свои строки в общих списках

`docs/user-guide`, Postman, `svt-api-v1-developers.md` — роль docs. `docs/api/svt-api-v1.openapi.json` — роль finalize: снимок не регенерируй и `OpenApiContractTests` не запускай.

## Что читать

- `add-reference-book/csharp.md` — «Фаза 4», «Фаза 5» (фаза 6 — у finalize/reviewer).
- `add-reference-book/SKILL.md` — именование; для `simple` — «Стабильный словарь с enum».
- `add-reference-book/grid-column-glossary.md` — после генерации resx.
- Эталон модуля — из `add-reference-book/SKILL.md` по классу; тесты — `Grep` имени эталона в `tests/`.

## Батч-отличия (поверх фаз 4–5)

- В общих файлах только свои строки; перед правкой перечитай файл. После — `git diff`: чужие строки не должны пропасть.
- `dotnet build` и `dotnet test` — всегда с `--artifacts-path {RepoRoot}\.tmp-artifacts-{Entity}`; папку удали после последнего прогона.
- OpenAPI не трогай — его обновляет finalize. В отчёт — маршруты и схемы, которые должен добавить снимок.
- Если в промпте запрет «не переносить enum {X} / не править модуль {Y}» — не делай даже по фазе 4: перенос у первого по очереди. Отметь в отчёте.
- Integration своего справочника (`--filter "FullyQualifiedName~{Entity}"`) запускай. Флаки в чужих таблицах — в «РИСКИ».

## Шаги

Выполни фазы 4–5 `csharp.md` по spec и `sql-report.md` (enum из раздела 5, в том числе новый файл пары Ru/En, если его ещё нет; DTO, настройки, resx/глоссарий, модуль, меню, API, тесты класса). `Class=complex` — SyncJob, тесты контракта job по фазе 5 и сценарий `{Entity}_IncrementalSync_PropagatesLegacyChange` в `SnapshotSyncIntegrationTests` рядом с уже существующими: стимул в legacy, `RunIncrementalAsync`, проверка снимка, cleanup. `simple` — без `Sync/`, в список stable-without-sync.
