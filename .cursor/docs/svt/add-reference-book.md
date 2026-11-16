# add-reference-book

## Что делает

`add-reference-book` собирает read-only справочник из легаси MDM: discovery атрибутов, snapshot SQL, programmability, C#-модуль, грид и меню. Скил покрывает RO: грид, карточка, sync. Редактор и загрузчик остаются вехами backlog 7–10. Домен «Заявки» ведётся модулем 4. Сущности раздела «Вне скоупа», в том числе `TransportRequest` и `MassiveRequest`, в этот навык не входят.

## Когда брать

Агент берёт скил сам по запросу добавить справочник, перенести `PrimitiveEntityInfo` или собрать RO grid, карточку и export.

| Ситуация | Куда |
|---|---|
| Справочник в скоупе, RO grid + detail + export | Этот скил |
| Справочник вне скоупа | Остановиться. Состав — `catalogs-scope.md` в [`svt-architecture`](svt-architecture.md) |
| Редактор или пакетная загрузка | backlog 7–10 |
| Заявки на расчёт | модуль 4 |
| Накатить уже написанные SP на localhost | [`create-programmability`](create-programmability.md) |
| Опубликовать скрипты в `C:\publish\v2` | [`publish-all-sql`](publish-all-sql.md) |

## Что нужно до старта

- Системное имя из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`). Без имени шаги не начинаются.
- Справочник в скоупе. Домен, видимость меню ЕО и тип работ — из реестра.
- Локальная база `mdm` на `localhost` для discovery и верификации. Логин и пароль — `Database:MdmDb`. `appsettings.json` не менять.

## Два вида снимка

| Вид | Эталон | Что есть у среза |
|---|---|---|
| watermark-sync | `Modules/TransportRate` | SyncJob, `PopulateAffectedKeys`, инициализация `SyncState` |
| Стабильный словарь с enum, разовый снимок | `Modules/Currency` | Снимок заполняется один раз. SyncJob нет |

`{Entity}` — имя модуля, папки, DTO и снимка. `{LegacyName}` — имя основного примитива. Правило `_`, таблица переименований и список словарей с enum — в [`SKILL.md`](../../skills/add-reference-book/SKILL.md). `TransportType` в класс разового снимка не входит: собственный RO-грид делается со watermark-sync, хотя enum уже есть.

Фазы читаются по одной: `discovery.md`, затем `sql.md`, затем `csharp.md`. Короткие заголовки грида после генерации resx — `grid-column-glossary.md`.

Запросы к легаси и DDL в discovery выполняет read-only субагент. Если родитель Claude, его модель — Grok 4.7. Resx, SQL и C# остаются у родителя.

## Частые вопросы

**У справочника уже есть enum. Делать разовый снимок?**

Только если он в классе стабильного словаря с enum. Список и исключения — в `SKILL.md`. Enum сам по себе класс не задаёт.

**Чем `{Entity}` отличается от `{LegacyName}`.**

`{Entity}` — модуль и снимок. `{LegacyName}` — основной примитив в discovery-SQL и легаси-view. Имена фиксируются до SQL и C#.

**Редактор, загрузчик, заявки.**

Редактор и загрузчик — backlog 7–10. Заявки на расчёт — модуль 4.

## Сработало, если

- Имена `{LegacyName}` и `{Entity}` зафиксированы до SQL и C#.
- У watermark-sync есть SyncJob, `PopulateAffectedKeys` и инициализация `SyncState`. У стабильного словаря с enum снимок заполнен один раз, без этих трёх.
- В `catalogs-scope.md` колонка RO остаётся `[ ]`, пока справочник не на проде.
- Пункт backlog не отмечен до ревью.
- `dotnet build` и `dotnet test` проходят.

## Куда встаёт

Шаг после сверки с [`svt-architecture`](svt-architecture.md) / `catalogs-scope.md`. Дальше накат на localhost — [`create-programmability`](create-programmability.md), копия в каталог деплоя — [`publish-all-sql`](publish-all-sql.md). Цепочка — [`docs/README.md`](../README.md).
