## Задача: read-only справочник MTR (Продукты)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси (Angular) на **Blazor Server + MS SQL**. Сейчас тиражируем справочники **только для чтения** (грид, карточка, Excel, API для Атласа). Запись (редактор/загрузчик) — позже, **не в этой задаче**.

Источник истины по составу и приоритетам:  
`.cursor\skills\svt-architecture\catalogs-scope.md`  
Чеклист работ:  
`.cursor\skills\svt-architecture\backlog.md` → пункт **0.2.3.33**

### Что сделать

Перенести из легаси справочник **`MTR`** (Продукты, PrimitiveEntityInfo id **2005**) в СВТ 2.0 как **read-only** модуль:

- грид + карточка + выгрузка Excel
- пункт меню (домен **Продукты**; в реестре **меню ЕО = да**)
- **watermark-sync** (снимок + SyncJob) — данные приходят по интеграции и уже участвуют в каскаде ставок
- регистрация в **API** (`AddCatalogApi`), чтобы Атлас мог читать справочник

Это **третий** справочник в очереди Атласа (`Атлас = 3`): коды продуктов (`productCode`) для smart-lookup; обобщение до транспортной группы — через `ProductGroup` (желательно уже перенесённый, 0.2.3.32).

### Как делать (обязательный алгоритм)

1. Прочитать skill `.cursor\skills\add-reference-book\SKILL.md` и следовать ему **целиком**.
2. Свериться со строкой `MTR` в `.cursor\skills\svt-architecture\catalogs-scope.md` (домен, меню, приоритет Атласа, комментарий).
3. **Эталон по типу справочника:** `BlazorSvt\Modules\LocationsNodes\` + `BlazorSvt\SqlScripts\Modules\LocationsNodes\` — RO со **снимком и SyncJob**, данные с интеграции (не паттерн RateType/Currency без sync).
4. MTR **не** входит в класс «стабильный словарь с enum»: нужны `SnapshotSource`, `PopulateAffectedKeys`, `SyncJob`, RowVer на основной PED, инициализация `SyncState`. В каскаде чужих job’ов MTR уже есть (`PrimitiveEntityData_1015` у TransportRate / AverageRateLevel3 / ParityRates) — при своём SyncJob сверить skill и комментарии PED → имя справочника.
5. C# писать **только после** успешного наката и проверки SQL на локальной БД `mdm` (`localhost`).
6. После модуля: unit contract-тест DTO + SyncJob; integration smoke грида/FTS; `CatalogApiCoverageTests` зелёный; при изменении контракта API — обновить снимок OpenAPI по правилу в skill/patterns.

Подключение к БД: `Server=localhost`, база `mdm`, логин и пароль из `Database:MdmDb` в `BlazorSvt\appsettings.json`. **Файл `appsettings.json` не менять.**

### Важно не перепутать

| Это | Не это |
|-----|--------|
| `MTR` — продукты | `MTR_DPO` — материалы DPO (отдельный пункт 0.2.3.35) |
| `MTR` — в скоупе | `MTRType` — вне скоупа, не переносить |
| `ProductGroup` — транспортные группы (Атлас=2) | соседний справочник; редактор у MTR **не** нужен (в реестре E/L = —) |
| Только **RO** сейчас | Редактор/загрузчик для MTR в скоупе не планируются |

Замечание из реестра («поменять столбцы местами, лёгкая/полная выгрузка») — **не** блокер RO: сначала discovery/грид по skill; переработку колонок и полной выгрузки не включать без отдельного согласования.

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\MTR\` (List/Detail/Sync/Module/Resources), меню в домене Продукты, видимость по реестру
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\MTR\` (Structure + Programmability SnapshotSource/Detail/PopulateAffectedKeys)
- [ ] SyncJob зарегистрирован; инкрементальный sync работает; снимок заполняется
- [ ] Грид/карточка/Excel работают локально
- [ ] `AddCatalogApi` зарегистрирован; API отдаёт каталог (список/схема/поиск/карточка/export)
- [ ] Тесты по образцу LocationsNodes / skill; покрытие API не красное
- [ ] В backlog можно отметить **0.2.3.33** (после ревью)

### Документы «прочитать до кода» (по порядку)

1. `.cursor\skills\svt-architecture\SKILL.md` — зачем проект и как устроен
2. `.cursor\skills\svt-architecture\catalogs-scope.md` — строка MTR + раздел «Очередь RO для API Атласа»
3. `.cursor\skills\add-reference-book\SKILL.md` — полный вертикальный срез **с sync** (не раздел «Стабильный словарь с enum»)
4. Код-эталон: `BlazorSvt\Modules\LocationsNodes\`