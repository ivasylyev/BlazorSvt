> Рабочая постановка SVT-5338. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник PlanFact (План-факт)

### Проблема

Справочник PlanFact входит в скоуп СВТ 2.0 (домен «Продукты»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «План-факт» из меню «Продукты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Данные приходят по интеграции, поэтому снимок грида обновляется watermark-sync, а не заливается один раз.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Продукты», строка PlanFact.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.33**.
- Эталон каркаса watermark-sync — `.cursor/skills/svt-migrate-catalog/SKILL.md`, начало; фазы SQL и C# — `sql.md`, `csharp.md`.
- В класс «стабильный словарь с enum» справочник не входит (`svt-migrate-catalog`, раздел «Стабильный словарь с enum»).
- Тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | PlanFact, «План-факт», EntityInfo **2157** |
| Основной примитив | `PlanFact`, `dbo.PrimitiveEntityData_2138` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 3753 (всего 3753) |
| Сложность / приоритет | Низкая, пр. RO = 2 |
| Меню ЕО | да |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «План-факт» |
| Комментарий реестра | «В меню Confluence — «Группа продуктов (План-факт)». Редактор и загрузчик не нужны» |

Имя модуля `{Entity}` = `PlanFact`, `{LegacyName}` = `PlanFact`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Products` с `VisibleToEditor: true`. Маршрут `/planfact`. Заголовок грида и пункт меню — «План-факт»; фраза реестра про Confluence — заметка, не заголовок.
- Watermark-sync: снимок, `PlanFactSyncJob`, `RowVer`, `sp_PlanFact_PopulateAffectedKeys`, инициализация `v2.SyncState`. Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Enum не создавать.
- `RowVer` на `dbo.PrimitiveEntityData_2138` (PlanFact) — в `SqlScripts/Sync/01.Legacy_AddRowVersion.sql`, maintenance-окно. На 2026-11-25 в этой таблице `RowVer` нет. NC-индекс по `RowVer` — по размеру таблицы, решает перенос.
- Sources job — основная таблица `PrimitiveEntityData_2138`, с комментарием «PlanFact». Чужой справочник добавлять источником, только если проекция его читает и правило «Каскад» (`discovery-rules.md` §6) это велит. Стабильные словари с enum в каскад не входят.
- `Code` — `INT`. Генератор в легаси Manual, по факту все коды целые (3753 из 3753); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `plan-facts`, домен `Products`, ключ `HeaderMenu.PlanFact`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`, `TableName` = `v2.PlanFact_Snapshot`, ключ `PlanFactId`). `PlanFactSyncJob` добавить в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). `PlanFactDto` в `stableWithoutSyncJob` не добавлять.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`PlanFactFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.33 backlog дописать номер `SVT-5338`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Enum PlanFact: справочник большой, enum не создаётся.
- Строка PlanFact в таблице класса «стабильный словарь с enum» `svt-migrate-catalog`.
- Изменение модулей, которые читают другие справочники.

### Решения

- Watermark-sync: справочник приходит по интеграции (`catalogs-scope.md`), в класс «стабильный словарь с enum» не входит (`svt-migrate-catalog`).
- `Code` — `INT` по решению пользователя, несмотря на генератор Manual (`discovery-rules.md` §2). В watermark-sync нецелый код даст `NULL` в проекции, и синхронизация строки упадёт на `NOT NULL`.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Разовый снимок `Modules/RateType` и `Modules/Currency` не копировать.
2. Строка PlanFact в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Данные для решения о каскаде (если проекция читает чужой длинный словарь) — `MAX(LastChangeDate)`, изменения за 3 года, fan-out, наличие `RowVer`, пишет ли `stg.Load*` эту колонку.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `SnapshotSyncJobContractTests` и `CatalogContractCoverageTests` опровергают «job зарегистрирован» и состав Sources.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует; первый и следующий циклы sync на `localhost` проходят без ошибки.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/PlanFact/` с гридом, карточкой, меню, `PlanFactSyncJob` и `AddCatalogApi` (`plan-facts`)
- [ ] SQL `BlazorSvt/SqlScripts/Modules/PlanFact/`: Structure 01–03, SnapshotSource, Detail, PopulateAffectedKeys; `RowVer` в `Sync/01.Legacy_AddRowVersion.sql`; `v2.SyncState` инициализирован
- [ ] `PlanFactSyncJob` зарегистрирован, снимок заполнен. `Code` — `INT`
- [ ] Enum не создан; строка в таблицу класса «стабильный словарь с enum» не добавлена
- [ ] Unit-тест контракта DTO, `SnapshotSyncJobContractTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `plan-facts`
- [ ] В пункт 15.10.33 дописан `SVT-5338`. Чекбокс 15.10.33 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2138` 3753 неархивных строки из 3753.
- `Code`: `0` не целых из 3753, генератор Manual, длина до 5 знаков.
- Короткий список на 2026-11-25 (число — неархивные заполненные строки): `Code` (3753), `NSICode` (3753), `NameRu` (3753), `NameEn` (3684), `DirectionName` (19), `PlanFactLevel` (3753), `DepartmentNSI` (1796, `bigint`), `ParentNameRu` (3740), `ParentNSICode` (3740). У `DirectionName`, `PlanFactLevel` и `DepartmentNSI` одинаковый ранг 40, у `ParentNameRu` и `ParentNSICode` — 50: порядок внутри ранга discovery не задаёт. Состав и порядок колонок решает discovery в момент переноса.
- `DepartmentNSI` по совпадению значений с `Id` на 2026-11-25 — ссылка на DepartmentNSI (1796 из 1796; в реестре в скоупе, `catalogs-scope.md`). DepartmentNSI — длинный словарь, поэтому разворот и решение о каскаде — `discovery-rules.md` §6 в момент переноса.
- `RowVer` в `PrimitiveEntityData_2138` на 2026-11-25 нет.
- Родственные постановки: SVT-5327 … SVT-5337, SVT-5339 … SVT-5345, одна папка релиза.
