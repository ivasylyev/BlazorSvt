> Рабочая постановка SVT-5345. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник IntegrationDataType (Тип данных для интеграции)

### Проблема

Справочник IntegrationDataType входит в скоуп СВТ 2.0 (домен «Вспомогательные»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Тип данных для интеграции» из меню «Вспомогательные», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `IntegrationDataTypeRu` / `IntegrationDataTypeEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Вспомогательные», строка IntegrationDataType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.60**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | IntegrationDataType, «Тип данных для интеграции», EntityInfo **2111** |
| Основной примитив | `IntegrationDataType`, `dbo.PrimitiveEntityData_2086` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 13 (всего 14) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | 36.5.5 и 36.5.6 (в реестре есть, в эту задачу не входят) |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип данных для интеграции» |
| Комментарий реестра | нет |

Имя модуля `{Entity}` = `IntegrationDataType`, `{LegacyName}` = `IntegrationDataType`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Other` с `VisibleToEditor: false`. Маршрут `/integrationdatatype`.
- Разовый снимок: `02.IntegrationDataType_Insert.sql` из `v2.vw_IntegrationDataType_SnapshotSource`. Без `RowVer`, `IntegrationDataTypeSyncJob`, `sp_IntegrationDataType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератора в легаси нет, все коды целые (14 из 14); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `integration-data-types`, домен `Other`, ключ `HeaderMenu.IntegrationDataType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `IntegrationDataTypeRu` / `IntegrationDataTypeEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_IntegrationDataType`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку IntegrationDataType в таблицу класса в `svt-migrate-catalog/SKILL.md`. **Имена членов и английский `Display` не определены:** на 2026-11-25 цепочка `discovery-rules.md` §7 пуста (см. «Заметки»). Это стоп и вопрос пользователю до генерации (§7 п.8); имена не придумывать.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `IntegrationDataTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`IntegrationDataTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.60 backlog дописать номер `SVT-5345`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор 36.5.5 и загрузчик 36.5.6.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Модуль или меню для IntegrationDataSource: в реестре он вне скоупа («модуля нет, таблица/view остаются для FK»).
- Разворот колонки `DataType` в Integration (SVT-5342) и переключение чужих читателей на enum.
- Поздний функционал, который будет читать `IntegrationDataTypeRu` / `IntegrationDataTypeEn`.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал, в том числе Integration (SVT-5342).
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`).
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка IntegrationDataType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_IntegrationDataType`: порог 100, коллизии имён, идентификаторы. Имена членов — после ответа на вопрос.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_IntegrationDataType` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/IntegrationDataType/` с гридом, карточкой, меню и `AddCatalogApi` (`integration-data-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/IntegrationDataType/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `IntegrationDataTypeRu`, `IntegrationDataTypeEn` в `Platform/Domain/IdsEnum`; строка IntegrationDataType в таблице класса `svt-migrate-catalog`; вопрос об именах членов закрыт
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `integration-data-types`
- [ ] В пункт 15.10.60 дописан `SVT-5345`. Чекбокс 15.10.60 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2086` 13 неархивных строк из 14. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `0` не целых из 14, генератора нет, длина до 2 знаков.
- Неархивные строки на 2026-11-25: `Id` 9687422 … 35510883 (13 значений), `Code` 2 … 14.
- Цепочка имён членов на 2026-11-25: `Code` — числа, не годится; колонки `SearchCode` во `vw_IntegrationDataType` нет; `DataType` («ПБД: продукты», «SAP TM», «ЦБД: ставки», «МК: все данные» и т. п.) — кириллица с пробелами и двоеточиями, не идентификатор; колонок `NameEn`, `NameRu` и остальных звеньев п.3 нет. Источника английского `Display` тоже нет. Ru `Display` — `DataType`.
- Короткий список на 2026-11-25: `Code` (13), `DataType` (13, `nvarchar(255)`), `DataSource` (13, `bigint`). Состав колонок решает discovery в момент переноса.
- `DataSource` по совпадению значений с `Id` на 2026-11-25 — ссылка на IntegrationDataSource (13 из 13; в реестре вне скоупа, enum и модуля нет). Как показывать ссылку на справочник вне скоупа, `discovery-rules.md` §6 однозначно не говорит; решение — в момент переноса.
- Правка IntegrationDataType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_IntegrationDataType`: в `BlazorSvt` не найдено ни одного читателя (поиск по `*.sql` и `*.cs` на 2026-11-25).
- Родственные постановки: SVT-5327 … SVT-5344 (SVT-5342 читает ссылку `DataType` на этот справочник), одна папка релиза.
