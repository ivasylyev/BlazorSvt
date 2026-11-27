> Рабочая постановка SVT-5342. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник Integration (Интеграция)

### Проблема

Справочник Integration входит в скоуп СВТ 2.0 (домен «Вспомогательные»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Интеграция» из меню «Вспомогательные», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Снимок грида обновляется watermark-sync.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API). Integration — справочник в реестре, не модуль платформы.

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Вспомогательные», строка Integration.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.57**.
- Эталон каркаса watermark-sync — `.cursor/skills/svt-migrate-catalog/SKILL.md`, начало; фазы SQL и C# — `sql.md`, `csharp.md`.
- В класс «стабильный словарь с enum» справочник не входит.
- Тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | Integration, «Интеграция», EntityInfo **2112** |
| Основной примитив | `Integration`, `dbo.PrimitiveEntityData_2087` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 12789 (всего 12789) |
| Сложность / приоритет | Средняя, пр. RO = 1 |
| Меню ЕО | да |
| Редактор, загрузчик | редактор 36.5.1 (в реестре есть, в эту задачу не входит); загрузчика нет |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Интеграция» |
| Комментарий реестра | «Справочник (не отдельный инфрамиодуль). Загрузчика нет» |

Имя модуля `{Entity}` = `Integration`, `{LegacyName}` = `Integration`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Other` с `VisibleToEditor: true`. Маршрут `/integration`.
- Watermark-sync: снимок, `IntegrationSyncJob`, `RowVer`, `sp_Integration_PopulateAffectedKeys`, инициализация `v2.SyncState`. Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Enum не создавать.
- `RowVer` на `dbo.PrimitiveEntityData_2087` (Integration) — в `SqlScripts/Sync/01.Legacy_AddRowVersion.sql`, maintenance-окно. На 2026-11-25 `RowVer` в этой таблице нет. NC-индекс по `RowVer` — по размеру таблицы, решает перенос.
- Sources job — основная таблица `PrimitiveEntityData_2087`, с комментарием «Integration». Чужой справочник добавлять источником, только если проекция его читает и правило «Каскад» (`discovery-rules.md` §6) это велит. Стабильные словари с enum в каскад не входят.
- `Code` — `INT`. Генератора в легаси нет, все коды целые (12789 из 12789); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `integrations`, домен `Other`, ключ `HeaderMenu.Integration`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`, `TableName` = `v2.Integration_Snapshot`, ключ `IntegrationId`). `IntegrationSyncJob` добавить в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). `IntegrationDto` в `stableWithoutSyncJob` не добавлять.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`IntegrationFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.57 backlog дописать номер `SVT-5342`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.
- **Открыто: разворот `DataType` и порядок с SVT-5345.** `DataType` ссылается на IntegrationDataType, enum которого создаёт SVT-5345. Порядок двух задач и разворот этой колонки до появления enum постановка не решает; спросить пользователя при взятии в релиз.

### Что не входит

- Редактор 36.5.1.
- Загрузчик: в реестре его нет.
- Enum Integration.
- Строка Integration в таблице класса «стабильный словарь с enum».
- Модуль или меню для IntegrationDataSource: в реестре он вне скоупа («модуля нет, таблица/view остаются для FK»).
- Изменение механики интеграций платформы: модуль — справочник, не инфраструктура.

### Решения

- Watermark-sync: справочник в класс «стабильный словарь с enum» не входит (`svt-migrate-catalog`), данные меняются в легаси.
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`). В watermark-sync нецелый код даст `NULL` в проекции, и синхронизация строки упадёт на `NOT NULL`.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Разовый снимок `Modules/RateType` и `Modules/Currency` не копировать.
2. Строка Integration в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Данные для решения о каскаде (если проекция читает чужой длинный словарь) — `MAX(LastChangeDate)`, изменения за 3 года, fan-out, наличие `RowVer`, пишет ли `stg.Load*` эту колонку.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `SnapshotSyncJobContractTests` и `CatalogContractCoverageTests` опровергают «job зарегистрирован» и состав Sources.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует; циклы sync на `localhost` проходят без ошибки.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/Integration/` с гридом, карточкой, меню, `IntegrationSyncJob` и `AddCatalogApi` (`integrations`)
- [ ] SQL `BlazorSvt/SqlScripts/Modules/Integration/`: Structure 01–03, SnapshotSource, Detail, PopulateAffectedKeys; `RowVer` в `Sync/01.Legacy_AddRowVersion.sql`; `v2.SyncState` инициализирован
- [ ] `IntegrationSyncJob` зарегистрирован, снимок заполнен. `Code` — `INT`
- [ ] Enum не создан; строка в таблицу класса не добавлена
- [ ] Вопрос о развороте `DataType` и порядке с SVT-5345 закрыт
- [ ] Unit-тест контракта DTO, `SnapshotSyncJobContractTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `integrations`
- [ ] В пункт 15.10.57 дописан `SVT-5342`. Чекбокс 15.10.57 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2087` 12789 неархивных строк из 12789.
- `Code`: `0` не целых из 12789, генератора нет, длина до 5 знаков.
- Короткий список на 2026-11-25 в порядке рангов (число — неархивные заполненные строки): `Code` (12789), `DataType` (12789, `bigint`), `DataSource` (12788, `bigint`), `StartDate` (12789, `datetime2`), `EndDate` (12789, `datetime2`), `RowCount` (12789, `decimal`), `AutoSchedule` (12789, `bit`), `Note` (123, `nvarchar` без длины во view), `cbd_Version_Id` (106, `nvarchar(50)`), `IncludeArchiveData` (1401, `bit`). Состав и порядок колонок решает discovery в момент переноса.
- `StartDate` и `EndDate` — «Начало передачи» и «Окончание передачи», `datetime2` во view. `CatalogPeriod` — по правилу: только если в list-DTO две даты `DateOnly`.
- Ссылки (`bigint`) по совпадению значений с `Id` на 2026-11-25: `DataType` — IntegrationDataType (12789 из 12789), `DataSource` — IntegrationDataSource (12788 из 12788; в реестре вне скоупа, enum и модуля нет). Как показывать ссылку на справочник вне скоупа, `discovery-rules.md` §6 однозначно не говорит; решение — в момент переноса.
- `RowVer` в `PrimitiveEntityData_2087` на 2026-11-25 нет.
- Кто уже читает легаси-представление `vw_Integration`: в `BlazorSvt` не найдено ни одного читателя (поиск по `*.sql` и `*.cs` на 2026-11-25).
- Родственные постановки: SVT-5327 … SVT-5341, SVT-5343 … SVT-5345 (SVT-5345 создаёт enum IntegrationDataType), одна папка релиза.
