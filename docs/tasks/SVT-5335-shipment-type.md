> Рабочая постановка SVT-5335. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник ShipmentType (Тип отправки)

### Проблема

Справочник ShipmentType входит в скоуп СВТ 2.0 (домен «Маршруты»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Тип отправки» из меню «Маршруты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `ShipmentTypeRu` / `ShipmentTypeEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Маршруты», строка ShipmentType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.30**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | ShipmentType, «Тип отправки», EntityInfo **2161** |
| Основной примитив | `ShipmentType`, `dbo.PrimitiveEntityData_2142` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 5 (всего 5) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | да |
| Редактор, загрузчик | 33.1.17 и 33.1.18 (в реестре есть, в эту задачу не входят) |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип отправки» |
| Комментарий реестра | нет |

Имя модуля `{Entity}` = `ShipmentType`, `{LegacyName}` = `ShipmentType`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Routes` с `VisibleToEditor: true`. Маршрут `/shipmenttype`.
- Разовый снимок: `02.ShipmentType_Insert.sql` из `v2.vw_ShipmentType_SnapshotSource`. Без `RowVer`, `ShipmentTypeSyncJob`, `sp_ShipmentType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — **`NVARCHAR`**: коды нецелые (5 из 5, значения `TVD`, `PVG`, `GPP`, `MRT`, `ZHN`; генератор Manual). Длина — из легаси (`dbo\Tables`) через `LEFT(..., N)` в проекции. В list- и detail-DTO `string Code`.
- `AddCatalogApi`: код `shipment-types`, домен `Routes`, ключ `HeaderMenu.ShipmentType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `ShipmentTypeRu` / `ShipmentTypeEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_ShipmentType`. Имена членов — всегда из `Code` (`discovery-rules.md` §7, исключение для `vw_ShipmentType`): `TVD`, `PVG`, `GPP`, `MRT`, `ZHN`. `Display` — дословно из колонки базы: английский enum `NameEn`, русский `NameRu`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку ShipmentType в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `ShipmentTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`ShipmentTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.30 backlog дописать номер `SVT-5335`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор 33.1.17 и загрузчик 33.1.18.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Переход `ShipmentTypeCodeT` у TransportLeg на enum или JOIN. Атрибут остаётся сырым `NVARCHAR`: несколько кодов через `/`.
- Переключение TransportRate, AverageRateLevel3 и Leadtime на новый снимок или на enum. Они читают легаси-представление `vw_ShipmentType`.
- Поздний функционал, который будет читать `ShipmentTypeRu` / `ShipmentTypeEn`.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал.
- Имена членов — из `Code`: так задано для ShipmentType в `discovery-rules.md` §7 и в `svt-migrate-catalog`.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка ShipmentType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_ShipmentType`: порог 100, коллизии имён, идентификаторы.
5. TransportRate, AverageRateLevel3, Leadtime и TransportLeg не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_ShipmentType` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/ShipmentType/` с гридом, карточкой, меню и `AddCatalogApi` (`shipment-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/ShipmentType/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `NVARCHAR`
- [ ] Enum `ShipmentTypeRu`, `ShipmentTypeEn` в `Platform/Domain/IdsEnum`; строка ShipmentType в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `shipment-types`
- [ ] В пункт 15.10.30 дописан `SVT-5335`. Чекбокс 15.10.30 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2142` 5 неархивных строк из 5. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `5` не целых из 5, генератор Manual. Все пять значения валидны как идентификаторы C#.
- Строки на 2026-11-25 (`Id`, `Code`, `NameRu`, `NameEn`): 27584768 `TVD` Твердая / Tverdaya; 27584769 `PVG` Повагонная / Povagonnaya; 27584770 `GPP` Групповая / Gruppovaya; 27584771 `MRT` Маршрутная / Marshrutnaya; 27584772 `ZHN` Жесткая нитка / Zhestkaya nitka.
- Короткий список на 2026-11-25: `Code` (5), `Priority` (5, `decimal`), `NameRu` (5), `NameEn` (5), `TransportKind` (4, `bigint`), `Description` (0), `IsMain` (5, `bit`). `Description` с нулём заполненных в грид и карточку не входит. Состав колонок решает discovery в момент переноса.
- `TransportKind` по совпадению значений с `Id` на 2026-11-25 — ссылка на TransportKind (4 из 4); enum `TransportKindRu` / `TransportKindEn` есть в Platform. Разворот — discovery §6 в момент переноса.
- Правка ShipmentType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_ShipmentType`: TransportRate, AverageRateLevel3, Leadtime (`vw_Leadtime_Detail`). Модули в задаче не переключать на новый снимок и на enum.
- Родственные постановки: SVT-5327 … SVT-5334, SVT-5336 … SVT-5345, одна папка релиза.
