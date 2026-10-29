## Задача: read-only справочник ProductGroup (Транспортные группы)

### Контекст (для новичка)

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси (Angular) на **Blazor Server + MS SQL**. Сейчас тиражируем справочники **только для чтения** (грид, карточка, Excel, API для Атласа). Запись (редактор/загрузчик) — позже, **не в этой задаче**.

Источник истины по составу и приоритетам:  
`.cursor\skills\svt-architecture\catalogs-scope.md`  
Чеклист работ:  
`.cursor\skills\svt-architecture\backlog.md` → пункт **0.2.3.32**

### Что сделать

Перенести из легаси справочник **`ProductGroup`** (Транспортные группы, PrimitiveEntityInfo id **2002**) в СВТ 2.0 как **read-only** модуль:

- грид + карточка + выгрузка Excel
- пункт меню (домен **Продукты**; в реестре **меню ЕО = да**)
- **watermark-sync** (снимок + SyncJob) — справочник меняется и уже участвует в каскаде ставок
- регистрация в **API** (`AddCatalogApi`), чтобы Атлас мог читать справочник

Это **второй** справочник в очереди Атласа (`Атлас = 2`): транспортные группы нужны для обобщения в smart-lookup (продукт → транспортная группа).

### Как делать (обязательный алгоритм)

1. Прочитать skill `.cursor\skills\add-reference-book\SKILL.md` и следовать ему **целиком**.
2. Свериться со строкой `ProductGroup` в `.cursor\skills\svt-architecture\catalogs-scope.md` (домен, меню, приоритет Атласа, комментарий).
3. **Эталон по типу справочника:** `BlazorSvt\Modules\LocationsNodes\` + `BlazorSvt\SqlScripts\Modules\LocationsNodes\` — обычный RO со **снимком и SyncJob** (не копировать паттерн RateType/Currency без sync).
4. ProductGroup **не** входит в класс «стабильный словарь с enum»: нужны `SnapshotSource`, `PopulateAffectedKeys`, `SyncJob`, RowVer на основной PED, инициализация `SyncState`. В каскаде чужих job’ов ProductGroup уже есть (`PrimitiveEntityData_1013` у TransportRate / AverageRateLevel3 / ParityRates) — при добавлении своего SyncJob сверить skill и комментарии PED → имя справочника.
5. C# писать **только после** успешного наката и проверки SQL на локальной БД `mdm` (`localhost`).
6. После модуля: unit contract-тест DTO + SyncJob; integration smoke грида/FTS; `CatalogApiCoverageTests` зелёный; при изменении контракта API — обновить снимок OpenAPI по правилу в skill/patterns.

Подключение к БД: `Server=localhost`, база `mdm`, логин и пароль из `Database:MdmDb` в `BlazorSvt\appsettings.json`. **Файл `appsettings.json` не менять.**

### Важно не перепутать

| Это | Не это |
|-----|--------|
| `ProductGroup` — транспортные группы | `MTR` — продукты (следующий в очереди Атласа, 0.2.3.33) |
| `PlanFact` — другой справочник («Группа продуктов (План-факт)» в меню Confluence) | не путать с ProductGroup |
| Только **RO** сейчас | Редактор/загрузчик — волна записи Продуктов (MVP 0.7), не сейчас |
| Нужен **watermark-sync** | Не делать разовую заливку «как Currency» |

Замечание из реестра («поменять столбцы местами, лёгкая выгрузка») — **не** блокер RO: сначала discovery/грид по skill; переработку колонок не включать в эту задачу без отдельного согласования.

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\ProductGroup\` (List/Detail/Sync/Module/Resources), меню в домене Продукты, видимость по реестру
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\ProductGroup\` (Structure + Programmability SnapshotSource/Detail/PopulateAffectedKeys)
- [ ] SyncJob зарегистрирован; инкрементальный sync работает; снимок заполняется
- [ ] Грид/карточка/Excel работают локально
- [ ] `AddCatalogApi` зарегистрирован; API отдаёт каталог (список/схема/поиск/карточка/export)
- [ ] Тесты по образцу LocationsNodes / skill; покрытие API не красное
- [ ] В backlog можно отметить **0.2.3.32** (после ревью)

### Документы «прочитать до кода» (по порядку)

1. `.cursor\skills\svt-architecture\SKILL.md` — зачем проект и как устроен
2. `.cursor\skills\svt-architecture\catalogs-scope.md` — строка ProductGroup + раздел «Очередь RO для API Атласа»
3. `.cursor\skills\add-reference-book\SKILL.md` — полный вертикальный срез **с sync** (не раздел «Стабильный словарь с enum»)
4. Код-эталон: `BlazorSvt\Modules\LocationsNodes\`
