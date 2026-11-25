> Рабочая постановка SVT-5329. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник CoefficientCondition (Условие коэффициента)

### Проблема

Справочник CoefficientCondition входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Условие коэффициента» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `CoefficientConditionRu` / `CoefficientConditionEn`. Эти enum сразу используются в SVT-5328 (дочерняя таблица CoefficientTypeCondition).

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка CoefficientCondition.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.16**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7, п. 2а.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | CoefficientCondition, «Условие коэффициента», EntityInfo **2107** |
| Основной примитив | `CoefficientCondition`, `dbo.PrimitiveEntityData_2078` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 8 |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Комментарий реестра | «Задаёт приоритеты для коэффициентов» |

Имя модуля `{Entity}` = `CoefficientCondition`, `{LegacyName}` = `CoefficientCondition`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/coefficientcondition`.
- Разовый снимок: `02.CoefficientCondition_Insert.sql` из `v2.vw_CoefficientCondition_SnapshotSource`. Без `RowVer`, `CoefficientConditionSyncJob`, `sp_CoefficientCondition_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`: все коды целые (8 из 8, генератора нет). Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `coefficient-conditions`, домен `Rates`, ключ `HeaderMenu.CoefficientCondition`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `CoefficientConditionRu` / `CoefficientConditionEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_CoefficientCondition`. Имена членов и `Display` — `discovery-rules.md` §7, п. 2а: `Code` невалиден как идентификатор (числа), имя члена = `SearchCode`; `Display` английского enum = `SearchCode`, русского = `Name`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку CoefficientCondition в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `CoefficientConditionDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`CoefficientConditionFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.16 backlog дописать номер `SVT-5329`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик CoefficientCondition: backlog 21.21.11 и 21.21.12.
- Перезаливка снимка и пополнение enum после будущих правок CoefficientCondition в легаси.
- Дочерняя таблица CoefficientTypeCondition на карточке CoefficientType: SVT-5328. Здесь только enum, на который она опирается.
- Поздний функционал, который будет читать `CoefficientConditionRu` / `CoefficientConditionEn`.
- Модуль Coefficient и его секция `CoefficientConditionValue` (другой примитив), умный подбор, backlog 9.1.16.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`). Прецедент с редактором в бэклоге — RateType.
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его читает поздний функционал и дочерняя секция SVT-5328.
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Имена членов enum — по новому п. 2а `discovery-rules.md` §7: без него цепочка имён пуста и ведёт к остановке.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка CoefficientCondition в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_CoefficientCondition`: порог 100, коллизии имён, идентификаторы.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_CoefficientCondition` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/CoefficientCondition/` с гридом, карточкой, меню и `AddCatalogApi` (`coefficient-conditions`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/CoefficientCondition/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `CoefficientConditionRu`, `CoefficientConditionEn` в `Platform/Domain/IdsEnum`; строка CoefficientCondition в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `coefficient-conditions`
- [ ] В пункт 15.10.16 дописан `SVT-5329`. Чекбокс 15.10.16 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2078` 8 неархивных строк из 8. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `0` не целых, генератора нет.
- Короткий список на 2026-11-25: `Code`, `Name`, `SearchCode`, `WeightRatio`, все заполнены в 8 строках. Состав колонок решает discovery в момент переноса.
- `SearchCode` — `TransportKind`, `TransportType`, `FromLocation`, `ToLocation`, `ProductGroup`, `Product`, `TransportTypeLevel2`, `VehicleType`. Все значения валидны как идентификаторы C#.
- Правка CoefficientCondition в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Родственные постановки: SVT-5327, SVT-5328 (читает эти enum), SVT-5330 … SVT-5332, одна папка релиза.
