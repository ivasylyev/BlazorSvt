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
Легаси-имена в реестре — `Rates` и `TransportLegs`. В коде это модули `TransportRate` и `TransportLeg`, бизнес-ключи `TransportRateId` и `TransportLegId`.  
**Именование:** для **новых** справочников — соглашения ниже.

## Когда применять

- Команда «добавить справочник», «создать модуль для {PrimitiveEntityInfo}»
- Перенос сущности из легаси MDM в BlazorSVT (read-only grid + detail + export)

Перед добавлением сверься с [catalogs-scope.md](../svt-architecture/catalogs-scope.md): справочник должен быть **в скоупе**; домен, видимость меню ЕО и тип работ — из реестра. Сущности из раздела «Вне скоупа», в том числе `TransportRequest` и `MassiveRequest`, этим навыком не переносятся. Этот навык покрывает только **RO** (грид/карточка/sync). Редактор и загрузчик — отдельные этапы (backlog 0.5–0.8). Домен «Заявки» — модуль 0.9, не справочник.

## Шаг 0 — имя справочника

1. Если пользователь не указал имя — **уточни** системное имя легаси (`PrimitiveEntityInfo.Name`).
2. Без имени дальнейшие шаги **не выполнять**.
3. Имя легаси всегда **системное** (например `TransportLeg`, не «Транспортные плечи»).

Два обозначения:

- `{LegacyName}` — `PrimitiveEntityInfo.Name`. Им пользоваться в discovery-SQL, в `pei/{LegacyName}/ai` и в легаси-view `vw_{LegacyName}`.
- `{Entity}` — имя модуля, папки, DTO, снимка `v2.{Entity}_Snapshot` и маршрута. Если в `{LegacyName}` нет `_` и нет строки в таблице ниже, имена совпадают. Если `_` есть, убрать его и поднять первую букву каждого сегмента, остальное не менять: `TransportType_level_3` → `TransportTypeLevel3`, `Priority_Rate` → `PriorityRate`, `Federal_Districts` → `FederalDistricts`, `OrganizationDPO_type` → `OrganizationDPOType`, `MTR_DPO` → `MTRDPO`.

Отдельные переименования (подчёркивания нет):

| Легаси `{LegacyName}` | Модуль `{Entity}` |
|---|---|
| `Rates` | `TransportRate` |
| `TransportLegs` | `TransportLeg` |

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
- `Code`
- `IsArchive` (`CASE WHEN ISNULL(PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END`)
- `CreationDate`, `LastChangeDate`

**Бизнес-флаги** (например `CanBeUsed`) — только если есть в коротком списке (см. [discovery.md](discovery.md) §1).

### Партиционирование (по умолчанию)

Как у `TransportLeg` и `TransportRate`: partition function/scheme по `IsArchive`, PK `(IsArchive, Id)`, SEQUENCE для `Id`.

## Порядок фаз (progressive disclosure)

Читать файл фазы **перед** её выполнением. Не загружать все фазы сразу.

| Фаза | Файл | Содержание |
|------|------|------------|
| 1 | [discovery.md](discovery.md) | MDM discovery, ссылки, IdsEnum, алиасы, resx |
| 2–3 | [sql.md](sql.md) | Structure + Programmability, деплой, верификация БД |
| 4–6 | [csharp.md](csharp.md) | C#-модуль, API, тесты, финальная проверка |

Короткие заголовки грида после генерации resx — [grid-column-glossary.md](grid-column-glossary.md).

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

`TransportType` в этот класс не входит. Enum уже есть: `Platform/Domain/IdsEnum/TransportTypeRu`, `TransportTypeEn`. Собственный RO-грид делается со watermark-sync: `RowVer`, `TransportTypeSyncJob`, `sp_TransportType_PopulateAffectedKeys`, инициализация `v2.SyncState`. Чужие гриды этот справочник в каскад не включают и показывают enum. То, что чужой грид не каскадит на справочник, разовый снимок для его собственного грида не задаёт.

`ShipmentType` в класс не входит: отдельного enum нет. У TransportLeg тип отправки — сырой `ShipmentTypeCodeT`. Справочники «хранит константы» без enum не входят, пока enum не появится.

`RateSyncClass` в этот класс не входит: в реестре это обычный RO со sync (backlog 0.2.3.7). Enum сам по себе класс не задаёт. `TypePlace` вне скоупа (исторический, вместо него TypeNode): свой RO-грид не делать; enum `LocationType*` остаётся для LocationsNodes.

Решение для собственного RO-грида такого справочника (принято на RateType, то же для остальных строк таблицы):

1. Снимок заполняется один раз из `v2.vw_{Entity}_SnapshotSource` (`02.{Entity}_Insert.sql`). `RowVer` на его `PrimitiveEntityData_*` не добавлять. `{Entity}SyncJob` не создавать и не регистрировать. `sp_{Entity}_PopulateAffectedKeys` не создавать. `v2.SyncState` для этого справочника не инициализировать. В каскад чужих справочников не включать: их гриды показывают enum, не живое имя из этого снимка.
2. Объём навыка — только RO-грид и карточка. Редактор и загрузчик остаются своими пунктами бэклога.
3. Перед генерацией сверить неархивные строки `vw_{LegacyName}` (`PrimitiveEntityDataStateId = 1`) с существующим enum: `Id` и состав членов. Расхождение — стоп и вопрос. Enum не перегенерировать.
4. Английский из Dictionary, если это системное имя поля, в resx не копировать. Человеко-читаемый EN — по [grid-column-glossary.md](grid-column-glossary.md); если перевода там нет — согласовать и дописать глоссарий.

Остальной вертикальный срез (Structure 01–03, SnapshotSource, Detail, C#-модуль без `Sync/`, грид, карточка, тесты DTO) — по фазам ниже.

## Чеклист

- [ ] Имена `{LegacyName}` и `{Entity}` получены
- [ ] Короткий и длинный списки + фильтрация неиспользуемых ([discovery.md](discovery.md))
- [ ] Переводы из Dictionary в `Modules/{Entity}/Resources/{Entity}.resx` + `HeaderMenu.{Entity}` в Platform.resx
- [ ] SQL Structure 01–03 + SnapshotSource + Detail + PopulateAffectedKeys (у стабильного словаря с enum PopulateAffectedKeys нет) ([sql.md](sql.md))
- [ ] Все новые/изменённые `.sql` в SqlScripts — UTF-8 with BOM
- [ ] Комментарии «PED_* → справочник» в Insert Sources / PopulateAffectedKeys / SyncJob
- [ ] RowVer (+ индекс) в `Sync/01.Legacy_AddRowVersion.sql` при новой основной таблице (у стабильного словаря с enum не добавлять)
- [ ] SyncState init с `LastRowVersion` / `LastRunUtc` (у стабильного словаря с enum не инициализировать)
- [ ] README + `Publish-AllSql.ps1` обновлены
- [ ] Structure / Programmability задеплоены и верифицированы
- [ ] C# модуль (List/Detail/Sync), DI с `AddCatalogMenu`, resx (у стабильного словаря с enum папки `Sync/` нет) ([csharp.md](csharp.md))
- [ ] `AddCatalogApi` (код, домен, период), `[ApiReference]` на кодах чужих справочников; снимок OpenAPI перегенерирован
- [ ] Общие enum в `Platform/Domain/IdsEnum` (рефакторинг затронутых модулей)
- [ ] Unit: `GridColumnMetadataBuilderTests` + `SnapshotSyncJobContractTests` (второй — только если есть SyncJob)
- [ ] Integration: grid + detail smoke + FTS
- [ ] `dotnet build` / `dotnet test` OK

## Связанные навыки

- **create-programmability** — накат Programmability на локальную `mdm` (`localhost`)
- **publish-all-sql** — публикация скриптов в `C:\publish\v2` (`-Mode All` / `Release` / …)
- **svt-architecture** — архитектурный контекст и антипаттерны
