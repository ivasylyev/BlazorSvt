> Рабочая постановка SVT-5341. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник TransportTypeForProduct (Тип транспорта для продукта)

### Проблема

Справочник TransportTypeForProduct входит в скоуп СВТ 2.0 (домен «Продукты»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Тип транспорта для продукта» из меню «Продукты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Продукты», строка TransportTypeForProduct.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.36**.
- Класс разового снимка — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum». TransportTypeForProduct в класс не входит и enum не получает; от класса берётся только разовый снимок.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | TransportTypeForProduct, «Тип транспорта для продукта», EntityInfo **2121** |
| Основной примитив | `TransportTypeForProduct`, `dbo.PrimitiveEntityData_2100` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 569 (всего 1103) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | да |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип транспорта для продукта» |
| Комментарий реестра | «Для автоподбора ставок в заявке. Возможно, после рефакторинга заявок будет неактуален. Редактор и загрузчик не нужны» |

Имя модуля `{Entity}` = `TransportTypeForProduct`, `{LegacyName}` = `TransportTypeForProduct`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Products` с `VisibleToEditor: true`. Маршрут `/transporttypeforproduct`.
- Разовый снимок без enum: `02.TransportTypeForProduct_Insert.sql` из `v2.vw_TransportTypeForProduct_SnapshotSource`. Без `RowVer`, `TransportTypeForProductSyncJob`, `sp_TransportTypeForProduct_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси OrdinalNumber, все коды целые (1103 из 1103); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: домен `Products`, ключ `HeaderMenu.TransportTypeForProduct`. **Код API не зафиксирован:** множественное число в имени с предлогом неочевидно (`csharp.md`: если пара неочевидна — спросить). Спросить пользователя при переносе и сверить с уже зарегистрированными кодами. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum не создавать. Ссылки на справочники с enum разворачиваются по `discovery-rules.md` §6 в момент переноса.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `TransportTypeForProductDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`TransportTypeForProductFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.36 backlog дописать номер `SVT-5341`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Enum TransportTypeForProduct: в класс «стабильный словарь с enum» справочник не входит.
- Перезаливка снимка после будущих правок легаси.
- Заявка и автоподбор ставок: они остаются как есть. Актуальность справочника после рефакторинга заявок — вне задачи.

### Решения

- Разовый снимок без enum: справочник в класс «стабильный словарь с enum» не входит (`svt-migrate-catalog`). Нет `RowVer`, SyncJob, `PopulateAffectedKeys`, `v2.SyncState`.
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`).
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка TransportTypeForProduct в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Блокеры discovery из заметок (атрибуты без колонок во view) закрыть до SQL.
5. Заявку и подбор ставок не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/TransportTypeForProduct/` с гридом, карточкой, меню и `AddCatalogApi`, без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/TransportTypeForProduct/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum не создан; строка в таблицу класса не добавлена
- [ ] Код API согласован с пользователем и сверен с зарегистрированными
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит согласованный код
- [ ] В пункт 15.10.36 дописан `SVT-5341`. Чекбокс 15.10.36 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2100` 569 неархивных строк из 1103.
- `Code`: `0` не целых из 1103, генератор OrdinalNumber.
- Короткий список на 2026-11-25 в порядке рангов (число — неархивные заполненные строки): `Code` (569), `TransportTypeCode`, `TransportType` (569, `bigint`), `TransportKind`, `ProductCode`, `Product` (453, `bigint`), `ProductGroupCode`, `ProductGroup` (547, `bigint`). Состав и порядок колонок решает discovery в момент переноса.
- Блокер discovery на 2026-11-25, решается в момент переноса остановкой и вопросом пользователю (`discovery-queries.md`: нет атрибута во view): `TransportTypeCode`, `TransportKind`, `ProductCode`, `ProductGroupCode` — `AttributeType` 9 (вычисляемые по `JsonPath` от ссылочного атрибута), колонок во `dbo.vw_TransportTypeForProduct` нет, счётчик §3 по view не считается.
- Ссылки (`bigint`) по совпадению значений с `Id` на 2026-11-25: `TransportType` — TransportType_level_3 (569 из 569; enum `TransportTypeLevel3Ru` / `TransportTypeLevel3En` есть в Platform), `Product` — MTR (453 из 453), `ProductGroup` — ProductGroup (547 из 547). Разворот — discovery §6 в момент переноса.
- Правка TransportTypeForProduct в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_TransportTypeForProduct`: в `BlazorSvt` не найдено ни одного читателя (поиск по `*.sql` и `*.cs` на 2026-11-25).
- Родственные постановки: SVT-5327 … SVT-5340, SVT-5342 … SVT-5345, одна папка релиза.
