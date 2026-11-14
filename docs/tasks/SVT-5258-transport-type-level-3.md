> Архив реализованной работы. Не исходные требования и не алгоритм новых задач. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник TransportType_level_3 (Типы транспорта, 3 уровень)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси (Angular) на **Blazor Server + MS SQL**. Сейчас тиражируем справочники **только для чтения** (грид, карточка, Excel, API для Атласа). Запись (редактор/загрузчик) — позже, **не в этой задаче**.

Источник истины по составу и приоритетам:  
`.cursor\skills\svt-architecture\catalogs-scope.md`  
Чеклист работ:  
`.cursor\skills\svt-architecture\backlog.md` → пункт **3.3.25**

### Что сделать

Перенести из легаси справочник **`TransportType_level_3`** (Типы транспорта 3 уровень, PrimitiveEntityInfo id **2069**) в СВТ 2.0 как **read-only** модуль:

- грид + карточка + выгрузка Excel
- пункт меню (домен **Маршруты**; в реестре **меню ЕО = да**)
- регистрация в **API** (`AddCatalogApi`), чтобы Атлас мог читать справочник

Это **четвёртый** (последний) справочник в очереди Атласа (`Атлас = 4`): коды `transportTypeCode` для smart-lookup; обобщение тип → вид.

### Как делать (обязательный алгоритм)

1. Прочитать skill `.cursor\skills\add-reference-book\SKILL.md` и следовать ему **целиком**.
2. Свериться со строкой `TransportType_level_3` в `.cursor\skills\svt-architecture\catalogs-scope.md` (домен, меню, приоритет Атласа, комментарий).
3. **Эталон по типу справочника:** `BlazorSvt\Modules\RateType\` + `BlazorSvt\SqlScripts\Modules\RateType\` — **стабильный словарь с enum**: разовая заливка снимка, **без** SyncJob / watermark / PopulateAffectedKeys / RowVer.
4. Справочник уже в классе «стабильный словарь с enum» (enum `TransportTypeLevel3Ru` / `TransportTypeLevel3En` в `BlazorSvt\Platform\Domain\IdsEnum\`). Перед генерацией — сверить неархивные строки легаси с enum; при расхождении — **стоп**, спросить, enum самому не переписывать. Имя модуля/папки — системное `TransportType_level_3` (PrimitiveEntityInfo.Name) по skill.
5. C# писать **только после** успешного наката и проверки SQL на локальной БД `mdm` (`localhost`).
6. После модуля: unit contract-тест DTO (SyncJob не тестировать — job нет); integration smoke грида/FTS; `CatalogApiCoverageTests` зелёный; при изменении контракта API — обновить снимок OpenAPI по правилу в skill/patterns.

Подключение к БД: `Server=localhost`, база `mdm`, логин и пароль из `Database:MdmDb` в `BlazorSvt\appsettings.json`. **Файл `appsettings.json` не менять.**

### Важно не перепутать

| Это | Не это |
|-----|--------|
| `TransportType_level_3` — типы 3 уровня | `TransportType` — тип транспорта (другой справочник) |
| `TransportKind` — вид транспорта | соседний; схлопывание Kind/Type/Level3 — **только после Gate D**, в этой задаче as-is |
| Только **RO** сейчас | Редактор/загрузчик — волна записи Маршрутов (веха 9), не сейчас |
| Стабильный словарь → **без** инкрементального sync | Не копировать SyncJob с LocationsNodes / TransportRate |

### Критерии готовности

- [x] Модуль `BlazorSvt\Modules\TransportType_level_3\` (List/Detail/Module/Resources), меню в домене Маршруты, видимость по реестру
- [x] SQL в `BlazorSvt\SqlScripts\Modules\TransportType_level_3\` (Structure + Programmability SnapshotSource/Detail); **без** SyncJob/PopulateAffectedKeys
- [x] Разовая заливка снимка; грид/карточка/Excel работают локально
- [x] `AddCatalogApi` зарегистрирован; API отдаёт каталог (список/схема/поиск/карточка/export)
- [x] Тесты по образцу RateType / skill; покрытие API не красное
- [x] В backlog можно отметить **3.3.25** (после ревью)

### Документы «прочитать до кода» (по порядку)

1. `.cursor\skills\svt-architecture\SKILL.md` — зачем проект и как устроен
2. `.cursor\skills\svt-architecture\catalogs-scope.md` — строка TransportType_level_3 + раздел «Очередь RO для API Атласа»
3. `.cursor\skills\add-reference-book\SKILL.md` — особенно раздел **«Стабильный словарь с enum»**
4. Код-эталон: `BlazorSvt\Modules\RateType\`