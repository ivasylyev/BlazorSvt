# Дочерние примитивы на карточке

Открывать, когда у справочника есть строка `EntityStructure` с `ParentId IS NOT NULL` (шаг 0 [SKILL.md](SKILL.md)). Правило одно для нового справочника и для готового модуля.

Термины `child primitive`, `relationship child`, `link child` — [glossary.md](../svt-architecture/glossary.md). Эталон — Coefficient: `Modules/Coefficient/Detail/*ValueSection.cs`, `*ValueDto.cs`, `SqlScripts/Modules/Coefficient/Programmability/vw_Coefficient*Value_Detail.sql`, тесты `CoefficientChildSections*`.

Platform не меняется: секция, загрузка, предел строк `Grid:ChildSectionRowLimit`, отбор по архиву родителя (`DetailChildRowStates`) и вывод в `GenericDetailView` уже есть. Дочерние строки не попадают в snapshot, sync, API, OpenAPI и полный Excel.

`{Child}` — имя дочернего примитива по правилу `_` шага 0 (`MTR_DPO_Directory` → `MTRDPODirectory`). `{ChildLegacyName}` — `PrimitiveEntityInfo.Name` ребёнка.

## Discovery

Запросы — [discovery-queries.md](discovery-queries.md) §6.

1. **Состав.** Ребёнок с `dbo.vw_{ChildLegacyName}` и хотя бы одной неархивной строкой входит в модуль по умолчанию. Без view или без неархивных строк — не входит, в отчёт без вопроса. Вложенный ребёнок (ребёнок ребёнка) — не входит, в отчёт.
2. **Порядок групп** — по `EntityStructure.Id`.
3. **Заголовок группы** — RU `PrimitiveEntityInfo.Description` ребёнка, EN — запрос «Заголовки» по `{ChildLegacyName}`.
4. **Колонки** — короткий список §1 [discovery-rules.md](discovery-rules.md) по `{ChildLegacyName}` после фильтра §3. `Code` — всегда. Подписи — §5 по `pei/{ChildLegacyName}/ai`, полные имена, short не задаётся ([grid-column-glossary.md](grid-column-glossary.md)).
5. **Ссылки** — по §6 `discovery-rules.md`, разворот в SQL-проекции: enum, если он уже есть в `Platform/Domain/IdsEnum`; иначе имя (`Name_ru`/`Name_en` или `NameRu`/`NameEn`, если есть; иначе `Name`). Узлы — имя без кода. Ссылка на справочник вне скоупа разворачивается так же, без enum и модуля.
6. **Связка (`link child`).** Кандидат — ребёнок, у которого среди заполненных неархивных атрибутов, кроме системных, есть ссылка на справочник со своей карточкой (не enum-словарь), а остальные — порядок или признак связи (пример: `TransportRouteLeg` — `Leg` и `Step`). Кандидат — вопрос стадии 3 «это связка?». Рекомендуемые поля цели — колонки грида цели, видимые по умолчанию, кроме колонок, которые уже есть на карточке родителя. Код ребёнка остаётся. Скрытые колонки грида цели не предлагать. Ссылки этих полей разворачиваются по п.5. Не связка — ссылка показывается только именем или кодом, как у Coefficient. Поля цели без согласования не добавлять.
7. **Объём.** По каждому ребёнку — строк всего и неархивных, максимум на родителя всего и неархивных. Оба максимума — в spec: нагрузку запроса задаёт число всех строк на родителя.
8. **Вопросы стадии 3** (рекомендация первой):
   - ребёнок одновременно основной примитив сущности реестра, или его ссылки ведут вне скоупа — «показать / не показывать», рекомендация «показать»;
   - `relationship child`, у которого максимум неархивных на родителя больше `Grid:ChildSectionRowLimit`, — «показать с пределом / не показывать», рекомендация «показать с пределом»;
   - кандидат в связку — п.6.
9. **Пересечение с карточкой.** Поле родителя уже показывает детей (`STRING_AGG`, колонки `Leg_01..03` основного примитива) — поле остаётся, группа добавляется. Убрать поле — отдельное решение пользователя. Пересечение — в spec.
10. **Аномалии данных** (несколько строк с одинаковым порядком, расхождение view и `PrimitiveEntityItem`) — риск в разделе 9 spec, работу не останавливают.

## SQL

`SqlScripts/Modules/{Entity}/Programmability/vw_{Child}_Detail.sql` → `v2.vw_{Child}_Detail`. В шапке — номер таблицы `PrimitiveEntityData_NNNN` ребёнка и имя родителя.

- Источник — `dbo.vw_{ChildLegacyName}`. Колонки: `{Entity}Id` (`CAST({ParentLegacyName}Id AS BIGINT)` — колонка родителя во view ребёнка), `Id`, `PrimitiveEntityDataStateId`, колонки таблицы.
- Ссылки и поля цели связки — `LEFT JOIN` на `dbo.vw_*`. Отбор по родителю и состоянию — в запросе секции, не во view.
- Комментарий `vw_{Entity}_Detail` «дочерние … не выводятся», если есть, — обновить.
- Проекция идёт в `Publish-AllSql.ps1` после `vw_{Entity}_Detail.sql`, в таблицу модуля `SqlScripts/README.md` и в снимок `All_Programmability.sql` (не дельтой).
- Проверка после наката: `SELECT TOP 5 * FROM v2.vw_{Child}_Detail WHERE {Entity}Id = <родитель с неархивными детьми>` — строки есть, ссылки развёрнуты.

## C#

- `Modules/{Entity}/Detail/{Child}Dto.cs` — `[DetailChildSource("v2.vw_{Child}_Detail", "{Entity}Id")]`; свойства `{Entity}Id`, `Id`, `PrimitiveEntityDataStateId`, `IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival`, колонки. Nullable-enum ссылки — перегрузка `AddEnum` для nullable.
- `Modules/{Entity}/Detail/{Child}Section.cs` — наследник `DetailChildSection<{Entity}DetailDto, {Child}Dto>`: `Title` из `{Child}Dto.Title`, `ParentKey` — `{Entity}Id`, `ParentIsArchive` — `IsArchive` родителя, колонки через `DetailSettingsBuilder`. Колонка состояния (`AddArchiveStatus`) — только при архивном родителе.
- `{Entity}Module.cs` — `services.AddScoped<IDetailChildSection<{Entity}DetailDto>, {Child}Section>();` по одной строке на ребёнка, в порядке групп.
- Resx модуля: `{Child}Dto.Title`, `{Child}Dto.{Field}`. Подписи полей цели связки — тоже в resx модуля-родителя; ресурсы и классы модуля цели не использовать.
- `grid-column-glossary.md` — блок «Таблицы дочерних примитивов на карточке {Entity}» в разделе справочника.
- `{Entity}DetailDto`, `AddCatalogApi` и снимок OpenAPI не меняются.

## Тесты

- Unit `tests/BlazorSvt.UnitTests/Modules/{Entity}/{Entity}ChildSectionsTests.cs`: порядок групп по регистрации в `Add{Entity}Module`, состав и порядок колонок, подписи и заголовки RU/EN из настоящих resx, не ключи.
- Integration `tests/BlazorSvt.IntegrationTests/Modules/{Entity}/{Entity}ChildSectionsIntegrationTests.cs`: по каждой проекции на родителе с неархивными детьми — строки есть, ссылки развёрнуты в имена.
- Тесты `DetailChildRowStates`, предела строк и `AddEnum` уже есть в Platform — на модуль не повторять.
- `OpenApiContractTests` проходит без перегенерации снимка.

## Готовый модуль

Модуль уже есть, а группы ребёнка из п.1 Discovery на карточке нет.

- **Оркестратор, доработка.** Пользователь уже попросил таблицы детей. В `plan.md` статус `в работе`, примечание `доработка`, в промптах `Режим: доработка`. После закрытых вопросов раздела 9 сразу sql, затем csharp. Ревьюер — после записи. Вопрос «исправлять?» не задавать: без него прогон заканчивается отчётом и не меняет базу и C#.
- **Оркестратор, проверка** («проверь», без просьбы добавить): discovery заполняет раздел 3а, ревьюер пишет отсутствие группы дефектом `[sql|csharp]`. Sql до согласия на этот список не запускается. Согласие — отдельные sql и csharp; статус «уже мигрирован» не повод их пропустить, если в промпте есть дефект `[sql]` / `[csharp]`.
- **Без оркестратора**: разделы Discovery → SQL (накат — skill `create-programmability`) → C# → Тесты этого файла.

В обоих случаях Structure 01–03, SnapshotSource, SyncJob, `PopulateAffectedKeys`, `RowVer`, API и уже выведенные поля карточки не трогаются. Снимок `All_Programmability.sql` — в текущей `Migrations/{VersionPrefix}/` (выкаченную папку не править).
