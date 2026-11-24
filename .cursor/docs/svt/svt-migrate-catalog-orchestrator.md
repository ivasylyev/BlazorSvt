# svt-migrate-catalog-orchestrator

## Что делает

`svt-migrate-catalog-orchestrator` мигрирует один или несколько read-only справочников конвейером субагентов: classify → discovery → решения → SQL на весь батч → C# и docs → finalize → ревью. Код оркестратор не пишет: правила берёт из [`svt-migrate-catalog`](svt-migrate-catalog.md).

## Когда брать

Только по явному вызову: человек называет скил (в Cursor — `/svt-migrate-catalog-orchestrator`). Сам агент его не берёт (`disable-model-invocation: true`); обычная просьба добавить или перенести справочник ведёт в [`svt-migrate-catalog`](svt-migrate-catalog.md).

| Ситуация | Куда |
|---|---|
| Справочник в скоупе, RO grid + detail + export (один или пачка), скил вызван явно | Этот скил |
| Та же задача без явного вызова | [`svt-migrate-catalog`](svt-migrate-catalog.md) |
| Поправить правила фаз, именование, таблицу enum-класса | [`svt-migrate-catalog`](svt-migrate-catalog.md) |
| Справочник вне скоупа | Остановиться. Состав — `catalogs-scope.md` в [`svt-architecture`](svt-architecture.md) |
| Редактор или пакетная загрузка | backlog, с вехи 21 |
| Накатить уже написанные SP на localhost | [`svt-create-programmability`](svt-create-programmability.md) |
| Опубликовать скрипты в `C:\publish\v2` | [`svt-publish-all-sql`](svt-publish-all-sql.md) |

## Что нужно до старта

- Системные имена из реестра (`EntityInfo.Name`, колонка «Имя» в `catalogs-scope.md`). Без имён конвейер не стартует.
- Справочники в скоупе. Домен, меню ЕО, тип работ — из реестра.
- Чистое git-дерево (или решение пользователя). Локальная `mdm` на `localhost`; логин — `Database:MdmDb`. `appsettings.json` не менять.

## Идея скила

Один конвейер на simple и complex: класс задаёт таблица «Стабильный словарь с enum» в `svt-migrate-catalog`. SQL на весь батч один агент (единая нумерация релизной папки и один снимок programmability); C# — параллельно по справочнику. Спеки и отчёты ролей — файлы в `%LOCALAPPDATA%\svt-agent-team\…`, не чат.

## Частые вопросы

**Один справочник — другой скил?**

Нет. Батч из одного имени, стадии те же.

**Как готовый модуль получает таблицы дочерних примитивов?**

Запуском «проверь {Entity}». Discovery заполняет раздел 3а spec, ревьюер пишет отсутствующую группу дефектом. Исправление — с согласия пользователя: sql (только проекции и снимок), csharp (секции и тесты), finalize, повторный ревью.

**Где правила SQL и C#?**

Только в [`svt-migrate-catalog`](svt-migrate-catalog.md). Роли оркестратора ссылаются на фазы, не копируют их.

## Сработало, если

- В ответе — класс и статус по каждому имени, числа тестов, папка релиза и снимок OpenAPI.
- Бэклог и RO в catalogs-scope не отмечены; коммита нет.
- Страницы `/{entity}` пользователь проверяет в обычном браузере.

## Куда встаёт

Шаг после сверки с [`svt-architecture`](svt-architecture.md) / `catalogs-scope.md`. Канон — [`svt-migrate-catalog`](svt-migrate-catalog.md). Дальше накат — [`svt-create-programmability`](svt-create-programmability.md), деплой-копия — [`svt-publish-all-sql`](svt-publish-all-sql.md). Цепочка — [`docs/README.md`](../README.md).
