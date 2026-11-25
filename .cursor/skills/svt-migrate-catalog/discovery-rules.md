# Правила экрана discovery

Запросы к `mdm` и DDL — [discovery-queries.md](discovery-queries.md). Этот файл их не содержит. Открывай его, только если роль велела решать состав экрана. Запросы не открывай, если выборка уже есть.

`{LegacyName}` — имя основного примитива, `{Entity}` — имя модуля. Шаг 0 — [SKILL.md](SKILL.md).

Состав экрана: `IsMain = 1` — кандидат в короткий список, не решение о колонке. Атрибут с нулём заполненных неархивных значений (§3) в грид и карточку не входит; фраза «заполняет интеграция» этот счётчик не отменяет. Примитив сущности вне скоупа — шаг 0. Дочерний примитив — [child-primitives.md](child-primitives.md). Согласованный состав конкретного справочника — в постановке и в комментарии `catalogs-scope.md`.

## 1. Короткий список

Запрос — [discovery-queries.md](discovery-queries.md) §1.

**Порядок колонок грида** по умолчанию — как в этом списке после фильтрации §3. Другой порядок, согласованный для справочника, задаётся в `{Entity}GridSettingsService`, не в SQL. Порядок карточки — §4.

## 2. Типы атрибутов

Сопоставить короткий список с `vw_{LegacyName}`.  
Колонки view, которых нет в коротком списке — **игнорировать**.  
Длины `NVARCHAR` — из `dbo\Tables` legacy + `LEFT(..., N)` в Insert.

**Тип `Code`.** Запрос — [discovery-queries.md](discovery-queries.md) §2а. В легаси `Code` всегда `nvarchar(100)`, по типу колонки число не отличить. Кто ставит код, задаёт `AttributeInfo.ValueGeneratorId` (`dbo.ValueGenerator`): `1` Guid и `2` OrdinalNumber — система, `3` Manual — пользователь.

- `NotInt = 0`: все коды, включая архивные, — целые и помещаются в `INT`. **Стоп, вопрос пользователю** с рекомендацией `INT`. В вопросе назвать генератор. При Manual — риск: нецелый код, который введут в легаси, даст `NULL` в проекции, и синхронизация строки упадёт на `NOT NULL`.
- Иначе — `NVARCHAR` длиной из легаси. Генератор сам по себе тип не задаёт: у ProductGroup генератор `2`, а коды `T1`…`T9`.
- Guid (`1`) — `NVARCHAR`, не `uniqueidentifier`: поиск по началу кода (`LIKE`) работает только по строке. Full-text на нём нет — [sql.md](sql.md), фаза 2, 03.
- Ответ `INT`: проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`. Образец — `TransportRate`, `TransportRoute`.

## 3. Фильтр неиспользуемых атрибутов

Счётчик — [discovery-queries.md](discovery-queries.md) §3.

- Результат `0` — исключить атрибут из грида и карточки. Утверждение, что интеграция колонку заполняет, нулевой счётчик не отменяет.
- Учитывать только **неархивные** записи (`PrimitiveEntityDataStateId = 1`).
- Если значение есть **только** у архивных — исключить.

То же для длинного списка (§4).

## 4. Длинный список

Запрос — [discovery-queries.md](discovery-queries.md) §4.

**Порядок полей деталей** — как в этом запросе, после фильтрации §3: `GroupRank`, затем `AttributeRank`. Он может не совпадать с порядком грида.

## 5. Переводы заголовков колонок (resx)

Запросы перевода и заголовка — [discovery-queries.md](discovery-queries.md) §5 и «Заголовки». Resx пишет `scripts/generate-module-resx.py`, не роль discovery конвейера.

Приоритет — **Dictionary** (`Context.Name = 'pei/' + {LegacyName} + '/ai'`).  
В таблице `Locale`: **Id 1 = en**, **Id 2 = ru** (проверено по `dbo.Locale`).

| Файл | Источник |
|------|----------|
| `Modules/{Entity}/Resources/{Entity}.resx` | Dictionary `LocaleId = 1` (en) |
| `Modules/{Entity}/Resources/{Entity}.ru-RU.resx` | Dictionary `LocaleId = 2` (ru) |

Fallback для **ru-RU** — `AttributeInfo.Description` (обычно русский).  
Для **default resx** fallback на `Description` **не использовать** (там русский текст) — только `LocaleId = 1`, иначе имя атрибута.

Если значение `LocaleId = 1` совпадает с системным именем атрибута — в resx его не писать. Взять согласованный человеко-читаемый EN из [grid-column-glossary.md](grid-column-glossary.md) (раздел «Согласованный EN»). Если строки там нет — остановиться и согласовать перевод, затем дописать глоссарий.

**Генерация resx:** скрипт `.cursor/skills/svt-migrate-catalog/scripts/generate-module-resx.py`

```powershell
python .cursor/skills/svt-migrate-catalog/scripts/generate-module-resx.py `
  --entity LocationsNodes `
  --output-dir BlazorSvt/Modules/LocationsNodes/Resources `
  --platform-resources-dir BlazorSvt/Platform/Resources
```

`--entity` — `{Entity}` (ключи resx и имя файла). Если `{LegacyName}` отличается, добавь `--legacy-name {LegacyName}`: этим именем скрипт ищет строку в `PrimitiveEntityInfo`.

Требования к генерации:
- `sqlcmd -f o:65001`, вывод в файл (не stdout) — иначе ломается кодировка на Windows
- **Не путать** колонки En/Ru при записи в resx
- Ключи resx — имена свойств DTO (`{Entity}DetailDto.{Field}`), для переименованных полей — маппинг в скрипте
- После генерации spot-check: `Pobox` → en=`Pobox`, ru=`Адрес: Индекс`

> В скрипте групп detail (ContextId 258) маппинг LocaleId **другой**: `1` → ValueEn, `2` → ValueRu — не смешивать с `pei/{LegacyName}/ai`.

**Пост-обработка коротких имён грида (обязательно):** скрипт пишет **длинные** подписи из MDM. Сразу после генерации (и при правках локализации) примени глоссарий:

→ [grid-column-glossary.md](grid-column-glossary.md)

Читать раздел «Правила» и найти имя поля поиском. Блоки `###` чужих справочников не читать.

Кратко: `{Entity}Dto.*` ← short; парный `{Entity}DetailDto.*` ← `short (full)` если short ≠ full; full для нового модуля — из только что записанного MDM-значения; Detail-only / Groups / Title / мёртвые ключи — не трогать; неизвестные поля — сократить по правилам глоссария, дописать в глоссарий и назвать в отчёте. Вопрос — только если два правила расходятся.

**Заголовок справочника** (`{Entity}Grid.Title` и `HeaderMenu.{Entity}`):

Оба ключа получают одно и то же значение из MDM. Пункт меню совпадает с заголовком страницы, без сокращений.

Русский title — `Description` из MDM по `{LegacyName}` → `{Entity}.ru-RU.resx` (`{Entity}Grid.Title`) и то же значение → `Platform.ru-RU.resx` (`HeaderMenu.{Entity}`).

Пустой `Description` → **стоп, спросить пользователя**.

Английский title — из `Dictionary` (при исторических дубликатах достаточно `TOP 1`, не спрашивать) → `{Entity}.resx` (`{Entity}Grid.Title`) и то же значение → `Platform.resx` (`HeaderMenu.{Entity}`).

Если запрос не вернул строку — перевод `Description` **агентом** при генерации; передать в скрипт `--title-en "..."`.

Скрипт: `--platform-resources-dir BlazorSvt/Platform/Resources` — пишет в Platform resx тот же `HeaderMenu.{Entity}`, что и `{Entity}Grid.Title`.

## 6. Ссылочные атрибуты

**Признак ссылки:** FK-колонка в legacy table/view **или** целочисленный тип (`int`/`bigint`) и имя совпадает с другой сущностью / есть в [таблице алиасов](#таблица-алиасов-сущностей).

**JOIN:** приоритет по **Id**; если нет — по **Code** (например legacy-атрибут хранит `Code` связанной сущности → `vw_{Ref}.Code`).

**Не JOIN:** multi-code строковые атрибуты с суффиксом `T` (несколько кодов через `/`, напр. `ShipmentTypeCodeT` у TransportLeg) — класть в snapshot как сырое `NVARCHAR`, без резолва в Id/enum.

**Эвристика без явного FK:**

- `bigint NULL` (или `int`) + имя совпадает с `PrimitiveEntityInfo` / `vw_{Name}` → ссылка.
- Иначе — **спросить пользователя**: число или ссылка? Если ссылка — на какой справочник?

### Короткий словарь (<100, enum)

- **Snapshot:** `{X}Id` + денорм. `Code`/`Name*` (INSERT через JOIN).
- **Get / grid DTO:** только `{X}IdRu`, `{X}IdEn` (enum, `required`).
- **Grid:** одна видимая колонка по enum (`GetDisplayName`); **без** `Code`, **без** `Name*`.
- **Detail:** IdRu/IdEn допустимы (+ поля из `vw_*_Detail`).

### Длинный словарь (общий случай: Country и т.п.)

- **Snapshot:** `{X}Id` для ETL (не отдавать в Get); `{X}NameRu`, `{X}NameEn`.
- **Get / grid DTO:** только `{X}NameRu`, `{X}NameEn` (NVARCHAR, FTS).
- **Grid:** одна видимая колонка `NameRu`/`NameEn`; **без Id**, **без Code**.
- **Detail:** IdRu/IdEn допустимы.

### Whitelist «Code» (скрытый по умолчанию, FTS, пользователь может сделать видимым)

`NodeFrom`, `NodeTo`, `ProxyNode`, `ProductGroup`, `Product`, `TransportKind`, `TransportType`, **`RegionCode`** (при ссылке на Region).

- Строковые осмысленные сокращения (англ.).
- `{X}Code` + `{X}NameRu/En` в Get/grid DTO; **без Id**.
- Для `TransportKind`/`TransportType` (короткие из whitelist): enum + скрытый `{X}Code`.

**Не whitelist:** `CountryCode` и прочие legacy-числовые коды — **не включать** в Get/DTO/snapshot.

### Индексы (snapshot)

- NCIX по Id **длинных** ссылок (Region, Country, …) **не создавать**.
- NCIX по Id **коротких** enum-ссылок (`LocationTypeId`, `TypeNodeId`, …) — **оставлять** (фильтр `Equals` по enum).
- FTS по `Name*` существующий **не трогать**; для whitelist добавлять `{X}Code` в FTS.

**Snapshot vs Detail:** detail может содержать IdRu/IdEn и полный набор полей из joined views. Get/grid — по правилам выше.

**Эталон grid:** `TransportRate` (узлы, ProductGroup); `LocationsNodes` (Region с `RegionCode`, Country только Name).

### Каскад

Длинный словарь, чьё имя денормализовано в снимок, — источник sync. Данные для решения: `MAX(LastChangeDate)`, изменения за 3 года, fan-out, есть ли `RowVer`, пишет ли `stg.Load*` эту колонку. Предложение по умолчанию — каскад включить. Стабильный словарь с enum в чужой каскад не включать — [SKILL.md](SKILL.md).

## 7. IdsEnum (маленькие словари)

Создавать enum только если в `vw_{Dictionary}` **< 100** неархивных записей (`PrimitiveEntityDataStateId = 1`; архивные исключить).

**Исключения:**

- `vw_Currency` — один enum `CurrencyRuEn` в `Platform/Domain/IdsEnum/` с `[Display(Name = "RUB"|"EUR"|…)]` (коды валют); **без** разделения на Ru/En.
- `vw_ShipmentType` — если когда-либо понадобится enum: имена полей **всегда** из `Code` (`TVD`, `PVG`, …). У TransportLeg enum **не** используется: в snapshot/grid лежит сырой `ShipmentTypeCodeT` (1–много кодов через `/`).

**Общие короткие словари** (`RateType`, `TransportTypeLevel3`, `TransportKind`, `Currency`, …) — сразу в
`Platform/Domain/IdsEnum/`. Если enum уже лежит в другом модуле — **перенести в Platform** и поправить
использующие модули (часть задачи). Модульный `List/IdsEnum/` — только если словарь **уникален** для
этого справочника и не переиспользуется.

Два файла: `{Dictionary}Ru.cs`, `{Dictionary}En.cs` (кроме `CurrencyRuEn`) — в Platform или в модуле по правилу выше.

**Значение каждого члена enum** = `Id` из `vw_{Dictionary}`.

**Имена полей** (одинаковые в `{Dictionary}En.cs` и `{Dictionary}Ru.cs`):

1. **Ветка Code** — если **каждое** значение `Code` валидно как идентификатор C#: ASCII, не начинается с цифры, не ключевое слово C#, символы только `[A-Za-z0-9_]`. Тогда имя поля = `Code`.
2. Если **хотя бы одно** значение `Code` невалидно — `Code` **не используем для всего словаря**.
2а. **Ветка SearchCode.** Если `Code` невалиден (п.2) и у словаря есть `SearchCode`, каждое значение которого валидно как идентификатор C# (правила ветки Code), имя члена = `SearchCode`. Если `SearchCode` нет, а `Name` валидно как идентификатор C#, имя члена = `Name`. В обоих случаях `Display` английского enum берётся дословно из той же колонки, что и имя члена. `Display` русского enum — дословно `Name`, а при имени члена из `Name` — `FullName`, если оно есть. Коллизии и невалидные идентификаторы — стоп, как в п.7 и п.9. Ветка Name из п.3 остаётся для словарей без подходящих `SearchCode` и `Name`.
3. **Ветка Name** — цепочка источников (первое непустое):
   `NameEn` → `Name_en` → `NameEnRu` (legacy: без кириллицы — как `NameEn`; иначе — как `NameRuEn`) → `NameRuEn` (часть без кириллицы, разделитель `/`) → `NameRu` → `Name_ru` → `ShortNameEn` → `ShortNameRu` → `FullNameEn` → `FullNameRu`.
4. Многословные имена: разделители (пробел, `/`, `-`, `|`, …) → `_`; первая буква заглавная (`Railway_station`, `Auto_10`).
5. Запрещённые в C# символы → `_`.
6. Кириллица в источнике (после исчерпания en-полей) → транслит **ГОСТ 7.79** (`Ж/Д` → `ZhD`).
7. **Коллизии** имён после санитизации → **стоп, спросить пользователя**.
8. Пустая цепочка fallback → **стоп, спросить пользователя**.
9. Невалидный `Code` при ожидании ASCII → **стоп, спросить пользователя**.
10. Автогенерация **всех** неархивных записей; лишнее убирается на ревью.

**`[Display(Name = "...")]`:** En enum — английское имя; Ru enum — русское (из `NameRu` / `Name` / `Name_ru`). Текст копируется из выбранной колонки базы дословно. Составную фразу не собирать и не дописывать слова, которых в ячейке нет.

В grid settings: `typeof(XxxRu).GetDisplayName(dto.XxxIdRu.ToString())` / `XxxEn` по языку (без null-check для enum-полей).

**Scope правил:** новые справочники. Существующие enum не перегенерировать, кроме явного одноразового запроса в задаче.

## Таблица алиасов сущностей

Пополнять по мере выявления. Перед JOIN проверять эту таблицу, если имя поля ≠ имени `vw_*`.

| Поле во `vw_{LegacyName}` | Целевой справочник / view |
|-----------------------|---------------------------|
| `NodeFrom` | `vw_LocationsNodes` |
| `NodeTo` | `vw_LocationsNodes` |
| `ProxyNode` | `vw_LocationsNodes` |
| `ProxyNode1` / `ProxyNode2` / `ProxyNode3` (и любой числовой суффикс) | `vw_LocationsNodes` |
| `NodeFromCode` / `NodeToCode` (у ParityRates — BIGINT Id, не строковый Code) | `vw_LocationsNodes` |
