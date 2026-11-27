> Рабочая постановка SVT-5343. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник FeatureToggle (Переключатель фич)

### Проблема

Справочник FeatureToggle входит в скоуп СВТ 2.0 (домен «Вспомогательные»), но своего грида, карточки, выгрузки и API в СВТ 2.0 у него нет. Платформа читает из него только один переключатель, `V2SyncEnabled`.

### Что меняется в работе

Читатель открывает «Переключатель фич» из меню «Вспомогательные», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Снимок грида обновляется watermark-sync.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Вспомогательные», строка FeatureToggle.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.58**.
- Эталон каркаса watermark-sync — `.cursor/skills/svt-migrate-catalog/SKILL.md`, начало; фазы SQL и C# — `sql.md`, `csharp.md`.
- В класс «стабильный словарь с enum» справочник не входит.
- Тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | FeatureToggle, «Переключатель фич», EntityInfo **2166** |
| Основной примитив | `FeatureToggle`, `dbo.PrimitiveEntityData_2147` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 12 (всего 14) |
| Сложность / приоритет | Низкая, пр. RO = 1 |
| Меню ЕО | да |
| Редактор, загрузчик | редактор 36.5.2 (в реестре есть, в эту задачу не входит); загрузчика нет |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Переключатель фич» |
| Комментарий реестра | «Справочник; фичи переключаются точечно, загрузчик не нужен. В легаси нет отдельного модуля инфраструктуры» |

Имя модуля `{Entity}` = `FeatureToggle`, `{LegacyName}` = `FeatureToggle`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Other` с `VisibleToEditor: true`. Маршрут `/featuretoggle`.
- Watermark-sync: снимок, `FeatureToggleSyncJob`, `RowVer`, `sp_FeatureToggle_PopulateAffectedKeys`, инициализация `v2.SyncState`. Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Enum не создавать.
- `RowVer` на `dbo.PrimitiveEntityData_2147` (FeatureToggle) — в `SqlScripts/Sync/01.Legacy_AddRowVersion.sql`, maintenance-окно. На 2026-11-25 `RowVer` в этой таблице нет. NC-индекс по `RowVer` — по размеру таблицы, решает перенос.
- Sources job — основная таблица `PrimitiveEntityData_2147`, с комментарием «FeatureToggle». Чужой справочник добавлять источником, только если проекция его читает и правило «Каскад» (`discovery-rules.md` §6) это велит.
- `Code` — **`NVARCHAR`**: коды нецелые (14 из 14, значения вроде `LogEnabled`, `V2SyncEnabled`; генератор Manual). Длина — из легаси (`dbo\Tables`) через `LEFT(..., N)` в проекции. В list- и detail-DTO `string Code`.
- `AddCatalogApi`: код `feature-toggles`, домен `Other`, ключ `HeaderMenu.FeatureToggle`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`, `TableName` = `v2.FeatureToggle_Snapshot`, ключ `FeatureToggleId`). `FeatureToggleSyncJob` добавить в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). `FeatureToggleDto` в `stableWithoutSyncJob` не добавлять.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`FeatureToggleFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.58 backlog дописать номер `SVT-5343`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор 36.5.2.
- Загрузчик: в реестре его нет.
- Enum FeatureToggle.
- Перевод kill switch на снимок грида: `V2SyncFeatureToggle` по-прежнему читает `V2SyncEnabled` из `dbo.vw_FeatureToggle`.
- Строка FeatureToggle в таблице класса «стабильный словарь с enum».

### Решения

- Watermark-sync: справочник в класс «стабильный словарь с enum» не входит (`svt-migrate-catalog`), состояние переключателей меняется в легаси.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Kill switch остаётся на легаси-представлении `dbo.vw_FeatureToggle` (решение брифа). Из кода: `SnapshotSyncScheduler` при выключенном `V2SyncEnabled` приостанавливает все job.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Разовый снимок `Modules/RateType` и `Modules/Currency` не копировать.
2. Строка FeatureToggle в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. `Platform/Sync/V2SyncFeatureToggle.cs` не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `SnapshotSyncJobContractTests` и `CatalogContractCoverageTests` опровергают «job зарегистрирован» и состав Sources.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует; циклы sync на `localhost` проходят без ошибки.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/FeatureToggle/` с гридом, карточкой, меню, `FeatureToggleSyncJob` и `AddCatalogApi` (`feature-toggles`)
- [ ] SQL `BlazorSvt/SqlScripts/Modules/FeatureToggle/`: Structure 01–03, SnapshotSource, Detail, PopulateAffectedKeys; `RowVer` в `Sync/01.Legacy_AddRowVersion.sql`; `v2.SyncState` инициализирован
- [ ] `FeatureToggleSyncJob` зарегистрирован, снимок заполнен. `Code` — `NVARCHAR`
- [ ] `V2SyncFeatureToggle` по-прежнему читает `V2SyncEnabled` из `dbo.vw_FeatureToggle`
- [ ] Enum не создан; строка в таблицу класса не добавлена
- [ ] Unit-тест контракта DTO, `SnapshotSyncJobContractTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `feature-toggles`
- [ ] В пункт 15.10.58 дописан `SVT-5343`. Чекбокс 15.10.58 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2147` 12 неархивных строк из 14.
- `Code`: `14` не целых из 14, генератор Manual, длина до 37 знаков.
- Коды неархивных строк на 2026-11-25: `WarehouseASVERP`, `LeadtimeMustBeUpdatedMM`, `LeadtimeMustBeUpdatedRail`, `LeadtimeMustBeUpdatedAuto`, `RateMustBeUpdatedMM`, `RateMustBeUpdatedRail`, `RateMustBeUpdatedAuto`, `ExportRatesToEdsViaSharepointStream20`, `IntegrationsConfirmationIsActive`, `LogEnabled`, `V2SyncEnabled`, `JobSyncRateUseUploadDate`.
- Короткий список на 2026-11-25: `Code` (12), `ToggleName` (12), `ToggleState` (12, `bit`), `Description` (12). Состав колонок решает discovery в момент переноса.
- `SnapshotSyncScheduler` при выключенном `V2SyncEnabled` пропускает работу всех job, в том числе `FeatureToggleSyncJob`. Пока переключатель выключен, снимок грида FeatureToggle не обновляется. Сам `V2SyncEnabled` читается из легаси и от этого не зависит.
- `RowVer` в `PrimitiveEntityData_2147` на 2026-11-25 нет.
- Кто уже читает легаси-представление `vw_FeatureToggle`: `Platform/Sync/V2SyncFeatureToggle` (`V2SyncEnabled`). Модуль в задаче не переключать на снимок.
- Родственные постановки: SVT-5327 … SVT-5342, SVT-5344, SVT-5345, одна папка релиза.
