> Рабочая постановка SVT-5340. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник DangerClass (Класс опасности)

### Проблема

Справочник DangerClass входит в скоуп СВТ 2.0 (домен «Продукты»), но своего грида, карточки, выгрузки и API в СВТ 2.0 у него нет. Его enum в коде есть, читает его модуль MTR.

### Что меняется в работе

Читатель открывает «Класс опасности» из меню «Продукты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Продукты», строка DangerClass.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.35**.
- Класс разового снимка и существующий enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | DangerClass, «Классы опасности», EntityInfo **2043** |
| Основной примитив | `DangerClass`, `dbo.PrimitiveEntityData_2018` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 9 (всего 9) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | да |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Класс опасности» |
| Комментарий реестра | «Формально есть в ставках, но не заполняется. Редактор и загрузчик не нужны» |

Имя модуля `{Entity}` = `DangerClass`, `{LegacyName}` = `DangerClass`: имена совпали.

Заголовок грида и пункта меню берётся из `Description` примитива (`discovery-rules.md` §5): «Класс опасности». В реестре название «Классы опасности»; расхождение между ними — вопрос для переноса, если пользователь хочет заголовок реестра.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Products` с `VisibleToEditor: true`. Маршрут `/dangerclass`.
- Разовый снимок: `02.DangerClass_Insert.sql` из `v2.vw_DangerClass_SnapshotSource`. Без `RowVer`, `DangerClassSyncJob`, `sp_DangerClass_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси Manual, но по факту все коды целые (9 из 9); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `danger-classes`, домен `Products`, ключ `HeaderMenu.DangerClass`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum **уже есть**: `Platform/Domain/IdsEnum/DangerClassRu`, `DangerClassEn`. Не создавать и не перегенерировать. Перед заливкой сверить неархивные строки `vw_DangerClass` (`PrimitiveEntityDataStateId = 1`) с enum: `Id` и состав членов. Расхождение — стоп, вопрос. Подписи `Display` в сверку не входят.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `DangerClassDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`DangerClassFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.35 backlog дописать номер `SVT-5340`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перегенерация `DangerClassRu` / `DangerClassEn`.
- Перезаливка снимка после будущих правок легаси.
- Переключение join в `vw_MTR_SnapshotSource` на новый снимок. Он остаётся на `dbo.vw_DangerClass`; модуль MTR не менять.

### Решения

- Разовый снимок: справочник стабильный, enum уже в коде; класс «стабильный словарь с enum» — `svt-migrate-catalog`, `svt-development-patterns`.
- `Code` — `INT` по решению пользователя, несмотря на генератор Manual (`discovery-rules.md` §2: риск нецелого кода).
- Меню ЕО = да (`catalogs-scope.md`), поэтому `VisibleToEditor: true`. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка DangerClass в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед заливкой повторить сверку неархивных строк `vw_DangerClass` с enum.
5. Модуль MTR не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Сверка неархивных строк `vw_DangerClass` с enum — в заметках, на `localhost` до заливки; unit-тест без базы её не видит.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: true` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/DangerClass/` с гридом, карточкой, меню и `AddCatalogApi` (`danger-classes`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/DangerClass/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `DangerClassRu`, `DangerClassEn` не перегенерированы; сверка с базой выполнена
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `danger-classes`
- [ ] В пункт 15.10.35 дописан `SVT-5340`. Чекбокс 15.10.35 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2018` 9 неархивных строк из 9.
- Сверка с enum на 2026-11-25: `Id` (42873 … 42881) совпали в `DangerClassRu` и `DangerClassEn`, по 9 членов. Повторить в момент переноса.
- Английской колонки имени во `vw_DangerClass` нет: из колонок имени есть только `Name`.
- `Code`: `0` не целых из 9, генератор Manual.
- Короткий список на 2026-11-25: `Code` (9), `Name` (9). Состав колонок решает discovery в момент переноса.
- Правка DangerClass в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_DangerClass`: MTR (`vw_MTR_SnapshotSource`). Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327 … SVT-5339, SVT-5341 … SVT-5345, одна папка релиза.
