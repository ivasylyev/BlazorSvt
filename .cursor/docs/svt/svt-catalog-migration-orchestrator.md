# svt-catalog-migration-orchestrator

## Что делает

`svt-catalog-migration-orchestrator` мигрирует один или несколько read-only справочников конвейером субагентов: classify → discovery → решения → SQL на весь батч → C# и docs → finalize → ревью. Код оркестратор не пишет: правила берёт из [`add-reference-book`](add-reference-book.md).

## Когда брать

Только по явному вызову: человек называет скил (в Cursor — `/svt-catalog-migration-orchestrator`). Сам агент его не берёт (`disable-model-invocation: true`); обычная просьба добавить или перенести справочник ведёт в [`add-reference-book`](add-reference-book.md).

| Ситуация | Куда |
|---|---|
| Справочник в скоупе, RO grid + detail + export (один или пачка), скил вызван явно | Этот скил |
| Та же задача без явного вызова | [`add-reference-book`](add-reference-book.md) |
| Поправить правила фаз, именование, таблицу enum-класса | [`add-reference-book`](add-reference-book.md) |
| Справочник вне скоупа | Остановиться. Состав — `catalogs-scope.md` в [`svt-architecture`](svt-architecture.md) |
| Редактор или пакетная загрузка | backlog, с вехи 21 |
| Накатить уже написанные SP на localhost | [`create-programmability`](create-programmability.md) |
| Опубликовать скрипты в `C:\publish\v2` | [`publish-all-sql`](publish-all-sql.md) |

## Что нужно до старта

- Системные имена из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`). Без имён конвейер не стартует.
- Справочники в скоупе. Домен, меню ЕО, тип работ — из реестра.
- Чистое git-дерево (или решение пользователя). Локальная `mdm` на `localhost`; логин — `Database:MdmDb`. `appsettings.json` не менять.

## Идея скила

Один конвейер на simple и complex: класс задаёт таблица «Стабильный словарь с enum» в `add-reference-book`. SQL на весь батч один агент (единая нумерация релизной папки и один снимок programmability); C# — параллельно по справочнику. Спеки и отчёты ролей — файлы в `%LOCALAPPDATA%\svt-agent-team\…`, не чат.

## Частые вопросы

**Один справочник — другой скил?**

Нет. Батч из одного имени, стадии те же.

**Где правила SQL и C#?**

Только в [`add-reference-book`](add-reference-book.md). Роли оркестратора ссылаются на фазы, не копируют их.

## Сработало, если

- В ответе — класс и статус по каждому имени, числа тестов, папка релиза и снимок OpenAPI.
- Бэклог и RO в catalogs-scope не отмечены; коммита нет.
- Страницы `/{entity}` пользователь проверяет в обычном браузере.

## Куда встаёт

Шаг после сверки с [`svt-architecture`](svt-architecture.md) / `catalogs-scope.md`. Канон — [`add-reference-book`](add-reference-book.md). Дальше накат — [`create-programmability`](create-programmability.md), деплой-копия — [`publish-all-sql`](publish-all-sql.md). Цепочка — [`docs/README.md`](../README.md).
