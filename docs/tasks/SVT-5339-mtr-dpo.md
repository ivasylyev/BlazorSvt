> Рабочая постановка SVT-5339. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник MTR_DPO (Материалы DPO) с дочерней таблицей на карточке

### Проблема

Справочник MTR_DPO входит в скоуп СВТ 2.0 (домен «Продукты»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API. Справочные материалы DPO (дочерний примитив) нигде не видны.

### Что меняется в работе

Читатель открывает «Материалы DPO» из меню «Продукты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. На карточке после групп атрибутов появляется таблица «Справочные материалы DPO». Данные приходят по интеграции, поэтому снимок грида обновляется watermark-sync.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Продукты», строка MTR_DPO.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.34**.
- Эталон каркаса watermark-sync — `.cursor/skills/svt-migrate-catalog/SKILL.md`, начало; фазы SQL и C# — `sql.md`, `csharp.md`.
- Имя модуля из реестра с `_` — `.cursor/skills/svt-migrate-catalog/SKILL.md`, шаг 0.
- Дочерние примитивы — `.cursor/skills/svt-migrate-catalog/child-primitives.md`. Эталон — Coefficient (`Modules/Coefficient/Detail`, `SqlScripts/Modules/Coefficient`).
- В класс «стабильный словарь с enum» справочник не входит.
- Тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | MTR_DPO, «Материалы DPO», EntityInfo **2101** |
| Основной примитив | `MTR_DPO`, `dbo.PrimitiveEntityData_2071` |
| Дочерний примитив | `MTR_DPO_Directory` (`PrimitiveEntityData_2072`), «Справочные материалы DPO»; `EntityStructure.Id` 2123, `RelationshipType` 1 |
| Неархивных строк на 2026-11-25 | 2615 (корень, всего 4221), 1107 (2072, всего 1107) |
| Сложность / приоритет | Средняя, пр. RO = 2 |
| Меню ЕО | да |
| Редактор, загрузчик | нет (в реестре «—»): справочник приходит по интеграции |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Материалы DPO» |
| Комментарий реестра | «Приходит по интеграции, редактор не нужен» |

Имя модуля `{Entity}` = `MTRDPO` (`_` убирается, сегменты с заглавной), `{LegacyName}` = `MTR_DPO`: имена разошлись только подчёркиванием. Это не модуль `MTR`.

### Что сделать

**Модуль**

- Модуль read-only `MTRDPO`: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Products` с `VisibleToEditor: true`. Маршрут `/mtrdpo`.
- Watermark-sync: снимок, `MTRDPOSyncJob`, `RowVer`, `sp_MTRDPO_PopulateAffectedKeys`, инициализация `v2.SyncState`. Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Enum не создавать. Дети в снимок и sync не входят.
- `RowVer` на `dbo.PrimitiveEntityData_2071` (MTR_DPO) — в `SqlScripts/Sync/01.Legacy_AddRowVersion.sql`, maintenance-окно. На 2026-11-25 `RowVer` в этой таблице нет. NC-индекс по `RowVer` — по размеру таблицы, решает перенос.
- Sources job — основная таблица `PrimitiveEntityData_2071`, с комментарием «MTR_DPO». Чужой справочник добавлять источником, только если проекция его читает и правило «Каскад» (`discovery-rules.md` §6) это велит. Стабильные словари с enum в каскад не входят.
- `Code` — `INT`. Генератора в легаси нет, все коды целые (4221 из 4221); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: домен `Products`, ключ `HeaderMenu.MTRDPO`. **Код API не зафиксирован:** пара неочевидна (имя интерфейса `MTR` даёт код `products`, `csharp.md`). Спросить пользователя при переносе и сверить с уже зарегистрированными кодами. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`, `TableName` = `v2.MTRDPO_Snapshot`, ключ `MTRDPOId`). `MTRDPOSyncJob` добавить в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). `MTRDPODto` в `stableWithoutSyncJob` не добавлять.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`MTRDPOFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.34 backlog дописать номер `SVT-5339`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

**Дочерняя таблица на карточке** (по `child-primitives.md`)

- Группа «Справочные материалы DPO» (2072). RU-заголовок — `PrimitiveEntityInfo.Description` ребёнка, EN — запрос «Заголовки» по `MTR_DPO_Directory`.
- Проекция `v2.vw_MTRDPODirectory_Detail` в `SqlScripts/Modules/MTRDPO/Programmability/`. Имя ребёнка `{Child}` = `MTRDPODirectory` (`_` убирается). Источник — `dbo.vw_MTR_DPO_Directory`. Колонки: `MTRDPOId` (`CAST(MTR_DPOId AS BIGINT)` из колонки родителя во view ребёнка), `Id`, `PrimitiveEntityDataStateId`, колонки таблицы. Отбор по родителю и состоянию — в запросе секции, не во view.
- Колонки — короткий список §1 после фильтра §3 по `MTR_DPO_Directory`; `Code` показывается всегда. На 2026-11-25 заполнены: `Code` (1107 из 1107), `CodeDPO` (1107), `MTRCode` (1098), `IsObjective` (1107).
- Ссылки разворачиваются в SQL-проекции по `child-primitives.md` п.5. Цели по совпадению значений с `Id` (см. «Заметки»): `MTRCode` — MTR, `CodeDPO` — MTR_DPO. Enum у них нет, значит имя.
- `Code` ребёнка — `INT` (генератора нет, нецелых 0): `long Code` в DTO строки.
- C#: `Modules/MTRDPO/Detail/MTRDPODirectoryDto.cs`, `MTRDPODirectorySection.cs`. Строка `AddScoped<IDetailChildSection<MTRDPODetailDto>, MTRDPODirectorySection>` в `MTRDPOModule`. Подписи — в resx модуля. Блок «Таблицы дочерних примитивов на карточке MTRDPO» — в разделе справочника `grid-column-glossary.md`. `MTRDPODetailDto`, `AddCatalogApi` и снимок OpenAPI из-за ребёнка не меняются.
- Unit `tests/BlazorSvt.UnitTests/Modules/MTRDPO/MTRDPOChildSectionsTests.cs` по `child-primitives.md`, раздел «Тесты». `AddMTRDPOModule()` добавить в список `SectionTypes` теста `DetailChildSectionFiltersTests`.
- Integration `tests/BlazorSvt.IntegrationTests/Modules/MTRDPO/MTRDPOChildSectionsIntegrationTests.cs`: на родителе с неархивными детьми строки есть, ссылки развёрнуты.
- **Открыто: связка или нет.** У ребёнка есть ссылка на справочник со своей карточкой (MTR), остальные поля — код и признак `IsObjective`. По `child-primitives.md` п.6 это кандидат в связку и вопрос стадии 3 при переносе. Пока ответа нет, ребёнок — не связка: ссылки показываются только именем или кодом, поля цели на карточку не добавлять.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Enum MTR_DPO.
- Модуль `MTR`: он остаётся своим справочником, его снимок и sync не меняются.
- `MTR_DPO_Directory` как отдельный справочник скоупа: он не основной примитив ни одной сущности реестра (проверено по `EntityStructure`).
- Поля цели связки на карточке без отдельного согласования; вложенные дети, API, OpenAPI и полный Excel для детей.

### Решения

- Watermark-sync: справочник приходит по интеграции (`catalogs-scope.md`), в класс «стабильный словарь с enum» не входит (`svt-migrate-catalog`).
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`). В watermark-sync нецелый код даст `NULL` в проекции, и синхронизация строки упадёт на `NOT NULL`.
- Дочерний примитив входит в модуль группой-таблицей: правило `child-primitives.md` п.1 (view есть, неархивных строк 1107, ребёнок не вложенный). Дети в snapshot и sync не попадают; карточка читает легаси-представление.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом (проекция ребёнка входит в него, не дельтой). В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком и `child-primitives.md`; эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`; эталон детей — Coefficient. Разовый снимок `Modules/RateType` и `Modules/Currency` не копировать.
2. Строка MTR_DPO в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Данные для решения о каскаде (если проекция читает чужой длинный словарь) — `MAX(LastChangeDate)`, изменения за 3 года, fan-out, наличие `RowVer`, пишет ли `stg.Load*` эту колонку.
5. Модуль MTR не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `SnapshotSyncJobContractTests` и `CatalogContractCoverageTests` опровергают «job зарегистрирован» и состав Sources.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- Секция — unit-тест порядка групп, колонок и подписей и integration-тест на 2072. Пустой результат проекции опровергает разворот ссылок.
- `DetailChildSectionFiltersTests` опровергает тип колонки, для которого фильтр не строится.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI. `OpenApiContractTests` для ребёнка проходит без перегенерации.
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует; надпись «Показано N из M» при превышении предела; циклы sync на `localhost` проходят без ошибки.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/MTRDPO/` с гридом, карточкой, меню, `MTRDPOSyncJob` и `AddCatalogApi`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/MTRDPO/`: Structure 01–03, SnapshotSource, Detail, PopulateAffectedKeys, проекция ребёнка; `RowVer` в `Sync/01.Legacy_AddRowVersion.sql`; `v2.SyncState` инициализирован
- [ ] `MTRDPOSyncJob` зарегистрирован, снимок заполнен. `Code` — `INT`
- [ ] Код API согласован с пользователем и сверен с зарегистрированными
- [ ] Enum не создан
- [ ] На карточке есть группа «Справочные материалы DPO»; ссылки показаны именем или кодом
- [ ] Unit-тесты (контракт DTO, `SnapshotSyncJobContractTests`, секция), integration smoke (грид, карточка, FTS) и integration-тест секции зелёные
- [ ] Снимок OpenAPI содержит согласованный код
- [ ] В пункт 15.10.34 дописан `SVT-5339`. Чекбокс 15.10.34 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` неархивных строк: 2615 из 4221 (корень, `PrimitiveEntityData_2071`), 1107 из 1107 (2072).
- `Code`: у корня `0` не целых из 4221, генератора нет, длина до 6 знаков; у ребёнка `0` не целых из 1107, генератора нет.
- Короткий список корня на 2026-11-25 (число — неархивные заполненные строки, у всех атрибутов ранг 0, кроме `Mtrasvnsi1…5` с рангами 10–18, поэтому порядок по рангу не задан): `Code` (2615), `Name` (2615), `CodeDPO` (2615), `Is_main` (2615), `Link_Maindpo` (1327, `bigint`), `Unit` (2615, `bigint`), `Is_Production` (19), `Is_Rawmaterial` (2615), `Is_Prepack` (2615), `Status` (0), `ObjectiveMTR_TransportGroup_Code` (1282, `bigint`), `ETSNG_Code` (1288, `bigint`), `МТРcodeESO` (1288), `TransportGroup_CodeDPO` (1282, `bigint`), `ETSNG_Code1` (2615, `bigint`), `CodeTG`, `Mtrasvnsi1` (1288), `Mtrasvnsi2` (522), `Mtrasvnsi3` (313), `Mtrasvnsi4` (214), `Mtrasvnsi5` (141). `Status` с нулём заполненных в грид и карточку не входит. Состав и порядок колонок решает discovery в момент переноса.
- Блокеры discovery на 2026-11-25, решаются в момент переноса остановкой и вопросом пользователю: `CodeTG` в коротком списке (`AttributeType` 9, вычисляемый по `JsonPath`), но колонки во `dbo.vw_MTR_DPO` нет, счётчик §3 по view посчитать нельзя; системное имя `МТРcodeESO` начинается с кириллических букв `МТР` — имя колонки снимка и свойства DTO не определено.
- Ссылочные кандидаты (`bigint`) по совпадению значений с `Id` на 2026-11-25: `Link_Maindpo` — MTR_DPO (1327 из 1327), `Unit` — Unit (2615 из 2615; в реестре вне скоупа, enum `UnitRu` / `UnitEn` есть в Platform), `ETSNG_Code`, `ETSNG_Code1` — ETSNG (в реестре вне скоупа), `ObjectiveMTR_TransportGroup_Code`, `TransportGroup_CodeDPO` — ProductGroup (по 1282 из 1282). Разворот и решение о каскаде — `discovery-rules.md` §6 в момент переноса.
- У ребёнка `MTRCode` (`bigint`, 1098) — MTR (1098 из 1098), `CodeDPO` (`bigint`, 1107) — MTR_DPO (1107 из 1107).
- Объём ребёнка: 2072 — всего 1107, неархивных 1107, максимум на родителя 1 / 1. `Grid:ChildSectionRowLimit` (100) не срабатывает.
- `RowVer` в `PrimitiveEntityData_2071` на 2026-11-25 нет.
- Кто уже читает легаси-представления: `vw_MTR_DPO` и `vw_MTR_DPO_Directory` в `SqlScripts/Modules` не читает ни один модуль (поиск на 2026-11-25).
- Родственные постановки: SVT-5327 … SVT-5338, SVT-5340 … SVT-5345, одна папка релиза.
