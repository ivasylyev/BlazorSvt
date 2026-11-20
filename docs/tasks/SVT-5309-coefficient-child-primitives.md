> Рабочая постановка SVT-5309. При расхождении с кодом и skill верны код и skill.

## Задача: таблицы дочерних примитивов на карточке Coefficient

### Проблема

Карточка справочника показывает только атрибуты основного примитива. Записи дочерних примитивов (child primitive) на карточке не видны. У Coefficient это условия и составляющие коэффициента: чтобы их увидеть, пользователю приходится идти в легаси.

### Что меняется в работе

В карточке Coefficient после всех групп атрибутов появляются две группы, каждая с таблицей строк: «Условие по коэффициенту» и «Составляющая по коэффициенту». На английском интерфейсе они называются «Coefficient conditions» и «Coefficient components». В таблице нет фильтров и сортировки. Если строк больше предела, под таблицей выводится «Показано N из M» («Shown N of M»).

### Контекст

- Термин `child primitive` — `.cursor/skills/svt-architecture/glossary.md`.
- Сущность реестра `Coefficient` — EntityInfo 2108, основной примитив `Coefficient` (PEI 2081). Строка реестра — `.cursor/skills/svt-architecture/catalogs-scope.md`.
- Дочерние примитивы:
  - `CoefficientConditionValue` — PEI 2080, `dbo.vw_CoefficientConditionValue`, «Условие по коэффициенту»;
  - `CoefficientComponentValue` — PEI 2083, `dbo.vw_CoefficientComponentValue`, «Составляющая по коэффициенту».
- Связь с родителем: `CoefficientId` в дочернем view = `PrimitiveEntityItem.ParentId` = `vw_Coefficient.Id`.
- Текущая карточка: `BlazorSvt/Platform/Grid/Components/GenericDetailView.razor`, настройки — `BlazorSvt/Modules/Coefficient/Detail/CoefficientDetailSettingsService.cs`, проекция — `BlazorSvt/SqlScripts/Modules/Coefficient/Programmability/vw_Coefficient_Detail.sql`.

### Что сделать

**SQL**
- Создать проекции `v2.vw_CoefficientConditionValue_Detail` и `v2.vw_CoefficientComponentValue_Detail` в `BlazorSvt/SqlScripts/Modules/Coefficient/Programmability/`.
  - Источник — `dbo.vw_{Child}`. Проекция отдаёт `CoefficientId`, `Id`, `PrimitiveEntityDataStateId` и колонки таблицы.
  - Ссылки разворачиваются:
    - `TransportKind` — через enum `TransportKindRu` / `TransportKindEn`;
    - `TransportTypeLevel2` — через enum `TransportTypeRu` / `TransportTypeEn`;
    - `FromLocation`, `ToLocation` — имя узла из `vw_LocationsNodes` (`Name_ru` / `Name_en`), без кода;
    - `VehicleType` — `Name` из `vw_VehicleType`;
    - `Component` — `Name` из `vw_CoefficientComponent`.
- Запрос строк секции отбирает по `CoefficientId` и по набору состояний, переданному параметром. Строки упорядочены по `Id` и ограничены пределом. Число M — отдельный `COUNT` с тем же отбором.
- В `vw_Coefficient_Detail.sql` обновить комментарий «Дочерние CoefficientConditionValue и CoefficientComponentValue не выводятся».

**Platform**
- Добавить обобщённую дочернюю секцию: заголовок группы, загрузчик строк по ключу родителя, колонки с подписями.
- `GenericDetailView` рисует дочерние секции после всех групп родителя.
- Кнопка «Развернуть/Свернуть всё» учитывает дочерние группы.
- Секция без строк не выводится.
- Строки секции загружаются отдельно от основной детали. Отмена — тем же токеном, что у основной детали.
- Сбой загрузки секции: `LogError`, сообщение выводится внутри этой секции так же, как у основной детали в `GenericDetailView`. Остальная карточка работает.
- В секции нет фильтров, сортировки, экспорта и выбора колонок. Пустая ячейка остаётся пустой. Колонка, пустая во всех строках, не скрывается.
- Предел строк — `GridOptions.ChildSectionRowLimit`, по умолчанию 100. Ключ `"ChildSectionRowLimit": 100` добавить в секцию `Grid` файла `BlazorSvt/appsettings.json`.
- Если строк больше предела, под таблицей выводится «Показано N из M» / «Shown N of M». Строки — в `Platform.resx` и `Platform.ru-RU.resx`.

**Отбор строк (C#)**
- C# вычисляет набор состояний по `IsArchive` родителя из `CoefficientDetailDto`:
  - `IsArchive = 0` — `{1}`;
  - `IsArchive = 1` — `{1, 2}`.
- Набор передаётся параметром в запрос.
- Unit-тест проверяет обе ветки.

**Модуль Coefficient**
- Свои DTO на каждый дочерний примитив. `CoefficientDetailDto` не меняется.
- Регистрация двух секций в модуле. Порядок групп: «Условие по коэффициенту», затем «Составляющая по коэффициенту».
- Колонки «Условие по коэффициенту» по порядку: `Code`, `Except`, `TransportKind`, `TransportTypeLevel2`, `VehicleType`, `FromLocation`, `ToLocation`.
- Колонки «Составляющая по коэффициенту» по порядку: `Code`, `Component`, `Value`.
- Колонка состояния строки видна только у архивного родителя. Подпись «Архив» / «Archive», формат как у `AddArchiveStatus` в `DetailSettingsBuilder`.
- `Except` показывается как «Да/Нет», как `AddYesNo`. `Value` — 4 знака после запятой.
- Подписи — в `Coefficient.resx` и `Coefficient.ru-RU.resx`:

| Поле | RU | EN |
|---|---|---|
| группа 2080 | Условие по коэффициенту | Coefficient conditions |
| группа 2083 | Составляющая по коэффициенту | Coefficient components |
| `Code` | Код | Code |
| `Except` | Исключать | Exclude |
| `TransportKind` | Вид транспорта | Transport kind |
| `TransportTypeLevel2` | Тип транспорта (2 уровень) | Transport type (level 2) |
| `VehicleType` | Тип подвижного состава | Rolling stock type |
| `FromLocation` | Направление от | From |
| `ToLocation` | Направление до | To |
| `Component` | Составляющая | Component |
| `Value` | Значение | Value |

**Тесты**
- Unit-тест: вычисление набора состояний для обеих веток `IsArchive`.
- Unit-тест: сборка настроек двух секций (порядок групп, состав и порядок колонок, подписи).
- Integration smoke: обе проекции по `CoefficientId = 26857587` возвращают строки (по одной в локальной `mdm`), ссылки развёрнуты в имена.

### Что не входит

- API, снимок OpenAPI и полная Excel-выгрузка: дочерние строки туда не попадают.
- Snapshot, синхронизация, `RowVer` и каскад для дочерних примитивов.
- Постраничный вывод в секции.
- Колонки `TransportType` (3 уровень), `ProductGroup` и `Product`: у них ноль заполненных неархивных значений (фильтр §3 `discovery.md`).
- Дочерние примитивы других справочников.
- Правка `add-reference-book` и `svt-catalog-migration-orchestrator` под эту задачу — после Coefficient.

### Решения

- Карточка читает проекцию легаси, не snapshot — правило `svt-architecture-principles` (CQRS).
- Ссылки на чужие справочники разворачиваются в SQL-проекции, а не через классы чужих модулей — правило `svt-development-patterns`, «Модульная регистрация».
- Отбор строк по состоянию родителя — бизнес-правило, поэтому в C# с unit-тестом. SQL только фильтрует по переданному набору — правило `svt-architecture-principles`.
- Метаданные легаси (`EntityStructure`, `AttributeSettings`) в рантайме не читаются, состав колонок фиксируется в коде. Типизированная модель — `svt-architecture-principles`. Демонтаж легаси — `.cursor/skills/svt-architecture/cutover.md`, Gate D.
- Состав колонок — короткий список §1 после фильтра §3 — `.cursor/skills/add-reference-book/discovery.md`. `Code` показывается всегда, исключение — позже решением пользователя.
- Английские подписи из таблицы выше уже записаны в `.cursor/skills/add-reference-book/grid-column-glossary.md`, раздел Coefficient.
- `appsettings.json` меняется только добавлением ключа `Grid:ChildSectionRowLimit`. Это точечное исключение из правила проекта, согласованное в разговоре.

### Как делать

- Skill `.cursor/skills/add-reference-book/` — `discovery.md` (§1, §3, §5), `sql.md` (SoT, Programmability, релизная копия), `csharp.md`.
- Эталон модуля — `BlazorSvt/Modules/Coefficient`, `BlazorSvt/SqlScripts/Modules/Coefficient`.
- Накат на `localhost` / `mdm` — skill `create-programmability`.

### Как проверяем

- Unit-тест набора состояний опровергает правило «активный родитель — `{1}`, архивный — `{1, 2}`».
- Unit-тест настроек секций опровергает порядок групп, состав и порядок колонок и подписи RU/EN.
- Integration smoke проекций на `26857587` опровергает связь по `CoefficientId` и разворот ссылок.
- `OpenApiContractTests` опровергает «API не меняется».
- Допущения стенда (тесты их не видят): ветка архивного родителя на данных, надпись «Показано N из M», скрытие пустой группы, кнопка «Развернуть/Свернуть всё», вывод сбоя внутри секции.

### Критерии готовности

- [ ] Карточка Coefficient `26857587` показывает группы «Условие по коэффициенту» и «Составляющая по коэффициенту» после групп родителя
- [ ] Колонки и подписи — как в таблице «Что сделать», ссылки показаны именами
- [ ] У активного родителя видны только строки в состоянии 1, у архивного — в состояниях 1 и 2
- [ ] Больше `Grid:ChildSectionRowLimit` строк не выводится, под таблицей «Показано N из M» / «Shown N of M»
- [ ] API, снимок OpenAPI и полная Excel-выгрузка не изменились
- [ ] Unit- и integration-тесты из «Что сделать» проходят, `dotnet build` / `dotnet test` OK

### Заметки

- В локальной `mdm` все 126 коэффициентов активны. Ветку архивного родителя подтверждает только unit-тест.
- У `vw_VehicleType` и `vw_CoefficientComponent` нет английского имени: на обоих языках показывается `Name`.
- Объёмы в локальной `mdm`: условий 180 (не больше 5 на коэффициент), составляющих 688 (не больше 14). Предел 100 на Coefficient не срабатывает.
- Пробел: пункт `backlog.md` для задачи в разговоре не назван.
