> Рабочая постановка SVT-5337. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник VehicleType (Тип подвижного состава) с дочерней таблицей на карточке

### Проблема

Справочник VehicleType входит в скоуп СВТ 2.0 (домен «Маршруты»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API. Продукты по типам подвижного состава (дочерний примитив) нигде не видны.

### Что меняется в работе

Читатель открывает «Тип подвижного состава» из меню «Маршруты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. На карточке после групп атрибутов появляется таблица «Продукты по типам подвижного состава». Для функционала, который будет читать справочник, в коде появляется пара enum `VehicleTypeRu` / `VehicleTypeEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Маршруты», строка VehicleType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.32**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7.
- Дочерние примитивы — `.cursor/skills/svt-migrate-catalog/child-primitives.md`. Эталон — Coefficient (`Modules/Coefficient/Detail`, `SqlScripts/Modules/Coefficient`).
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | VehicleType, «Тип подвижного состава», EntityInfo **2124** |
| Основной примитив | `VehicleType`, `dbo.PrimitiveEntityData_2103` |
| Дочерний примитив | `VehicleTypeProduct` (`PrimitiveEntityData_2104`), «Продукты по типам подвижного состава»; `EntityStructure.Id` 2155, `RelationshipType` 1 |
| Неархивных строк на 2026-11-25 | 5 (корень, всего 5), 34 (2104, всего 34) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип подвижного состава» |
| Комментарий реестра | «Вспомогательный для ставок: подбор дефляторов. Перенесён в домен Маршруты из Ставок. В меню ЕО не показываем» |

Имя модуля `{Entity}` = `VehicleType`, `{LegacyName}` = `VehicleType`: имена совпали.

### Что сделать

**Модуль**

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Routes` с `VisibleToEditor: false`. Маршрут `/vehicletype`.
- Разовый снимок: `02.VehicleType_Insert.sql` из `v2.vw_VehicleType_SnapshotSource`. Без `RowVer`, `VehicleTypeSyncJob`, `sp_VehicleType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси OrdinalNumber, все коды целые (5 из 5); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `vehicle-types`, домен `Routes`, ключ `HeaderMenu.VehicleType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `VehicleTypeRu` / `VehicleTypeEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_VehicleType`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку VehicleType в таблицу класса в `svt-migrate-catalog/SKILL.md`. **Имена членов и английский `Display` не определены:** на 2026-11-25 цепочка `discovery-rules.md` §7 пуста (см. «Заметки»). Это стоп и вопрос пользователю до генерации (§7 п.8); имена не придумывать.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `VehicleTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`VehicleTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.32 backlog дописать номер `SVT-5337`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

**Дочерняя таблица на карточке** (по `child-primitives.md`)

- Группа «Продукты по типам подвижного состава» (2104). RU-заголовок — `PrimitiveEntityInfo.Description` ребёнка, EN — запрос «Заголовки» по `VehicleTypeProduct`.
- Проекция `v2.vw_VehicleTypeProduct_Detail` в `SqlScripts/Modules/VehicleType/Programmability/`. Источник — `dbo.vw_VehicleTypeProduct`. Колонки: `VehicleTypeId` (из `VehicleTypeId` дочернего view), `Id`, `PrimitiveEntityDataStateId`, колонки таблицы. Отбор по родителю и состоянию — в запросе секции, не во view.
- Колонки — короткий список §1 после фильтра §3 по `VehicleTypeProduct`; `Code` показывается всегда. На 2026-11-25 заполнены `Code` (34 из 34) и `ProductGroup` (34 из 34); `Product` (0 из 34) в таблицу не входит.
- Ссылка `ProductGroup` разворачивается в SQL-проекции по `child-primitives.md` п.5: ProductGroup — справочник со своим модулем, enum у него нет, значит имя.
- `Code` ребёнка — `INT` (генератор OrdinalNumber, нецелых 0): `long Code` в DTO строки.
- C#: `Modules/VehicleType/Detail/VehicleTypeProductDto.cs`, `VehicleTypeProductSection.cs`. Строка `AddScoped<IDetailChildSection<VehicleTypeDetailDto>, VehicleTypeProductSection>` в `VehicleTypeModule`. Подписи — в resx модуля. Блок «Таблицы дочерних примитивов на карточке VehicleType» — в разделе справочника `grid-column-glossary.md`. `VehicleTypeDetailDto`, `AddCatalogApi` и снимок OpenAPI из-за ребёнка не меняются.
- Unit `tests/BlazorSvt.UnitTests/Modules/VehicleType/VehicleTypeChildSectionsTests.cs` по `child-primitives.md`, раздел «Тесты». `AddVehicleTypeModule()` добавить в список `SectionTypes` теста `DetailChildSectionFiltersTests`.
- Integration `tests/BlazorSvt.IntegrationTests/Modules/VehicleType/VehicleTypeChildSectionsIntegrationTests.cs`: на родителе с неархивными детьми строки есть, ссылка развёрнута в имя.
- **Открыто: связка или нет.** У ребёнка, кроме `Code`, одна заполненная ссылка на справочник со своей карточкой (ProductGroup). По `child-primitives.md` п.6 это кандидат в связку и вопрос стадии 3 при переносе. Пока ответа нет, ребёнок — не связка: ссылка показывается только именем, поля цели на карточку не добавлять.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Поля цели связки на карточке без отдельного согласования; вложенные дети, API, OpenAPI и полный Excel для детей.
- `VehicleTypeProduct` как отдельный справочник скоупа: он не основной примитив ни одной сущности реестра (проверено по `EntityStructure`).
- Переключение join в `vw_CoefficientConditionValue_Detail` на `dbo.vw_VehicleType`. Он остаётся как есть; карточка Coefficient по-прежнему показывает имя из легаси.
- Поздний функционал, который будет читать `VehicleTypeRu` / `VehicleTypeEn`; подбор дефляторов.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал.
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`).
- Дочерний примитив входит в модуль группой-таблицей: правило `child-primitives.md` п.1 (view есть, неархивных строк 34, ребёнок не вложенный).
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом (проекция ребёнка входит в него, не дельтой). В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком и `child-primitives.md`; эталон разового снимка — `Modules/RateType`, `Modules/Currency`; эталон детей — Coefficient.
2. Строка VehicleType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_VehicleType`: порог 100, коллизии имён, идентификаторы. Имена членов — после ответа на вопрос.
5. Модуль Coefficient не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- Секция — unit-тест порядка групп, колонок и подписей и integration-тест на 2104. Пустой результат проекции опровергает разворот ссылки.
- `DetailChildSectionFiltersTests` опровергает тип колонки, для которого фильтр не строится.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI. `OpenApiContractTests` для ребёнка проходит без перегенерации.
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_VehicleType` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: false`, надпись «Показано N из M» при превышении предела.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/VehicleType/` с гридом, карточкой, меню и `AddCatalogApi` (`vehicle-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/VehicleType/`: Structure 01–03, SnapshotSource, Detail, проекция ребёнка. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `VehicleTypeRu`, `VehicleTypeEn` в `Platform/Domain/IdsEnum`; строка VehicleType в таблице класса `svt-migrate-catalog`; вопрос об именах членов закрыт
- [ ] На карточке есть группа «Продукты по типам подвижного состава»; ссылка показана именем
- [ ] Unit-тесты (контракт DTO, `CatalogContractCoverageTests`, секция), integration smoke (грид, карточка, FTS) и integration-тест секции зелёные
- [ ] Снимок OpenAPI содержит код `vehicle-types`
- [ ] В пункт 15.10.32 дописан `SVT-5337`. Чекбокс 15.10.32 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` неархивных строк: 5 из 5 (корень, `PrimitiveEntityData_2103`), 34 из 34 (2104). Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: у корня `0` не целых из 5 (OrdinalNumber); у ребёнка `0` не целых из 34 (OrdinalNumber).
- Цепочка имён членов на 2026-11-25: `Code` — числа `1`…`5`, не годится; колонки `SearchCode` во `vw_VehicleType` нет; `Name` («Газовый ВЦ», «Химический или нефтебензиновый ВЦ», «Крытый вагон», «Газовый ТК», «Химический или нефтебензиновый ТК») — кириллица с пробелами, не идентификатор; колонок `NameEn`, `NameRu` и остальных звеньев п.3 нет. Источника английского `Display` тоже нет. Ru `Display` — `Name`.
- Строки корня: `Id` 10260038 … 10260042 (`Code` 1…5).
- `TransportTypeLevel2` (`bigint`, 5 из 5): по совпадению значений с `Id` на 2026-11-25 — ссылка на TransportType (5 из 5); с TransportKind и TransportTypeLevel3 совпадений нет. Enum `TransportTypeRu` / `TransportTypeEn` есть в Platform. Цель по имени колонки не определяется — подтвердить по discovery §6 в момент переноса.
- Короткий список корня на 2026-11-25: `Code`, `Name`, `TransportTypeLevel2`, все заполнены в 5 строках. Состав колонок решает discovery в момент переноса.
- Объём ребёнка: 2104 — всего 34, неархивных 34, максимум на родителя 14 / 14. `Grid:ChildSectionRowLimit` (100) не срабатывает.
- Ребёнок не основной примитив другой сущности реестра и не вложенный (проверено по `EntityStructure`).
- Правка VehicleType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_VehicleType`: Coefficient (`vw_CoefficientConditionValue_Detail`). Модуль в задаче не переключать.
- Родственные постановки: SVT-5327 … SVT-5336, SVT-5338 … SVT-5345, одна папка релиза.
