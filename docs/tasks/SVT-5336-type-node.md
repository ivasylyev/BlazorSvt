> Рабочая постановка SVT-5336. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник TypeNode (Тип узла)

### Проблема

Справочник TypeNode входит в скоуп СВТ 2.0 (домен «Маршруты»), но своего грида, карточки, выгрузки и API в СВТ 2.0 у него нет. Его enum в коде есть, читает его модуль LocationsNodes.

### Что меняется в работе

Читатель открывает «Тип узла» из меню «Маршруты», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Маршруты», строка TypeNode.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.31**.
- Класс разового снимка и существующий enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | TypeNode, «Тип узла», EntityInfo **2151** |
| Основной примитив | `TypeNode`, `dbo.PrimitiveEntityData_2132` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 10 (всего 10) |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |
| Заголовок (`PrimitiveEntityInfo.Description`) | «Тип узла» |
| Комментарий реестра | «Хранит константы. Вспомогательный для местоположений-узлов. Используется в фильтрации узлов в заявке в зависимости от вида транспорта» |

Имя модуля `{Entity}` = `TypeNode`, `{LegacyName}` = `TypeNode`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Routes` с `VisibleToEditor: false`. Маршрут `/typenode`.
- Разовый снимок: `02.TypeNode_Insert.sql` из `v2.vw_TypeNode_SnapshotSource`. Без `RowVer`, `TypeNodeSyncJob`, `sp_TypeNode_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`. Генератор в легаси Manual, но по факту все коды целые (10 из 10); решение пользователя от 2026-11-25. Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `type-nodes`, домен `Routes`, ключ `HeaderMenu.TypeNode`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum **уже есть**: `Modules/LocationsNodes/List/IdsEnum/TypeNodeRu`, `TypeNodeEn`. Не переносить в Platform и не перегенерировать. Перед заливкой сверить неархивные строки `vw_TypeNode` (`PrimitiveEntityDataStateId = 1`) с enum: `Id` и состав членов. Расхождение — стоп, вопрос. Подписи `Display` в сверку не входят.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `TypeNodeDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`TypeNodeFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.31 backlog дописать номер `SVT-5336`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перенос `TypeNodeRu` / `TypeNodeEn` в Platform и перегенерация enum.
- Перезаливка снимка после будущих правок легаси.
- Переключение LocationsNodes на новый снимок. Он читает легаси-представление `vw_TypeNode` и enum.
- Изменение заявки и фильтрации узлов в ней.

### Решения

- Разовый снимок: справочник стабильный, enum уже в коде; класс «стабильный словарь с enum» — `svt-migrate-catalog`, `svt-development-patterns`.
- `Code` — `INT` по решению пользователя, несмотря на генератор Manual (`discovery-rules.md` §2: риск нецелого кода).
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Enum остаётся в `Modules/LocationsNodes/List/IdsEnum` (решение брифа).
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка TypeNode в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед заливкой повторить сверку неархивных строк `vw_TypeNode` с enum.
5. LocationsNodes и заявку не менять.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления. `NULL` в `Code` проекции опровергает допущение «все коды целые».
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Сверка неархивных строк `vw_TypeNode` с enum — в заметках, на `localhost` до заливки; unit-тест без базы её не видит.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/TypeNode/` с гридом, карточкой, меню и `AddCatalogApi` (`type-nodes`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/TypeNode/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Enum `TypeNodeRu`, `TypeNodeEn` остались в `Modules/LocationsNodes/List/IdsEnum`, не перегенерированы; сверка с базой выполнена
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `type-nodes`
- [ ] В пункт 15.10.31 дописан `SVT-5336`. Чекбокс 15.10.31 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2132` 10 неархивных строк из 10.
- Сверка с enum на 2026-11-25: `Id` совпали в `TypeNodeRu` и `TypeNodeEn`, по 10 членов. Повторить в момент переноса.
- `Code`: `0` не целых из 10, генератор Manual. Нецелый код, введённый в легаси позже, сломает перезаливку снимка; сейчас перезаливка в задачу не входит.
- Короткий список на 2026-11-25: `Code` (10), `NameRu` (10), `NameEn` (10), `Description` (8). Состав колонок решает discovery в момент переноса.
- Правка TypeNode в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представление `vw_TypeNode`: LocationsNodes. Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327 … SVT-5335, SVT-5337 … SVT-5345, одна папка релиза.
