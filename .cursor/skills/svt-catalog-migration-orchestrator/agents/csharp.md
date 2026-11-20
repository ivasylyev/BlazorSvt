# Роль: C#

Сначала прочитай `{AgentsDir}\common.md`, `{WorkDir}\spec.md` и `{WorkDir}\sql-report.md`.

Задача — фазы 4–5 и п.6 фазы 6 `add-reference-book/csharp.md` для своего `{Entity}`: модуль, ресурсы, регистрация, API и тесты. SQL уже накатан: колонки снимка — из `sql-report.md`, имена и заголовки — из spec.

Параллельно работают csharp-агенты других справочников батча и один docs-агент.

## Твои файлы

Как в фазах 4–5 `csharp.md` и чеклисте `add-reference-book`:

- `BlazorSvt/Modules/{Entity}/**`
- `BlazorSvt/Platform/Domain/IdsEnum/*` — только новые enum из раздела 5; существующие не перегенерировать
- перенос существующего enum из другого модуля в `Platform/Domain/IdsEnum/` и правка использующих его модулей — по §7 `discovery-rules.md`, если spec его выносит и в промпте нет запрета (батч-отличия)
- `BlazorSvt/Host/Program.cs`, `Platform/Resources/Platform.resx`, `Platform.ru-RU.resx` — свои строки
- `.cursor/skills/add-reference-book/grid-column-glossary.md` — только новые строки
- `tests/**` — свои тесты и свои строки в общих списках
- `.cursor/skills/add-reference-book/SKILL.md` — только строка своего справочника в таблице «Стабильный словарь с enum», если в разделе 8 решение «simple» и enum создаёшь ты

`docs/user-guide`, Postman, `svt-api-v1-developers.md` — роль docs. `docs/api/svt-api-v1.openapi.json` — роль finalize: снимок не регенерируй и `OpenApiContractTests` не запускай.

## Что читать

`add-reference-book/csharp.md`. Глоссарий — раздел «Правила» и поиск имени поля. Таблицу enum в `SKILL.md` открывай только если дописываешь строку simple. `discovery-rules.md` §7 — только если spec выносит перенос или создание enum. Эталон модуля — `Grep` имени эталона в `tests/`, файл эталона целиком не читать.

## Батч-отличия (поверх фаз 4–5)

- В общих файлах только свои строки; перед правкой перечитай файл. После — `git diff`: чужие строки не должны пропасть.
- `dotnet build` и `dotnet test` — всегда с `--artifacts-path {RepoRoot}\.tmp-artifacts-{Entity}`; папку удали после последнего прогона.
- OpenAPI не трогай — его обновляет finalize. В отчёт — маршруты и схемы, которые должен добавить снимок.
- Если в промпте запрет «не создавать / не переносить enum {X} / не править модуль {Y}» — не делай даже по фазе 4: это делает первый по очереди. Отметь в отчёте. В промпте «enum {X} уже создан: путь» — используй его по этому пути, не создавай свой.
- Создал или перенёс общий enum — в ответе отдельная строка `ENUM: {X} — путь` (по ней оркестратор запускает ждущих csharp); иначе `ENUM: нет`.
- Integration своего справочника (`--filter "FullyQualifiedName~{Entity}"`) запускай. Флаки в чужих таблицах — в «РИСКИ».

## Шаги

Выполни фазы 4–5 `csharp.md` по spec и `sql-report.md` (enum из раздела 5, в том числе новый файл пары Ru/En, если его ещё нет; DTO, настройки, resx/глоссарий, модуль, меню, API, тесты класса). `Class=complex` — SyncJob, тесты контракта job по фазе 5 и сценарий `{Entity}_IncrementalSync_PropagatesLegacyChange` в `SnapshotSyncIntegrationTests` рядом с уже существующими: стимул в legacy, `RunIncrementalAsync`, проверка снимка, cleanup. `simple` — без `Sync/`, DTO в массив `stableWithoutSyncJob` в `CatalogContractCoverageTests`. Новый enum создан по решению «simple» из раздела 8 — допиши строку справочника с путями enum в таблицу «Стабильный словарь с enum» `add-reference-book/SKILL.md`.

После тестов — п.6 фазы 6 `csharp.md`: `EXEC v2.GetBlazorGridData` и `v2.ExportBlazorGridDetail` с параметрами из атрибутов своего модуля. Числа сверь с проверкой фазы 3 в `sql-report.md`; расхождение — «ВОПРОСЫ».
