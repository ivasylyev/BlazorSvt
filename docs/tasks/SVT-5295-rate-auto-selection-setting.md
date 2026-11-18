> Рабочая постановка SVT-5295. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник RateAutoSelectionSetting (Настройка автоподбора ставок)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Эта задача переносит справочник **только для чтения** (грид, карточка, Excel, API). Редактор в реестре нужен, но в эту задачу не входит (backlog 21.21.13). Загрузчика в реестре нет. Комментарий «Надо улучшить» в эту задачу не входит.

Источник истины по составу: `.cursor\skills\svt-architecture\catalogs-scope.md`.  
Чеклист: `.cursor\skills\svt-architecture\backlog.md` → пункт **15.10.20**.

### Что сделать

Перенести сущность реестра **RateAutoSelectionSetting** (EntityInfo **2122**, «Настройка автоподбора ставок») как read-only модуль.

Имя модуля — `RateAutoSelectionSetting`: в имени реестра нет `_`. `{LegacyName}` — имя основного примитива из `EntityStructure`; его не подставлять именем реестра, пока шаг 0 не подтвердит, что они совпадают.

Из реестра, без discovery:

- домен **Ставки** (`CatalogDomain.Rates`), меню ЕО = да (`VisibleToEditor: true`), маршрут `/rateautoselectionsetting`
- сложность Низкая, пр. RO = 3
- комментарий реестра: надо улучшить; загрузчика нет. Состава колонок в нём нет. Улучшение экрана — не эта задача
- в класс «стабильный словарь с enum» в skill не входит. Отдельного enum RateAutoSelectionSetting в коде нет. Enum этого справочника, найденный при discovery, не перегенерировать: остановиться по правилу skill

- грид + карточка + выгрузка Excel
- пункт меню домена **Ставки**
- watermark-sync (снимок, SyncJob, RowVer, SyncState). Каркас — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`: Structure, Programmability, SyncJob, модуль. Узлы, продукт, срок действия и WHERE членства TransportRate не переносить. Чужой справочник в Sources — только если проекция RateAutoSelectionSetting его читает. Стабильные словари (RateType, TransportKind, Currency и остальные из таблицы skill) в каскад не входят, даже если проекция читает их Id. RowVer на основную таблицу — по skill, если его там ещё нет
- API: `AddCatalogApi`, код `rate-auto-selection-settings` (обычное множественное число skill), домен `Rates`, ключ `HeaderMenu.RateAutoSelectionSetting`. `CatalogPeriod` передавать только если в list-DTO есть две даты `DateOnly`

Отдельного согласованного списка колонок нет, в постановку его не додумывать. Короткий список и порядок грида — discovery §1 после фильтра §3. Порядок карточки — §4 после того же фильтра. Атрибут с нулём заполненных неархивных значений в грид и карточку не входит. Дочерний примитив в модуль не входит, пока его явно не включат. Подписи — Dictionary и `grid-column-glossary.md`. Стоп и согласование по skill: пустой заголовок справочника, английский ярлык совпал с системным именем и его нет в глоссарии, неизвестное короткое имя колонки, ссылка без однозначного FK. Согласованное исключение из этого фильтра дописать в комментарий строки `RateAutoSelectionSetting` в `catalogs-scope.md`. Колонку RO там не отмечать: `[x]` ставится, когда справочник на проде.

### Как делать

1. Skill `.cursor\skills\add-reference-book\SKILL.md` целиком.
2. Строка `RateAutoSelectionSetting` в `catalogs-scope.md` — домен, меню, приоритет.
3. Эталон каркаса — `Modules/TransportRate` и `SqlScripts/Modules/TransportRate`. Разовый снимок RateType/Currency не копировать. Если discovery найдёт существующий enum этого справочника, остановиться по правилу skill.
4. Релизная копия — `Migrations/2.3.49/` (текущий `VersionPrefix`, папка ещё не закрыта). `2.3.48` не менять. Дельта — следующий свободный номер после уже лежащих файлов. Contractor занимает `001`–`007`, `008` — снимок `All_Programmability.sql`. SVT-5291 и постановки, которые попадут в папку раньше, забирают номера до этой задачи; `009` заранее не занимать. Снимок programmability пересобрать из SoT и оставить последним файлом: прежний снимок в середине не остаётся. В `C:\publish` само по себе не копировать.
5. C# только после наката и проверки SQL на `localhost` / `mdm`.
6. Unit contract-тест DTO и SyncJob; integration smoke грида, карточки и FTS по текстовым полям, которые discovery оставила в гриде; снимок OpenAPI обновить по skill.

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\RateAutoSelectionSetting\`
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\RateAutoSelectionSetting\`
- [ ] SyncJob зарегистрирован, снимок заполнен. В Sources нет узлов и продукта TransportRate. Стабильные словари в Sources не входят
- [ ] Грид, карточка и выгрузка читают локальную базу
- [ ] `AddCatalogApi` (`rate-auto-selection-settings`)
- [ ] Тесты по skill
- [ ] Пункт 15.10.20 в backlog не отмечать до ревью
