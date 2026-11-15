# add-reference-book

## Что делает

`add-reference-book` собирает read-only справочник из легаси MDM: discovery атрибутов, snapshot SQL, programmability, C#-модуль, грид и меню.

Навык покрывает RO: грид, карточка, sync. Редактор и загрузчик остаются вехами backlog 7–10. Домен «Заявки» ведётся модулем 4. Сущности раздела «Вне скоупа», в том числе `TransportRequest` и `MassiveRequest`, в этот навык не входят.

## Когда брать

| Ситуация | Куда |
|---|---|
| «Добавить справочник», перенос `PrimitiveEntityInfo`, RO grid + detail + export, справочник в скоупе | Этот скил |
| Справочник вне скоупа | Остановиться. Состав — `catalogs-scope.md` в [`svt-architecture`](svt-architecture.md) |
| Редактор или пакетная загрузка | backlog 7–10 |
| Заявки на расчёт | модуль 4 |
| Накатить уже написанные SP на localhost | [`create-programmability`](create-programmability.md) |
| Опубликовать скрипты в `C:\publish\v2` | [`publish-all-sql`](publish-all-sql.md) |

Внутри скила два вида снимка:

| Вид | Эталон |
|---|---|
| watermark-sync | `Modules/TransportRate` |
| Стабильный словарь с enum, разовый снимок | `Modules/Currency` |

## Что нужно до старта

- Системное имя из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`). Без имени шаги не начинаются.
- Справочник в скоупе. Домен, видимость меню ЕО и тип работ — из реестра.
- Локальная база `mdm` на `localhost` для discovery и верификации. Логин и пароль — `Database:MdmDb`. `appsettings.json` не менять.

## Как устроено

Процедура — [`SKILL.md`](../../skills/add-reference-book/SKILL.md). Фазы читаются по одной, до выполнения:

| Фаза | Файл |
|---|---|
| 1 | `discovery.md` |
| 2–3 | `sql.md` |
| 4–6 | `csharp.md` |

Короткие заголовки грида после генерации resx — `grid-column-glossary.md`.

`{Entity}` — имя модуля, папки, DTO и снимка. `{LegacyName}` — имя основного примитива. Правило `_` и отдельные переименования (`Rates` → `TransportRate`, `TransportLegs` → `TransportLeg`, `LegalType` → `Contractor`) записаны в `SKILL.md`.

Список стабильных словарей с enum и исключения из него — в `SKILL.md`, раздел «Стабильный словарь с enum». `TransportType` в этот класс не входит: собственный RO-грид делается со watermark-sync, хотя enum уже есть.

## Сработало, если

- Имена `{LegacyName}` и `{Entity}` зафиксированы до SQL и C#.
- У watermark-sync есть SyncJob, `PopulateAffectedKeys` и инициализация `SyncState`. У стабильного словаря с enum их нет: снимок заполняется один раз.
- В `catalogs-scope.md` колонка RO остаётся `[ ]`, пока справочник не на проде.
- Пункт backlog не отмечен до ревью.
- `dotnet build` и `dotnet test` проходят.

## Куда встаёт

После сверки с [`svt-architecture`](svt-architecture.md) / `catalogs-scope.md`. Накат Programmability на localhost — [`create-programmability`](create-programmability.md). Публикация в `C:\publish\v2` — [`publish-all-sql`](publish-all-sql.md).
