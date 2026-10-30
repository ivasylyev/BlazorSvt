## Задача: read-only справочник Currency (Валюты)

### Контекст (для новичка)

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси (Angular) на **Blazor Server + MS SQL**. Сейчас тиражируем справочники **только для чтения** (грид, карточка, Excel, API для Атласа). Запись (редактор/загрузчик) — позже, **не в этой задаче**.

Источник истины по составу и приоритетам:  
`.cursor\skills\svt-architecture\catalogs-scope.md`  
Чеклист работ:  
`.cursor\skills\svt-architecture\backlog.md` → пункт **0.2.3.61**

### Что сделать

Перенести из легаси справочник **`Currency`** (Валюты, PrimitiveEntityInfo id **2041**) в СВТ 2.0 как **read-only** модуль:

- грид + карточка + выгрузка Excel
- пункт меню (домен **Вспомогательные**; в реестре **меню ЕО = нет** → виден только Админу)
- регистрация в **API** (`AddCatalogApi`), чтобы Атлас мог читать справочник

Это **первый** справочник в очереди Атласа (`Атлас = 1`): коды валют (`currencyCode`) нужны для умного подбора ставки.

### Как делать (обязательный алгоритм)

1. Прочитать skill `.cursor\skills\add-reference-book\SKILL.md` и следовать ему **целиком**.
2. Свериться со строкой `Currency` в `.cursor\skills\svt-architecture\catalogs-scope.md` (домен, меню, приоритет Атласа, комментарий).
3. **Эталон по типу справочника:** `BlazorSvt\Modules\RateType\` + `BlazorSvt\SqlScripts\Modules\RateType\` — это тоже **стабильный словарь с enum**: разовая заливка снимка, **без** SyncJob / watermark / PopulateAffectedKeys / RowVer.
4. Currency уже числится в skill как стабильный словарь с enum (как RateType). Перед генерацией — сверить неархивные строки легаси с enum в коде; при расхождении — **стоп**, спросить, enum самому не переписывать.
5. C# писать **только после** успешного наката и проверки SQL на локальной БД `mdm` (`localhost`).
6. После модуля: unit contract-тест DTO; integration smoke грида/FTS по образцу других модулей; `CatalogApiCoverageTests` зелёный; при изменении контракта API — обновить снимок OpenAPI по правилу в skill/patterns.

Подключение к БД: `Server=localhost`, база `mdm`, логин и пароль из `Database:MdmDb` в `BlazorSvt\appsettings.json`. **Файл `appsettings.json` не менять.**

### Важно не перепутать

| Это | Не это |
|-----|--------|
| `Currency` — валюты (словарные коды) | `CurrencyRate` — плановые курсы (отдельная задача) |
| Только **RO** сейчас | Редактор/загрузчик Currency — MVP 0.8, не сейчас |
| Стабильный словарь → **без** инкрементального sync | Не копировать SyncJob с TransportRate «на всякий случай» |

### Критерии готовности

- [ ] Модуль `BlazorSvt\Modules\Currency\` (List/Detail/Module/Resources), меню в домене Вспомогательные, видимость по реестру
- [ ] SQL в `BlazorSvt\SqlScripts\Modules\Currency\` (Structure + Programmability SnapshotSource/Detail); **без** SyncJob/PopulateAffectedKeys
- [ ] Разовая заливка снимка; грид/карточка/Excel работают локально
- [ ] `AddCatalogApi` зарегистрирован; API отдаёт каталог (список/схема/поиск/карточка/export)
- [ ] Тесты по образцу RateType / skill; покрытие API не красное
- [ ] В backlog можно отметить **0.2.3.61** (после ревью)

### Документы «прочитать до кода» (по порядку)

1. `.cursor\skills\svt-architecture\SKILL.md` — зачем проект и как устроен
2. `.cursor\skills\svt-architecture\catalogs-scope.md` — строка Currency + раздел «Очередь RO для API Атласа»
3. `.cursor\skills\add-reference-book\SKILL.md` — особенно раздел **«Стабильный словарь с enum»**
4. Код-эталон: `BlazorSvt\Modules\RateType\`