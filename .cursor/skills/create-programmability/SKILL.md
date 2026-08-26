---
name: create-programmability
description: >-
  Накат SQL Programmability на dev БД BlazorSVT. Используй когда пользователь
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

- Читает `Database:MdmDb` из `BlazorSvt/appsettings.json`
- Выполняет все `*.sql` из `SqlScripts/Platform/Programmability/` и `SqlScripts/Modules/*/Programmability/`
- Останавливается при первой ошибке sqlcmd (`-b`)
- Structure-скрипты не затрагивает

## После запуска

- Ошибка → показать вывод sqlcmd, не продолжать
- Успех → сообщить количество выполненных скриптов
