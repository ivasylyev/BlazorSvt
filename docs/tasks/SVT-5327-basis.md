> Рабочая постановка SVT-5327. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник Basis (Базисы поставки)

### Проблема

Справочник Basis входит в скоуп СВТ 2.0 (домен «Ставки»), но в СВТ 2.0 не открывается: нет грида, карточки, выгрузки и API.

### Что меняется в работе

Читатель открывает «Базисы поставки» из меню «Ставки», видит грид и карточку, выгружает грид в Excel. Тот же справочник читается через API. Для будущего функционала в коде появляется пара enum `BasisRu` / `BasisEn`.

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Задача переносит справочник **только для чтения** (грид, карточка, Excel, API).

- Состав, меню, приоритет и комментарий реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`, домен «Ставки», строка Basis.
- Чеклист — `.cursor/skills/svt-architecture/backlog.md`, пункт **15.10.14**.
- Класс разового снимка, создание enum — `.cursor/skills/svt-migrate-catalog/SKILL.md`, раздел «Стабильный словарь с enum».
- Правила имён членов enum — `.cursor/skills/svt-migrate-catalog/discovery-rules.md` §7, п. 2а.
- Таблица «вне каскада» и тесты — `.cursor/rules/svt-development-patterns.mdc`.

| Поле | Значение |
|---|---|
| Реестр | Basis, «Базисы поставки», EntityInfo **2040** |
| Основной примитив | `Basis`, `dbo.PrimitiveEntityData_2015` |
| Дочерних примитивов | нет (проверено по `EntityStructure`) |
| Неархивных строк на 2026-11-25 | 12 |
| Сложность / приоритет | Низкая, пр. RO = 3 |
| Меню ЕО | нет |

Имя модуля `{Entity}` = `Basis`, `{LegacyName}` = `Basis`: имена совпали.

### Что сделать

- Модуль read-only: грид, карточка, выгрузка Excel грида, пункт меню домена `CatalogDomain.Rates` с `VisibleToEditor: false`. Маршрут `/basis`.
- Разовый снимок: `02.Basis_Insert.sql` из `v2.vw_Basis_SnapshotSource`. Без `RowVer`, `BasisSyncJob`, `sp_Basis_PopulateAffectedKeys` и строки `v2.SyncState`. Эталон — `Modules/RateType` и `Modules/Currency`. Каркас watermark-sync `Modules/TransportRate` не копировать. В каскад чужих гридов не включать.
- `Code` — `INT`: все коды целые (12 из 12, генератор OrdinalNumber). Проекция `TRY_CAST(x.Code AS INT) AS Code`, снимок `Code INT NOT NULL`, в list- и detail-DTO `long Code`.
- `AddCatalogApi`: код `bases`, домен `Rates`, ключ `HeaderMenu.Basis`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`.
- Заголовок справочника — **«Базисы поставки»** (`Basis.ru-RU.resx` `BasisGrid.Title` и `Platform.ru-RU.resx` `HeaderMenu.Basis`). Это исключение из `discovery-rules.md` §5: `Description` примитива 2015 равен `Basis`, его не брать. Английский заголовок — из Dictionary (`pei/Basis/ai`, `TOP 1`); если строки нет — «Delivery basis» (перевод агента, `--title-en`).
- Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Подписи — Dictionary и `grid-column-glossary.md`.
- Enum `BasisRu` / `BasisEn` в `Platform/Domain/IdsEnum`. Значение члена = `Id` из `vw_Basis`. Имена членов и `Display` — `discovery-rules.md` §7, п. 2а: `Code` невалиден как идентификатор C# (числа), `SearchCode` у Basis нет, поэтому имя члена = `Name`; `Display` английского enum = `Name`, русского = `FullName`. Неархивных строк 100 или больше — стоп, enum не создавать, вопрос. После появления файлов дописать строку Basis в таблицу класса в `svt-migrate-catalog/SKILL.md`.
- Unit-тест контракта list-DTO (`GridColumnMetadataBuilderTests`). Добавить `BasisDto` в `stableWithoutSyncJob` в `CatalogContractCoverageTests`: иначе тест принимает DTO за справочник с SyncJob и падает. SyncJob не тестировать: job нет.
- Integration smoke грида и карточки (`ModuleGridIntegrationTests`) и FTS по текстовым полям, которые discovery оставила в гриде (`BasisFtsIntegrationTests`). Снимок OpenAPI обновить по skill.
- В пункт 15.10.14 backlog дописать номер `SVT-5327`. Чекбокс не ставить. Колонку RO в `catalogs-scope.md` не отмечать.

### Что не входит

- Редактор и загрузчик Basis: backlog 21.21.9 и 21.21.10.
- Перезаливка снимка и пополнение enum после будущих правок Basis в легаси.
- Переключение TransportRate и AverageRateLevel3 на новый снимок или на enum. Они читают легаси-представление; в `TransportRateDetailDto` `Basis` остаётся строкой.
- Поздний функционал, который будет читать `BasisRu` / `BasisEn`.
- Модуль Coefficient, умный подбор, backlog 9.1.16.

### Решения

- Разовый снимок, потому что справочник стабильный и входит в класс «стабильный словарь с enum» (`svt-migrate-catalog`, `svt-development-patterns`). Прецедент с редактором в бэклоге — RateType.
- Enum создаётся в этом же переносе и лежит в `Platform/Domain/IdsEnum`: его будет читать поздний функционал.
- Меню ЕО = нет (`catalogs-scope.md`), поэтому `VisibleToEditor: false`, как у Currency. Шапка до ролевой модели 21.1 по флагу не фильтрует.
- Имена членов enum — по новому п. 2а `discovery-rules.md` §7: без него цепочка имён для Basis пуста и ведёт к остановке.
- Заголовок «Базисы поставки» берётся из реестра, а не из `Description` примитива.
- `VersionPrefix` на момент постановки неизвестен. Папка `Migrations/{VersionPrefix}` выбирается, когда задачу берут в релиз. Номер дельты — следующий после последнего файла этой папки. Снимок `All_Programmability.sql` пересобрать из SoT и оставить последним файлом. В `C:\publish` само по себе не копировать.
- Все новые `.sql` — UTF-8 with BOM.

### Как делать

1. Skill `.cursor/skills/svt-migrate-catalog/SKILL.md` целиком; эталон разового снимка — `Modules/RateType`, `Modules/Currency` и их `SqlScripts/Modules/*`.
2. Строка Basis в `catalogs-scope.md` — домен, меню, приоритет.
3. C# только после наката и проверки SQL на `localhost` / `mdm` (накат — skill `svt-create-programmability`). Строку `Database:MdmDb` и `appsettings.json` не менять.
4. Перед генерацией enum повторить запрос неархивных строк `vw_Basis`: порог 100, коллизии имён, идентификаторы.

### Как проверяем

- Контракт list-DTO — unit-тест `GridColumnMetadataBuilder`. Падение опровергает состав колонок и ключ снимка.
- `CatalogContractCoverageTests` опровергает «SyncJob нет».
- Появление `ISnapshotSyncJob` в модуле — провал ревью.
- Грид, карточка и FTS — integration smoke на `localhost` / `mdm`. Пустой результат или ошибка процедуры опровергают снимок и представления.
- `AddCatalogApi` — `CatalogApiCoverageTests` и обновлённый снимок OpenAPI (`OpenApiContractTests`).
- Enum: файлы компилируются; `Id` членов равны неархивным `Id` из `vw_Basis` на момент переноса. Эту сверку unit-тест без базы не видит: она в заметках и делается на `localhost` до генерации. Порог «100 или больше» проверяется тем же запросом.
- Допущение стенда (тесты его не видят): меню `VisibleToEditor: false` видно в регистрации модуля, шапка по флагу не фильтрует.

### Критерии готовности

- [ ] Модуль `BlazorSvt/Modules/Basis/` с гридом, карточкой, меню и `AddCatalogApi` (`bases`), без `Sync/`
- [ ] SQL `BlazorSvt/SqlScripts/Modules/Basis/`: Structure 01–03, SnapshotSource, Detail. SyncJob, PopulateAffectedKeys, RowVer и SyncState отсутствуют
- [ ] Снимок заполнен один раз. `Code` — `INT`
- [ ] Заголовок «Базисы поставки» в меню и гриде, EN — по Dictionary или «Delivery basis»
- [ ] Enum `BasisRu`, `BasisEn` в `Platform/Domain/IdsEnum`; строка Basis в таблице класса `svt-migrate-catalog`
- [ ] Unit-тест контракта DTO, `CatalogContractCoverageTests`, integration smoke грида, карточки и FTS зелёные
- [ ] Снимок OpenAPI содержит код `bases`
- [ ] В пункт 15.10.14 дописан `SVT-5327`. Чекбокс 15.10.14 и колонка RO не закрыты

### Заметки

- На 2026-11-25 в `localhost` / `mdm` у `PrimitiveEntityData_2015` 12 неархивных строк из 12. Повторная сверка обязательна в момент переноса: порог 100 и состав enum берутся из базы тогда.
- `Code`: `0` не целых, генератор OrdinalNumber.
- Короткий список на 2026-11-25: `Code`, `Name`, `FullName`, все заполнены в 12 строках. Состав колонок решает discovery в момент переноса.
- `Name` — латиница (`CIF`, `CPT`, `DAP`, `FCA`, `CFR`, `CIP`, `DAT`, `DDP`, `EXW`, `FAS`, `FOB`, `ITT`), `FullName` — по-русски. Все значения валидны как идентификаторы C#.
- Комментарий реестра: «Формально есть в ставках, но не заполняется. Возможно, потребуется в будущем?». Справочник при этом заполнен.
- Правка Basis в легаси после заливки в грид не попадёт, пока снимок не перезальют отдельно. Новый член enum требует правки кода. Эта перезаливка в задачу не входит.
- Родственные постановки: SVT-5328 … SVT-5332 (остальные пять справочников домена «Ставки»), одна папка релиза.
