# SQL Migrations (релизные артефакты)

История **накатов на стенды**. SoT разработки по-прежнему в `Platform/`, `Modules/`, `Sync/`.
Эта папка — то, что реально уходило / будет уходить на БД в составе релиза приложения.

## Структура

```
Migrations/
├── README.md          ← этот файл
├── 2.0.0/             ← baseline (immutable после выката)
│   └── 001.…037.…
├── 2.0.1/             ← upgrade с 2.0.0 (immutable после выката)
│   └── 038.…054.…
├── 2.3.46/            ← RateType (не править)
│   └── 001.…005.…
├── 2.3.48/            ← очередь Атласа (не править)
│   └── 001.…020.…
├── 2.3.49/            ← Contractor … TransportKind, SVT-5317 (не править)
│   └── 001.…058.…
└── 2.3.50/            ← текущий VersionPrefix
    └── 001.…004.…
```

Пакет одной версии (приложение, склейка этой папки, zip) собирает skill `svt-publish`:

```powershell
.\BlazorSvt\SqlScripts\svt-publish.ps1 -Mode all
```

Папка пакета — `C:\publish\{VersionPrefix}`, zip — рядом. Цепочку всех папок `Migrations/` этот скрипт не собирает. На уже развёрнутую базу по-прежнему кладут ещё не применённые папки по порядку версий; greenfield — та же цепочка ниже.

## Два контура

| Контур | Содержимое | Свойство | Где живёт |
|--------|------------|----------|-----------|
| **Schema / Migrations** | CREATE TABLE, ALTER, индексы, one-shot data fix | один раз на релиз | `Migrations/{VersionPrefix}/` |
| **Programmability** | views, SP, fn (`CREATE OR ALTER`) | идемпотентно, всегда latest | SoT для локального наката; в папке версии — последний файл `All_Programmability.sql` |

Накат на **уже развёрнутую** БД: скрипты ещё не применённой папки версии по порядку номеров (снимок programmability — последний).
Greenfield: `2.0.0` → `2.0.1` → `2.3.46` → `2.3.48` → `2.3.49` → `2.3.50`.

Рекомендуется (пока вручную) вести учёт: какая версия уже на стенде (хотя бы запись в wiki / будущая `v2.SchemaVersion`).

## Правила текущего релиза

Имя папки — `VersionPrefix` из `BlazorSvt/BlazorSvt.csproj` (сейчас `2.3.50`), без суффикса числа коммитов. Это не «следующий номер после 2.0.1» и не пункт бэклога `2.0.2`.

1. Перед записью скриптов выяснить, выкачена ли папка текущего `VersionPrefix`. Выкаченная папка read-only: `VersionPrefix` поднимается, новая папка начинается с `001`, старая не правится. Папка, которая ещё не выкатывалась, продолжается следующим номером. Баг уже выкаченного релиза уходит новым скриптом в папку новой версии.
2. В папке версии — дельта схемы (Structure, `ALTER`, индексы, data fix), затем **последним файлом полный снимок** всей programmability `v2` (`All_Programmability.sql`). В текущем релизе последний снимок — `2.3.50/004`. В `2.3.49` это `058` (2026-11-24). В `2.3.48` это `020` (2026-11-03). В `2.3.46` это `005`. В `2.0.1` такой пакет стоял дважды: `045` и `052`.
3. Новая папка версии нумеруется с `001`. Невыкаченная папка продолжается следующим номером, а не с `001`. Исторические `038…` в `2.0.1` не переименовываем.
4. SoT остаётся в `Platform/` и `Modules/`: локальный `Create-Programmability.ps1` читает его, а не `Migrations/`. В папку версии попадает копия дельты и снимок programmability.
5. Запись скриптов в папку версии пакет не собирает. Пакет — `svt-publish.ps1` (skill `svt-publish`).

## Снимок All_Programmability

Последний файл папки версии. Собирается из SoT в порядке `Create-Programmability.ps1`, на БД не исполняется. Кодировка — UTF-8 with BOM. Снимок уже выкаченной папки не пересобирать.

Порядок файлов: сначала `Platform/Programmability/*.sql`, затем `Modules/*/Programmability/*.sql` по имени папки модуля. Внутри папки ключ сортировки, затем имя файла: `fn_*` и `vw_*` — `010`, `sp_*` и `*_Get.sql` — `020`, `*_ExportFull.sql` — `030`, прочие — `100`.

Шапка и разделитель каждого скрипта:

```
/*
  Auto-generated: All v2 Programmability
  Generated: yyyy-MM-dd HH:mm:ss
  Scripts: N
  Order: Create-Programmability.ps1 (Platform -> Modules by name)
*/
GO

--------------------------------------------------------------------------------
-- [1/N] Platform :: fn_GetDateSqlOperator.sql
-- Source: Platform\Programmability\fn_GetDateSqlOperator.sql
--------------------------------------------------------------------------------
GO
```

`Source` — путь от `SqlScripts` с обратными слэшами. В заголовке блока имя области — `Platform` или имя папки модуля, без префикса `Modules\`. Тело файла — как в SoT, без BOM и без хвостовых пустых строк. Между скриптами одна пустая строка.

## Архив 2.0.0 / 2.0.1 (факт выкладки)

Скопировано из `C:\publish\v2` **как есть**, без правок содержимого.

| Релиз | Файлы | Что это |
|-------|-------|---------|
| **2.0.0** | `001`–`037` | Baseline: schema + programmability Platform + TransportRate, AverageRateLevel3, TransportLeg, LocationsNodes |
| **2.0.1** | `038`–`054` | ParityRates, повторный Legacy_AddRowVersion, повтор TransportLeg, два `All_Programmability`, data-fix |

Особенности `2.0.1` (не идеал, но история):

- `038` — повтор `Legacy_AddRowVersion` (идемпотентный/дополняющий)
- `039`–`044` — ParityRates
- `045`, `052` — пакеты `All_Programmability` (накат SP/view пачкой)
- `046`–`051` — повторный набор TransportLeg (эволюция snapshot после baseline)
- `053`–`054` — точечные data-fix

Полный снимок programmability в конце папки версии — штатный артефакт, не повторное переиздание одного модуля.

## Релиз 2.3.46

Папка RateType. `VersionPrefix` сменился на `2.3.48`: файлы `2.3.46` не править.

| Файл | Что это |
|------|---------|
| `001.01.RateType_CreateTable.sql` | Первое появление `v2.RateType_Snapshot`: partition function/scheme, sequence, таблица |
| `002.vw_RateType_SnapshotSource.sql` | Проекция из `dbo.vw_RateType`; выполняется до заливки |
| `003.02.RateType_Insert.sql` | Разовая заливка из этой проекции. `SyncState` не создаётся |
| `004.03.RateType_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы, full-text по `Name` и `Comment` |
| `005.All_Programmability.sql` | Полный снимок programmability, 26 скриптов (2026-09-25) |

RateType — стабильный словарь с enum. В миграции нет `sp_RateType_PopulateAffectedKeys`, `RowVer` и `SyncState`. В снимке `005` у RateType только `vw_RateType_Detail` и `vw_RateType_SnapshotSource`.

## Релиз 2.3.48

Очередь Атласа. `VersionPrefix` сменился на `2.3.49`: файлы `2.3.48` не править.

| Файл | Что это |
|------|---------|
| `001.01.Currency_CreateTable.sql` | Первое появление `v2.Currency_Snapshot`: partition function/scheme, sequence, таблица |
| `002.vw_Currency_SnapshotSource.sql` | Проекция из `dbo.vw_Currency`; выполняется до заливки |
| `003.02.Currency_Insert.sql` | Разовая заливка из этой проекции. `SyncState` не создаётся |
| `004.03.Currency_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы, full-text по `Name`, `Symbolic`, `Code`, `Code_NSI` |
| `005.01.ProductGroup_CreateTable.sql` | Первое появление `v2.ProductGroup_Snapshot`: partition function/scheme, sequence, таблица |
| `006.vw_ProductGroup_SnapshotSource.sql` | Проекция из `dbo.vw_ProductGroup`; выполняется до заливки |
| `007.02.ProductGroup_Insert.sql` | Полная заливка из этой проекции и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_1013` |
| `008.03.ProductGroup_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы по `Code` и `RateSyncClassId`, индекс по ключу, full-text по текстовым полям |
| `010.01.MTR_CreateTable.sql` | Первое появление `v2.MTR_Snapshot`: partition function/scheme, sequence, таблица |
| `011.vw_MTR_SnapshotSource.sql` | Проекция из `dbo.vw_MTR`; выполняется до заливки |
| `012.02.MTR_Insert.sql` | Полная заливка из этой проекции и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_1015` и `dbo.PrimitiveEntityData_1013` |
| `013.03.MTR_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы по `Code`, `UnitId` и `DangerClassId`, индексы по ключу и `ProductGroupId`, full-text |
| `016.01.TransportTypeLevel3_CreateTable.sql` | Первое появление `v2.TransportTypeLevel3_Snapshot`: partition function/scheme, sequence, таблица |
| `017.vw_TransportTypeLevel3_SnapshotSource.sql` | Проекция из `dbo.vw_TransportType_level_3`; выполняется до заливки |
| `018.02.TransportTypeLevel3_Insert.sql` | Разовая заливка из этой проекции. `SyncState` не создаётся |
| `019.03.TransportTypeLevel3_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы по `Code`, виду и типу транспорта, ключу; full-text по текстовым полям |
| `020.All_Programmability.sql` | Полный снимок programmability, 36 скриптов (2026-11-03) |

Файлы `009`, `014` и `015` — прежние имена снимка programmability: после каждой новой дельты его переименовывали в следующий номер.

Currency — стабильный словарь с enum. В миграции нет `sp_Currency_PopulateAffectedKeys`, `RowVer` и `SyncState`. В снимке `020` у Currency только `vw_Currency_Detail` и `vw_Currency_SnapshotSource`.

ProductGroup — справочник с watermark-sync. `RowVer` на `dbo.PrimitiveEntityData_1013` уже добавлен в `2.0.0/004` (источник каскада ставок), отдельного скрипта нет. `vw_ProductGroup_Detail` и `sp_ProductGroup_PopulateAffectedKeys` — в снимке `020`.

MTR — справочник с watermark-sync. `RowVer` на `dbo.PrimitiveEntityData_1015` уже был (источник каскада ставок). В каскад своего job входят основная таблица и ProductGroup; `012` инициализирует курсоры для 1015 и 1013. ETSNG — легаси-справочник, не меняется и не отслеживается. `vw_MTR_Detail` и `sp_MTR_PopulateAffectedKeys` — в снимке `020`.

TransportTypeLevel3 — стабильный словарь с enum. В миграции нет `sp_TransportTypeLevel3_PopulateAffectedKeys`, `RowVer` и `SyncState`. В снимке `020` у TransportTypeLevel3 только `vw_TransportTypeLevel3_Detail` и `vw_TransportTypeLevel3_SnapshotSource`.

## Релиз 2.3.49

Папка выкачена. `VersionPrefix` сменился на `2.3.50`: файлы `2.3.49` не править.

| Файл | Что это |
|------|---------|
| `001.Contractor_AddRowVersion.sql` | RowVer и NC-индекс на `dbo.PrimitiveEntityData_2028` (LegalType, реестр Contractor) |
| `002.01.Contractor_CreateTable.sql` | Первое появление `v2.Contractor_Snapshot` |
| `003.vw_Contractor_SnapshotSource.sql` | Проекция из `dbo.vw_LegalType` |
| `004.02.Contractor_Insert.sql` | Полная заливка и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_2028` |
| `005.03.Contractor_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code`, индекс по ключу, full-text |
| `006.vw_Contractor_Detail.sql` | Карточка Contractor |
| `007.sp_Contractor_PopulateAffectedKeys.sql` | Детекция изменений LegalType |
| `009.Coefficient_AddRowVersion.sql` | RowVer на `dbo.PrimitiveEntityData_2081` (Coefficient), без индекса: таблица мала |
| `010.01.Coefficient_CreateTable.sql` | Первое появление `v2.Coefficient_Snapshot` |
| `011.vw_Coefficient_SnapshotSource.sql` | Проекция из `dbo.vw_Coefficient`; выполняется до заливки |
| `012.02.Coefficient_Insert.sql` | Полная заливка и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_2081` |
| `013.03.Coefficient_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code`, датам, типу коэффициента и типу ставки, индекс по ключу, full-text |
| `015.01.RateSyncClass_CreateTable.sql` | Первое появление `v2.RateSyncClass_Snapshot` |
| `016.vw_RateSyncClass_SnapshotSource.sql` | Проекция из `dbo.vw_RateSyncClass`; выполняется до заливки |
| `017.02.RateSyncClass_Insert.sql` | Разовая заливка, без курсора `v2.SyncState` |
| `018.03.RateSyncClass_CreateIndexes.sql` | Уникальный индекс, фильтр по `Code`, индекс по ключу, full-text по `Name` |
| `019.01.RateCalcType_CreateTable.sql` | Первое появление `v2.RateCalcType_Snapshot` |
| `020.vw_RateCalcType_SnapshotSource.sql` | Проекция из `dbo.vw_RateCalcType`; выполняется до заливки |
| `021.02.RateCalcType_Insert.sql` | Разовая заливка, без курсора `v2.SyncState` |
| `022.03.RateCalcType_CreateIndexes.sql` | Уникальный индекс, фильтр по `Code`, индекс по ключу, full-text по `Code`, `Name`, `SearchCode` |
| `023.RateAutoSelectionSetting_AddRowVersion.sql` | RowVer на `dbo.PrimitiveEntityData_2101` (RateAutoSelectionSetting), без индекса: таблица мала |
| `024.01.RateAutoSelectionSetting_CreateTable.sql` | Первое появление `v2.RateAutoSelectionSetting_Snapshot` |
| `025.vw_RateAutoSelectionSetting_SnapshotSource.sql` | Проекция из `dbo.vw_RateAutoSelectionSetting`; выполняется до заливки |
| `026.02.RateAutoSelectionSetting_Insert.sql` | Полная заливка и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_2101` |
| `027.03.RateAutoSelectionSetting_CreateIndexes.sql` | Уникальный индекс, фильтр по `Code`, full-text по `Code` |
| `028.ReportConstructor_AddRowVersion.sql` | RowVer на `dbo.PrimitiveEntityData_2130` (ReportConstructor), без индекса: таблица мала |
| `029.01.ReportConstructor_CreateTable.sql` | Первое появление `v2.ReportConstructor_Snapshot` |
| `030.vw_ReportConstructor_SnapshotSource.sql` | Проекция из `dbo.vw_ReportConstructor`; выполняется до заливки |
| `031.02.ReportConstructor_Insert.sql` | Полная заливка и курсор `v2.SyncState` по 2130, 1014, 1013, 1015 |
| `032.03.ReportConstructor_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code` и коротким enum, индексы по ключу и скрытым FK, full-text |
| `034.01.Relevance_CreateTable.sql` | Первое появление `v2.Relevance_Snapshot` |
| `035.vw_Relevance_SnapshotSource.sql` | Проекция из `dbo.vw_Relevance`; выполняется до заливки |
| `036.02.Relevance_Insert.sql` | Разовая заливка, без курсора `v2.SyncState` |
| `037.03.Relevance_CreateIndexes.sql` | Уникальный индекс, фильтр по `Code`, индекс по ключу, full-text по `Name` |
| `038.TransportRoute_AddRowVersion.sql` | RowVer и NC-индекс на `dbo.PrimitiveEntityData_2010` (TransportRoute) |
| `039.01.TransportRoute_CreateTable.sql` | Первое появление `v2.TransportRoute_Snapshot` |
| `040.vw_TransportRoute_SnapshotSource.sql` | Проекция из `dbo.vw_TransportRoute`; выполняется до заливки |
| `041.02.TransportRoute_Insert.sql` | Полная заливка и курсор `v2.SyncState` по 2010, 1014, 1008 |
| `042.03.TransportRoute_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code` и коротким enum, индексы по ключу и скрытым FK узлов, full-text |
| `043.01.Region_CreateTable.sql` | Первое появление `v2.Region_Snapshot` |
| `044.vw_Region_SnapshotSource.sql` | Проекция из `dbo.vw_Region`; выполняется до заливки |
| `045.02.Region_Insert.sql` | Полная заливка и курсор `v2.SyncState` по 1008, 1009 |
| `046.03.Region_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code` и коротким enum, индексы по ключу и `CountryId`, full-text |
| `047.01.Country_CreateTable.sql` | Первое появление `v2.Country_Snapshot` |
| `048.vw_Country_SnapshotSource.sql` | Проекция из `dbo.vw_Country`; выполняется до заливки |
| `049.02.Country_Insert.sql` | Полная заливка и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_1009` |
| `050.03.Country_CreateIndexes.sql` | Уникальный индекс, фильтр по `Code` и `StatusId`, индекс по ключу, full-text |
| `052.01.TransportKind_CreateTable.sql` | Первое появление `v2.TransportKind_Snapshot` |
| `053.vw_TransportKind_SnapshotSource.sql` | Проекция из `dbo.vw_TransportKind`; выполняется до заливки |
| `054.02.TransportKind_Insert.sql` | Разовая заливка, без курсора `v2.SyncState` |
| `055.03.TransportKind_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code` и `StatusId`, индекс по ключу, full-text по `Code`, `Name`, `NameEnRu`, `Level`, `KindCode` |
| `057.SVT-5317_NumericCodes.sql` | SVT-5317: `Code` → `INT` у RateSyncClass, ReportConstructor, Coefficient, RateAutoSelectionSetting, TransportRoute, RateType, Country, MTR, Contractor; `Country.ISOCode`, `Currency.Digital`, `Currency.Code_NSI` → `INT`. Идемпотентно, в конце проверка типов с `THROW` |
| `058.All_Programmability.sql` | Полный снимок programmability, 75 скриптов (2026-11-24): `INT`-коды в проекциях, поиск по началу значения (`LIKE`) в `GetBlazorGridData` для колонок без full-text |

Contractor (SVT-5290) — справочник с watermark-sync. Примитив LegalType, чтение из `dbo.vw_LegalType`. `001` добавляет `RowVer` на `dbo.PrimitiveEntityData_2028`.

Coefficient — справочник с watermark-sync. `009` добавляет `RowVer` на `dbo.PrimitiveEntityData_2081`. CoefficientType (2077) и RateType (2048) — enum, в каскад не входят. `vw_Coefficient_Detail` и `sp_Coefficient_PopulateAffectedKeys` — в снимке `058`.

RateSyncClass — класс simple, разовый снимок. `RowVer` на `dbo.PrimitiveEntityData_2127` не добавлялся. `vw_RateSyncClass_Detail` — в снимке `058`. Процедуры PopulateAffectedKeys нет.

RateCalcType — класс simple, разовый снимок. `RowVer` на `dbo.PrimitiveEntityData_2092` не добавлялся. `vw_RateCalcType_Detail` — в снимке `058`. Процедуры PopulateAffectedKeys нет.

RateAutoSelectionSetting — справочник с watermark-sync. `023` добавляет новый `RowVer` на `dbo.PrimitiveEntityData_2101`. Других источников нет. `vw_RateAutoSelectionSetting_Detail` и `sp_RateAutoSelectionSetting_PopulateAffectedKeys` — в снимке `058`.

ReportConstructor — справочник с watermark-sync. `028` добавляет новый `RowVer` на `dbo.PrimitiveEntityData_2130`. `RowVer` на LocationsNodes (1014), ProductGroup (1013) и MTR (1015) уже был. `vw_ReportConstructor_Detail` и `sp_ReportConstructor_PopulateAffectedKeys` — в снимке `058`.

Relevance — класс simple, разовый снимок. `RowVer` на `dbo.PrimitiveEntityData_2108` не добавлялся. `vw_Relevance_Detail` — в снимке `058`. Процедуры PopulateAffectedKeys нет.

TransportRoute — справочник с watermark-sync. `038` добавляет новый `RowVer` и NC-индекс на `dbo.PrimitiveEntityData_2010`. `RowVer` на LocationsNodes (1014) и Region (1008) уже был. `vw_TransportRoute_Detail` и `sp_TransportRoute_PopulateAffectedKeys` — в снимке `058`. `v2.vw_TransportRouteLeg_Detail` (дочерний примитив, `dbo.PrimitiveEntityData_2035`) добавлен в тот же снимок `058`, отдельной дельты нет.

Region — справочник с watermark-sync. Новый `RowVer` не добавлялся: на `dbo.PrimitiveEntityData_1008` и `dbo.PrimitiveEntityData_1009` он уже есть. `vw_Region_Detail` и `sp_Region_PopulateAffectedKeys` — в снимке `058`.

Country — справочник с watermark-sync. Новый `RowVer` не добавлялся: на `dbo.PrimitiveEntityData_1009` он уже есть. `vw_Country_Detail` и `sp_Country_PopulateAffectedKeys` — в снимке `058`.

TransportRate — доработка. Проекции детей `v2.vw_TransportRateTenderServicePack_Detail` (`dbo.PrimitiveEntityData_2089`) и `v2.vw_TransportRateCoefficient_Detail` (`dbo.PrimitiveEntityData_2106`) добавлены в снимок `058`, отдельной дельты нет. Structure, `vw_TransportRate_SnapshotSource`, `vw_TransportRate_Detail`, `PopulateAffectedKeys`, `RowVer` и SyncState не менялись.

AverageRateLevel3 — доработка. Проекция ребёнка `v2.vw_RatesOfAverageRate_Detail` (`dbo.PrimitiveEntityData_2058`) добавлена в снимок `058`, отдельной дельты нет. Structure, `vw_AverageRateLevel3_SnapshotSource`, `vw_AverageRateLevel3_Detail`, `PopulateAffectedKeys`, `RowVer` и SyncState не менялись.

Contractor — доработка. Проекция ребёнка `v2.vw_Address_Detail` (`dbo.PrimitiveEntityData_2027`, ключ родителя `LegalTypeId`) добавлена в снимок `058`, отдельной дельты нет. Structure, `vw_Contractor_SnapshotSource`, `vw_Contractor_Detail`, `PopulateAffectedKeys`, `RowVer` и SyncState не менялись.

TransportLeg — доработка. Проекции детей `v2.vw_LegRate_Detail` (`dbo.PrimitiveEntityData_2044`), `v2.vw_LegAverageRateLevel3_Detail` (`dbo.PrimitiveEntityData_2059`) и `v2.vw_Leadtime_Detail` (`dbo.PrimitiveEntityData_2126`) добавлены в снимок `058`, отдельной дельты нет. Structure, `vw_TransportLeg_SnapshotSource`, `vw_TransportLeg_Detail`, `PopulateAffectedKeys`, `RowVer` и SyncState не менялись.

TransportKind — класс simple, разовый снимок. `RowVer` на `dbo.PrimitiveEntityData_2008` не добавлялся. `vw_TransportKind_Detail` — в снимке `058`. Процедуры PopulateAffectedKeys нет.

Файл `008` — прежнее имя снимка programmability: после дельты Coefficient его переименовали в `014`. Файл `014` — прежнее имя снимка: после дельты RateSyncClass, RateCalcType, RateAutoSelectionSetting и ReportConstructor его переименовали в `033`. Файл `033` — прежнее имя снимка: после дельты Relevance, TransportRoute, Region и Country его переименовали в `051`. Файл `051` — прежнее имя снимка: после дельты TransportKind его переименовали в `056`. Файл `056` — прежнее имя снимка: после дельты `057` (SVT-5317) его переименовали в `058`. Актуальный полный снимок — `058`.

## Текущий релиз 2.3.50

| Файл | Что это |
|------|---------|
| `001.vw_UserDisplayName.sql` | SVT-5320: `v2.vw_UserDisplayName` — ФИО из `dbo.[User]` (Name, при пустом — Login). Нужна до `002` и `003` |
| `002.SVT-5320_SnapshotSource.sql` | Копия 21 проекции `vw_*_SnapshotSource` с `CreationUserName` / `LastChangeUserName`. Идёт до `ALTER TABLE`: лишние колонки проекции `Sync_UpsertAffected` не мешают, окна сбоя sync нет. Нужна и для `004`: `vw_*_Detail` ссылается на новые колонки проекции и в снимке стоит раньше неё |
| `003.SVT-5320_Snapshot_AddUserColumns.sql` | `ALTER TABLE ADD CreationUserName / LastChangeUserName NVARCHAR(255) NULL` в 21 snapshot (идемпотентно, отсутствующая таблица пропускается) и backfill из легаси-вью пачками по 50 тыс. внутри партиции `IsArchive`. В конце проверка колонок с `THROW`. Локально около 40 с на 2,9 млн строк |
| `004.All_Programmability.sql` | Полный снимок programmability, 76 скриптов (2026-11-26): `vw_UserDisplayName`, авторы в `vw_*_SnapshotSource` и `vw_*_Detail`; `GetBlazorGridData` — парсинг дат фильтров без зависимости от языка сессии, голая дата в DATETIME-фильтре = весь день; втянут ранее пропущенный `IN`-мультивыбор по ID (`InValues`) |

SVT-5320 — колонки «Автор» и «Автор изм.» в гридах, карточках и Excel-отчётах 21 справочника. `PopulateAffectedKeys`, `RowVer`, SyncState и индексы не менялись. Переименование пользователя в `dbo.[User]` в snapshot не каскадируется — см. `SqlScripts/README.md`, раздел синхронизации.

## Чеклист нового релиза

1. Создать `Migrations/{VersionPrefix}/`. Нумерация с `001`: дельта схемы, последним — `All_Programmability.sql`.
2. Обновить этот README, если сменился `VersionPrefix`.
3. После выката и смены `VersionPrefix` файлы старой папки не трогать.
