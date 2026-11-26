---
name: svt-migrate-catalog
description: >-
  Добавление read-only справочника BlazorSVT из легаси MDM: discovery атрибутов,
  snapshot SQL, programmability, C#-модуль, grid, меню и таблицы дочерних примитивов
  на карточке. Используй когда пользователь просит добавить справочник, создать новый
  модуль справочника, перенести PrimitiveEntityInfo из старой системы или добавить
  дочерние примитивы на карточку готового модуля.
---

# Добавление справочника (read-only)

Полный вертикальный срез: MDM discovery → SQL (Structure + Programmability + SnapshotSync) → деплой и верификация БД → C#-модуль.

**Эталон зависит от вида справочника:**

- watermark-sync — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Срок действия и каскад переносить, только если проекция этого справочника их читает. У TransportRate это узлы и продукт; тот же приём каскада — у `Modules/AverageRateLevel3`. Источники чужого job не копировать. Стабильные словари с enum в чужой каскад не включать;
- стабильный словарь с enum, разовый снимок — `Modules/Currency`.

Партиции и грид — как у `TransportLeg` и `LocationsNodes`. Легаси-имена в реестре — `Rates` и `TransportLegs`. В коде это модули `TransportRate` и `TransportLeg`, бизнес-ключи `TransportRateId` и `TransportLegId`.  
**Именование:** для **новых** справочников — соглашения ниже.

## Когда применять

- Команда «добавить справочник», «создать модуль для {PrimitiveEntityInfo}»
- Перенос сущности из легаси MDM в BlazorSVT (read-only grid + detail + export)

Перед добавлением сверься с [catalogs-scope.md](../svt-architecture/catalogs-scope.md): справочник должен быть **в скоупе**; домен, видимость меню ЕО и тип работ — из реестра. Сущности из раздела «Вне скоупа», в том числе `TransportRequest` и `MassiveRequest`, этим навыком не переносятся. Этот навык покрывает только **RO** (грид/карточка/sync). Редактор и загрузчик — отдельные пакеты бэклога (с вехи 10). Домен «Заявки» — вехи 4–6, не справочник.

## Шаг 0 — имя справочника

1. Если пользователь не указал имя — **уточни** имя из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`).
2. Без имени дальнейшие шаги **не выполнять**.
3. Имя всегда **системное** (например `TransportLeg`, не «Транспортные плечи»).

Два обозначения:

- `{Entity}` — имя модуля, папки, DTO, снимка `v2.{Entity}_Snapshot` и маршрута. По умолчанию это имя реестра. Если в нём есть `_` и нет строки в таблице ниже, убрать `_` и поднять первую букву каждого сегмента, остальное не менять: `TransportType_level_3` → `TransportTypeLevel3`, `Priority_Rate` → `PriorityRate`, `Federal_Districts` → `FederalDistricts`, `OrganizationDPO_type` → `OrganizationDPOType`, `MTR_DPO` → `MTRDPO`.
- `{LegacyName}` — имя **основного примитива** (`PrimitiveEntityInfo.Name`). Им пользоваться в discovery-SQL, в `pei/{LegacyName}/ai` и в легаси-view, которая проецирует его таблицу.

Перед discovery сверь реестр и примитив:

```sql
SELECT e.Id AS EntityInfoId, e.Name AS EntityName,
       es.Id AS StructureId, es.ParentId,
       p.Id AS PrimitiveEntityInfoId, p.Name AS PrimitiveName
FROM dbo.EntityInfo e
JOIN dbo.EntityStructure es ON es.EntityInfoId = e.Id
JOIN dbo.PrimitiveEntityInfo p ON p.Id = es.PrimitiveEntityInfoId
WHERE e.Name = N'{RegistryName}';
```

- Id и имя в реестре — `EntityInfo` (URL `masterdata/{id}` и колонка «Имя»). Номер таблицы `PrimitiveEntityData_*` — `PrimitiveEntityInfo.Id` строки с `ParentId IS NULL`.
- Имена совпали — `{LegacyName}` = `{Entity}` (после правила `_` и таблицы переименований).
- Имена разошлись — `{LegacyName}` имя примитива, `{Entity}` имя реестра, если таблица ниже не говорит иное. Пример: реестр `Contractor` (`EntityInfo` 2054), примитив `LegalType` (2028), таблица `PrimitiveEntityData_2028`, представление `vw_LegalType`, словарь `pei/LegalType/ai`.
- Представление — то, которое читает эту таблицу. Одноимённое реестру тонкое представление, которое её не проецирует, устарело (`vw_Contractor`). Замены джойнов — только в SoT (`Modules/**/Programmability`). Файлы уже выкаченной `Migrations/{версия}/` не переписывать: новый `All_Programmability` забирает SoT.
- Строка с `ParentId IS NOT NULL` — дочерний примитив. Он входит в модуль по умолчанию группой-таблицей на карточке; состав, исключения и вопросы — [child-primitives.md](child-primitives.md). Примитив сущности из раздела «Вне скоупа» новой строкой реестра не становится.

Отдельные переименования (подчёркивания нет):

| Легаси `{LegacyName}` | Модуль `{Entity}` |
|---|---|
| `Rates` | `TransportRate` |
| `TransportLegs` | `TransportLeg` |
| `LegalType` | `Contractor` |

Состав экрана конкретного справочника — в его постановке и в комментарии `catalogs-scope.md`, не в этом навыке. Как отбирать атрибуты и чем порядок грида отличается от порядка карточки — [discovery-rules.md](discovery-rules.md) §1 и §4. Запросы этих списков — [discovery-queries.md](discovery-queries.md), их открывать только на время `SELECT`.

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
- `{Entity}Id` (бизнес-ключ, `CAST(Id AS …)` из `vw_{LegacyName}`)
- `Code`. Тип — [discovery-rules.md](discovery-rules.md) §2: `INT` по ответу пользователя, иначе `NVARCHAR`
- `IsArchive` (`CASE WHEN ISNULL(PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END`)
- `CreationDate`, `LastChangeDate`. `LastChangeDate` в снимке — `DATETIME NOT NULL`. Пустое значение легаси заменять на дату создания: `ISNULL(LastChangeDate, CreationDate)`. В DTO тип `DateTime`
- `CreationUserName` («Автор») перед `CreationDate`, `LastChangeUserName` («Автор изм.») перед `LastChangeDate` — `NVARCHAR(255) NULL`. ФИО из `v2.vw_UserDisplayName` по `CreationUserId` и `ISNULL(LastChangeUserId, CreationUserId)` легаси-представления (SVT-5320). В DTO тип `string?`

**Бизнес-флаги** (например `CanBeUsed`) — только если есть в коротком списке (см. [discovery-rules.md](discovery-rules.md) §1).

### Партиционирование (по умолчанию)

Как у `TransportLeg` и `TransportRate`: partition function/scheme по `IsArchive`, PK `(IsArchive, Id)`, SEQUENCE для `Id`.

## Порядок фаз (progressive disclosure)

Читать файл фазы **перед** её выполнением. Не загружать все фазы сразу.

| Фаза | Файл | Содержание |
|------|------|------------|
| 1 | [discovery-rules.md](discovery-rules.md) | Состав экрана, ссылки, enum, каскад. Запросы — [discovery-queries.md](discovery-queries.md), только на время `SELECT` |
| 2–3 | [sql.md](sql.md) | Structure + Programmability, деплой, верификация БД |
| 4–6 | [csharp.md](csharp.md) | C#-модуль, API, тесты, финальная проверка |
| 1–5 | [child-primitives.md](child-primitives.md) | У справочника есть дочерний примитив: группы-таблицы на карточке, в том числе у готового модуля |

Короткие заголовки грида после генерации resx — раздел «Правила» в [grid-column-glossary.md](grid-column-glossary.md) и поиск имени поля. Блоки `###` чужих справочников не читать.

## Стабильный словарь с enum (свой RO-грид)

Класс: справочник почти не меняется, и в коде уже есть enum. Сейчас это:

| Справочник | Enum |
|---|---|
| RateType | `Platform/Domain/IdsEnum/RateTypeRu`, `RateTypeEn` |
| TransportKind | `TransportKindRu`, `TransportKindEn` |
| TransportTypeLevel3 | `TransportTypeLevel3Ru`, `TransportTypeLevel3En` |
| Currency | `CurrencyRuEn` (один файл, без пары Ru/En) |
| Relevance | `RelevanceRu`, `RelevanceEn` |
| TypeNode | `Modules/LocationsNodes/List/IdsEnum/TypeNodeRu`, `TypeNodeEn` |
| RateSyncClass | `Platform/Domain/IdsEnum/RateSyncClassRu`, `RateSyncClassEn` |
| RateCalcType | `Platform/Domain/IdsEnum/RateCalcTypeRu`, `RateCalcTypeEn` |
| CoefficientType | `Platform/Domain/IdsEnum/CoefficientTypeRu`, `CoefficientTypeEn` |

`TransportType` и `ShipmentType` входят в класс «стабильный словарь с enum». У `TransportType` enum уже есть: `TransportTypeRu`, `TransportTypeEn`. Перед заливкой сверить неархивные строки `vw_TransportType` с enum: `Id` и состав членов. Расхождение — стоп и вопрос, enum не перегенерировать. У `ShipmentType` пары нет: `ShipmentTypeRu` / `ShipmentTypeEn` создаются при переносе, имена членов — из `Code`. Оба грида — разовый снимок, без `RowVer`, SyncJob, `PopulateAffectedKeys` и `v2.SyncState`. Чужие гриды эти справочники в каскад не включают. `ShipmentTypeCodeT` у TransportLeg остаётся сырым `NVARCHAR`.

`Federal_Districts`, `DangerClass`, `VehicleType` и `IntegrationDataType` входят в этот класс. У `Federal_Districts` и `DangerClass` enum уже есть. У `VehicleType` и `IntegrationDataType` enum создаётся при переносе. Свой грид — разовый снимок. Расхождение существующего enum с неархивными строками (`Id` и состав членов) — стоп и вопрос, enum не перегенерировать.

`TransportTypeForProduct` в класс не входит и enum не получает. Свой грид — разовый снимок, без `RowVer`, SyncJob, `PopulateAffectedKeys` и `v2.SyncState`.

`PlanFact`, `MTR_DPO`, `Integration`, `FeatureToggle` и `CurrencyRate` в класс не входят: свой грид — watermark-sync, enum нет.

`Basis`, `CoefficientCondition`, `CoefficientComponent`, `RateComponentType` и `RateKind` входят в этот класс. Enum создаётся при переносе RO-грида по `discovery-rules.md` §7: пара `{Entity}Ru` / `{Entity}En` в `Platform/Domain/IdsEnum`, значение члена = `Id` из `vw_{LegacyName}`. Неархивных строк 100 или больше — стоп и вопрос, enum не создавать. После появления файлов строка дописывается в таблицу выше. Дальше шаги RateType: разовый снимок, без `RowVer`, `SyncJob`, `PopulateAffectedKeys` и `v2.SyncState`. `Priority_Rate` в класс не входит.

`RateSyncClass` входит в таблицу выше: enum уже есть, свой грид — разовый снимок, без `RowVer` и `SyncJob`. Enum сам по себе класс не задаёт: в класс справочник попадает решением, не фактом enum. `TypePlace` вне скоупа (исторический, вместо него TypeNode): свой RO-грид не делать; enum `LocationType*` остаётся для LocationsNodes.

Решение для собственного RO-грида такого справочника (принято на RateType, то же для остальных строк таблицы):

1. Снимок заполняется один раз из `v2.vw_{Entity}_SnapshotSource` (`02.{Entity}_Insert.sql`). `RowVer` на его `PrimitiveEntityData_*` не добавлять. `{Entity}SyncJob` не создавать и не регистрировать. `sp_{Entity}_PopulateAffectedKeys` не создавать. `v2.SyncState` для этого справочника не инициализировать. В каскад чужих справочников не включать: их гриды показывают enum, не живое имя из этого снимка.
2. Объём навыка — только RO-грид и карточка. Редактор и загрузчик остаются своими пунктами бэклога.
3. Перед генерацией сверить неархивные строки `vw_{LegacyName}` (`PrimitiveEntityDataStateId = 1`) с существующим enum: `Id` и состав членов. Расхождение — стоп и вопрос. Enum не перегенерировать.
4. Английский из Dictionary, если это системное имя поля, в resx не копировать. Человеко-читаемый EN — по [grid-column-glossary.md](grid-column-glossary.md); если перевода там нет — согласовать и дописать глоссарий.

Остальной вертикальный срез (Structure 01–03, SnapshotSource, Detail, C#-модуль без `Sync/`, грид, карточка, тесты DTO) — по фазам ниже.

## Чеклист

- [ ] Имена `{LegacyName}` и `{Entity}` получены
- [ ] Короткий и длинный списки + фильтрация неиспользуемых ([discovery-rules.md](discovery-rules.md), запросы — [discovery-queries.md](discovery-queries.md))
- [ ] Переводы из Dictionary в `Modules/{Entity}/Resources/{Entity}.resx` + `HeaderMenu.{Entity}` в Platform.resx
- [ ] SQL Structure 01–03 + SnapshotSource + Detail + PopulateAffectedKeys (у стабильного словаря с enum PopulateAffectedKeys нет) ([sql.md](sql.md))
- [ ] Все новые/изменённые `.sql` в SqlScripts — UTF-8 with BOM
- [ ] Комментарии «PED_* → справочник» в Insert Sources / PopulateAffectedKeys / SyncJob
- [ ] RowVer (+ индекс) в `Sync/01.Legacy_AddRowVersion.sql` при новой основной таблице (у стабильного словаря с enum не добавлять)
- [ ] SyncState init с `LastRowVersion` / `LastRunUtc` (у стабильного словаря с enum не инициализировать)
- [ ] `Migrations/README.md` и таблица модуля в `SqlScripts/README.md` обновлены
- [ ] Structure / Programmability задеплоены и верифицированы
- [ ] C# модуль (List/Detail/Sync), DI с `AddCatalogMenu`, resx (у стабильного словаря с enum папки `Sync/` нет) ([csharp.md](csharp.md))
- [ ] `AddCatalogApi` (код, домен, период), `[ApiReference]` на кодах чужих справочников; снимок OpenAPI перегенерирован
- [ ] Общие enum в `Platform/Domain/IdsEnum` (рефакторинг затронутых модулей)
- [ ] Дочерние примитивы: проекции `v2.vw_{Child}_Detail`, секции карточки, тесты секций ([child-primitives.md](child-primitives.md)) — если у справочника есть дети
- [ ] Unit: `GridColumnMetadataBuilderTests` + `SnapshotSyncJobContractTests` (второй — только если есть SyncJob)
- [ ] Integration: grid + detail smoke + FTS
- [ ] `dotnet build` / `dotnet test` OK
- [ ] В `catalogs-scope.md` колонка RO остаётся `[ ]`: `[x]` ставится, когда справочник на проде
- [ ] Пункт backlog не отмечать до ревью

## Связанные навыки

- **svt-create-programmability** — накат Programmability на локальную `mdm` (`localhost`)
- **svt-publish** — пакет версии в `C:\publish\{VersionPrefix}`: приложение, склейка миграции, zip. Локальный накат склейки — режим `local`
- **svt-architecture** — архитектурный контекст и антипаттерны
