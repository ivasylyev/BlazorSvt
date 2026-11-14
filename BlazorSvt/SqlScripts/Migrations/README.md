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
└── 2.3.49/            ← текущий VersionPrefix
    └── 001.…008.…
```

Публикация наружу (для DBA):

```powershell
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode All
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode Release -Version 2.0.1
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode Programmability
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode FromSource   # legacy: плоский снимок из SoT
```

По умолчанию целевая папка: `C:\publish\v2` (можно `-TargetPath`).

## Два контура

| Контур | Содержимое | Свойство | Где живёт |
|--------|------------|----------|-----------|
| **Schema / Migrations** | CREATE TABLE, ALTER, индексы, one-shot data fix | один раз на релиз | `Migrations/{VersionPrefix}/` |
| **Programmability** | views, SP, fn (`CREATE OR ALTER`) | идемпотентно, всегда latest | SoT для локального наката; в папке версии — последний файл `All_Programmability.sql` |

Накат на **уже развёрнутую** БД: скрипты ещё не применённой папки версии по порядку номеров (снимок programmability — последний).
Greenfield: `2.0.0` → `2.0.1` → `2.3.46` → `2.3.48` → `2.3.49`.

Рекомендуется (пока вручную) вести учёт: какая версия уже на стенде (хотя бы запись в wiki / будущая `v2.SchemaVersion`).

## Правила текущего релиза

Имя папки — `VersionPrefix` из `BlazorSvt/BlazorSvt.csproj` (сейчас `2.3.49`), без суффикса числа коммитов. Это не «следующий номер после 2.0.1» и не пункт бэклога `2.0.2`.

1. **Папка уже выкаченного релиза — read-only.** Пока `VersionPrefix` не сменился, скрипты дописываются в текущую папку. После смены версии старую папку не правят: баг уходит новым скриптом в папку новой версии.
2. В папке версии — дельта схемы (Structure, `ALTER`, индексы, data fix), затем **последним файлом полный снимок** всей programmability `v2` (`All_Programmability.sql`). В текущем релизе последний снимок — `2.3.49/008`. В `2.3.48` это `020` (2026-11-03). В `2.3.46` это `005`. В `2.0.1` такой пакет стоял дважды: `045` и `052`.
3. Нумерация **внутри релиза** с `001`. Исторические `038…` в `2.0.1` не переименовываем.
4. SoT остаётся в `Platform/` и `Modules/`: локальный `Create-Programmability.ps1` читает его, а не `Migrations/`. В папку версии попадает копия дельты и снимок programmability.
5. Публикация в `C:\publish` этим правилом не требуется.

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

## Текущий релиз 2.3.49

| Файл | Что это |
|------|---------|
| `001.Contractor_AddRowVersion.sql` | RowVer и NC-индекс на `dbo.PrimitiveEntityData_2028` (LegalType, реестр Contractor) |
| `002.01.Contractor_CreateTable.sql` | Первое появление `v2.Contractor_Snapshot` |
| `003.vw_Contractor_SnapshotSource.sql` | Проекция из `dbo.vw_LegalType` |
| `004.02.Contractor_Insert.sql` | Полная заливка и курсор `v2.SyncState` по `dbo.PrimitiveEntityData_2028` |
| `005.03.Contractor_CreateIndexes.sql` | Уникальный индекс, фильтры по `Code`, индекс по ключу, full-text |
| `006.vw_Contractor_Detail.sql` | Карточка Contractor |
| `007.sp_Contractor_PopulateAffectedKeys.sql` | Детекция изменений LegalType |
| `008.All_Programmability.sql` | Полный снимок programmability, 39 скриптов, включая Contractor |

Contractor (SVT-5290) — справочник с watermark-sync. Примитив LegalType, чтение из `dbo.vw_LegalType`. `001` добавляет `RowVer` на `dbo.PrimitiveEntityData_2028`. Актуальный полный снимок — `008`.

## Чеклист нового релиза

1. Создать `Migrations/{VersionPrefix}/`. Нумерация с `001`: дельта схемы, последним — `All_Programmability.sql`.
2. Обновить этот README, если сменился `VersionPrefix`.
3. После выката и смены `VersionPrefix` файлы старой папки не трогать.
