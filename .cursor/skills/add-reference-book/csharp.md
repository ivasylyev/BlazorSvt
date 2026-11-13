# Фазы 4–6 — C#-модуль, тесты, финал

Читать только после успешного деплоя и верификации SQL ([sql.md](sql.md)).  
Стабильный словарь с enum — [SKILL.md](SKILL.md). Именование ссылок/enum — [discovery.md](discovery.md).

## Фаза 4 — C#-модуль

```
BlazorSvt/Modules/{Entity}/
├── {Entity}Module.cs
├── List/
│   ├── {Entity}Dto.cs
│   ├── {Entity}Grid.razor
│   ├── {Entity}Grid.razor.cs
│   ├── {Entity}GridSettingsService.cs
│   └── IdsEnum/          # только уникальные для модуля словари
├── Detail/
│   ├── {Entity}DetailDto.cs
│   └── {Entity}DetailSettingsService.cs
├── Sync/               # нет у стабильного словаря с enum
│   └── {Entity}SyncJob.cs
└── Resources/
    ├── {Entity}.resx
    └── {Entity}.ru-RU.resx
```

### DTO-атрибуты (обязательно)

```csharp
[GridSnapshot("v2.{Entity}_Snapshot")]
public class {Entity}Dto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long {Entity}Id { get; set; }

    [GridColumn(SqlColumn = "RateTypeId")]
    public required RateTypeRu RateTypeIdRu { get; set; }
    // ...
}

[DetailSource("v2.vw_{Entity}_Detail", "{Entity}Id")]
public class {Entity}DetailDto { ... }
```

`{Entity}Dto` — поля snapshot + `[GridColumn]` на каждое поле grid/фильтра.  
`{Entity}DetailDto` — поля `vw_{Entity}_Detail`.

**Enum-ссылки (IdsEnum):** поля, типизированные enum маленького словаря, объявлять **обязательными non-nullable** (`required`), как в `TransportRateDto`:

```csharp
public required RateTypeRu RateTypeIdRu { get; set; }
public required RateTypeEn RateTypeIdEn { get; set; }
```

Обычные FK **длинных** словарей в grid DTO — только `NameRu`/`NameEn` (+ `RegionCode` из whitelist); Id **не** включать. Эталон: `TransportRateDto` (узлы), `LocationsNodesDto` (Region/Country).

### GridSettingsService

- Наследник `BaseGridSettingsService<{Entity}Dto>`
- `StorageKey` → `"{Entity}GridColumnSettings"`
- `GetDefaultSettings`: порядок как **короткий список (discovery §1)**
- **Все колонки видимы**, кроме `IsArchive`, `CreationDate`, `LastChangeDate`
- `IsArchive`: `Visible = false`, `FilterValue = "False"` **всегда**
- `CreationDate`, `LastChangeDate`: `Visible = false` **всегда** (без `FilterValue`)
- **Короткий словарь (enum):** видимая колонка `{X}IdRu`/`{X}IdEn` с `GetDisplayName`; без `Code`/`Name*`.
- **Длинный словарь:** видимая `{X}NameRu`/`{X}NameEn`; **без Id**, без дублирующей скрытой Name-колонки.
- **Whitelist Code** (`NodeToCode`, `RegionCode`, …): `{X}Code` — `Visible = false`, `Filterable = true` (пользователь может включить в настройках).
- `Filterable = true` для текстовых и enum; `false` для вычисляемых числовых полей

### DetailSettingsService

- `IDetailSettingsService<{Entity}DetailDto>`
- Порядок и группы как **длинный список (discovery §4)**
- `GroupHeader` из resx: ключ `{Entity}DetailDto.Group.{GroupRank}.{SanitizedGroupNameEn}`
- `SanitizedGroupNameEn`: PascalCase, только буквы/цифры (пробелы и спецсимволы удалить)
- Пустые значения (`null` / `""` / whitespace) скрываются в UI при `Grid:HideEmptyDetailFields` (default `true`) — не дублировать `visible: dto => x is not null` для обычных optional-строк
- Явный `visible:` — только для условных блоков (proxy/leg, calc type и т.п.); скрытие пустых к нему AND в `GenericDetailView`

### Grid.razor

```razor
@page "/{entity}"   @* lowercase *@
@inherits BaseGridPage<{Entity}Dto, {Entity}DetailDto>
```

- `GenericGrid` + заглушка `DetailViewTemplate` (как `LegsGrid.razor`)
- `DetailKeySelector` → `{Entity}Id`

### Локализация

**Platform** (общие UI-компоненты): `Platform/Resources/Platform.resx`, `Platform.ru-RU.resx`  
Маркер: `BlazorSvt.Platform.Resources.Platform`, в коде — `IStringLocalizer<PlatformResources>` (alias в `GlobalUsings.cs`).

**Модуль** (строки справочника): `Modules/{Entity}/Resources/{Entity}.resx`, `{Entity}.ru-RU.resx`  
Маркер: `BlazorSvt.Modules.{Entity}.Resources.{Entity}`, в сервисах — `IStringLocalizer<Resources.{Entity}>`.

Ключи в **модульном** resx:

- `{Entity}Dto.{Field}`, `{Entity}DetailDto.{Field}`
- Для пар колонок Ru/En допустим **один** ключ заголовка без суффикса языка (например `NodeFromName` вместо `NodeFromNameRu`/`NodeFromNameEn`) — один заголовок на Ru/En колонку в `GridSettingsService`
- `{Entity}Grid.Title`
- `{Entity}DetailDto.Group.{GroupRank}.{SanitizedGroupNameEn}`

**Короткие заголовки грида:** см. [grid-column-glossary.md](grid-column-glossary.md) (пост-обработка после discovery §5; единообразие с существующими модулями; обогащение словаря при новых общих полях).

Ключи в **Platform** resx:

- `HeaderMenu.{Entity}` (пункт меню = `{Entity}Grid.Title`, без сокращений)
- Прочие cross-cutting строки не добавлять в модульный resx

В `{Entity}Grid.razor.cs` — `IStringLocalizer<Resources.{Entity}> EL` для `PageTitle` и специфичных сообщений; `SvtComponentBase.L` — только platform-строки.

### DI и меню

**`{Entity}Module.cs`:**

```csharp
services.AddScoped<IGridSettingsService<{Entity}Dto>, {Entity}GridSettingsService>();
services.AddScoped<IDetailSettingsService<{Entity}DetailDto>, {Entity}DetailSettingsService>();
services.AddSingleton<ISnapshotSyncJob, {Entity}SyncJob>();
services.AddCatalogMenu(new CatalogMenuContribution(
    CatalogDomain.{Domain},
    "{entity}",
    "HeaderMenu.{Entity}",
    VisibleToEditor: true));
```

`{entity}` — сегмент `@page` без ведущего слэша. В файле модуля нужен `using BlazorSvt.Platform.UI.Navigation;`. `{Domain}` — член `CatalogDomain` по домену справочника в [catalogs-scope.md](../svt-architecture/catalogs-scope.md):

| Домен в реестре | `CatalogDomain` |
|---|---|
| Ставки | `Rates` |
| Паритеты | `Parities` |
| Маршруты | `Routes` |
| Продукты | `Products` |
| Склады и ограничения | `Warehouses` |
| Матрица каналов | `MK` |
| Интеграция с ЦБД | `Cbd` |
| Вспомогательные | `Other` |

`VisibleToEditor` — колонка «Меню ЕО» (`да` → `true`, `нет` → `false`). До MVP 0.4 шапка по этому флагу не фильтрует. Порядок пункта не задаётся: шапка сортирует подписи по алфавиту текущего языка. `HeaderMenu` для нового справочника не менять.

**`{Entity}SyncJob`:** эталон `TransportRateSyncJob` / `AverageRateLevel3SyncJob` — consts `PrimitiveEntityData_*` с комментариями имён справочников; Sources = основная + каскад (узлы / ProductGroup / MTR и т.д. по проекции). Стабильные словари (RateType, Currency, …) **не** включать. Если сам `{Entity}` — стабильный словарь с enum, SyncJob нет: только разовая заливка.

**`Host/Program.cs`:** `builder.Services.Add{Entity}Module();`

URL вклада **должен совпадать** с `@page` в `.razor`.

### API для Атласа (обязательно)

Всё, что есть в СВТ 2.0, доступно через `/api/v1`. В том же `{Entity}Module.cs`, сразу после `AddCatalogMenu`:

```csharp
services.AddCatalogApi<{Entity}Dto, {Entity}DetailDto>(
    "{kebab-code}",
    CatalogDomain.{Domain},
    "HeaderMenu.{Entity}",
    new CatalogPeriod(nameof({Entity}Dto.StartDate), nameof({Entity}Dto.EndDate)));
```

- `using BlazorSvt.Platform.Api.Catalogs;`
- `{kebab-code}` — код в API, стабильный контракт: множественное число в kebab-case (`transport-rates`, `locations-nodes`). Сменить потом — только новой версией API
- Заголовок — тот же ключ, что у меню (`HeaderMenu.{Entity}`); домен — тот же `CatalogDomain`
- `period` — только если у справочника есть срок действия (две даты `DateOnly` в list-DTO); иначе не передавать. Даёт фильтр `asOf`
- `syncEntity` — имя в `v2.SyncState` для `dataAsOf`; по умолчанию имя DTO без `Dto`. У стабильного словаря с enum строки в `SyncState` нет — `dataAsOf` будет `null`, это нормально
- Коды других справочников в list-DTO (узлы и т.п.) — `[ApiReference("{код справочника}", "code")]` рядом с `[GridColumn]`
- Схема, фильтры, выгрузка и OpenAPI строятся из тех же `[GridColumn]`, grid- и detail-настроек — отдельного SQL и DTO не нужно

## Фаза 5 — Тесты

Эталоны: `GridColumnMetadataBuilderTests`, `SnapshotSyncJobContractTests`, `ModuleGridIntegrationTests`, `TransportRateFtsIntegrationTests` / `AverageRateLevel3FtsIntegrationTests`.

### Unit (обязательно)

1. Добавить `{Entity}Dto` в `[Theory]` contract-тест `GridColumnMetadataBuilderTests`:
   - `TableName` = `v2.{Entity}_Snapshot`
   - `EntityKeyPropertyName` = `{Entity}Id`
2. Добавить `{Entity}SyncJob` в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). Для стабильного словаря с enum пункт не делать: job нет.
3. При изменении Platform-логики — тесты в `GridQueryFactoryTests` / `GridColumnMetadataBuilderTests`
4. `CatalogApiCoverageTests` новых правок не требует, но должен пройти: справочник без `AddCatalogApi` или с несовпадающим ключом меню его валит
5. Перегенерировать снимок контракта API — в нём появятся маршруты справочника: `$env:UPDATE_OPENAPI_SNAPSHOT=1; dotnet test tests/BlazorSvt.IntegrationTests --filter OpenApiContractTests`, затем убрать переменную и прогнать ещё раз. Изменённый `docs/api/svt-api-v1.openapi.json` — в тот же коммит

### Integration (обязательно, read-only)

1. **Grid smoke** + **Detail view** — в `ModuleGridIntegrationTests.cs` (или `Modules/{Entity}/`)
2. **FTS** — `Modules/{Entity}/{Entity}FtsIntegrationTests.cs` по образцу TransportRate / AverageRateLevel3 (`FtsFilterTestSupport`: `IsArchive=False` + Contains по Name-полям; при наличии — фильтр enum TransportKind/TypeNode)

Только read-only на существующих данных локальной `mdm`. **Без** INSERT/ROLLBACK. Пометить `[Trait("Category", "Integration")]` + `[SkippableFact]`.

### Запуск

Перед Integration задай `BLAZORSVT_TEST_CONNECTION`: строка `Database:MdmDb` с `Server=localhost`. Без неё тесты возьмут сервер из `appsettings.json`. Файл не менять.

```powershell
$env:BLAZORSVT_TEST_CONNECTION = "<Database:MdmDb с Server=localhost>"
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

## Фаза 6 — Финальная верификация

1. `dotnet build` (проект `BlazorSvt`)
2. `dotnet test`
3. Страница `/{entity}` открывается, грид загружает данные
4. Детальный просмотр и экспорт не падают
5. В Swagger UI (`/api/swagger`, Development) справочник есть в `GET /api/v1/catalogs`; `schema` и `items/search` отвечают

При ошибке — сообщить и спросить дальнейшие действия.

## Рефакторинг legacy

- Для legacy-модулей (`Legs`, `Rates`) использовать только `git mv` для папок и файлов, затем правки содержимого in-place.
- Не удалять и не пересоздавать файлы/папки через copy+delete, чтобы сохранить git-history.
- При rename SQL сохранять существующие комментарии и нумерованные шаги; добавлять новые `DROP`-блоки в начало, а не заменять ими текущие блоки.
- В dev-среде wrapper'ы и миграции данных не требуются: удалять legacy и предыдущие имена объектов прямо в скриптах.
