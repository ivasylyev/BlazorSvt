> Рабочая постановка SVT-5333. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник TransportType (Тип транспорта)

### Проблема

Справочник TransportType входит в скоуп СВТ 2.0 (домен «Маршруты»), но своего грида, карточки, выгрузки и API в СВТ 2.0 у него нет. Его enum в коде есть, читают его только чужие модули.

### Что меняется в работе

Читатель открывает «Тип транспорта» из меню «Маршруты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Маршруты», строка TransportType.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.28**.
- Класс разового снимка и существующий enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | TransportType, «Тип транспорта», EntityInfo **2029** |
| Основной примитив | `TransportType`, `dbo.PrimitiveEntityData_2009` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 29 (всего 30) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | да |
| Редактор, загрузчик | 33.1.11 и 33.1.12 (в реестре есть, в эту задачу не входят) |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип транспорта» |
| Комментарий реестра | «Схлопнуть с видом транспорта можно, но только после полной миграции в СВТ 2.0 (задача после Gate D)» |

Имя модуля `{Entity}` = `TransportType`, `{LegacyName}` = `TransportType`: имена совпали.

`PrimitiveEntityData_2009` — таблица TransportType. `EntityInfo` 2009 — другой справочник, TransportKind. Два пространства идентификаторов не смешивать (`svt-development-patterns`).

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Routes` с `VisibleToEditor: true`. Маршрут `/transporttype`.
- Разовый снимок: `02.TransportType_Insert.sql` из `v2.vw_TransportType_SnapshotSource`. Без `RowVer`, `TransportTypeSyncJob`, `sp_TransportType_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — **`NVARCHAR`**: коды нецелые (30 из 30, значения вроде `Auto`, `Rail_35`; генератор Manual). Длина — из легаси (`dbo\Tables`) через `LEFT(..., N)` в проекции. В list- и detail-DTO `string Code`.
- `AddCatalogApi`: код `transport-types`, домен `Routes`, ключ `HeaderMenu.TransportType`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum **уже есть**: `Platform/Domain/IdsEnum/TransportTypeRu`, `TransportTypeEn`. Не создавать и не перегенерировать. Перед заливкой сверить неархивные строки `vw_TransportType` (`PrimitiveEntityDataStateId = 1`) с enum: `Id` и состав членов. Расхождение — стоп, вопрос. Подписи `Display` в сверку не входят.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `TransportTypeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`TransportTypeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.28 backlog дописать номер `SVT-5333`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор 33.1.11 и загрузчик 33.1.12.
- Схлопывание с видом транспорта (TransportKind): по комментарию реестра только после полной миграции, после Gate D.
- Перегенерация `TransportTypeRu` / `TransportTypeEn`.
- Перезаливка снимка после будущих правок легаси.
- Переключение TransportRate и AverageRateLevel3 на новый снимок. Они читают легаси-представление `vw_TransportType`, чужие гриды справочник в каскад не включают.

### Решения

- Разовый снимок: справочник стабильный, enum уже в коде; класс «стабильный словарь с enum» — `svt-migrate-catalog`, `svt-development-patterns`.
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка TransportType в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед заливкой повторить сверку неархивных строк `vw_TransportType` с enum.
5. TransportRate, AverageRateLevel3 и умный подбор не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Сверка неархивных строк `vw_TransportType` с enum — в заметках, на `localhost` до заливки; unit-тест без базы её не видит.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/TransportType/` с гридом, карточкой, меню и `AddCatalogApi` (`transport-types`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/TransportType/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `NVARCHAR`
- [ ] Enum `TransportTypeRu`, `TransportTypeEn` не перегенерированы; сверка с базой выполнена
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `transport-types`
- [ ] В пункт 15.10.28 дописан `SVT-5333`. Чекбокс 15.10.28 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2009` 29 неархивных строк из 30. Архивная одна: `Mix_sea_SMP` (`Id` 39059645, состояние 2). В enum её нет, это не расхождение.
- Сверка с enum на 2026-11-25: `Id` и `Code` = имя члена совпали в `TransportTypeRu` и `TransportTypeEn`, по 29 членов. Повторить в момент переноса.
- `Code`: `30` не целых из 30, генератор Manual.
- Короткий список на 2026-11-25: `Code` (29), `Name` (29), `NameEnRu` (29), `Status` (29), `TypeCode` (29), `SAPCode` (15), `OrionCode` (14), `ParentCode` (29); числа — неархивные заполненные строки. Состав колонок решает discovery в момент переноса.
- `Status` и `ParentCode` — `bigint`. По совпадению значений с `Id` на 2026-11-25: `ParentCode` — TransportKind (29 из 29), `Status` — StatusElement (29 из 29; в реестре вне скоупа, enum `StatusElementRu` / `StatusElementEn` есть в Platform). Разворот ссылок и решение, нужна ли колонка `Status`, — discovery §6 в момент переноса.
- Правка TransportType в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_TransportType`: TransportRate, AverageRateLevel3. Модули в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327 … SVT-5332, SVT-5334 … SVT-5345, одна папка релиза.
