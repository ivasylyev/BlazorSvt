> Рабочая постановка SVT-5344. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник CurrencyRate (Курсы валют (плановые))

### Проблема

Справочник CurrencyRate входит в скоуп СВТ 2.0 (домен «Вспомогательные»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Курсы валют (плановые)» из меню «Вспомогательные», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Курсы приходят из АСВ НСИ раз в сутки, снимок грида обновляется watermark-sync.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Вспомогательные», строка CurrencyRate.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.59**.
- Эталон каркаса watermark-sync — `.cursor/skills/svt-migrate-catalog/SKILL.md`, начало; фазы SQL и C# — `sql.md`, `csharp.md`.
- В класс «стабильный словарь с enum» справочник не входит.
- Тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | CurrencyRate, «Курсы валют (плановые)», EntityInfo **2042** |
| Основной примитив | `CurrencyRate`, `dbo.PrimitiveEntityData_2017` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 12608 (всего 21746) |
| Сложность / приоритет | Низкая, пр. RO = 2 |
| Меню ЕО | да |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Курсы валют (плановые)» |
| Комментарий реестра | «Приходит автоматически из АСВ НСИ раз в сутки. Редактор не нужен» |

Имя модуля `{Entity}` = `CurrencyRate`, `{LegacyName}` = `CurrencyRate`: имена совпали. Это не модуль `Currency` и не `CurrencyRateActual`.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Other` с `VisibleToEditor: true`. Маршрут `/currencyrate`.
- Watermark-sync: снимок, `CurrencyRateSyncJob`, `RowVer`, `sp_CurrencyRate_PopulateAffectedKeys`, инициализация `v2.SyncState`. Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Enum не создавать.
- `RowVer` на `dbo.PrimitiveEntityData_2017` (CurrencyRate) — в `SqlScripts/Sync/01.Legacy_AddRowVersion.sql`, maintenance-окно. На 2026-11-25 `RowVer` в этой таблице нет. NC-индекс по `RowVer` — по размеру таблицы (21746 строк), решает перенос.
- Sources job — основная таблица `PrimitiveEntityData_2017`, с комментарием «CurrencyRate». Валюта (Currency) — стабильный словарь с enum, в каскад чужого job не входит.
- `Code` — **`NVARCHAR`**: коды нецелые (21551 из 21746; 195 целых, значения смешанные; генератор Manual). Длина — из легаси (`dbo\Tables`) через `LEFT(..., N)` в проекции. В list- и detail-DTO `string Code`.
- `AddCatalogApi`: код `currency-rates`, домен `Other`, ключ `HeaderMenu.CurrencyRate`. `CatalogPeriod` передавать только если в list-DTO две даты `DateOnly` (кандидаты — `BeginDate`, `EndDate`).
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`, `TableName` = `v2.CurrencyRate_Snapshot`, ключ `CurrencyRateId`). `CurrencyRateSyncJob` добавить в `SnapshotSyncJobContractTests` (`RegisteredJobs` + `AllJobs`). `CurrencyRateDto` в `stableWithoutSyncJob` не добавлять.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`CurrencyRateFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.59 backlog дописать номер `SVT-5344`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор: в реестре его нет.
- Enum CurrencyRate.
- Backlog 62.1.9 (замена CurrencyRateActual на плановые курсы).
- Модуль `Currency` и справочник CurrencyRateActual.
- Строка CurrencyRate в таблице класса «стабильный словарь с enum».

### Решения

- Watermark-sync: курсы приходят из АСВ НСИ раз в сутки (`catalogs-scope.md`), в класс «стабильный словарь с enum» справочник не входит (`svt-migrate-catalog`).
- `Code` — `NVARCHAR`, потому что `discovery-rules.md` §2 разрешает `INT` только при `NotInt = 0`, а здесь `NotInt = 21551`.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Разовый снимок `Modules/RateType` и `Modules/Currency` не копировать.
2. Строка CurrencyRate в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Модули Currency и CurrencyRateActual не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `SnapshotSyncJobContractTests` и `CatalogContractCoverageTests` опровергают «job зарегистрирован» и состав Sources.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Допущения стенда (тесты их не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует; циклы sync на `localhost` проходят без ошибки.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/CurrencyRate/` с гридом, карточкой, меню, `CurrencyRateSyncJob` и `AddCatalogApi` (`currency-rates`)
- [ ] SQL `BlazorSvt/SqlScripts/Modules/CurrencyRate/`: Structure 01–03, SnapshotSource, Detail, PopulateAffectedKeys; `RowVer` в `Sync/01.Legacy_AddRowVersion.sql`; `v2.SyncState` инициализирован
- [ ] `CurrencyRateSyncJob` зарегистрирован, снимок заполнен. `Code` — `NVARCHAR`
- [ ] Enum не создан; строка в таблицу класса не добавлена
- [ ] Unit-тест контракта DTO, `SnapshotSyncJobContractTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `currency-rates`
- [ ] В пункт 15.10.59 дописан `SVT-5344`. Чекбокс 15.10.59 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2017` 12608 неархивных строк из 21746.
- `Code`: `21551` не целых из 21746 (195 целых), генератор Manual, длина до 17 знаков. В представлении атрибут описан как `RateCode`.
- Короткий список на 2026-11-25 (число — неархивные заполненные строки, у всех атрибутов ранг 0, порядок по рангу не задан): `Code` (12608), `SourceCurrency` (12608, `bigint`), `ReceiverCurrency` (12608, `bigint`), `BeginDate` (12608, `datetime2`), `EndDate` (12608, `datetime2`), `Type` (12608, `nvarchar(255)`), `Name` (0). `Name` с нулём заполненных в грид и карточку не входит. Состав и порядок колонок решает discovery в момент переноса.
- `SourceCurrency` и `ReceiverCurrency` по совпадению значений с `Id` на 2026-11-25 — ссылки на Currency (по 12608 из 12608). В Platform есть один enum `CurrencyRuEn` (без пары Ru/En, `discovery-rules.md` §7). Разворот — discovery §6 в момент переноса. Имена колонок не совпадают с именем справочника, поэтому цель подтверждается там же.
- `BeginDate` и `EndDate` — «Начало» и «Конец периода действия курса». Нужен ли `CatalogPeriod`, решает тип этих свойств в list-DTO.
- `RowVer` в `PrimitiveEntityData_2017` на 2026-11-25 нет.
- Кто уже читает легаси-представление `vw_CurrencyRate`: в `BlazorSvt` не найдено ни одного читателя (поиск по `*.sql` и `*.cs` на 2026-11-25).
- Родственные постановки: SVT-5327 … SVT-5343, SVT-5345, одна папка релиза.
