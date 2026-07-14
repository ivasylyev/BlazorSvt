# Глоссарий коротких имён колонок грида

Канонический словарь для заголовков `{Entity}Dto.*` / парных `{Entity}DetailDto.*` в модульных resx.  
Используется при добавлении справочника (пост-обработка после генерации resx) и при правках локализации существующих модулей.

Эталоны: `AverageRateLevel3`, `TransportLeg`, `TransportRate`, `LocationsNodes`.

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
| Detail формат | `Короткое (полное)`, **только если** короткое ≠ текущему полному |
| Detail-only | Поля без пары в гриде — **не** сокращать |
| Groups / Title | `{Entity}DetailDto.Group.*`, `{Entity}Grid.Title` — **не** трогать по этому глоссарию |
| Мёртвые ключи | Ключи не из `*GridSettingsService` — **не** трогать |
| Exact pair | По умолчанию `{Entity}Dto.X` ↔ `{Entity}DetailDto.X` |
| Белый список | Доп. семантические пары — см. [Белый список](#белый-список-семантических-пар) |
| `Name` vs `NameRu`/`NameEn` | Grid `Name` сокращать; Detail `NameRu`/`NameEn` **не** авто-парить с `Name` |
| Неизвестное поле | Сократить по правилам выше, **предложить пользователю на approve**, затем внести в этот глоссарий |
| Обогащение | Новое общее поле или approve неизвестного → **дописать строку** в таблицу глоссария |

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
   - если нет → предложить short (правила выше), получить approve, **дописать в глоссарий**, затем применить.
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
| Архив | `IsArchive` | Архив | — | Archive | — |
| Дата создания | `CreationDate` | Создано | — | Created | — |
| Дата изменения | `LastChangeDate` | Изменено | — | Changed | — |
| Дата начала | `StartDate` | Начало | — | Start | — |
| Дата окончания | `EndDate` | Окончание | — | End | — |
| Валюта | `Currency` / `CurrencyCode` | Валюта | — | Currency | — |
| Ср.взвеш. ставка | `RateLevel3` | Ср. ставка | — | Avg rate | — |
| Эфф. загрузка (т) | `EffectiveLoadOfTransportType` | Загрузка | — | Load | — |
| Стоимость за тонну | `TotalCostTon` | За тонну | — | Per ton | — |
| Стоимость за ТС | `TotalCostTransport` | За ТС | — | Per vehicle | — |
| Действует | `CanBeUsed` | Действует | — | Active | — |
| Тип отправки | `ShipmentTypeName` | Тип отправки | — | Shipment | — |
| Время поиска | `SearchTime` | Поиск | — | Search | — |
| Время погрузки | `LoadTime` | Зарузка | — | Load | — |
| Дни ожидания | `DaysWaiting` | Ожидание | — | Waiting | — |
| Время в пути | `TravelTime` | В пути | — | Travel | — |
| Время разгрузки | `UnLoadTime` | Разгрузка | — | Unload | — |
| Время транспортировки | `TransportationTime` | Транспортировка | — | Transportation | — |
| Наименование (grid) | `Name` | Название | — | Name | — |
| Страна | `CountryName` / `CountryCode` | Страна | Код страны | Country | Country code |
| Регион | `RegionName` / `RegionCode` | Регион | Код региона | Region | Region code |
| Тип узла | `TypeNodeName` / `TypeNodeCode` | Тип узла | Код тип узла | Node type | Type code |
| Тип местоположения | `LocationTypeName` / `LocationTypeCode` | Тип места | Код типа места | Loc. type | Loc. code |

`RateTypeCode` в grid settings может отсутствовать — short применяют только если ключ реально используется в гриде; иначе ключ считается мёртвым для правки.
