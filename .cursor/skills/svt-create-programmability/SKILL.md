---
name: svt-create-programmability
description: >-
  Накат SQL Programmability на локальную БД mdm (localhost). Используй когда пользователь
  просит create_programmability, накатить процедуры/функции, обновить
  programmability или пересоздать SP/fn/view в схеме v2.
---

# create_programmability

## Команда

```powershell
& ".\BlazorSvt\SqlScripts\svt-create-programmability.ps1"
```

Запускать из корня репозитория `c:\Work\BlazorSVT`.

## Что делает

- Берёт имя базы, логин и пароль из `Database:MdmDb` в `BlazorSvt/appsettings.json`
- Сервер всегда `localhost`. Поле Server из этой строки не использует (`appsettings.json` не менять)
- Выполняет все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`
- Останавливается при первой ошибке sqlcmd (`-b`)
- Structure-скрипты не затрагивает
- Требует SQL-логин (`User ID`). При `Trusted_Connection` скрипт не запускать: каждый файл Programmability — `sqlcmd -S localhost -d mdm -E -b -f 65001 -i <файл>`. Порядок тот же, что у скрипта: Platform, затем модули по имени; внутри папки `fn`/`vw` перед `sp`

Тот же порядок — у снимка `All_Programmability.sql`. Как его собрать — `BlazorSvt/SqlScripts/Migrations/README.md`, раздел «Снимок All_Programmability». Этот скрипт снимок не пишет.

## После запуска

- Ошибка → показать вывод sqlcmd, не продолжать
- Успех → сообщить количество выполненных скриптов
