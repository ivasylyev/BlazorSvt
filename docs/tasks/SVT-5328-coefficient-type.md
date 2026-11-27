> Рабочая постановка SVT-5328. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник CoefficientType (Тип коэффициента) с дочерними таблицами на карточке

### Проблема

Справочник CoefficientType входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API. Условия и составляющие каждого типа коэффициента (дочерние примитивы) нигде не видны.

### Что меняется в работе

Читатель открывает «Тип коэффициента» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. На карточке после групп атрибутов две таблицы: «Условие по типу коэффициента» и «Составляющая по типу коэффициента».

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения**.

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка CoefficientType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.15**.
- Класс разового снимка — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Дочерние примитивы — `.cursor/skills/svt-migrate-catalog/child-primitives.md`. Эталон — Coefficient (`Modules/Coefficient/Detail`, `SqlScripts/Modules/Coefficient`).
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | CoefficientType, «Тип коэффициента», EntityInfo **2106** |
| Основной примитив | `CoefficientType`, `dbo.PrimitiveEntityData_2077` |
| Дочерние примитивы | `CoefficientTypeCondition` (`PrimitiveEntityData_2079`), `CoefficientTypeComponent` (`PrimitiveEntityData_2084`) |
| Неархивных строк на 2026-11-25 | 2 (корень), 12 (2079), 21 (2084) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |

Имя модуля `{Entity}` = `CoefficientType`, `{LegacyName}` = `CoefficientType`: имена совпали.

### Что сделать

**Модуль**

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/coefficienttype`.
- Разовый снимок: `02.CoefficientType_Insert.sql` из `v2.vw_CoefficientType_SnapshotSource`. Без `RowVer`, `CoefficientTypeSyncJob`, `sp_CoefficientType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`: все коды целые (2 из 2, генератора нет). Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `coefficient-types`, домен `Rates`, ключ `HeaderMenu.CoefficientType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum **уже есть**: `Platform/Domain/IdsEnum/CoefficientTypeRu`, `CoefficientTypeEn`. Не перегенерировать. Перед заливкой сверить неархивные строки `vw_CoefficientType` с enum: `Id` и состав членов. Расхождение — стоп, вопрос.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `CoefficientTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`CoefficientTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.15 backlog дописать номер `SVT-5328`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

**Дочерние таблицы на карточке** (по `child-primitives.md`)

- Группы по порядку `EntityStructure.Id`: «Условие по типу коэффициента» (2079), затем «Составляющая по типу коэффициента» (2084). RU-заголовок — `PrimitiveEntityInfo.Description` ребёнка, EN — запрос «Заголовки» по `{ChildLegacyName}`.
- Проекции `v2.vw_CoefficientTypeCondition_Detail` и `v2.vw_CoefficientTypeComponent_Detail` в `SqlScripts/Modules/CoefficientType/Programmability/`. Источник — `dbo.vw_CoefficientTypeCondition` и `dbo.vw_CoefficientTypeComponent`. Колонки: `CoefficientTypeId` (из `CoefficientTypeId` дочернего view), `Id`, `PrimitiveEntityDataStateId`, колонки таблицы. Отбор по родителю и состоянию — в запросе секции, не во view.
- Колонки — короткий список §1 после фильтра §3 по `{ChildLegacyName}`; `Code` показывается всегда. На 2026-11-25 заполнены: у 2079 — `Condition` (12 из 12); у 2084 — `TransportKind` (20 из 21) и `Component` (21 из 21).
- Ссылки разворачиваются через enum:
  - `Condition` → `CoefficientConditionRu` / `CoefficientConditionEn` (SVT-5329);
  - `Component` → `CoefficientComponentRu` / `CoefficientComponentEn` (SVT-5330);
  - `TransportKind` → nullable `TransportKindRu` / `TransportKindEn` (enum уже есть; в одной строке 2084 ссылка пуста).
  Колонки проекции — пары `{X}IdRu` / `{X}IdEn` по образцу `vw_LegRate_Detail`. Проекции и DTO ссылаются на enum из `Platform/Domain/IdsEnum`, не на классы чужих модулей.
- Дочерние примитивы — **не связки** (`link child`): цели — словари с enum, не справочники с собственной карточкой. Поля цели (`WeightRatio`, `Total`) на карточку не добавлять.
- `Code` ребёнка — `INT` (генератор OrdinalNumber, нецелых 0): `long Code` в DTO строки.
- C#: `Modules/CoefficientType/Detail/CoefficientTypeConditionDto.cs`, `CoefficientTypeConditionSection.cs`, `CoefficientTypeComponentDto.cs`, `CoefficientTypeComponentSection.cs`. Две строки `AddScoped<IDetailChildSection<CoefficientTypeDetailDto>, …>` в `CoefficientTypeModule` в порядке групп. Подписи — в resx модуля. Блок «Таблицы дочерних примитивов на карточке CoefficientType» — в разделе справочника `grid-column-glossary.md`. `CoefficientTypeDetailDto`, `AddCatalogApi` и снимок OpenAPI из-за детей не меняются.
- Unit `tests/BlazorSvt.UnitTests/Modules/CoefficientType/CoefficientTypeChildSectionsTests.cs`: порядок групп по регистрации, состав и порядок колонок, имена свойств колонок без повторов, подписи RU/EN из настоящих resx, пустой `TransportKind` — пустая ячейка. `AddCoefficientTypeModule()` добавить в список `SectionTypes` теста `DetailChildSectionFiltersTests`.
- Integration `tests/BlazorSvt.IntegrationTests/Modules/CoefficientType/CoefficientTypeChildSectionsIntegrationTests.cs`: по каждой проекции на родителе с неархивными детьми строки есть, enum-ссылки определены (`Enum.IsDefined`).

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перегенерация `CoefficientTypeRu` / `CoefficientTypeEn`.
- Перезаливка снимка после будущих правок легаси.
- Переключение модуля Coefficient на новый снимок. Он читает легаси-представление `vw_CoefficientType`.
- Изменение модуля Coefficient и его дочерних секций `CoefficientConditionValue` / `CoefficientComponentValue` (другие примитивы).
- Умный подбор, backlog 9.1.16.
- Дочерние 2079 и 2084 как отдельные справочники скоупа; вложенные дети, API, OpenAPI и полный Excel для детей.
- Поля цели связки на карточке (`WeightRatio`, `Total`).

### Решения

- Разовый снимок: справочник стабильный, enum уже в коде; класс «стабильный словарь с enum» — `svt-migrate-catalog`, `svt-development-patterns`.
- Дочерний примитив входит в модуль группой-таблицей: правило `child-primitives.md`. Ссылки ведут на enum, поэтому это не связки.
- Дочерние секции опираются на enum, которые создают SVT-5329 и SVT-5330. До появления `CoefficientConditionRu/En` и `CoefficientComponentRu/En` в коде эту часть не начинать; корень и грид от них не зависят.
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом (проекции детей входят в него, не дельтой). В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком и `child-primitives.md`; эталон разового снимка — `Modules/RateType`, `Modules/Currency`; эталон детей — Coefficient.
2. Строка CoefficientType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед заливкой сверить неархивные строки `vw_CoefficientType` с enum: `Id` и состав членов.
5. Модуль Coefficient и умный подбор не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- Секции — unit-тест порядка групп, колонок и подписей и integration-тест на 2079 и 2084. Пустой результат проекции или неопределённый enum опровергают разворот ссылок.
- `DetailChildSectionFiltersTests` опровергает тип колонки, для которого фильтр не строится.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI. `OpenApiContractTests` для детей проходит без перегенерации.
- Сверка неархивных строк `vw_CoefficientType` с enum — в заметках, на `localhost` до заливки; unit-тест без базы её не видит.
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: false`, надпись «Показано N из M» при превышении предела.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/CoefficientType/` с гридом, карточкой, меню и `AddCatalogApi` (`coefficient-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/CoefficientType/`: Structure 01–03, SnapshotSource, Detail, проекции детей. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `CoefficientTypeRu`, `CoefficientTypeEn` не перегенерированы; сверка с базой выполнена
- [ ] На карточке есть группы «Условие по типу коэффициента» и «Составляющая по типу коэффициента»; ссылки показаны через enum
- [ ] Unit-тесты (контракт DTO, `CatalogContractCoverageTests`, секции), integration smoke (грид, карточка, FTS) и integration-тест секций зелёные
- [ ] Снимок OpenAPI содержит код `coefficient-types`
- [ ] В пункт 15.10.15 дописан `SVT-5328`. Чекбокс 15.10.15 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` неархивных строк: 2 (корень), 12 (2079), 21 (2084).
- Сверка с enum на 2026-11-25: `vw_CoefficientType` содержит `Id` 9687424 (`Code` 1, `SearchCode` Deflator) и 9687425 (`Code` 2, `SearchCode` Slush); `CoefficientTypeRu` / `CoefficientTypeEn` содержат те же значения. Повторить в момент переноса.
- `Code`: у корня 0 не целых, генератора нет; у обоих детей 0 не целых, генератор OrdinalNumber.
- Объём детей: 2079 — всего 12, неархивных 12, максимум на родителя 8 / 8; 2084 — всего 21, неархивных 21, максимум на родителя 20 / 20. `Grid:ChildSectionRowLimit` (100) не срабатывает.
- Дети не являются основным примитивом другой сущности реестра (проверено по `EntityStructure`).
- Короткий список корня на 2026-11-25: `Code`, `Name`, `SearchCode`, все заполнены в 2 строках. Состав колонок решает discovery в момент переноса.
- Правка CoefficientType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представления: `vw_CoefficientType` — Coefficient. Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327, SVT-5329 … SVT-5332, одна папка релиза.
