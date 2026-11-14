> Рабочая постановка SVT-5301. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник TransportKind (Вид транспорта)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Эта задача переносит справочник **только для чтения** (грид, карточка, Excel, API). Редактор и загрузчик в реестре нужны, но в эту задачу не входят (backlog 9.4.7, 9.4.8). Схлопывание вида и типа транспорта — backlog **14.2.1**, после Gate D. Умный подбор с легаси-представления на снимок не переключать: это backlog **5.2.15**.

Источник истины по составу: `.cursor\skills\svt-architecture\catalogs-scope.md`.  
Чеклист: `.cursor\skills\svt-architecture\backlog.md` → пункт **3.3.23**.

### Что сделать

Перенести сущность реестра **TransportKind** (EntityInfo **2009**, «Вид транспорта») как read-only модуль.

Имя модуля — `TransportKind`: в имени реестра нет `_`. `{LegacyName}` — имя основного примитива из `EntityStructure`; его не подставлять именем реестра, пока шаг 0 не подтвердит, что они совпадают.

Из реестра, без discovery:

- домен **Маршруты** (`CatalogDomain.Routes`), меню ЕО = да (`VisibleToEditor: true`), маршрут `/transportkind`
- сложность Низкая, пр. RO = 3
- комментарий реестра про схлопывание с типом транспорта относится к backlog 14.2.1. Состава колонок в комментарии нет
- входит в класс «стабильный словарь с enum»: `TransportKindRu`, `TransportKindEn`. Разовый снимок, без watermark-sync

- грид + карточка + выгрузка Excel
- пункт меню домена **Маршруты**
- разовый снимок из `v2.vw_TransportKind_SnapshotSource`. Эталон — `Modules/Currency` и `Modules/RateType` (и их SQL). SyncJob, `sp_TransportKind_PopulateAffectedKeys`, RowVer и инициализацию `v2.SyncState` не создавать. В каскад чужих справочников не включать: их гриды показывают enum
- перед генерацией сверить неархивные строки (`PrimitiveEntityDataStateId = 1`) с `TransportKindRu` / `TransportKindEn`: `Id` и состав членов. Расхождение — стоп. Enum не перегенерировать
- API: `AddCatalogApi`, код `transport-kinds` (обычное множественное число skill), домен `Routes`, ключ `HeaderMenu.TransportKind`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`. Строки в `v2.SyncState` нет, `dataAsOf` будет пустым — это нормально для этого класса

Отдельного согласованного списка колонок нет, в постановку его не додумывать. Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Дочерний примитив в модуль не входит, пока его явно не включат. Подписи — Dictionary и `grid-column-glossary.md`. Стоп и согласование по skill: пустой заголовок справочника, английский ярлык совпал с системным именем и его нет в глоссарии, неизвестное короткое имя колонки, ссылка без однозначного FK. Согласованное исключение из этого фильтра дописать в комментарий строки `TransportKind` в `catalogs-scope.md`. Колонку RO там не отмечать: `[x]` ставится, когда справочник на проде.

### Как делать

1. Skill `.cursor\skills\add-reference-book\SKILL.md` целиком, включая раздел «Стабильный словарь с enum».
2. Строка `TransportKind` в `catalogs-scope.md` — домен, меню, приоритет.
3. Эталон — `Modules/Currency` и `Modules/RateType`. Watermark-sync TransportRate не копировать. Перед генерацией сверить неархивные строки с enum. Расхождение — стоп, enum не перегенерировать.
4. Релизная копия — `Migrations/2.3.49/` (текущий `VersionPrefix`, папка ещё не закрыта). `2.3.48` не менять. Дельта — следующий свободный номер после уже лежащих файлов. Contractor занимает `001`–`007`, `008` — снимок `All_Programmability.sql`. SVT-5291 и постановки, которые попадут в папку раньше, забирают номера до этой задачи; `009` заранее не занимать. В дельте нет RowVer, PopulateAffectedKeys и инициализации SyncState. Снимок programmability пересобрать из SoT и оставить последним файлом: прежний снимок в середине не остаётся. В `C:\publish` само по себе не копировать.
5. C# только после наката и проверки SQL на `localhost` / `mdm`. Папки `Sync/` нет.
6. Unit contract-тест DTO. Тест SyncJob не добавлять: job нет. Integration smoke грида, карточки и FTS по текстовым полям, которые discovery оставила в гриде; снимок OpenAPI обновить по skill. Умный подбор не менять.

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\TransportKind\` без `Sync/`
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\TransportKind\` без `sp_TransportKind_PopulateAffectedKeys`
- [ ] Снимок заполнен один раз. SyncJob, RowVer и SyncState нет
- [ ] Грид, карточка и выгрузка читают локальную базу
- [ ] `AddCatalogApi` (`transport-kinds`)
- [ ] Тесты по skill
- [ ] Пункт 3.3.23 в backlog не отмечать до ревью. Пункт 5.2.15 не делать. Enum `TransportKindRu` / `TransportKindEn` не перегенерировать. Пункт 14.2.1 не делать
