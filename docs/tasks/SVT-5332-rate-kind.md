> Рабочая постановка SVT-5332. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник RateKind (Вид ставки)

### Проблема

Справочник RateKind входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Вид ставки» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для функционала, который будет читать справочник, в коде появляется пара enum `RateKindRu` / `RateKindEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка RateKind.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.21**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7, п. 2а.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | RateKind, «Вид ставки», EntityInfo **2125** |
| Основной примитив | `RateKind`, `dbo.PrimitiveEntityData_2105` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 2 |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |
| Редактор, загрузчик | нет (в реестре «—») |
| Комментарий реестра | «Вспомогательный для ставок» |

Имя модуля `{Entity}` = `RateKind`, `{LegacyName}` = `RateKind`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/ratekind`.
- Разовый снимок: `02.RateKind_Insert.sql` из `v2.vw_RateKind_SnapshotSource`. Без `RowVer`, `RateKindSyncJob`, `sp_RateKind_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — **`NVARCHAR`**: коды нецелые (2 из 2, значения `D` и `S`; генератор Manual). Длина — из легаси (`dbo\Tables`) через `LEFT(..., N)` в проекции. В list- и detail-DTO `string Code`.
- `AddCatalogApi`: код `rate-kinds`, домен `Rates`, ключ `HeaderMenu.RateKind`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `RateKindRu` / `RateKindEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_RateKind`. Имена членов и `Display` — `discovery-rules.md` §7, п. 2а: `Code` (`D`, `S`) не годится как имя члена, имя члена = `SearchCode`; `Display` английского enum = `SearchCode`, русского = `Name`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку RateKind в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `RateKindDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`RateKindFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.21 backlog дописать номер `SVT-5332`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик: в реестре их нет.
- Перезаливка снимка и пополнение enum после будущих правок легаси.
- Переключение TransportRate на новый снимок или на enum. Он читает легаси-представление `vw_RateKind`.
- Поздний функционал, который будет читать `RateKindRu` / `RateKindEn`.
- Модуль TransportRate, умный подбор, backlog 9.1.16.

### Решения

- Разовый снимок: справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`).
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал.
- `Code` — `NVARCHAR`, потому что `discovery-rules.md` §2 разрешает `INT` только при `NotInt = 0`, а здесь `NotInt = 2`.
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Имена членов enum — по новому п. 2а `discovery-rules.md` §7: без него цепочка имён пуста и ведёт к остановке.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка RateKind в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_RateKind`: порог 100, коллизии имён, идентификаторы.
5. Нужен ли full-text на `Code` (две однобуквенные строки), решает правило `sql.md` («`Code` — только если значения осмысленный текст»), не постановка.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет». Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_RateKind` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/RateKind/` с гридом, карточкой, меню и `AddCatalogApi` (`rate-kinds`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/RateKind/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `NVARCHAR`
- [ ] Enum `RateKindRu`, `RateKindEn` в `Platform/Domain/IdsEnum`; строка RateKind в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `rate-kinds`
- [ ] В пункт 15.10.21 дописан `SVT-5332`. Чекбокс 15.10.21 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2105` 2 неархивные строки из 2. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `2` не целых из 2 (`D`, `S`), генератор Manual.
- Короткий список на 2026-11-25: `Code`, `Name`, `SearchCode`, все заполнены в обеих строках. Состав колонок решает discovery в момент переноса.
- `SearchCode` — `Deflated` и `Standard`. Оба валидны как идентификаторы C#.
- Правка RateKind в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Кто уже читает легаси-представления: `vw_RateKind` — TransportRate. Модуль в задаче не переключать на новый снимок.
- Родственные постановки: SVT-5327 … SVT-5331, одна папка релиза.
