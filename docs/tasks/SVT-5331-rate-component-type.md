> Рабочая постановка SVT-5331. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник RateComponentType (Тип составляющей ставки)

### Проблема

Справочник RateComponentType входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Тип составляющей ставки» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `RateComponentTypeRu` / `RateComponentTypeEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка RateComponentType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.19**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7, п. 2а.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | RateComponentType, «Тип составляющей ставки», EntityInfo **2120** |
| Основной примитив | `RateComponentType`, `dbo.PrimitiveEntityData_2097` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 14 |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |
| Комментарий реестра | «Вспомогательный для ставок, хранит константы» |

Имя модуля `{Entity}` = `RateComponentType`, `{LegacyName}` = `RateComponentType`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/ratecomponenttype`.
- Разовый снимок: `02.RateComponentType_Insert.sql` из `v2.vw_RateComponentType_SnapshotSource`. Без `RowVer`, `RateComponentTypeSyncJob`, `sp_RateComponentType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси Manual, но по факту все коды целые (14 из 14; подтверждено пользователем). Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `rate-component-types`, домен `Rates`, ключ `HeaderMenu.RateComponentType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `RateComponentTypeRu` / `RateComponentTypeEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_RateComponentType`. Имена членов и `Display` — `discovery-rules.md` §7, п. 2а: `Code` невалиден как идентификатор (числа), имя члена = `SearchCode`; `Display` английского enum = `SearchCode`, русского = `Name`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку RateComponentType в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `RateComponentTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`RateComponentTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.19 backlog дописать номер `SVT-5331`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Поздний функционал, который будет читать `RateComponentTypeRu` / `RateComponentTypeEn`.
- Изменение модулей TransportRate, AverageRateLevel3, Coefficient, умного подбора, backlog 9.1.16.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал.
- `Code` — `INT` по решению пользователя, несмотря на генератор Manual (`discovery-rules.md` §2: риск нецелого кода).
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Имена членов enum — по новому п. 2а `discovery-rules.md` §7: без него цепочка имён пуста и ведёт к остановке.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка RateComponentType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_RateComponentType`: порог 100, коллизии имён, идентификаторы. Повторить проверку `Code` (§2а): нецелое значение — стоп, вопрос.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_RateComponentType` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/RateComponentType/` с гридом, карточкой, меню и `AddCatalogApi` (`rate-component-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/RateComponentType/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `RateComponentTypeRu`, `RateComponentTypeEn` в `Platform/Domain/IdsEnum`; строка RateComponentType в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `rate-component-types`
- [ ] В пункт 15.10.19 дописан `SVT-5331`. Чекбокс 15.10.19 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2097` 14 неархивных строк из 14. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `0` не целых, генератор Manual. Нецелый код, введённый в легаси позже, сломает перезаливку снимка; сейчас перезаливка в задачу не входит.
- Короткий список на 2026-11-25: `Code`, `Name`, `SearchCode`, все заполнены в 14 строках. Состав колонок решает discovery в момент переноса.
- `SearchCode` — `LoadedRF`, `LoadedCIS`, `EmptyRF`, `EmptyCIS`, `ProvisionTransport`, `Ferryboat`, `TEFrom`, `PNPFrom`, `TETo`, `PNPTo`, `DrainLoading`, `Transshipment`, `Freight`, `AdditionalFeesCIS`. Все значения валидны как идентификаторы C#.
- Правка RateComponentType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Родственные постановки: SVT-5327 … SVT-5330, SVT-5332, одна папка релиза.
