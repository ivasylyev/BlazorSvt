# svt-create-programmability

## Что делает

`svt-create-programmability` накатывает SQL Programmability на локальную базу `mdm` (`localhost`): все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`. Structure-скрипты остаются на месте. Снимок `All_Programmability.sql` этот запуск не пишет. Каталог деплоя собирает [`svt-publish-all-sql`](svt-publish-all-sql.md).

## Когда брать

Человек вызывает `/create_programmability`. Агент берёт скил сам, когда просят накатить процедуры, функции или пересоздать SP, fn и view схемы v2. Оба входа ведут в один [`SKILL.md`](../../skills/svt-create-programmability/SKILL.md).

| Ситуация | Куда |
|---|---|
| Обновить SP, fn и view схемы v2 на локальной `mdm` | Этот скил |
| Собрать файлы деплоя в `C:\publish\v2` | [`svt-publish-all-sql`](svt-publish-all-sql.md) |
| Написать новый справочник | [`svt-migrate-catalog`](svt-migrate-catalog.md); накат — после SQL-фазы |

## Что нужно до старта

Запуск из корня репозитория `c:\Work\BlazorSVT`.

Имя базы, логин и пароль скрипт берёт из `Database:MdmDb` в `BlazorSvt/appsettings.json`. Сервер в команде всегда `localhost`. Поле Server из строки не используется. `appsettings.json` не менять.

Нужен SQL-логин (`User ID`). При `Trusted_Connection` скрипт не запускать. Команда и порядок файлов — в `SKILL.md`.

## Накат

Скил выполняет Programmability на `localhost` / `mdm` и останавливается на первой ошибке sqlcmd. Тот же порядок файлов у снимка `All_Programmability.sql` описан в `BlazorSvt/SqlScripts/Migrations/README.md`, раздел «Снимок All_Programmability».

## Частые вопросы

**После запуска обновился `C:\publish\v2`.**

Накат идёт на локальную `mdm`. Копия в каталог деплоя — [`svt-publish-all-sql`](svt-publish-all-sql.md).

**Выполнятся ли Structure-скрипты.**

Скил выполняет файлы Programmability. Structure остаётся как есть.

## Сработало, если

- В ответе есть число выполненных скриптов.
- При ошибке показан вывод sqlcmd, и выполнение остановлено.
- `appsettings.json` не изменён. Structure-скрипты не затронуты.

## Куда встаёт

Шаг после того, как [`svt-migrate-catalog`](svt-migrate-catalog.md) положил Programmability в SoT. Рядом [`svt-publish-all-sql`](svt-publish-all-sql.md) копирует исходники в каталог деплоя. Цепочка — [`docs/README.md`](../README.md).
