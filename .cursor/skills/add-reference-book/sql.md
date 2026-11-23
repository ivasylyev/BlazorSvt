# Фазы 2–3 — SQL и деплой БД

Читать после фазы 1 ([discovery-rules.md](discovery-rules.md)). Запросы discovery повторно не открывать. C# — только после успешной верификации здесь → [csharp.md](csharp.md).  
Стабильный словарь с enum — правила в [SKILL.md](SKILL.md).

## Фаза 2 — SQL-артефакты

**Кодировка (обязательно):** все файлы `BlazorSvt/SqlScripts/**/*.sql` — **UTF-8 with BOM** (`EF BB BF`). Без исключений: Structure, Programmability, Platform, Sync. Иначе Visual Studio на русской Windows открывает кириллические комментарии как «кракозябры». При создании/правке сохранять с BOM (не UTF-8 без BOM).

**`THROW` (обязательно):** внутри `BEGIN…END` всегда с ведущей точкой с запятой:

```sql
BEGIN
    ;THROW 50000, N'Message.', 1;
END
```

Иначе SSDT в Visual Studio подчёркивает ошибку; вариант `IF … ;THROW` без `BEGIN…END` ломает sqlcmd.

Папки:

```
BlazorSvt/SqlScripts/Modules/{Entity}/
├── Structure/
│   ├── 01.{Entity}_CreateTable.sql
│   ├── 02.{Entity}_Insert.sql
│   └── 03.{Entity}_CreateIndexes.sql
└── Programmability/
    ├── vw_{Entity}_SnapshotSource.sql
    ├── vw_{Entity}_Detail.sql
    └── sp_{Entity}_PopulateAffectedKeys.sql   # нет у стабильного словаря с enum
```

`sp_{Entity}_PopulateAffectedKeys.sql` не создавать для стабильного словаря с enum и не добавлять его в `Publish-AllSql.ps1`. Для остальных обновить манифест в порядке: CreateTable → SnapshotSource → Insert → Indexes → Detail → PopulateAffectedKeys.

Релизная копия — `SqlScripts/Migrations/{VersionPrefix}/`, `{VersionPrefix}` из `BlazorSvt/BlazorSvt.csproj`. Перед первой копией выяснить, выкачена ли эта папка (канон — пользователь; папка с пометкой «не править» в README миграций уже выкачена).

- Папка ещё не выкатывалась — следующий свободный номер в ней.
- Папка уже на стенде — сначала поднять `VersionPrefix`. Новая папка начинается с `001`. Старую не править.
- «Нумерация с `001`» относится только к новой папке. Последний файл папки — полный снимок `All_Programmability.sql`. В `C:\publish` само по себе не копировать. Как собрать снимок — `Migrations/README.md`.

### Structure

**01 — CreateTable:** системные поля + атрибуты короткого списка (порядок discovery §1); партиционирование; SEQUENCE; DEFAULT на `Id`, `CreationDate`; скрытые FK (`*_Id`) для каскада sync.  
Образец: `SqlScripts/Modules/TransportRate/Structure/01.TransportRate_CreateTable.sql`.

**02 — Insert:** `INSERT INTO v2.{Entity}_Snapshot … SELECT … FROM v2.vw_{Entity}_SnapshotSource` (не напрямую из legacy `vw_{LegacyName}`).  
После INSERT — инициализация `v2.SyncState` для всех источников job’а. Колонки SyncState: **`LastRowVersion`** (не `LastRowVer`), `LastRunUtc`. Копировать MERGE из эталона (`TransportRate` / `AverageRateLevel3`). У стабильного словаря с enum SyncState не инициализировать: заливка разовая, job нет.  
На каждой строке `PrimitiveEntityData_*` в CTE Sources — комментарий с именем справочника:

```sql
SELECT N'dbo.PrimitiveEntityData_2012'   -- TransportRate (основная)
UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
```

**Внимание:** если Insert успел залить snapshot и упал на SyncState — повторный прогон даст дубликаты. Перед повторным Insert очищать snapshot или дедуплицировать по `{Entity}Id`.

**03 — Indexes:**

- `UX_{Entity}_Snapshot_Id` на `[PRIMARY]` (для FTS при партициях)
- FULLTEXT (языки 1033 / 1049):
  - `*NameEn`, `*NameRu`
  - whitelist Codes (`NodeFromCode`, …)
  - **все прочие NVARCHAR-поля короткого списка / грида** (свободный текст: `Comment`, `DataSource`, `Methodology`, …)
  - `Code` сущности: **только** если `NVARCHAR` **и** значения — осмысленный текст для поиска. Если `Code` — GUID / UUID-строка (как у ParityRates) — **FTS не ставить**, достаточно фильтрованных NCIX по `Code`
  - числовой `Code` (`INT`) — FTS не нужен, только фильтрованные индексы
  - `decimal` / даты / bit / Id — **не** в FTS
- `ALTER FULLTEXT … SET STOPLIST = OFF`
- Фильтрованные индексы на `Code` сущности и ID **коротких** enum-ссылок (`LocationTypeId`, …); **не** создавать NCIX по Id длинных ссылок (Region, Country)
- NCIX на `{Entity}Id` и скрытые FK каскада (`NodeFromId`, …)
- `UPDATE STATISTICS … WITH FULLSCAN`

### Programmability

**vw_{Entity}_SnapshotSource** — единая проекция для первичной заливки и incremental MERGE. WHERE-фильтр членства из эталона не переносить. Отсечение (NOT NULL ключевой метрики, `EXISTS` «без связанных X — битая запись») писать только для бизнес-правила этого справочника. Форма такого отсечения — у `vw_TransportRate_SnapshotSource` и `vw_AverageRateLevel3_SnapshotSource`.

**vw_{Entity}_Detail** — карточка и полный Excel. Читает легаси-представление основной таблицы (шаг 0: это не всегда `dbo.vw_{Entity}`) и связанные `dbo.vw_*`; грид читает `v2.{Entity}_Snapshot`. Поля длинного списка (discovery §4) + системные; ссылки: Code/Name из joined views; при необходимости плечи/лидтаймы (эталон: `vw_TransportRate_Detail`, `vw_AverageRateLevel3_Detail`).  
**Дочерний примитив** — своя проекция `v2.vw_{Child}_Detail` и группа-таблица на карточке: [child-primitives.md](child-primitives.md), раздел «SQL». Вычисляемое поле через `STRING_AGG` **только в detail / export**, не в snapshot, — для связанной сущности, которая не является дочерним примитивом.

**sp_{Entity}_PopulateAffectedKeys** — детекция по `@Source` / `@Lo` / `@Hi`. В шапке и у каждой ветки `IF` / `FROM` — комментарий «номер таблицы → справочник» (как у TransportRate / AverageRateLevel3). Для стабильного словаря с enum процедуру не создавать.

**RowVer:** расширить `SqlScripts/Sync/01.Legacy_AddRowVersion.sql` для основной `PrimitiveEntityData_*` (+ NC-индекс по RowVer, если таблица крупная). Maintenance-окно (Sch-M). Для стабильного словаря с enum RowVer не добавлять.

**Grid read** — без `{Entity}_Get`. Метаданные колонок на `{Entity}Dto`: `[GridSnapshot("v2.{Entity}_Snapshot")]` + `[GridColumn]` на свойствах.  
`GridDataService` вызывает `v2.GetBlazorGridData` с `@TableName`, `@AllowedColumnsJson`, `@SelectList`, сформированными в C# (`GridColumnMetadataBuilder`).

Правила `[GridColumn]`:

| Ситуация | Атрибут |
|----------|---------|
| 1:1 со snapshot (`string`, `bool`, `long`, `DateTime`, …) | `[GridColumn]` — тип выводится из CLR-типа свойства |
| Enum Ru/En → одна snapshot-колонка | `[GridColumn(SqlColumn = "{X}Id")]` на `{X}IdRu` и `{X}IdEn` |
| `DateOnly` в DTO, `DATETIME` в snapshot | `[GridColumn]` — auto `CAST` в SELECT |
| Бизнес-ключ | `IsEntityKey = true` на `{Entity}Id` |
| Только отображение / сортировка | `Filterable = false` |
| Невыводимый CLR-тип + фильтрация | явный `ColumnType = GridColumnType.…` |

`GridColumnType` для whitelist:

| SQL-тип | GridColumnType |
|---------|----------------|
| `BIT` | `Bit` |
| FK / `INT` | `Id` |
| `NVARCHAR` | `Nvarchar` |
| `DATETIME` | `Date` |
| `DECIMAL` | `Decimal` |

**Full export** — platform-процедура `v2.ExportBlazorGridDetail`: grid-метаданные из list-DTO, `@DetailViewName` и `@EntityKeyColumn` из `[DetailSource]` на detail-DTO. Per-module export-процедура **не нужна**. Вызов грида внутри неё идёт с `@ReturnTotalCount = 0`: второй результат `v2.GetBlazorGridData` — число строк, и `INSERT…EXEC` иначе записывает это число как ключ строки.

Обновить `BlazorSvt/SqlScripts/README.md` (секция нового модуля).

## Фаза 3 — Деплой и верификация БД (до C#)

### Порядок деплоя

1. **RowVer** (если новая основная таблица) — `Sync/01.Legacy_AddRowVersion.sql`
2. **01 CreateTable** → **SnapshotSource** → **02 Insert** → **03 Indexes** → **Detail** → **PopulateAffectedKeys**  
   (Insert зависит от SnapshotSource; Indexes — после залитых данных)
3. Либо Structure 01–03 вручную, затем полный прогон Programmability. Скрипт ниже берёт SQL-логин из `Database:MdmDb` и без `User ID` не запускается. Structure он не накатывает. При `Trusted_Connection` его не вызывать: каждый файл — `sqlcmd -E`, как в примере ниже.

```powershell
& ".\BlazorSvt\SqlScripts\Create-Programmability.ps1"
```

Из корня `c:\Work\BlazorSVT`.

### Structure: когда прогонять

| Ситуация | Действие |
|----------|----------|
| **Новый** справочник | Всегда `01`, `02`, `03` |
| **Существующий** | Спросить пользователя: нужен ли sqlcmd и **какие именно** файлы |

Сервер `localhost`, база `mdm`. Поле Server из `Database:MdmDb` не использовать. `appsettings.json` не менять.

В строке есть `User ID`:

```powershell
sqlcmd -S localhost -d mdm -U <User ID> -P <Password> -b -f 65001 -i "BlazorSvt\SqlScripts\Modules\{Entity}\Structure\01.{Entity}_CreateTable.sql"
```

`Trusted_Connection`, логина нет:

```powershell
sqlcmd -S localhost -d mdm -E -b -f 65001 -i "BlazorSvt\SqlScripts\Modules\{Entity}\Structure\01.{Entity}_CreateTable.sql"
```

Повторить для остальных файлов порядка деплоя. Пакет с JSON класть во временный `.sql` и вызывать через `-i`: `-Q` ломает кавычки, `-W` и `-y` вместе нельзя. Временный файл не класть в `SqlScripts` и после проверки удалить. `v2.GetBlazorGridData` печатает динамический SQL (`PRINT`); это не результат запроса.

### Верификация SQL (обязательна)

1. `EXEC v2.GetBlazorGridData`: `@TableName` = `v2.{Entity}_Snapshot`, `@AllowedColumnsJson` и `@SelectList` собрать по короткому списку discovery §1 и системным полям снимка (простой и сложный FTS, если применимо). Атрибутов DTO ещё нет — из них параметры не брать. `@ReturnTotalCount` не передавать: по умолчанию 1, второй результат — число строк.
2. `EXEC v2.ExportBlazorGridDetail`: `@DetailViewName` = `v2.vw_{Entity}_Detail`, `@EntityKeyColumn` = `{Entity}Id` (имена из [SKILL.md](SKILL.md), не из detail-DTO), `@PageSize = 1`. Ровно одна строка карточки. Вторая строка, у которой ключ равен общему числу строк снимка, значит, что в ключи попал `TotalCount`: выгрузка должна звать грид с `@ReturnTotalCount = 0`.
3. `SELECT TOP 1 * FROM v2.{Entity}_Snapshot`
4. `SELECT TOP 1 * FROM v2.vw_{Entity}_Detail`

**При ошибке деплоя или верификации — остановиться, C# не писать, спросить пользователя.**

Дальше: [csharp.md](csharp.md).
