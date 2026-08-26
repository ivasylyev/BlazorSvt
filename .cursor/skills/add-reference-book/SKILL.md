---
name: add-reference-book
description: >-
  Добавление read-only справочника BlazorSVT из легаси MDM: discovery атрибутов,
  snapshot SQL, programmability, C#-модуль, grid и меню. Используй когда пользователь
  просит добавить справочник, создать новый модуль справочника или перенести
  PrimitiveEntityInfo из старой системы.
---

# Добавление справочника (read-only)

Полный вертикальный срез: MDM discovery → SQL (Structure + Programmability + SnapshotSync) → деплой и верификация БД → C#-модуль.

**Эталоны реализации:** `Modules/TransportRate`, `Modules/AverageRateLevel3`, `Modules/TransportLeg`, `Modules/LocationsNodes`.  
Legacy `Legs`/`Rates` — не эталон и **не трогать** без отдельной команды (`LegId`, `RateId`).  
**Именование:** для **новых** справочников — соглашения ниже.

## Когда применять

- Команда «добавить справочник», «создать модуль для {PrimitiveEntityInfo}»
- Перенос сущности из легаси MDM в BlazorSVT (read-only grid + detail + export)

## Шаг 0 — имя справочника

1. Если пользователь не указал имя — **уточни** системное имя (`PrimitiveEntityInfo.Name`).
2. Без имени дальнейшие шаги **не выполнять**.
3. Имя всегда **системное** (например `TransportLeg`, не «Транспортные плечи»).

Обозначение в навыке: `{Entity}` = `PrimitiveEntityInfo.Name`.

---

## Соглашения об именовании (новые справочники)

| Объект | Шаблон | Пример |
|--------|--------|--------|
| Папка C# и SQL | `Modules/{Entity}/` | `Modules/TransportLeg/` |
| Snapshot-таблица | `v2.{Entity}_Snapshot` | `v2.TransportLeg_Snapshot` |
| Detail view | `v2.vw_{Entity}_Detail` | `v2.vw_TransportLeg_Detail` |
| Get / Export | `v2.GetBlazorGridData`, `v2.ExportBlazorGridDetail` (Platform) | — |
| SEQUENCE | `v2.seq_{Entity}Id` | `v2.seq_TransportLegId` |
| Partition function | `v2_pf_{Entity}_IsArchive` | |
| Partition scheme | `v2_ps_{Entity}` | |
| DTO | `{Entity}Dto`, `{Entity}DetailDto` | `TransportLegDto` |
| Бизнес-ключ | `{Entity}Id` | `TransportLegId` |
| Маршрут / URL меню | `/{entity}` lowercase | `/transportleg` |
| Grid | `{Entity}Grid.razor` | `@page "/transportleg"` |
| Module | `{Entity}Module.cs` → `Add{Entity}Module()` | |

Все объекты — в схеме **v2**.

### Системные поля snapshot (всегда)

Добавлять **вне зависимости** от короткого списка атрибутов:

- `Id` (SEQUENCE + DEFAULT)
- `{Entity}Id` (бизнес-ключ, `CAST(Id AS …)` из `vw_{Entity}`)
- `Code`
- `IsArchive` (`CASE WHEN ISNULL(PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END`)
- `CreationDate`, `LastChangeDate`

**Бизнес-флаги** (например `CanBeUsed`) — только если есть в коротком списке (п. Discovery §1).

### Партиционирование (по умолчанию)

Как `Legs`/`Rates`: partition function/scheme по `IsArchive`, PK `(IsArchive, Id)`, SEQUENCE для `Id`.

---

## Фаза 1 — Discovery (MDM + legacy)

Connection string: `Database:MdmDb` из `BlazorSvt/appsettings.json`.  
Legacy DDL/views: `C:\Work\SVT\DB\SVT.DB.MDM\dbo\Views` и `dbo\Tables`.  
**Нет доступа** — спросить пользователя путь к репозиторию.

### 1. Короткий список атрибутов (snapshot / грид)

```sql
;WITH SnapshotCTE AS (
    SELECT
        ai.[Name]  AS AttributeSystemName,
        ai.[Description] AS AttributeNameRu,
        s.MainRank AS AttributeRank
    FROM AttributeInfo ai
    JOIN AttributeSettings s ON ai.Id = s.AttributeInfoId
    WHERE ai.PrimitiveEntityInfoId = (
        SELECT TOP 1 Id FROM PrimitiveEntityInfo WHERE [name] = '{Entity}'
    )
    AND s.IsMain = 1
)
SELECT * FROM SnapshotCTE
ORDER BY AttributeRank;
```

**Порядок колонок грида** — как в этом списке (после фильтрации п.3).

### 2. Типы атрибутов

Сопоставить короткий список с `vw_{Entity}` (скрипт создания во legacy `dbo\Views`).  
Колонки view, которых нет в коротком списке — **игнорировать**.  
Длины `NVARCHAR` — из `dbo\Tables` legacy + `LEFT(..., N)` в Insert.

### 3. Фильтр неиспользуемых атрибутов

Для **каждого** атрибута короткого списка:

```sql
SELECT COUNT(*) FROM vw_{Entity}
WHERE [{AttributeSystemName}] IS NOT NULL
  AND [{AttributeSystemName}] <> ''
  AND PrimitiveEntityDataStateId = 1;
```

- Результат `0` — исключить атрибут.
- Учитывать только **неархивные** записи (`PrimitiveEntityDataStateId = 1`).
- Если значение есть **только** у архивных — исключить.

Повторить для длинного списка (п.4) тем же алгоритмом.

### 4. Длинный список (detail view / группы)

```sql
;WITH DetailCTE AS (
    SELECT
        ai.[Name] AS AttributeSystemName,
        ai.[Description] AS AttributeNameRu,
        ISNULL(ag.[Rank], 0) AS GroupRank,
        s.[Rank] AS AttributeRank,
        ISNULL(xx.ValueEn, 'Default') AS GroupNameEn,
        ISNULL(xx.ValueRu, 'По умолчанию') AS GroupNameRu
    FROM AttributeInfo ai
    JOIN AttributeSettings s ON ai.Id = s.AttributeInfoId
    LEFT JOIN AttributesGroup ag ON ag.Id = s.GroupId
    OUTER APPLY (
        SELECT
            LOWER([Key]) AS [Key],
            MAX(CASE WHEN LocaleId = 1 THEN Value END) AS ValueEn,
            MAX(CASE WHEN LocaleId = 2 THEN Value END) AS ValueRu
        FROM Dictionary d
        WHERE LOWER(d.[Key]) = LOWER(ag.[Name]) AND ContextId = 258
        GROUP BY LOWER([Key])
    ) xx
    WHERE ai.PrimitiveEntityInfoId = (
        SELECT TOP 1 Id FROM PrimitiveEntityInfo WHERE [name] = '{Entity}'
    )
)
SELECT * FROM DetailCTE
ORDER BY GroupRank, AttributeRank;
```

**Порядок полей деталей** — как здесь (после фильтрации п.3).

### 5. Переводы заголовков колонок (resx)

Приоритет — **Dictionary** (`Context.Name = 'pei/' + {Entity} + '/ai'`).  
В таблице `Locale`: **Id 1 = en**, **Id 2 = ru** (проверено по `dbo.Locale`).

| Файл | Источник |
|------|----------|
| `Modules/{Entity}/Resources/{Entity}.resx` | Dictionary `LocaleId = 1` (en) |
| `Modules/{Entity}/Resources/{Entity}.ru-RU.resx` | Dictionary `LocaleId = 2` (ru) |

Fallback для **ru-RU** — `AttributeInfo.Description` (обычно русский).  
Для **default resx** fallback на `Description` **не использовать** (там русский текст) — только `LocaleId = 1`, иначе имя атрибута.

Проверка одного атрибута:

```sql
DECLARE
    @PrimitiveEntityInfoName NVARCHAR(100) = '{Entity}',
    @AttributeSystemName NVARCHAR(100) = '{AttributeSystemName}',
    @AttributeNameTranslationEn NVARCHAR(100),
    @AttributeNameTranslationRu NVARCHAR(100),
    @ContextID INT;

SELECT @ContextID = [Id]
FROM [dbo].[Context]
WHERE [Name] = 'pei/' + @PrimitiveEntityInfoName + '/ai';

SELECT @AttributeNameTranslationEn = [Value] FROM Dictionary
WHERE [key] = @AttributeSystemName AND ContextId = @ContextID AND LocaleId = 1;

SELECT @AttributeNameTranslationRu = [Value] FROM Dictionary
WHERE [key] = @AttributeSystemName AND ContextId = @ContextID AND LocaleId = 2;

SELECT @AttributeNameTranslationRu, @AttributeNameTranslationEn;
-- 1-я колонка → ru-RU.resx, 2-я → .resx
```

Пакетная выборка всех атрибутов:

```sql
DECLARE @ContextID INT;
SELECT @ContextID = [Id] FROM [dbo].[Context] WHERE [Name] = 'pei/{Entity}/ai';

SELECT
    ai.[Name] AS AttributeSystemName,
    dEn.[Value] AS TranslationEn,
    dRu.[Value] AS TranslationRu,
    ai.[Description] AS DescriptionRu
FROM AttributeInfo ai
LEFT JOIN Dictionary dEn ON dEn.[key] = ai.[Name] AND dEn.ContextId = @ContextID AND dEn.LocaleId = 1
LEFT JOIN Dictionary dRu ON dRu.[key] = ai.[Name] AND dRu.ContextId = @ContextID AND dRu.LocaleId = 2
WHERE ai.PrimitiveEntityInfoId = (SELECT TOP 1 Id FROM PrimitiveEntityInfo WHERE [name] = '{Entity}')
ORDER BY ai.[Name];
```

**Генерация resx:** скрипт `.cursor/skills/add-reference-book/scripts/generate-module-resx.py`

```powershell
python .cursor/skills/add-reference-book/scripts/generate-module-resx.py `
  --entity LocationsNodes `
  --output-dir BlazorSvt/Modules/LocationsNodes/Resources `
  --platform-resources-dir BlazorSvt/Platform/Resources
```

Требования к генерации:
- `sqlcmd -f o:65001`, вывод в файл (не stdout) — иначе ломается кодировка на Windows
- **Не путать** колонки En/Ru при записи в resx
- Ключи resx — имена свойств DTO (`{Entity}DetailDto.{Field}`), для переименованных полей — маппинг в скрипте
- После генерации spot-check: `Pobox` → en=`Pobox`, ru=`Адрес: Индекс`

> В скрипте групп detail (ContextId 258) маппинг LocaleId **другой**: `1` → ValueEn, `2` → ValueRu — не смешивать с `pei/{Entity}/ai`.

**Пост-обработка коротких имён грида (обязательно):** скрипт пишет **длинные** подписи из MDM. Сразу после генерации (и при правках локализации) примени глоссарий:

→ [grid-column-glossary.md](grid-column-glossary.md)

Кратко: `{Entity}Dto.*` ← short; парный `{Entity}DetailDto.*` ← `short (full)` если short ≠ full; full для нового модуля — из только что записанного MDM-значения; Detail-only / Groups / Title / мёртвые ключи — не трогать; неизвестные поля — предложить short на approve и **дописать в глоссарий**.

**Заголовок справочника** (`{Entity}Grid.Title` и `HeaderMenu.{Entity}`):

**По умолчанию** оба ключа получают одно и то же значение из MDM (ниже).  
**Исключение:** пользователь может задать **укороченный** пункт меню (`HeaderMenu.{Entity}`), оставив `{Entity}Grid.Title` полным из MDM. Пример: меню «Паритеты» / `Parities`, title «Паритетные ставки» / `Parity rates`. Без явного запроса на укорочение меню — держать значения одинаковыми.

Русский title — из MDM (`PrimitiveEntityInfo.Name` = системное имя `{Entity}`):

```sql
SELECT TOP 1 [Description]
FROM PrimitiveEntityInfo
WHERE [Name] = '{Entity}';
```

→ `{Entity}.ru-RU.resx` (`{Entity}Grid.Title`); по умолчанию то же → `Platform.ru-RU.resx` (`HeaderMenu.{Entity}`).

Пустой `Description` → **стоп, спросить пользователя**.

Английский title — из `Dictionary` (при исторических дубликатах достаточно `TOP 1`, не спрашивать):

```sql
SELECT TOP 1 DEn.[Value]
FROM Dictionary DEn
INNER JOIN Dictionary DRu
    ON DEn.ContextId = DRu.ContextId AND DEn.[Key] = DRu.[Key]
WHERE DRu.[Value] = (
    SELECT TOP 1 [Description]
    FROM PrimitiveEntityInfo
    WHERE [Name] = '{Entity}'
)
  AND DEn.LocaleId = 1
  AND DRu.LocaleId = 2;
```

→ `{Entity}.resx` (`{Entity}Grid.Title`); по умолчанию то же → `Platform.resx` (`HeaderMenu.{Entity}`).

Если запрос не вернул строку — перевод `Description` **агентом** при генерации; передать в скрипт `--title-en "..."`.

Скрипт: `--platform-resources-dir BlazorSvt/Platform/Resources` — обновляет `HeaderMenu.{Entity}` в Platform resx (при укороченном меню — править `HeaderMenu.*` вручную после скрипта или не перезаписывать короткие значения).

### 6. Ссылочные атрибуты

**Признак ссылки:** FK-колонка в legacy table/view **или** целочисленный тип (`int`/`bigint`) и имя совпадает с другой сущностью / есть в [таблице алиасов](#таблица-алиасов-сущностей).

**JOIN:** приоритет по **Id**; если нет — по **Code** (например legacy-атрибут хранит `Code` связанной сущности → `vw_{Ref}.Code`).

**Не JOIN:** multi-code строковые атрибуты с суффиксом `T` (несколько кодов через `/`, напр. `ShipmentTypeCodeT` у TransportLeg) — класть в snapshot как сырое `NVARCHAR`, без резолва в Id/enum.

**Эвристика без явного FK:**

- `bigint NULL` (или `int`) + имя совпадает с `PrimitiveEntityInfo` / `vw_{Name}` → ссылка.
- Иначе — **спросить пользователя**: число или ссылка? Если ссылка — на какой справочник?

#### Короткий словарь (<100, enum)

- **Snapshot:** `{X}Id` + денорм. `Code`/`Name*` (INSERT через JOIN).
- **Get / grid DTO:** только `{X}IdRu`, `{X}IdEn` (enum, `required`).
- **Grid:** одна видимая колонка по enum (`GetDisplayName`); **без** `Code`, **без** `Name*`.
- **Detail:** IdRu/IdEn допустимы (+ поля из `vw_*_Detail`).

#### Длинный словарь (общий случай: Country и т.п.)

- **Snapshot:** `{X}Id` для ETL (не отдавать в Get); `{X}NameRu`, `{X}NameEn`.
- **Get / grid DTO:** только `{X}NameRu`, `{X}NameEn` (NVARCHAR, FTS).
- **Grid:** одна видимая колонка `NameRu`/`NameEn`; **без Id**, **без Code**.
- **Detail:** IdRu/IdEn допустимы.

#### Whitelist «Code» (скрытый по умолчанию, FTS, пользователь может сделать видимым)

`NodeFrom`, `NodeTo`, `ProxyNode`, `ProductGroup`, `Product`, `TransportKind`, `TransportType`, **`RegionCode`** (при ссылке на Region).

- Строковые осмысленные сокращения (англ.).
- `{X}Code` + `{X}NameRu/En` в Get/grid DTO; **без Id**.
- Для `TransportKind`/`TransportType` (короткие из whitelist): enum + скрытый `{X}Code`.

**Не whitelist:** `CountryCode` и прочие legacy-числовые коды — **не включать** в Get/DTO/snapshot.

#### Индексы (snapshot)

- NCIX по Id **длинных** ссылок (Region, Country, …) **не создавать**.
- NCIX по Id **коротких** enum-ссылок (`LocationTypeId`, `TypeNodeId`, …) — **оставлять** (фильтр `Equals` по enum).
- FTS по `Name*` существующий **не трогать**; для whitelist добавлять `{X}Code` в FTS.

**Snapshot vs Detail:** detail может содержать IdRu/IdEn и полный набор полей из joined views. Get/grid — по правилам выше.

**Эталон grid:** `TransportRate` (узлы, ProductGroup); `LocationsNodes` (Region с `RegionCode`, Country только Name).

### 7. IdsEnum (маленькие словари)

Создавать enum только если в `vw_{Dictionary}` **< 100** неархивных записей (`PrimitiveEntityDataStateId = 1`; архивные исключить).

**Исключения:**

- `vw_Currency` — один enum `Currency` в `Platform/Domain/IdsEnum/` с `[Display(Name = "RUB"|"EUR"|…)]` (коды валют); **без** разделения на Ru/En.
- `vw_ShipmentType` — если когда-либо понадобится enum: имена полей **всегда** из `Code` (`TVD`, `PVG`, …). У TransportLeg enum **не** используется: в snapshot/grid лежит сырой `ShipmentTypeCodeT` (1–много кодов через `/`).

**Общие короткие словари** (`RateType`, `TransportTypeLevel3`, `TransportKind`, `Currency`, …) — сразу в
`Platform/Domain/IdsEnum/`. Если enum уже лежит в другом модуле — **перенести в Platform** и поправить
использующие модули (часть задачи). Модульный `List/IdsEnum/` — только если словарь **уникален** для
этого справочника и не переиспользуется.

Два файла: `{Dictionary}Ru.cs`, `{Dictionary}En.cs` (кроме `Currency`) — в Platform или в модуле по правилу выше.

**Значение каждого члена enum** = `Id` из `vw_{Dictionary}`.

**Имена полей** (одинаковые в `{Dictionary}En.cs` и `{Dictionary}Ru.cs`):

1. **Ветка Code** — если **каждое** значение `Code` валидно как идентификатор C#: ASCII, не начинается с цифры, не ключевое слово C#, символы только `[A-Za-z0-9_]`. Тогда имя поля = `Code`.
2. Если **хотя бы одно** значение `Code` невалидно — `Code` **не используем для всего словаря**.
3. **Ветка Name** — цепочка источников (первое непустое):
   `NameEn` → `Name_en` → `NameEnRu` (legacy: без кириллицы — как `NameEn`; иначе — как `NameRuEn`) → `NameRuEn` (часть без кириллицы, разделитель `/`) → `NameRu` → `Name_ru` → `ShortNameEn` → `ShortNameRu` → `FullNameEn` → `FullNameRu`.
4. Многословные имена: разделители (пробел, `/`, `-`, `|`, …) → `_`; первая буква заглавная (`Railway_station`, `Auto_10`).
5. Запрещённые в C# символы → `_`.
6. Кириллица в источнике (после исчерпания en-полей) → транслит **ГОСТ 7.79** (`Ж/Д` → `ZhD`).
7. **Коллизии** имён после санитизации → **стоп, спросить пользователя**.
8. Пустая цепочка fallback → **стоп, спросить пользователя**.
9. Невалидный `Code` при ожидании ASCII → **стоп, спросить пользователя**.
10. Автогенерация **всех** неархивных записей; лишнее убирается на ревью.

**`[Display(Name = "...")]`:** En enum — английское имя; Ru enum — русское (из `NameRu` / `Name` / `Name_ru`).

В grid settings: `typeof(XxxRu).GetDisplayName(dto.XxxIdRu.ToString())` / `XxxEn` по языку (без null-check для enum-полей).

**Scope правил:** новые справочники. Существующие enum не перегенерировать, кроме явного одноразового запроса в задаче.

---

## Таблица алиасов сущностей

Пополнять по мере выявления. Перед JOIN проверять эту таблицу, если имя поля ≠ имени `vw_*`.

| Поле во `vw_{Entity}` | Целевой справочник / view |
|-----------------------|---------------------------|
| `NodeFrom` | `vw_LocationsNodes` |
| `NodeTo` | `vw_LocationsNodes` |
| `ProxyNode` | `vw_LocationsNodes` |
| `ProxyNode1` / `ProxyNode2` / `ProxyNode3` (и любой числовой суффикс) | `vw_LocationsNodes` |
| `NodeFromCode` / `NodeToCode` (у ParityRates — BIGINT Id, не строковый Code) | `vw_LocationsNodes` |

---

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
    └── sp_{Entity}_PopulateAffectedKeys.sql
```

Обновить также `SqlScripts/Publish-AllSql.ps1` (порядок: CreateTable → SnapshotSource → Insert → Indexes → Detail → PopulateAffectedKeys).

### Structure

**01 — CreateTable:** системные поля + атрибуты короткого списка (порядок п.1); партиционирование; SEQUENCE; DEFAULT на `Id`, `CreationDate`; скрытые FK (`*_Id`) для каскада sync.  
Образец: `SqlScripts/Modules/TransportRate/Structure/01.TransportRate_CreateTable.sql`.

**02 — Insert:** `INSERT INTO v2.{Entity}_Snapshot … SELECT … FROM v2.vw_{Entity}_SnapshotSource` (не напрямую из legacy `vw_{Entity}`).  
После INSERT — инициализация `v2.SyncState` для всех источников job’а. Колонки SyncState: **`LastRowVersion`** (не `LastRowVer`), `LastRunUtc`. Копировать MERGE из эталона (`TransportRate` / `AverageRateLevel3`).  
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

**vw_{Entity}_SnapshotSource** — единая проекция для первичной заливки и incremental MERGE. WHERE-фильтр членства (NOT NULL ключевые метрики, валидный Code/RateType/Product и т.п.) — как у эталона; при бизнес-правиле «без связанных X — битая запись» — `EXISTS` / отсечение в проекции.

**vw_{Entity}_Detail** — поля длинного списка (п.4) + системные; ссылки: Code/Name из joined views; при необходимости плечи/лидтаймы (эталон: `vw_TransportRate_Detail`, `vw_AverageRateLevel3_Detail`).  
**Дочерние сущности без sub-grid:** вычисляемое поле через `STRING_AGG` (или аналог) **только в detail / export**, не в snapshot. Эталон: `TransportRateCodes` в `AverageRateLevel3`.

**sp_{Entity}_PopulateAffectedKeys** — детекция по `@Source` / `@Lo` / `@Hi`. В шапке и у каждой ветки `IF` / `FROM` — комментарий «номер таблицы → справочник» (как у TransportRate / AverageRateLevel3).

**RowVer:** расширить `SqlScripts/Sync/01.Legacy_AddRowVersion.sql` для основной `PrimitiveEntityData_*` (+ NC-индекс по RowVer, если таблица крупная). Maintenance-окно (Sch-M).

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

**Full export** — platform-процедура `v2.ExportBlazorGridDetail`: grid-метаданные из list-DTO, `@DetailViewName` и `@EntityKeyColumn` из `[DetailSource]` на detail-DTO. Per-module export-процедура **не нужна**.

Обновить `BlazorSvt/SqlScripts/README.md` (секция нового модуля).

---

## Фаза 3 — Деплой и верификация БД (до C#)

### Порядок деплоя

1. **RowVer** (если новая основная таблица) — `Sync/01.Legacy_AddRowVersion.sql`
2. **01 CreateTable** → **SnapshotSource** → **02 Insert** → **03 Indexes** → **Detail** → **PopulateAffectedKeys**  
   (Insert зависит от SnapshotSource; Indexes — после залитых данных)
3. Либо Structure 01–03 вручную, затем полный прогон Programmability:

```powershell
& ".\BlazorSvt\SqlScripts\Create-Programmability.ps1"
```

Из корня `c:\Work\BlazorSVT`.

### Structure: когда прогонять

| Ситуация | Действие |
|----------|----------|
| **Новый** справочник | Всегда `01`, `02`, `03` |
| **Существующий** | Спросить пользователя: нужен ли sqlcmd и **какие именно** файлы |

Пример sqlcmd (подставить connection string из `appsettings.json`):

```powershell
$sql = "<connection string из Database:MdmDb>"
sqlcmd -S ... -d mdm -i "BlazorSvt\SqlScripts\Modules\{Entity}\Structure\01.{Entity}_CreateTable.sql" -b
# повторить для 02, 03
```

### Верификация SQL (обязательна)

1. `EXEC v2.GetBlazorGridData` с примерами `@TableName`, `@AllowedColumnsJson`, `@SelectList` из атрибутов DTO (простой и сложный FTS, если применимо)
2. `EXEC v2.ExportBlazorGridDetail` с `@DetailViewName` / `@EntityKeyColumn` из detail-DTO
3. `SELECT TOP 1 * FROM v2.{Entity}_Snapshot`
4. `SELECT TOP 1 * FROM v2.vw_{Entity}_Detail`

**При ошибке деплоя или верификации — остановиться, C# не писать, спросить пользователя.**

---

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
├── Sync/
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
- `GetDefaultSettings`: порядок как **короткий список (п.1)**
- **Все колонки видимы**, кроме `IsArchive`, `CreationDate`, `LastChangeDate`
- `IsArchive`: `Visible = false`, `FilterValue = "False"` **всегда**
- `CreationDate`, `LastChangeDate`: `Visible = false` **всегда** (без `FilterValue`)
- **Короткий словарь (enum):** видимая колонка `{X}IdRu`/`{X}IdEn` с `GetDisplayName`; без `Code`/`Name*`.
- **Длинный словарь:** видимая `{X}NameRu`/`{X}NameEn`; **без Id**, без дублирующей скрытой Name-колонки.
- **Whitelist Code** (`NodeToCode`, `RegionCode`, …): `{X}Code` — `Visible = false`, `Filterable = true` (пользователь может включить в настройках).
- `Filterable = true` для текстовых и enum; `false` для вычисляемых числовых полей

### DetailSettingsService

- `IDetailSettingsService<{Entity}DetailDto>`
- Порядок и группы как **длинный список (п.4)**
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

**Короткие заголовки грида:** см. [grid-column-glossary.md](grid-column-glossary.md) (пост-обработка после п.5; единообразие с существующими модулями; обогащение словаря при новых общих полях).

Ключи в **Platform** resx:

- `HeaderMenu.{Entity}` (пункт меню; по умолчанию = `{Entity}Grid.Title`, либо укороченный текст по явному запросу — см. п.5)
- Прочие cross-cutting строки не добавлять в модульный resx

В `{Entity}Grid.razor.cs` — `IStringLocalizer<Resources.{Entity}> EL` для `PageTitle` и специфичных сообщений; `SvtComponentBase.L` — только platform-строки.

### DI и меню

**`{Entity}Module.cs`:**

```csharp
services.AddScoped<IGridSettingsService<{Entity}Dto>, {Entity}GridSettingsService>();
services.AddScoped<IDetailSettingsService<{Entity}DetailDto>, {Entity}DetailSettingsService>();
services.AddSingleton<ISnapshotSyncJob, {Entity}SyncJob>();
```

**`{Entity}SyncJob`:** эталон `TransportRateSyncJob` / `AverageRateLevel3SyncJob` — consts `PrimitiveEntityData_*` с комментариями имён справочников; Sources = основная + каскад (узлы / ProductGroup / MTR и т.д. по проекции). Стабильные словари (RateType, Currency, …) **не** включать.

**`Host/Program.cs`:** `builder.Services.Add{Entity}Module();`

**`HeaderMenu.razor.cs`:** пункт с `Url = "{entity}"` (lowercase), текст `L["HeaderMenu.{Entity}"]`, иконка — произвольно по аналогии.

URL меню **должен совпадать** с `@page` в `.razor`.

---

## Фаза 5 — Тесты

Эталоны: `GridColumnMetadataBuilderTests`, `SnapshotSyncJobContractTests`, `ModuleGridIntegrationTests`, `TransportRateFtsIntegrationTests` / `AverageRateLevel3FtsIntegrationTests`.

### Unit (обязательно)

1. Добавить `{Entity}Dto` в `[Theory]` contract-тест `GridColumnMetadataBuilderTests`:
   - `TableName` = `v2.{Entity}_Snapshot`
   - `EntityKeyPropertyName` = `{Entity}Id`
2. Добавить `{Entity}SyncJob` в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`)
3. При изменении Platform-логики — тесты в `GridQueryFactoryTests` / `GridColumnMetadataBuilderTests`

### Integration (обязательно, read-only)

1. **Grid smoke** + **Detail view** — в `ModuleGridIntegrationTests.cs` (или `Modules/{Entity}/`)
2. **FTS** — `Modules/{Entity}/{Entity}FtsIntegrationTests.cs` по образцу TransportRate / AverageRateLevel3 (`FtsFilterTestSupport`: `IsArchive=False` + Contains по Name-полям; при наличии — фильтр enum TransportKind/TypeNode)

Только read-only на существующих данных dev-БД. **Без** INSERT/ROLLBACK. Пометить `[Trait("Category", "Integration")]` + `[SkippableFact]`.

### Запуск

```powershell
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

---

## Фаза 6 — Финальная верификация

1. `dotnet build` (проект `BlazorSvt`)
2. `dotnet test`
3. Страница `/{entity}` открывается, грид загружает данные
4. Детальный просмотр и экспорт не падают

При ошибке — сообщить и спросить дальнейшие действия.

---

## Рефакторинг legacy

- Для legacy-модулей (`Legs`, `Rates`) использовать только `git mv` для папок и файлов, затем правки содержимого in-place.
- Не удалять и не пересоздавать файлы/папки через copy+delete, чтобы сохранить git-history.
- При rename SQL сохранять существующие комментарии и нумерованные шаги; добавлять новые `DROP`-блоки в начало, а не заменять ими текущие блоки.
- В dev-среде wrapper'ы и миграции данных не требуются: удалять legacy и предыдущие имена объектов прямо в скриптах.

---

## Чеклист

- [ ] Имя `{Entity}` получено
- [ ] Короткий и длинный списки + фильтрация неиспользуемых
- [ ] Переводы из Dictionary в `Modules/{Entity}/Resources/{Entity}.resx` + `HeaderMenu.{Entity}` в Platform.resx
- [ ] SQL Structure 01–03 + SnapshotSource + Detail + PopulateAffectedKeys
- [ ] Все новые/изменённые `.sql` в SqlScripts — UTF-8 with BOM
- [ ] Комментарии «PED_* → справочник» в Insert Sources / PopulateAffectedKeys / SyncJob
- [ ] RowVer (+ индекс) в `Sync/01.Legacy_AddRowVersion.sql` при новой основной таблице
- [ ] SyncState init с `LastRowVersion` / `LastRunUtc`
- [ ] README + `Publish-AllSql.ps1` обновлены
- [ ] Structure / Programmability задеплоены и верифицированы
- [ ] C# модуль (List/Detail/Sync), DI, меню, resx
- [ ] Общие enum в `Platform/Domain/IdsEnum` (рефакторинг затронутых модулей)
- [ ] Unit: `GridColumnMetadataBuilderTests` + `SnapshotSyncJobContractTests`
- [ ] Integration: grid + detail smoke + FTS
- [ ] `dotnet build` / `dotnet test` OK

---

## Связанные навыки

- **create-programmability** — накат Programmability на dev БД
- **publish-all-sql** — плоская публикация скриптов в `C:\publish\v2`
- **svt-architecture** — архитектурный контекст и антипаттерны
