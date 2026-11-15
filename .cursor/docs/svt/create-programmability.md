# create-programmability

## Что делает

`create-programmability` накатывает SQL Programmability на локальную базу `mdm` (`localhost`): все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`.

Structure-скрипты не выполняет. Снимок `All_Programmability.sql` не пишет. Тот же порядок файлов у снимка описан в `BlazorSvt/SqlScripts/Migrations/README.md`, раздел «Снимок All_Programmability».

## Когда брать

| Ситуация | Куда |
|---|---|
| Обновить SP, fn и view схемы v2 на локальной `mdm` | Этот скил или `/create_programmability` |
| Собрать файлы деплоя в `C:\publish\v2` | [`publish-all-sql`](publish-all-sql.md) |
| Написать новый справочник | [`add-reference-book`](add-reference-book.md); накат — после SQL-фазы |

## Что нужно до старта

Запуск из корня репозитория `c:\Work\BlazorSVT`.

Имя базы, логин и пароль скрипт берёт из `Database:MdmDb` в `BlazorSvt/appsettings.json`. Сервер в команде всегда `localhost`. Поле Server из строки не используется. `appsettings.json` не менять.

Нужен SQL-логин (`User ID`). При `Trusted_Connection` скрипт не запускать: каждый файл Programmability выполняется как `sqlcmd -S localhost -d mdm -E -b -f 65001 -i <файл>`. Порядок тот же: Platform, затем модули по имени; внутри папки `fn` и `vw` перед `sp`.

## Как устроено

Процедура — [`SKILL.md`](../../skills/create-programmability/SKILL.md).

```powershell
& ".\BlazorSvt\SqlScripts\Create-Programmability.ps1"
```

Скрипт останавливается на первой ошибке sqlcmd (`-b`).

Человек вызывает `/create_programmability`. Агент берёт скил сам, когда просят накатить процедуры, функции или пересоздать SP, fn и view схемы v2. Оба входа ведут в один `SKILL.md`.

## Сработало, если

- В ответе есть число выполненных скриптов.
- При ошибке показан вывод sqlcmd, и выполнение остановлено.
- `appsettings.json` не изменён. Structure-скрипты не затронуты.

## Куда встаёт

Шаг после того, как [`add-reference-book`](add-reference-book.md) положил Programmability в SoT. [`publish-all-sql`](publish-all-sql.md) копирует исходники в каталог деплоя и на базу их не накатывает.
