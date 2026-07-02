---
name: add-reference-book
description: >-
  Добавление read-only справочника BlazorSVT из легаси MDM: discovery атрибутов,
  snapshot SQL, programmability, C#-модуль, grid и меню. Используй когда пользователь
  просит добавить справочник, создать новый модуль справочника или перенести
  PrimitiveEntityInfo из старой системы.
---

# Добавление справочника (read-only)

Полный вертикальный срез: MDM discovery → SQL (Structure + Programmability) → деплой и верификация БД → C#-модуль.

**Эталоны реализации:** `Modules/Legs`, `Modules/Rates` (паттерны кода и SQL).  
**Именование:** для **новых** справочников — соглашения ниже. Существующие `Legs`/`Rates` (`LegId`, `RateId`, папки `Legs`/`Rates`) **не трогать** — отдельный рефакторинг.

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
| Get / Export | `v2.{Entity}_Get`, `v2.{Entity}_ExportFull` | `v2.TransportLeg_Get` |
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

**Заголовок справочника** (`{Entity}Grid.Title` + `HeaderMenu.{Entity}`):

Русский — из MDM (`PrimitiveEntityInfo.Name` = системное имя `{Entity}`):

```sql
SELECT TOP 1 [Description]
FROM PrimitiveEntityInfo
WHERE [Name] = '{Entity}';
```

→ `{Entity}.ru-RU.resx` (`{Entity}Grid.Title`) и `Platform.ru-RU.resx` (`HeaderMenu.{Entity}`).

Пустой `Description` → **стоп, спросить пользователя**.

Английский — из `Dictionary` (при исторических дубликатах достаточно `TOP 1`, не спрашивать):

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

→ `{Entity}.resx` (`{Entity}Grid.Title`) и `Platform.resx` (`HeaderMenu.{Entity}`).

Если запрос не вернул строку — перевод `Description` **агентом** при генерации; передать в скрипт `--title-en "..."`.

Скрипт: `--platform-resources-dir BlazorSvt/Platform/Resources` — обновляет `HeaderMenu.{Entity}` в Platform resx.

### 6. Ссылочные атрибуты

**Признак ссылки:** FK-колонка в legacy table/view **или** целочисленный тип (`int`/`bigint`) и имя совпадает с другой сущностью / есть в [таблице алиасов](#таблица-алиасов-сущностей).

**Поля для ссылки** (snapshot и detail):

- `{Xxx}IdRu`, `{Xxx}IdEn` (detail и Get; в snapshot одно `{Xxx}Id`)
- `{Xxx}Code`
- `{Xxx}NameEn`, `{Xxx}NameRu`

**JOIN:** приоритет по **Id**; если нет — по **Code** (как `ShipmentTypeCodeT` → `vw_ShipmentType.Code`).

**Эвристика без явного FK:**

- `bigint NULL` (или `int`) + имя совпадает с `PrimitiveEntityInfo` / `vw_{Name}` → ссылка.
- Иначе — **спросить пользователя**: число или ссылка? Если ссылка — на какой справочник?

**Snapshot vs Detail:** обычно одинаковый набор полей из п.1 и п.4. Дополнительные вычисляемые поля (как `Leg1_*` у `TransportLeg`) — **только по явному запросу** после каркаса.

### 7. IdsEnum (маленькие словари)

Создавать enum только если в `vw_{Dictionary}` **< 100** неархивных записей (`PrimitiveEntityDataStateId = 1`; архивные исключить).

**Исключения:**

- `vw_Currency` — один enum `Currency` в `Platform/Domain/IdsEnum/` с `[Display(Name = "RUB"|"EUR"|…)]` (коды валют); **без** разделения на Ru/En.
- `vw_ShipmentType` — имена полей **всегда** из `Code` (`TVD`, `PVG`, …), даже если общие правила иначе.

Если enum уже есть в другом модуле — **перенести в** `Platform/Domain/IdsEnum/`.

Два файла: `{Dictionary}Ru.cs`, `{Dictionary}En.cs` в `Modules/{Entity}/List/IdsEnum/` (кроме `Currency`).

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

---

## Фаза 2 — SQL-артефакты

Папки:

```
BlazorSvt/SqlScripts/Modules/{Entity}/
├── Structure/
│   ├── 01.{Entity}_CreateTable.sql
│   ├── 02.{Entity}_Insert.sql
│   └── 03.{Entity}_CreateIndexes.sql
└── Programmability/
    ├── vw_{Entity}_Detail.sql
    ├── {Entity}_Get.sql
    └── {Entity}_ExportFull.sql
```

### Structure

**01 — CreateTable:** системные поля + атрибуты короткого списка (порядок п.1); партиционирование; SEQUENCE; DEFAULT на `Id`, `CreationDate`.  
Образец: `SqlScripts/Modules/Legs/Structure/01.TransportLegs_CreateTable.sql` (с новыми именами).

**02 — Insert:** `INSERT INTO v2.{Entity}_Snapshot … SELECT … FROM vw_{Entity}` + JOIN по ссылкам.  
Образец: `Legs/Structure/02.TransportLegs_Insert.sql`.

**03 — Indexes:**

- `UX_{Entity}_Snapshot_Id` на `[PRIMARY]` (для FTS при партициях)
- FULLTEXT на `Code` (только если `Code` — `NVARCHAR`; для числового `Code` FTS не нужен — достаточно фильтрованных индексов), `*NameEn`, `*NameRu` (языки 1033 / 1049)
- `ALTER FULLTEXT … SET STOPLIST = OFF`
- Фильтрованные индексы на `Code` и ID-ссылки (`WHERE IsArchive = 0/1`)
- `UPDATE STATISTICS … WITH FULLSCAN`

### Programmability

**vw_{Entity}_Detail** — поля длинного списка (п.4) + системные; ссылки: `IdRu`/`IdEn`, `Code`, `Name_en`/`Name_ru` из joined views.  
Образцы: `vw_TransportLegs_Detail.sql`, `vw_TransportRates_Detail.sql`.

**{Entity}_Get** — только поля snapshot; вызов `v2.GetBlazorGridData`; `@AllowedColumnsJson` + `@SelectList`.  
Id в SELECT: `XxxId AS XxxIdRu, XxxId AS XxxIdEn`.  
**Два примера** в комментарии: простой поиск и сложный (сортировка + ≥2 FTS-фильтра), если FTS-полей > 2.

`ColumnType` в `@AllowedColumnsJson`:

| SQL-тип | ColumnType |
|---------|------------|
| `BIT` | `BIT` |
| FK / `INT` | `ID` |
| `NVARCHAR` | `NVARCHAR` |
| `DATETIME` | `DATE` |

**{Entity}_ExportFull** — `@KeysOnly = 1` через `{Entity}_Get`, затем `SELECT d.* FROM v2.vw_{Entity}_Detail d JOIN #Filtered …`.

Обновить `BlazorSvt/SqlScripts/README.md` (секция нового модуля).

---

## Фаза 3 — Деплой и верификация БД (до C#)

### Порядок деплоя

1. **Structure** — `sqlcmd` по файлам `01 → 02 → 03`
2. **Programmability** — всегда полный прогон:

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

1. `EXEC` примеры из `{Entity}_Get` (оба, если применимо)
2. `SELECT TOP 1 * FROM v2.{Entity}_Snapshot`
3. `SELECT TOP 1 * FROM v2.vw_{Entity}_Detail`

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
│   └── IdsEnum/          # при необходимости
└── Detail/
    ├── {Entity}DetailDto.cs
    └── {Entity}DetailSettingsService.cs
```

### DTO-атрибуты (обязательно)

```csharp
[StoredProcedure("v2.{Entity}_Get")]
public class {Entity}Dto { ... }

[DetailSource("v2.vw_{Entity}_Detail", "{Entity}Id")]
[FullReportExport("v2.{Entity}_ExportFull")]
public class {Entity}DetailDto { ... }
```

`{Entity}Dto` — поля snapshot + типы под процедуру `_Get`.  
`{Entity}DetailDto` — поля `vw_{Entity}_Detail` и `_ExportFull`.

**Enum-ссылки (IdsEnum):** поля, типизированные enum маленького словаря, объявлять **обязательными non-nullable** (`required`), как в `TransportRateDto`:

```csharp
public required RateTypeRu RateTypeIdRu { get; set; }
public required RateTypeEn RateTypeIdEn { get; set; }
```

Обычные FK без enum (`long? RegionIdRu`), строки и опциональные ссылки остаются nullable. Эталон: `TransportRateDto`, `TransportLegDto`.

### GridSettingsService

- Наследник `BaseGridSettingsService<{Entity}Dto>`
- `StorageKey` → `"{Entity}GridColumnSettings"`
- `GetDefaultSettings`: порядок как **короткий список (п.1)**
- **Все колонки видимы**, кроме `IsArchive`, `CreationDate`, `LastChangeDate`
- `IsArchive`: `Visible = false`, `FilterValue = "False"` **всегда**
- `CreationDate`, `LastChangeDate`: `Visible = false` **всегда** (без `FilterValue`)
- Ссылки: отдельные колонки Ru/En (`XxxIdRu` / `XxxIdEn`)
- `Filterable = true` для текстовых и ID; `false` для вычисляемых числовых полей
- Коды ссылок (`XxxCode`) — часто `Visible = false` (как в Legs)

### DetailSettingsService

- `IDetailSettingsService<{Entity}DetailDto>`
- Порядок и группы как **длинный список (п.4)**
- `GroupHeader` из resx: ключ `{Entity}DetailDto.Group.{GroupRank}.{SanitizedGroupNameEn}`
- `SanitizedGroupNameEn`: PascalCase, только буквы/цифры (пробелы и спецсимволы удалить)

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

Ключи в **Platform** resx:

- `HeaderMenu.{Entity}` (пункт меню; **те же значения**, что `{Entity}Grid.Title` — см. п.5)
- Прочие cross-cutting строки не добавлять в модульный resx

В `{Entity}Grid.razor.cs` — `IStringLocalizer<Resources.{Entity}> EL` для `PageTitle` и специфичных сообщений; `SvtComponentBase.L` — только platform-строки.

### DI и меню

**`{Entity}Module.cs`:**

```csharp
services.AddScoped<IGridSettingsService<{Entity}Dto>, {Entity}GridSettingsService>();
services.AddScoped<IDetailSettingsService<{Entity}DetailDto>, {Entity}DetailSettingsService>();
```

**`Host/Program.cs`:** `builder.Services.Add{Entity}Module();`

**`HeaderMenu.razor.cs`:** пункт с `Url = "{entity}"` (lowercase), текст `L["HeaderMenu.{Entity}"]`, иконка — произвольно по аналогии.

URL меню **должен совпадать** с `@page` в `.razor`.

---

## Фаза 5 — Финальная верификация

1. `dotnet build` (проект `BlazorSvt`)
2. Страница `/{entity}` открывается, грид загружает данные
3. Детальный просмотр и экспорт не падают

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
- [ ] SQL Structure 01–03 + Programmability
- [ ] README обновлён
- [ ] Structure задеплоен (01–03 или по согласованию)
- [ ] Programmability задеплоен
- [ ] SQL-верификация пройдена
- [ ] C# модуль, DI, меню, модульный resx + Platform.resx (меню)
- [ ] `dotnet build` OK

---

## Связанные навыки

- **create-programmability** — накат Programmability на dev БД
- **svt-architecture** — архитектурный контекст и антипаттерны
