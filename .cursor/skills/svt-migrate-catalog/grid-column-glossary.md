# Глоссарий коротких имён колонок грида

Канонический словарь для заголовков `{Entity}Dto.*` / парных `{Entity}DetailDto.*` в модульных resx.  
Используется при добавлении справочника (пост-обработка после генерации resx) и при правках локализации существующих модулей.

Эталоны: `AverageRateLevel3`, `TransportLeg`, `TransportRate`, `LocationsNodes`.

Читать раздел «Правила». Строку поля искать по имени. Блоки `###` чужих справочников не читать.

---

## Правила

| Тема | Правило |
|------|---------|
| Grid short | 1–2 слова (макс. 3); цель ≤15 символов, жёстко ≤25 |
| Единообразие | Одинаковые поля → одинаковые подписи во всех справочниках (редкие исключения — только с пояснением в таблице ниже) |
| `*Code` | Отдельный короткий лейбл (не «Код» + полное имя), напр. «Код гр. пр.» / `Group code` |
| EN направления | From / To (не Origin / Destination) |
| Единицы измерения | Только в Detail (в скобках у полного имени), не в заголовке грида |
| Точки | После точки в аббревиатурах — пробел (`Рег. отпр.`, не `Рег.отпр.`) |
| Detail формат | `Короткое (полное)`, **только если** короткое ≠ текущему полному. Если полное уже начинается с короткого, на карточке остаётся полное, без второй обёртки: short `INN`, full `INN (taxpayer identification number)` → карточка `INN (taxpayer identification number)` |
| Detail-only | Поля без пары в гриде — **не** сокращать |
| Groups / Title | `{Entity}DetailDto.Group.*`, `{Entity}Grid.Title` — **не** трогать по этому глоссарию |
| Мёртвые ключи | Ключи не из `*GridSettingsService` — **не** трогать |
| Exact pair | По умолчанию `{Entity}Dto.X` ↔ `{Entity}DetailDto.X` |
| Белый список | Доп. семантические пары — см. [Белый список](#белый-список-семантических-пар) |
| `Name` vs `NameRu`/`NameEn` | Grid `Name` сокращать; Detail `NameRu`/`NameEn` **не** авто-парить с `Name` |
| Неизвестное поле | Сократить по правилам выше, применить и назвать подпись в отчёте. Вопрос пользователю — только если два правила этой таблицы дают разные подписи |
| Обогащение | Новое общее поле → **дописать строку** в таблицу глоссария. Отдельный approve на каждое поле не нужен |
| Дочерний примитив | Таблица на карточке — полные имена, short не задаётся. Блок «Таблицы дочерних примитивов на карточке {Entity}» в разделе справочника-родителя |

### Источник «полного» имени для скобок Detail

| Сценарий | Откуда брать полное |
|----------|---------------------|
| **Новый** справочник (только что сгенерировали resx из MDM) | Значение из MDM Dictionary (то, что записал `generate-module-resx.py`) |
| **Существующий** resx | Текущее значение в resx до сокращения; **повторно из MDM не тянуть** |

### Легаси-исключения (сохранять как есть в short)

| Поле | RU short | Почему |
|------|----------|--------|
| `LoadTime` (leadtime) | Зарузка | Привычное легаси-имя; не заменять на «Погрузка» |

Отличать от `EffectiveLoadOfTransportType` → **Загрузка** (тонны).

---

## Пост-обработка resx (обязательный шаг)

После `generate-module-resx.py` (или при ручной правке):

1. Взять ключи грида из `{Entity}GridSettingsService` (`L["{Entity}Dto.…"]`).
2. Для каждого ключа:
   - если поле есть в [глоссарии](#глоссарий) → подставить RU/EN short в `{Entity}Dto.*`;
   - если нет → сократить по правилам выше, применить и **дописать в глоссарий**. Подпись назвать в отчёте. Вопрос — только если два правила дают разные подписи.
3. Для парного Detail (exact или белый список):
   - `full` = текущее значение Detail (после генерации из MDM — это MDM full);
   - если `short == full` → Detail не менять;
   - иначе Detail = `{short} ({full})`.
4. Detail-only, Groups, Title, мёртвые ключи — пропустить.
5. RU и EN обрабатывать симметрично.

Скрипт генерации пишет **длинные** имена из MDM; короткие имена появляются **только** на этом шаге.

---

## Белый список семантических пар

| Grid key (`*Dto`) | Detail key (`*DetailDto`) | Комментарий |
|-------------------|---------------------------|-------------|
| `ProductGroupName` | `ProductGroupNameEnRu` | ARL3: в detail другое имя свойства |

Не в белом списке: `Name` ↛ `NameRu` / `NameEn`.

При появлении аналогичных расхождений — добавить строку сюда и применить пост-обработку.

---

## Глоссарий

| Семантика | Типичные keys | RU Name | RU Code | EN Name | EN Code |
|-----------|---------------|---------|---------|---------|---------|
| Код записи | `Code` | Код | — | Code | — |
| Узел отправления | `NodeFromName` / `NodeFromCode` | Отправление | Код отпр. | From | From code |
| Узел назначения | `NodeToName` / `NodeToCode` | Назначение | Код назн. | To | To code |
| Промежуточный узел | `ProxyNodeName` / `ProxyNodeCode` | Промежуточный | Код пром. | Proxy | Proxy code |
| Регион отправления | `RegionFromName` / `RegionFromCode` | Рег. отпр. | Код рег. отпр. | Region from | Reg. from |
| Регион назначения | `RegionToName` / `RegionToCode` | Рег. назн. | Код рег. назн. | Region to | Reg. to |
| Промежуточный регион | `ProxyRegionName` / `ProxyRegionCode` | Пром. рег. | Код Пр. рег. | Proxy region | Proxy reg. |
| Продукт | `ProductName` / `ProductCode` | Продукт | Код прод. | Product | Prod. code |
| Группа продуктов | `ProductGroupName` / `ProductGroupCode` | Группа прод. | Код гр. пр. | Group | Group code |
| Вид транспорта | `TransportKindName` / `TransportKindCode` | Вид ТС | Код вида ТС | Transport kind | Tr. kind code |
| Тип транспорта | `TransportTypeName` / `TransportTypeCode` | Тип ТС | Код типа ТС | Transport type | Tr. type code |
| Тип ставки | `RateTypeName` / `RateTypeCode` | Тип ставки | Код типа ст. | Rate type | Rate code |
| Дефлятор | `IsDefRate` | Дефлятор | — | Deflator | — |
| Номинация | `IsUseNomination` | Номинация | — | Nomination | — |
| Дефлируется | `IsDeflated` | Дефлируется | — | Deflated | — |
| Синхронизация | `IsIntoSyncClass` | Синхронизация | — | Sync | — |
| Архив | `IsArchive` | Архив | — | Archive | — |
| Дата создания | `CreationDate` | Создано | — | Created | — |
| Дата изменения | `LastChangeDate` | Изменено | — | Changed | — |
| Дата начала | `StartDate` | Начало | — | Start | — |
| Дата окончания | `EndDate` | Окончание | — | End | — |
| Тип отчёта | `ReportTypeName` | Тип отчёта | — | Report type | — |
| Период действия с | `DateFrom` | Период действия с | — | From | — |
| Период действия по | `DateTo` | Период действия по | — | To | — |
| Валюта | `Currency` / `CurrencyCode` | Валюта | — | Currency | — |
| Ср.взвеш. ставка | `RateLevel3` | Ср. ставка | — | Avg rate | — |
| Эфф. загрузка (т) | `EffectiveLoadOfTransportType` | Загрузка | — | Load | — |
| Стоимость за тонну | `TotalCostTon` | За тонну | — | Per ton | — |
| Стоимость за ТС | `TotalCostTransport` | За ТС | — | Per vehicle | — |
| Действует | `CanBeUsed` | Действует | — | Active | — |
| Код типа отправки | `ShipmentTypeCodeT` | Отправка | — | Shipment | — |
| Время поиска | `SearchTime` | Поиск | — | Search | — |
| Время погрузки | `LoadTime` | Зарузка | — | Load | — |
| Дни ожидания | `DaysWaiting` | Ожидание | — | Waiting | — |
| Время в пути | `TravelTime` | В пути | — | Travel | — |
| Время разгрузки | `UnLoadTime` | Разгрузка | — | Unload | — |
| Время транспортировки | `TransportationTime` | Транспортировка | — | Transportation | — |
| Наименование (grid) | `Name` | Название | — | Name | — |
| Обозначение валюты | `Symbolic` | Обозначение | — | Symbol | — |
| Цифровой код валюты | `Digital` | Цифр. код | — | Num. code | — |
| Код НСИ | `Code_NSI` | Код НСИ | — | NSI code | — |
| Сокращённое наименование | `ShortName` | Краткое имя | — | Short name | — |
| Код транспортной группы | `GroupCode` | Код группы | — | Group code | — |
| Транспортная группа ЦБД | `ProductGroupCBD` | Группа ЦБД | — | CBD group | — |
| Наименование (EN) | `NameEn` (отдельная колонка, не пара к `Name`) | Название (EN) | — | Name (EN) | — |
| Сокращённое наименование (EN) | `NameEnCode` | Краткое имя (EN) | — | Short name (EN) | — |
| Наименование EN-RU | `NameEnRu` | Имя EN-RU | — | Name EN-RU | — |
| Подниматься до группы | `SVTUseProductGroupWhenNoProductRateExists` | До группы | — | Group fallback | — |
| Класс синхронизации ставок | `RateSyncClass` (`RateSyncClassIdRu`/`En`) | Класс синхр. | — | Sync class | — |
| Страна | `CountryName` / `CountryCode` | Страна | Код страны | Country | Country code |
| Регион | `RegionName` / `RegionCode` | Регион | Код региона | Region | Region code |
| Тип узла | `TypeNodeName` / `TypeNodeCode` | Тип узла | Код тип узла | Node type | Type code |
| Тип местоположения | `LocationTypeName` / `LocationTypeCode` | Тип места | Код типа места | Loc. type | Loc. code |
| Актуальность | `RelevanceName` | Актуальность | — | Relevance | — |
| Пром. узел 1/2 | `ProxyNode1Name` / `ProxyNode2Name` (+ Code) | Пром.1 / Пром.2 | Код пром.1 / Код пром.2 | Proxy 1 / Proxy 2 | Proxy 1 code / Proxy 2 code |
| Опасный груз (уровень) | `Level_Danger_Product` | Оп. груз | — | Danger lvl | — |
| Загрузка ТС | `LoadOfTransport` | Загрузка | — | Load | — |
| Примечание | `Comment` | Примечание | — | Comment | — |
| Источник данных | `DataSource` | Источник | — | Source | — |
| Фактическая ставка | `FactRate` | Факт. ставка | — | Fact rate | — |
| Ставка БП | `BusinessPlanningRate` | Ставка БП | — | BP rate | — |
| ЗоО отдел | `DepartmentResponsibilityArea` | ЗоО отдел | — | Dept area | — |
| ЗоО сотрудник | `EmployeeResponsibilityArea` | ЗоО сотр. | — | Emp. area | — |
| Методология | `Methodology` | Методология | — | Methodology | — |
| Приоритет | `PriorityText` | Приоритет | — | Priority | — |
| Класс опасности | `DangerClass` (`DangerClassIdRu`/`En`) | Класс оп-ти | — | Hazard | — |
| Единица измерения | `Unit` (`UnitIdRu`/`En`) | Ед. изм. | — | Unit | — |
| ЕТСНГ (наименование груза, не код) | `EtsngName` | ЕТСНГ | — | ETSNG | — |
| Код ГНГ | `GngCode` | Код ГНГ | — | GNG code | — |
| Основной код DPO | `DpoCode` | Код DPO | — | DPO code | — |
| Вид МТР | `MtrType` | Вид МТР | — | MTR type | — |
| Полное наименование (RU) | `NameFullRu` | Полное (RU) | — | Full (RU) | — |
| Краткое наименование (EN) | `NameShortEn` | Краткое имя (EN) | — | Short (EN) | — |
| Основной тип транспорта для ЦБД | `IsBasic_forCBD` | Осн. ЦБД | — | CBD basic | — |
| Код типа транспорта (3 ур.) | `TypeCode` | Код типа | — | Type code | — |
| Код SAP | `SAPCode` | Код SAP | — | SAP code | — |
| Код Orion | `OrionCode` | Код Orion | — | Orion code | — |
| Статус записи | `Status` (`StatusIdRu`/`En`) | Статус | — | Status | — |
| Наименование для поиска | `NameSearch` | Для поиска | — | Search name | — |
| Код для поиска | `SearchCode` | Код поиска | — | Search code | — |
| Грузополучатель | `Consignee` | Получатель | — | Consignee | — |
| Трейдер | `IsTrader` | Трейдер | — | Trader | — |
| Переработчик | `IsProcessing` | Переработчик | — | Processor | — |
| Грузоотправитель | `IsSender` | Отправитель | — | Shipper | — |
| Дебитор | `IsDebtor` | Дебитор | — | Debtor | — |
| Кредитор | `IsCreditor` | Кредитор | — | Creditor | — |
| ИНН | `INN` | ИНН | — | INN | — |
| Подъём узла назначения до региона | `DestinationNodeToRegion` | До региона | — | To region | — |
| Подъём узла назначения до страны | `DestinationNodeToCountry` | До страны | — | To country | — |
| Подъём продукта до группы продуктов | `ProductToProductGroup` | До группы | — | To group | — |
| Любой тип транспорта | `AnyTransportKind` | Любой вид | — | Any kind | — |
| Код ISO страны | `ISOCode` | Код ISO | — | ISO code | — |
| Код для планирования | `PlanCode` | Код план. | — | Plan code | — |
| Макрорегион | `Market` | Макрорегион | — | Macroregion | — |
| Обозначение страны | `ISOCode3` | Обозначение | — | Designation | — |
| Уровень справочника | `Level` | Уровень | — | Level | — |
| Код вида транспорта | `KindCode` | Код вида | — | Kind code | — |

`RateTypeCode` в grid settings может отсутствовать — short применяют только если ключ реально используется в гриде; иначе ключ считается мёртвым для правки.

`IsDeflated` не путать с `IsDefRate`: у типа ставки признак «дефлируется», у ставки — «дефлятор».

### Согласованный EN, если Dictionary вернул системное имя

Для RateType `LocaleId = 1` совпал с именем атрибута. В resx писать полные имена отсюда, не системные. Short — из таблицы выше. Карточка: `{short} ({full})`, только если они различаются.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| `IsUseNomination` | Используется номинация | Uses nomination | Номинация | Nomination |
| `IsDeflated` | Дефлируется | Deflated | Дефлируется | Deflated |
| `IsIntoSyncClass` | Использовать синхронизацию | Use synchronization | Синхронизация | Sync |
| `Code_NSI` | Код НСИ | NSI code | Код НСИ | NSI code |
| `SVTUseProductGroupWhenNoProductRateExists` | Подниматься до группы если ставка для продукта не найдена | Fall back to group if no product rate found | До группы | Group fallback |
| `RateSyncClass` | Класс синхронизации | Rate synchronization class | Класс синхр. | Sync class |
| `SearchCode` | Код для поиска | Search code | Код поиска | Search code |
| `ProductCode` | Код продукта | Product code | Код продукта | Product code |
| `IsBasic_forCBD` | Основной тип транспорта для ЦБД | Basic transport type for CBD | Осн. ЦБД | CBD basic |
| `Market` | Макрорегион | Macroregion | Макрорегион | Macroregion |

### AverageRateLevel3

Таблицы дочерних примитивов на карточке AverageRateLevel3 — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `RatesOfAverageRate` | Ставки, вошедшие в расчет | Rate For Rate | — | — |
| `Code` | Код | Code | — | — |
| `TransportRateCode` | Код ставки | Rate code | — | — |
| `TotalCostTon` | Общая стоимость за тонну | Total cost of transportation per ton (excluding VAT) | — | — |
| `TotalCostTransport` | Общая стоимость за транспортное средство | Total cost of transportation per vehicle (excluding VAT) | — | — |

### Coefficient

Согласовано для грида и карточки Coefficient. `Code` — как у Contractor: в гриде «Код» / `Code`, на карточке «Код (Код записи)» / `Code`. `Name` и тип ставки — по общему глоссарию выше.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| `CoefficientTypeName` (`CoefficientTypeIdRu`/`En`) | Тип | Coefficient type | Тип | Type |
| `StartDate` | Начало действия | Start date | Начало | Start |
| `FinishDate` | Окончание действия | End date | Окончание | End |
| `Active` | Активный | Active | Активный | Active |
| `OverallValue` | Значение на ставку в целом | Value for the rate as a whole | На ставку | Whole rate |
| `RateTypeName` (`RateTypeIdRu`/`En`) | Тип ставки | Rate type | Тип ставки | Rate type |

Таблицы дочерних примитивов на карточке Coefficient — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `CoefficientConditionValue` | Условие по коэффициенту | Coefficient conditions | — | — |
| группа `CoefficientComponentValue` | Составляющая по коэффициенту | Coefficient components | — | — |
| `Except` | Исключать | Exclude | — | — |
| `TransportKind` | Вид транспорта | Transport kind | — | — |
| `TransportTypeLevel2` | Тип транспорта (2 уровень) | Transport type (level 2) | — | — |
| `VehicleType` | Тип подвижного состава | Rolling stock type | — | — |
| `FromLocation` | Направление от | From | — | — |
| `ToLocation` | Направление до | To | — | — |
| `Component` | Составляющая | Component | — | — |
| `Value` | Значение | Value | — | — |

### Contractor

Согласовано для грида и карточки Contractor. У `Code` английское полное имя — `Code`. У `INN` английское полное уже содержит короткое, поэтому карточка EN — `INN (taxpayer identification number)`. Строки «карточка» короткими именами не заменяются: колонки short у них пустые, в resx пишется полное имя.

| Key | Где | RU полное | EN полное | RU short | EN short |
|-----|-----|-----------|-----------|----------|----------|
| `Code` | грид | Код записи | Code | Код | Code |
| `NameSearch` | грид | Наименование для поиска | Name for search | Для поиска | Search name |
| `Consignee` | грид | Является грузополучателем | Is a consignee | Получатель | Consignee |
| `IsTrader` | грид | Является трейдером | Is a trader | Трейдер | Trader |
| `IsProcessing` | грид | Является переработчиком | Is a processor | Переработчик | Processor |
| `IsSender` | грид | Является грузоотправителем | Is a shipper | Отправитель | Shipper |
| `IsDebtor` | грид | Является Заказчиком\Получателем\Дебитором | Is a customer, recipient, or debtor | Дебитор | Debtor |
| `IsCreditor` | грид | Является Поставщиком\Кредитором | Is a supplier or creditor | Кредитор | Creditor |
| `INN` | грид | ИНН | INN (taxpayer identification number) | ИНН | INN |
| `ShortNameEGRUL` | карточка | Краткое наименование из ЕГРЮЛ | Short name as per the Unified State Register of Legal Entities | — | — |
| `FullNameEGRUL` | карточка | Полное наименование из ЕГРЮЛ | Full name as per the Unified State Register of Legal Entities | — | — |
| `Customs` | карточка | Таможня | Customs | — | — |
| `CustomCode` | карточка | Код таможни | Customs code | — | — |
| `OrgAffiliation` | карточка | Организационная принадлежность | Organisational affiliation | — | — |

Таблицы дочерних примитивов на карточке Contractor — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `Address` | Адреса контрагентов | Addresses of contractors | — | — |
| `Code` | Идентификатор адреса | Address ID | — | — |
| `AddressType` | Тип адреса | Address type | — | — |
| `City` | Город/населенный пункт | City/town | — | — |
| `Status` | Статус адреса | Address status | — | — |
| `AddressKind` | Вид адреса | Address type | — | — |
| `Pobox` | Индекс | Pobox | — | — |
| `RegionCode` | Код региона | Region code | — | — |
| `District_ru` | Район | District | — | — |
| `Street` | Улица | Street | — | — |
| `House` | Дом | House | — | — |
| `OfficeApart` | Офис/квартира | Office/apartment | — | — |
| `FullAddress_ru` | Полный адрес | Full address | — | — |
| `CoordinateW` | Географические координаты широты | Latitude | — | — |
| `CoordinateL` | Географические координаты долготы | Longitude | — | — |
| `FIASCodeRegion` | Код ФИАС региона | FIAS region code | — | — |
| `FIASCodeDistrict` | Код ФИАС района | FIAS district code | — | — |
| `FIASCodeSettlement` | Код ФИАС города/населенного пункта | FIAS settlement code | — | — |
| `Region` | Регион | Region | — | — |
| `Country` | Страна | Country | — | — |
| `IsArchive` | Архив | Archive | — | — |

### RateAutoSelectionSetting

Согласовано для грида и карточки RateAutoSelectionSetting. `Code` и системные поля — из общего глоссария. Русское полное `AnyTransportKind` — «Любой тип транспорта». RU short «До группы» у `ProductToProductGroup` совпадает с `SVTUseProductGroupWhenNoProductRateExists`; EN short другой: To group, не Group fallback.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| `DestinationNodeToRegion` | Подъём узла назначения до региона | Raise destination node to region | До региона | To region |
| `DestinationNodeToCountry` | Подъём узла назначения до страны | Raise destination node to country | До страны | To country |
| `ProductToProductGroup` | Подъём продукта до группы продуктов | Raise product to product group | До группы | To group |
| `AnyTransportKind` | Любой тип транспорта | Any transport kind | Любой вид | Any kind |

### Region

Согласовано для грида и карточки Region. `Code`, `Code_NSI`, `RegionCode` («Код региона» / `Region code`), `CountryName` и системные поля — из общего глоссария. В гриде `NameRu` и `NameEn` — одна колонка по языку интерфейса, ключ `Name` (как у плеч); на карточке — два поля, подписи не из строк `Name` / `NameEn`. `Status` в гриде нет (SVT-5317); на карточке полное имя — «Статус записи» / `Record status`.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| `RegionCodePlanning` | Код региона для планирования | Region code for planning | Код план. | Plan code |
| `RegionCodeRF` | Код региона РФ | Russian region code | Код РФ | RF code |
| `Name` | Регион | Region | Регион | Region |
| `NameRu` | Регион (RU) | Регион (RU) | Регион (RU) | Регион (RU) |
| `NameEn` | Регион (EN) | Регион (EN) | Регион (EN) | Регион (EN) |
| `Market` | Макрорегион | Macroregion | Макрорегион | Macroregion |
| `Status` (`StatusIdRu`/`En`) | Статус записи | Record status | Статус | Status |
| `FederalDistricts` (`FederalDistrictsIdRu`/`En`) | Федеральные округа | Federal district | Фед. округ | Fed. district |

### TransportRoute

Согласовано для грида и карточки TransportRoute. `Code` в гриде — «Код» / `Code`, на карточке RU — «Код (Код СВТ)». `StartNode*` повторяет short `NodeFrom*`, `EndNode*` — `NodeTo*`. `ProxyNode*`, `RegionFrom*`, `RegionTo*` и `RouteTransportKindName` (как `TransportKindName`) — из общего глоссария. `CodeSAP` — тот же short, что `SAPCode`. Detail-only без сокращения: `ShortNameRu`, `ShortNameEN`, `NameEN`, `Leg_01Code`, `Leg_02Code`, `Leg_03Code`.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| `Code` | Код СВТ | Code | Код | Code |
| `CodeSAP` | Код SAP | SAP code | Код SAP | SAP code |
| `Route` | Путь | Route | Путь | Route |
| `IsActive` | Маршрут активен | Route is active | Активен | Active |
| `ChainNodes` | Цепочка маршрута | Route chain | Цепочка | Chain |
| `Leg1_TransportTypeName` (`Leg1_TransportTypeIdRu`/`En`) | Плечо 1.Тип транспорта | Leg 1 transport type | Тип 1 | Type 1 |
| `Leg2_TransportTypeName` (`Leg2_TransportTypeIdRu`/`En`) | Плечо 2.Тип транспорта | Leg 2 transport type | Тип 2 | Type 2 |

Таблицы дочерних примитивов на карточке TransportRoute — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `TransportRouteLeg` | Плечо маршрута | Route leg | — | — |
| `Code` | Код | Code | — | — |
| `Step` | Этап | Stage | — | — |
| `LegCode` | Плечо | Leg | — | — |
| `NodeFrom` | Узел отправления | Departure node | — | — |
| `ProxyNode` | Узел промежуточный | Proxy node | — | — |
| `NodeTo` | Узел назначения | Destination node | — | — |
| `TransportKind` | Вид транспорта | Means of transportation | — | — |
| `Leg1_TransportType` | Плечо 1.Тип транспорта | Leg 1 transport type | — | — |
| `Leg2_TransportType` | Плечо 2.Тип транспорта | Leg 2 transport type | — | — |

### TransportRate

Таблицы дочерних примитивов на карточке TransportRate — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `TransportRateTenderServicePack` | Тендерный пакет услуг | Tender service pack | — | — |
| `Code` | Код | Code | — | — |
| `TenderServicePack` | Тендерный пакет услуг | Tender service pack | — | — |
| `LotRate` | Ставка лота | Lot rate | — | — |
| `LotCurrency` (`LotCurrencyId`) | Валюта лота | Lot currency | — | — |
| группа `TransportRateCoefficient` | Применённые коэффициенты | Applied coefficients | — | — |
| `CoefficientRateCode` | Код ставки с коэфициентом | Rate code with coefficient | — | — |
| `StartDate` | Начало периода | Start of period | — | — |
| `EndDate` | Окончание периода | End of period | — | — |
| `CoefficientName` | Коэффициент | Coefficient | — | — |
| `SourceRateCode` | Исходная ставка | Source rate | — | — |
| `SourceRateKindName` | Вид исходной ставки | Source rate kind | — | — |

### TransportLeg

Таблицы дочерних примитивов на карточке TransportLeg — полные имена, short не задаётся.

| Key | RU полное | EN полное | RU short | EN short |
|-----|-----------|-----------|----------|----------|
| группа `LegRate` | Ставки плеча | Leg rates | — | — |
| `RateCode` | Код ставки | Rate code | — | — |
| `IsDefRate` | Дефлятор | Deflator | — | — |
| `RateType` | Тип ставки | Rate type | — | — |
| `TransportType` | Тип транспорта | Type of vehicle | — | — |
| `ProductGroup` | Группа продуктов | Product group | — | — |
| `Product` | Продукт | Product | — | — |
| `StartDate` | Период действия с | Validity period from | — | — |
| `EndDate` | Период действия по | Validity period till | — | — |
| `TotalCostTon` | Общая стоимость за тонну | Total cost of transportation per ton (excluding VAT) | — | — |
| `TotalCostTransport` | Общая стоимость за транспортное средство | Total cost of transportation per vehicle (excluding VAT) | — | — |
| `Currency` | Валюта-эталон | Reference currency | — | — |
| группа `LegAverageRateLevel3` | Средневзвешенные ставки 3 уровня | Rate Level3 | — | — |
| `AverageRateCode` | Код ставки | Rate code | — | — |
| `IsDefRate` | Дефлятор | Deflator | — | — |
| `RateType` | Тип ставки | Rate type | — | — |
| `TransportType` | Тип транспорта | Type of vehicle | — | — |
| `ProductGroup` | Группа продуктов | Product group | — | — |
| `Product` | Продукт | Product | — | — |
| `StartDate` | Период действия с | Validity period from | — | — |
| `EndDate` | Период действия по | Validity period till | — | — |
| `RateLevel3` | Ставка ср.взвеш. (т) в валюте-эталоне | Avg.rate of level 3 in ref.currency | — | — |
| `EffectiveLoadOfTransportType` | Эффективная загрузка типа транспорта (в тоннах) | Effective loading of the transport vehicle type (in tons) | — | — |
| `Currency` | Валюта-эталон | Reference currency | — | — |
| группа `Leadtime` | Лидтаймы | Leadtime | — | — |
| `Code` | Код Лидтайма | Leadtime code | — | — |
| `StartDate` | Период действия с | Validity period from | — | — |
| `EndDate` | Период действия по | Validity period till | — | — |
| `ShipmentType` | Тип отправки | Shipment type | — | — |
| `LeadTimeIsActive` | Лидтайм действует | Leadtime is active | — | — |
| `CommercialLT` | Коммерческий лидтайм | Commercial leadtime | — | — |
| `SearchTime` | Поиск, сут | Search, days | — | — |
| `LoadTime` | Погрузка, сут | Loading, days | — | — |
| `TravelTime` | В пути, сут | In transit, days | — | — |
| `DaysWaiting` | Ожидание, сут | Waiting, days | — | — |
| `UnLoadTime` | Разгрузка, сут | Unloading, days | — | — |
| `TransportationTime` | Транспортировка, сут | Transportation, days | — | — |
| `Distance` | Расстояние, км | Distance, km | — | — |
| `Leg1_SearchTime` | Плечо 1.Поиск, сут | Leg 1. Search, days | — | — |
| `Leg1_LoadTime` | Плечо 1.Погрузка, сут | Leg 1. Loading, days | — | — |
| `Leg1_TravelTime` | Плечо 1.В пути, сут | Leg 1. In transit, days | — | — |
| `Leg1_DaysWaiting` | Плечо 1.Ожидание, сут | Leg 1. Waiting, days | — | — |
| `Leg1_TransportationTime` | Плечо 1.Транспортировка | Leg 1. Transportation | — | — |
| `Leg1_Distance` | Плечо 1.Расстояние, км | Leg 1. Distance, km | — | — |
| `Leg2_TravelTime` | Плечо 2.В пути, сут | Leg 2. In transit, days | — | — |
| `Leg2_DaysWaiting` | Плечо 2.Ожидание, сут | Leg 2. Waiting, days | — | — |
| `Leg2_UploadTime` | Плечо 2.Разгрузка, сут | Leg 2. Unloading, days | — | — |
| `Leg2_TransportationTime` | Плечо 2.Транспортировка | Leg 2. Transportation | — | — |
| `Leg2_Distance` | Плечо 2.Расстояние, км | Leg 2. Distance, km | — | — |
