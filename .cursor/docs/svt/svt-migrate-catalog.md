# svt-migrate-catalog

## Что делает

`svt-migrate-catalog` собирает read-only справочник из легаси MDM: discovery атрибутов, snapshot SQL, programmability, C#-модуль, грид и меню. Скил покрывает RO: грид, карточка, sync. Редактор и загрузчик остаются пакетами бэклога с вехи 21. Домен «Заявки» ведётся в вехах 9–11. Сущности раздела «Вне скоупа», в том числе `TransportRequest` и `MassiveRequest`, в этот навык не входят.

## Когда брать

Агент берёт скил сам по запросу добавить справочник, перенести `PrimitiveEntityInfo` или собрать RO grid, карточку и export.

| Ситуация | Куда |
|---|---|
| Справочник в скоупе, RO grid + detail + export | Этот скил |
| На карточке готового модуля нет таблиц дочерних примитивов | Этот скил, `child-primitives.md`, раздел «Готовый модуль» |
| Справочник вне скоупа | Остановиться. Состав — `catalogs-scope.md` в [`svt-architecture`](svt-architecture.md) |
| Редактор или пакетная загрузка | backlog, с вехи 21 |
| Заявки на расчёт | вехи 9–11 |
| Накатить уже написанные SP на localhost | [`svt-create-programmability`](svt-create-programmability.md) |
| Собрать пакет версии в `C:\publish` | [`svt-publish`](svt-publish.md) |

## Что нужно до старта

- Системное имя из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`). Без имени шаги не начинаются.
- Справочник в скоупе. Домен, видимость меню ЕО и тип работ — из реестра.
- Локальная база `mdm` на `localhost` для discovery и верификации. Логин и пароль — `Database:MdmDb`. `appsettings.json` не менять.

## Два вида снимка

| Вид | Эталон | Что есть у среза |
|---|---|---|
| watermark-sync | `Modules/TransportRate` | SyncJob, `PopulateAffectedKeys`, инициализация `SyncState` |
| Стабильный словарь с enum, разовый снимок | `Modules/Currency` | Снимок заполняется один раз. SyncJob нет |

`{Entity}` — имя модуля, папки, DTO и снимка. `{LegacyName}` — имя основного примитива. Правило `_`, таблица переименований и список словарей с enum — в [`SKILL.md`](../../skills/svt-migrate-catalog/SKILL.md). `TransportType` в класс разового снимка не входит: собственный RO-грид делается со watermark-sync, хотя enum уже есть.

Фазы читаются по одной: `discovery-rules.md`, затем `sql.md`, затем `csharp.md`. Запросы фазы 1 — `discovery-queries.md`, только на время `SELECT`. Короткие заголовки грида — раздел «Правила» в `grid-column-glossary.md` и поиск имени поля.

Запросы к легаси и DDL в discovery выполняет read-only субагент. Если родитель Claude, его модель — Grok 4.7. Resx, SQL и C# остаются у родителя.

## Частые вопросы

**У справочника уже есть enum. Делать разовый снимок?**

Только если он в классе стабильного словаря с enum. Список и исключения — в `SKILL.md`. Enum сам по себе класс не задаёт.

**Чем `{Entity}` отличается от `{LegacyName}`.**

`{Entity}` — модуль и снимок. `{LegacyName}` — основной примитив в discovery-SQL и легаси-view. Имена фиксируются до SQL и C#.

**Дочерние примитивы входят в модуль?**

Да, по умолчанию: каждый ребёнок с view и неархивными строками — группа-таблица после групп родителя. Исключения и вопросы пользователю — `child-primitives.md`. Связку (`link child`) показывают с полями связанной записи, список полей согласует пользователь. Snapshot, sync и API дети не меняют.

**Редактор, загрузчик, заявки.**

Редактор и загрузчик — backlog, с вехи 21. Заявки на расчёт — вехи 9–11.

## Сработало, если

- Имена `{LegacyName}` и `{Entity}` зафиксированы до SQL и C#.
- У watermark-sync есть SyncJob, `PopulateAffectedKeys` и инициализация `SyncState`. У стабильного словаря с enum снимок заполнен один раз, без этих трёх.
- У справочника с дочерними примитивами на карточке есть их группы-таблицы и тесты `{Entity}ChildSections*`.
- В `catalogs-scope.md` колонка RO остаётся `[ ]`, пока справочник не на проде.
- Пункт backlog не отмечен до ревью.
- `dotnet build` и `dotnet test` проходят.

## Куда встаёт

Шаг после сверки с [`svt-architecture`](svt-architecture.md) / `catalogs-scope.md`. Дальше накат на localhost — [`svt-create-programmability`](svt-create-programmability.md), пакет версии — [`svt-publish`](svt-publish.md). Цепочка — [`docs/README.md`](../README.md).
