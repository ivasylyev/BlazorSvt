# SQL-скрипты BlazorSVT

Структура повторяет модульный монолит C#: общие объекты в `Platform/`, объекты справочников — в `Modules/{Name}/`.

**Кодировка:** все `*.sql` в этой папке — **UTF-8 with BOM** (без исключений). Нужно для корректного отображения кириллицы в Visual Studio.

```
SqlScripts/
├── Platform/
│   ├── Structure/          # Схема v2, full-text catalog, v2.SyncState
│   └── Programmability/    # Универсальный grid engine
├── Sync/                   # Синхронизация legacy → snapshot (rowversion на legacy)
├── Modules/
│   ├── TransportRate/
│   │   ├── Structure/      # Snapshot-таблица, индексы, seed
│   │   └── Programmability/ # Detail view, export
│   ├── ParityRates/
│   │   ├── Structure/
│   │   └── Programmability/
│   ├── AverageRateLevel3/
│   │   ├── Structure/
│   │   └── Programmability/
│   ├── TransportLeg/
│   │   ├── Structure/
│   │   └── Programmability/ # Detail view + snapshot-source проекция
│   └── LocationsNodes/
│       ├── Structure/
│       └── Programmability/
└── Translations_info.txt   # Справочник переводов полей (не для деплоя)
```

## Порядок выполнения

### 1. Platform (обязательно первым)

| # | Скрипт |
|---|--------|
| 1 | `Platform/Structure/00.Create_Schema.sql` |
| 2 | `Platform/Structure/01.Create_Fulltext_Catalog.sql` |
| 3 | `Platform/Structure/02.Create_SyncState.sql` |
| 4 | `Platform/Programmability/fn_GetDateSqlOperator.sql` |
| 5 | `Platform/Programmability/sp_GetBlazorGridData.sql` |
| 6 | `Platform/Programmability/sp_ExportBlazorGridDetail.sql` |
| 7 | `Platform/Programmability/sp_SyncState_Get.sql` |
| 8 | `Platform/Programmability/sp_SyncState_Upsert.sql` |
| 9 | `Platform/Programmability/sp_SyncState_MarkReconciled.sql` |
| 10 | `Platform/Programmability/sp_Sync_GetHighWatermark.sql` |
| 11 | `Platform/Programmability/sp_Sync_UpsertAffected.sql` |
| 12 | `Platform/Programmability/sp_Sync_Reconcile.sql` |

### 2. Modules (по одному модулю: Structure → Programmability)

**TransportRate** (проекция создаётся до Insert — он из неё заливается)

| # | Скрипт |
|---|--------|
| 1 | `Modules/TransportRate/Structure/01.TransportRate_CreateTable.sql` |
| 2 | `Modules/TransportRate/Programmability/vw_TransportRate_SnapshotSource.sql` |
| 3 | `Modules/TransportRate/Structure/02.TransportRate_Insert.sql` |
| 4 | `Modules/TransportRate/Structure/03.TransportRate_CreateIndexes.sql` |
| 5 | `Modules/TransportRate/Programmability/vw_TransportRate_Detail.sql` |
| 6 | `Modules/TransportRate/Programmability/sp_TransportRate_PopulateAffectedKeys.sql` |

**ParityRates** (проекция создаётся до Insert — он из неё заливается)

| # | Скрипт |
|---|--------|
| 1 | `Modules/ParityRates/Structure/01.ParityRates_CreateTable.sql` |
| 2 | `Modules/ParityRates/Programmability/vw_ParityRates_SnapshotSource.sql` |
| 3 | `Modules/ParityRates/Structure/02.ParityRates_Insert.sql` |
| 4 | `Modules/ParityRates/Structure/03.ParityRates_CreateIndexes.sql` |
| 5 | `Modules/ParityRates/Programmability/vw_ParityRates_Detail.sql` |
| 6 | `Modules/ParityRates/Programmability/sp_ParityRates_PopulateAffectedKeys.sql` |

**AverageRateLevel3** (проекция создаётся до Insert — он из неё заливается)

| # | Скрипт |
|---|--------|
| 1 | `Modules/AverageRateLevel3/Structure/01.AverageRateLevel3_CreateTable.sql` |
| 2 | `Modules/AverageRateLevel3/Programmability/vw_AverageRateLevel3_SnapshotSource.sql` |
| 3 | `Modules/AverageRateLevel3/Structure/02.AverageRateLevel3_Insert.sql` |
| 4 | `Modules/AverageRateLevel3/Structure/03.AverageRateLevel3_CreateIndexes.sql` |
| 5 | `Modules/AverageRateLevel3/Programmability/vw_AverageRateLevel3_Detail.sql` |
| 6 | `Modules/AverageRateLevel3/Programmability/sp_AverageRateLevel3_PopulateAffectedKeys.sql` |

**TransportLeg** (проекция создаётся до Insert — он из неё заливается)

| # | Скрипт |
|---|--------|
| 1 | `Modules/TransportLeg/Structure/01.TransportLeg_CreateTable.sql` |
| 2 | `Modules/TransportLeg/Programmability/vw_TransportLeg_SnapshotSource.sql` |
| 3 | `Modules/TransportLeg/Structure/02.TransportLeg_Insert.sql` |
| 4 | `Modules/TransportLeg/Structure/03.TransportLeg_CreateIndexes.sql` |
| 5 | `Modules/TransportLeg/Programmability/vw_TransportLeg_Detail.sql` |
| 6 | `Modules/TransportLeg/Programmability/sp_TransportLeg_PopulateAffectedKeys.sql` |

**LocationsNodes** (проекция создаётся до Insert — он из неё заливается)

| # | Скрипт |
|---|--------|
| 1 | `Modules/LocationsNodes/Structure/01.LocationsNodes_CreateTable.sql` |
| 2 | `Modules/LocationsNodes/Programmability/vw_LocationsNodes_SnapshotSource.sql` |
| 3 | `Modules/LocationsNodes/Structure/02.LocationsNodes_Insert.sql` |
| 4 | `Modules/LocationsNodes/Structure/03.LocationsNodes_CreateIndexes.sql` |
| 5 | `Modules/LocationsNodes/Programmability/vw_LocationsNodes_Detail.sql` |
| 6 | `Modules/LocationsNodes/Programmability/sp_LocationsNodes_PopulateAffectedKeys.sql` |

## Grid read-модель

Список колонок для grid (`@AllowedColumnsJson`, `@SelectList`) формируется в C# из атрибутов `[GridSnapshot]` и `[GridColumn]` на `{Entity}Dto`.  
`GridDataService` вызывает `v2.GetBlazorGridData` напрямую; процедур-прослоек `{Entity}_Get` нет.

Full export: `v2.ExportBlazorGridDetail` (Platform) — `@DetailViewName` и `@EntityKeyColumn` из `[DetailSource]` на detail-DTO, grid-метаданные из list-DTO.

## Синхронизация legacy → snapshot (одностороннее, eventual consistency)

Инкрементальная синхронизация из legacy в v2 snapshot. В C#
(`Platform/Sync/`, фоновый `SnapshotSyncScheduler`) — только оркестрация
(расписание, blackout-окна, продвижение курсоров, изоляция ошибок); вся
SQL-логика вынесена в процедуры. Все обращения к БД идут через
`DbConnectionLogDecorator` (единое логирование/тайминги).

**Выключатели.** Синк идёт только если оба включены:

| Уровень | Источник | Поведение |
|---------|----------|-----------|
| Деплой | `Sync:Enabled` в appsettings | `false` — воркер не стартует (нужен рестарт) |
| Оперативный | `V2SyncEnabled` в `dbo.vw_FeatureToggle` | опрос каждый тик; `0` / нет строки / ошибка чтения — цикл пропускается без рестарта (fail-closed) |

**Процедуры (Platform/Programmability):**

| Процедура | Назначение |
|-----------|------------|
| `v2.Sync_GetHighWatermark` | Граница цикла `@Hi = MIN_ACTIVE_ROWVERSION() - 1` |
| `v2.Sync_UpsertAffected` | Generic партиционно-безопасный upsert из проекции по `#AffectedKeys` |
| `v2.Sync_Reconcile` | Generic anti-join удаление фантомов |
| `v2.SyncState_Get` / `_Upsert` / `_MarkReconciled` | Курсоры `v2.SyncState` |

**Процедура детекции (per-entity, Modules/{Name}/Programmability):**
`v2.{Name}_PopulateAffectedKeys @Source, @Lo, @Hi` — по одному источнику за вызов
наполняет `#AffectedKeys` бизнес-ключами затронутых записей (основная таблица +
каскад через FK-колонки snapshot). Temp-таблицу создаёт вызывающая C#-сессия.

**Механизм детекции — `rowversion`.** На базовых таблицах legacy
(`dbo.PrimitiveEntityData_*`) добавляется служебный столбец `RowVer`
(движок инкрементит его при любом INSERT/UPDATE). Воркер по границе
`MIN_ACTIVE_ROWVERSION() - 1` выбирает только гарантированно закоммиченные
изменения — без пропусков и без RCSI. `LastChangeDate` для детекции
**не** используется: пайплайн legacy местами меняет строки, не обновляя его.

**Каскад.** Snapshot денормализован (имена/коды из Region, LocationsNodes и
т.д.). Изменение справочника-источника инвалидирует зависимые строки через
скрытые FK-колонки snapshot (`*_Id`) — курсор ведётся по каждой таблице-источнику.

**Стабильные справочники** (не отслеживаются, без `RowVer` и без каскада):
`1007` TypePlace, `2132` TypeNode, `2008` TransportKind,
`2048` RateType, `2023` TransportType, `2016` Currency. Колонки `*_Id` в snapshot
остаются, если нужны grid-DTO. Полный список — `.cursor/rules/svt-development-patterns.mdc`.
У TransportLeg тип отправки — сырой `ShipmentTypeCodeT` (не FK/enum на `2142`).

**Удаления.** Ловятся не инкрементом, а `reconciliation` (anti-join snapshot ↔
проекция), который воркер запускает раз в сутки. Допустимо, что физически
удалённая запись «фантомит» в snapshot до суток.

### Разовый шаг на legacy (в maintenance-окне)

| # | Скрипт | Назначение |
|---|--------|------------|
| — | `Sync/01.Legacy_AddRowVersion.sql` | Добавляет `RowVer` в `dbo.PrimitiveEntityData_*` (аддитивно) |

Изменение не ломает пайплайн `stg.Validate/Preload/Load/PostLoad`: он пишет в
эти таблицы только с явным списком колонок, а все операции legacy идут через
вью (прямых `SELECT *` из базовых таблиц нет).

### Включение воркера

Секция `Sync` в `appsettings.json`. По умолчанию выключен (`Enabled: false`).

| Параметр | Назначение |
|----------|------------|
| `IntervalSeconds` | Период инкрементального цикла (по умолч. 90) |
| `ReconcileAtTime` | Время суток ежедневного reconcile (`"02:00"`) |
| `TimeZone` | Пояс для `ReconcileAtTime` и `BlackoutIntervals` (`"Russian Standard Time"`) |
| `CommandTimeoutSeconds` | Таймаут SQL-команд (по умолч. 300) |
| `BlackoutIntervals` | Окна, когда инкремент не запускается (см. ниже) |

**BlackoutIntervals.** Список окон с явным перечислением дней недели:
`{ "StartTime": "HH:mm:ss", "EndTime": "HH:mm:ss", "DaysOfWeek": ["Monday", ...] }`.
Семантика — полуоткрытый интервал `[StartTime; EndTime)`. Один день может
входить в несколько окон (несколько записей или пересекающиеся интервалы).
`DaysOfWeek` не может быть пустым. В окнах глушится только инкремент;
reconcile выполняется — окна задуманы под тяжёлые ночные операции.
Пример: ежедневно `01:00–02:30`, а в субботу дополнительно `04:00–06:00`.

`ReconcileAtTime` без catch-up: если приложение было выключено в момент отметки,
reconcile не догоняется, а ждёт следующих суток.

Первичная заливка (`02.*_Insert.sql`) сама инициализирует курсоры
`v2.SyncState` на текущую границу — воркер подхватит только последующие изменения.

Новый справочник подключается к синхронизации так: создать
`Modules/{Name}/Programmability/vw_{Name}_SnapshotSource.sql` (проекция) и
`sp_{Name}_PopulateAffectedKeys.sql` (детекция/каскад), реализовать
`ISnapshotSyncJob` в `Modules/{Name}/Sync/` и зарегистрировать его
`services.AddSingleton<ISnapshotSyncJob, {Name}SyncJob>()`.

## Добавление нового справочника

Создать `Modules/{Name}/Structure/` и `Modules/{Name}/Programmability/` по образцу `TransportRate/` или `TransportLeg/`.
Параллельно — C#-модуль в `BlazorSvt/Modules/{Name}/` с атрибутами на list-DTO.

Ключевая процедура `v2.GetBlazorGridData` (Platform) — универсальный query engine: whitelist колонок, фильтры, FTS, пагинация.

### THROW в T-SQL (обязательно)

`THROW` должен быть **первым** оператором в batch. Внутри `BEGIN…END` всегда пишите с ведущей точкой с запятой — иначе SSDT / SQL Server Data Tools в Visual Studio подчёркивает ошибку, а без `BEGIN…END` конструкция `IF … ;THROW` ломает sqlcmd:

```sql
BEGIN
    ;THROW 50000, N'Message.', 1;
END
```

## Накат Programmability (dev)

```powershell
.\BlazorSvt\SqlScripts\Create-Programmability.ps1
```

Скрипт читает строку подключения из `appsettings.json`, выполняет Platform и все модули; при ошибке останавливается.
В Cursor: `/create_programmability`.

## Публикация SQL для деплоя

```powershell
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1
```

Копирует все deploy-скрипты (Structure + Programmability) в плоскую папку `C:\publish\v2` с трёхзначными префиксами (`001.`, `002.`, …) в порядке из раздела «Порядок выполнения» выше. Сценарий — полный re-deploy v2; накат вручную, скрипт за скриптом.

В Cursor: `/publish_all_sql`.

При добавлении справочника обновить манифест в `Publish-AllSql.ps1` и таблицы порядка в этом README.
