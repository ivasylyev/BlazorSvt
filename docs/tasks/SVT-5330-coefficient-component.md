> Рабочая постановка SVT-5330. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник CoefficientComponent (Составляющая коэффициента)

### Проблема

Справочник CoefficientComponent входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Составляющая коэффициента» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `CoefficientComponentRu` / `CoefficientComponentEn`. Эти enum сразу используются в SVT-5328 (дочерняя таблица CoefficientTypeComponent).

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка CoefficientComponent.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.17**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7, п. 2а.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | CoefficientComponent, «Составляющая коэффициента», EntityInfo **2109** |
| Основной примитив | `CoefficientComponent`, `dbo.PrimitiveEntityData_2082` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 17 |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |
| Комментарий реестра | «Хранит константы» |

Имя модуля `{Entity}` = `CoefficientComponent`, `{LegacyName}` = `CoefficientComponent`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/coefficientcomponent`.
- Разовый снимок: `02.CoefficientComponent_Insert.sql` из `v2.vw_CoefficientComponent_SnapshotSource`. Без `RowVer`, `CoefficientComponentSyncJob`, `sp_CoefficientComponent_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`: все коды целые (17 из 17, генератора нет). Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `coefficient-components`, домен `Rates`, ключ `HeaderMenu.CoefficientComponent`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `CoefficientComponentRu` / `CoefficientComponentEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_CoefficientComponent`. Имена членов и `Display` — `discovery-rules.md` §7, п. 2а: `Code` невалиден как идентификатор (числа), имя члена = `SearchCode`; `Display` английского enum = `SearchCode`, русского = `Name`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку CoefficientComponent в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `CoefficientComponentDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`CoefficientComponentFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.17 backlog дописать номер `SVT-5330`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Дочерняя таблица CoefficientTypeComponent на карточке CoefficientType: SVT-5328. Здесь только enum, на который она опирается.
- Переключение модуля Coefficient на новый снимок. Он читает легаси-представление `vw_CoefficientComponent`.
- Поздний функционал, который будет читать `CoefficientComponentRu` / `CoefficientComponentEn`.
- Модуль Coefficient и его секция `CoefficientComponentValue` (другой примитив), умный подбор, backlog 9.1.16.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его читает поздний функционал и дочерняя секция SVT-5328.
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Имена членов enum — по новому п. 2а `discovery-rules.md` §7: без него цепочка имён пуста и ведёт к остановке.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка CoefficientComponent в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_CoefficientComponent`: порог 100, коллизии имён, идентификаторы.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_CoefficientComponent` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/CoefficientComponent/` с гридом, карточкой, меню и `AddCatalogApi` (`coefficient-components`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/CoefficientComponent/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `CoefficientComponentRu`, `CoefficientComponentEn` в `Platform/Domain/IdsEnum`; строка CoefficientComponent в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `coefficient-components`
- [ ] В пункт 15.10.17 дописан `SVT-5330`. Чекбокс 15.10.17 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2082` 17 неархивных строк из 17. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `0` не целых, генератора нет.
- Короткий список на 2026-11-25: `Code`, `Name`, `Total`, `SearchCode`, все заполнены в 17 строках. Состав колонок решает discovery в момент переноса.
- `SearchCode` — `Total`, `LoadedRF`, `LoadedCIS`, `EmptyRF`, `EmptyCIS`, `ProvisionTransport`, `Ferryboat`, `TEFrom`, `PNPFrom`, `TETo`, `PNPTo`, `DrainLoading`, `Transshipment`, `Freight`, `AdditionalFeesCIS`, `TEFrom_fix`, `TETo_fix`. Все значения валидны как идентификаторы C#.
- Правка CoefficientComponent в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представления: `vw_CoefficientComponent` — Coefficient. Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327, SVT-5328 (читает эти enum), SVT-5329, SVT-5331, SVT-5332, одна папка релиза.
