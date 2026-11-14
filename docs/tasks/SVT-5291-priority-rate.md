> Рабочая постановка SVT-5291. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник Priority_Rate (Приоритеты типов ставок)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Эта задача переносит справочник **только для чтения** (грид, карточка, Excel, API). Редактор и загрузчик в реестре нужны, но в эту задачу не входят.

Источник истины по составу: `.cursor\skills\svt-architecture\catalogs-scope.md`.  
Чеклист: `.cursor\skills\svt-architecture\backlog.md` → пункт **3.3.5**.

### Что сделать

Перенести сущность реестра **Priority_Rate** (EntityInfo **2144**, «Приоритеты типов ставок») как read-only модуль.

Имя модуля по skill, шаг 0: `PriorityRate` (`Priority_Rate` → без подчёркивания, первая буква каждого сегмента заглавная). `{LegacyName}` — имя основного примитива из `EntityStructure`; его не подставлять именем реестра, пока шаг 0 не подтвердит, что они совпадают.

Из реестра, без discovery:

- домен **Ставки**, меню ЕО = да, сложность Средняя, пр. RO = 2
- комментарий реестра: вспомогательный для ставок
- в класс «стабильный словарь с enum» в skill не входит

- грид + карточка + выгрузка Excel
- пункт меню домена **Ставки**
- watermark-sync (снимок, SyncJob, RowVer, SyncState), эталон `Modules/MTR`
- регистрация в API по skill (`AddCatalogApi`); код kebab-case выбирается skill, не фиксируется здесь

Состав колонок, порядок, подписи, дочерние примитивы и пустые ссылки — из discovery и согласования. В постановку их не додумывать. Пустой атрибут на неархивных строках на экран не выводится.

### Как делать

1. Skill `.cursor\skills\add-reference-book\SKILL.md` целиком.
2. Строка `Priority_Rate` в `catalogs-scope.md`.
3. Эталон — `Modules/MTR` и `SqlScripts/Modules/MTR`. Не копировать разовый снимок RateType/Currency. Если discovery найдёт существующий enum, остановиться по правилу skill.
4. Релизная копия — `Migrations/2.3.49/`, следующим номером. Папку `2.3.48` не менять.
5. C# только после наката и проверки SQL на `localhost` / `mdm`.
6. Unit contract-тест DTO и SyncJob; integration smoke грида, карточки и FTS; снимок OpenAPI обновить по skill.

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\PriorityRate\`
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\PriorityRate\`
- [ ] SyncJob зарегистрирован, снимок заполнен
- [ ] Грид, карточка и выгрузка читают локальную базу
- [ ] `AddCatalogApi`
- [ ] Тесты по skill
- [ ] Пункт 3.3.5 в backlog не отмечать до ревью
