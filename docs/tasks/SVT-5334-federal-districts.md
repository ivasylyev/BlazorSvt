> Рабочая постановка SVT-5334. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник Federal_Districts (Федеральные округа)

### Проблема

Справочник Federal_Districts входит в скоуп СВТ 2.0 (домен «Маршруты»), но своего грида, карточки, выгрузки и API в СВТ 2.0 у него нет. Его enum в коде есть, читает его модуль Region.

### Что меняется в работе

Читатель открывает «Федеральные округа» из меню «Маршруты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Маршруты», строка Federal_Districts.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.29**.
- Класс разового снимка и существующий enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Имя модуля из реестра с `_` — `.cursor/skills/svt-migrate-catalog/SKILL.md`, шаг 0.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | Federal_Districts, «Федеральные округа», EntityInfo **2139** |
| Основной примитив | `Federal_Districts`, `dbo.PrimitiveEntityData_2120` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 8 (всего 8) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | да |
| Редактор, загрузчик | 33.1.15 и 33.1.16 (в реестре есть, в эту задачу не входят) |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Федеральные округа» |
| Комментарий реестра | «Используется в иерархии между страной и регионом для РФ» |

Имя модуля `{Entity}` = `FederalDistricts` (`_` убирается, сегменты с заглавной), `{LegacyName}` = `Federal_Districts`: имена разошлись только подчёркиванием.

### Что сделать

- Модуль read-only `FederalDistricts`: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Routes` с `VisibleToEditor: true`. Маршрут `/federaldistricts`.
- Разовый снимок: `02.FederalDistricts_Insert.sql` из `v2.vw_FederalDistricts_SnapshotSource`. Источник — `dbo.vw_Federal_Districts`. Без `RowVer`, `FederalDistrictsSyncJob`, `sp_FederalDistricts_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси OrdinalNumber, все коды целые (8 из 8); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `federal-districts`, домен `Routes`, ключ `HeaderMenu.FederalDistricts`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum **уже есть**: `Platform/Domain/IdsEnum/FederalDistrictsRu`, `FederalDistrictsEn`. Не создавать и не перегенерировать. Перед заливкой сверить неархивные строки `vw_Federal_Districts` (`PrimitiveEntityDataStateId = 1`) с enum: `Id` и состав членов. Расхождение — стоп, вопрос. Подписи `Display` в сверку не входят.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `FederalDistrictsDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`FederalDistrictsFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.29 backlog дописать номер `SVT-5334`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор 33.1.15 и загрузчик 33.1.16.
- Перегенерация `FederalDistrictsRu` / `FederalDistrictsEn`.
- Правка расхождения подписи `North_Caucasian` (см. «Заметки»): ни enum, ни базу не чинить.
- Перезаливка снимка после будущих правок легаси.
- Переключение модуля Region на новый снимок. Он читает легаси-представление `vw_Federal_Districts` и enum.

### Решения

- Разовый снимок: справочник стабильный, enum уже в коде; класс «стабильный словарь с enum» — `svt-migrate-catalog`, `svt-development-patterns`.
- `Code` — `INT` по решению пользователя (`discovery-rules.md` §2: `NotInt = 0`, рекомендация `INT`).
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Расхождение подписи `North_Caucasian` — ошибка базы, не стоп (решение пользователя).
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка Federal_Districts в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед заливкой повторить сверку неархивных строк `vw_Federal_Districts` с enum.
5. Модуль Region не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Сверка неархивных строк `vw_Federal_Districts` с enum — в заметках, на `localhost` до заливки; unit-тест без базы её не видит.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/FederalDistricts/` с гридом, карточкой, меню и `AddCatalogApi` (`federal-districts`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/FederalDistricts/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `FederalDistrictsRu`, `FederalDistrictsEn` не перегенерированы; сверка с базой выполнена
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `federal-districts`
- [ ] В пункт 15.10.29 дописан `SVT-5334`. Чекбокс 15.10.29 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2120` 8 неархивных строк из 8, других состояний нет.
- Сверка с enum на 2026-11-25: `Id` совпали в `FederalDistrictsRu` и `FederalDistrictsEn`, по 8 членов. Повторить в момент переноса.
- Расхождение подписи: `Display` члена `North_Caucasian` — «Северо-Кавказский федеральный округ», `FullName` в `vw_Federal_Districts` — «Северо-Кавказский». Это ошибка базы.
- `Code`: `0` не целых из 8, генератор OrdinalNumber.
- Короткий список на 2026-11-25: `Code`, `FullName`, `FullNameEn`, `DisplayName`, `ShortName`, `AdmCenter`, `AdmCenterEn`, все заполнены в 8 строках. Состав колонок решает discovery в момент переноса.
- Правка Federal_Districts в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_Federal_Districts`: Region (`vw_Region_SnapshotSource`). Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327 … SVT-5333, SVT-5335 … SVT-5345, одна папка релиза.
