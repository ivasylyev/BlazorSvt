# svt-create-programmability

## Что делает

`svt-create-programmability` накатывает SQL Programmability на локальную базу `mdm` (`localhost`): все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`. Structure-скрипты остаются на месте. Снимок `All_Programmability.sql` этот запуск не пишет. Пакет версии собирает [`svt-publish`](svt-publish.md).

## Когда брать

Человек вызывает `/create_programmability`. Агент берёт скил сам, когда просят накатить процедуры, функции или пересоздать SP, fn и view схемы v2. Оба входа ведут в один [`SKILL.md`](../../skills/svt-create-programmability/SKILL.md).

| Ситуация | Куда |
|---|---|
| Обновить SP, fn и view схемы v2 на локальной `mdm` | Этот скил |
| Собрать пакет версии в `C:\publish` | [`svt-publish`](svt-publish.md) |
| Написать новый справочник | [`svt-migrate-catalog`](svt-migrate-catalog.md); накат — после SQL-фазы |

## Что нужно до старта

Запуск из корня репозитория `c:\Work\BlazorSVT`.

Имя базы, логин и пароль скрипт берёт из `Database:MdmDb` в `BlazorSvt/appsettings.json`. Сервер в команде всегда `localhost`. Поле Server из строки не используется. `appsettings.json` не менять.

Нужен SQL-логин (`User ID`). При `Trusted_Connection` скрипт не запускать. Команда и порядок файлов — в `SKILL.md`.

## Накат

Скил выполняет Programmability на `localhost` / `mdm` и останавливается на первой ошибке sqlcmd. Тот же порядок файлов у снимка `All_Programmability.sql` описан в `BlazorSvt/SqlScripts/Migrations/README.md`, раздел «Снимок All_Programmability».

## Частые вопросы

**После запуска обновился `C:\publish`.**

Накат идёт на локальную `mdm`. Пакет версии — [`svt-publish`](svt-publish.md).

**Выполнятся ли Structure-скрипты.**

Скил выполняет файлы Programmability. Structure остаётся как есть.

## Сработало, если

- В ответе есть число выполненных скриптов.
- При ошибке показан вывод sqlcmd, и выполнение остановлено.
- `appsettings.json` не изменён. Structure-скрипты не затронуты.

## Куда встаёт

Шаг после того, как [`svt-migrate-catalog`](svt-migrate-catalog.md) положил Programmability в SoT. Рядом [`svt-publish`](svt-publish.md) собирает пакет версии. Цепочка — [`docs/README.md`](../README.md).
