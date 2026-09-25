---
name: create-programmability
description: >-
  Накат SQL Programmability на локальную БД mdm (localhost). Используй когда пользователь
  просит create_programmability, накатить процедуры/функции, обновить
  programmability или пересоздать SP/fn/view в схеме v2.
---

# create_programmability

## Команда

```powershell
& ".\BlazorSvt\SqlScripts\Create-Programmability.ps1"
```

Запускать из корня репозитория `c:\Work\BlazorSVT`.

## Что делает

- Берёт имя базы, логин и пароль из `Database:MdmDb` в `BlazorSvt/appsettings.json`
- Сервер всегда `localhost`. Поле Server из этой строки не использует (`appsettings.json` не менять)
- Выполняет все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`
- Останавливается при первой ошибке sqlcmd (`-b`)
- Structure-скрипты не затрагивает

## После запуска

- Ошибка → показать вывод sqlcmd, не продолжать
- Успех → сообщить количество выполненных скриптов
