# create_programmability

Накатить SQL-скрипты **Programmability** (Platform + все модули) на локальную БД `mdm` (`localhost`).

## Действия

1. Запусти скрипт из корня репозитория:

```powershell
& ".\BlazorSvt\SqlScripts\Create-Programmability.ps1"
```

2. Дождись завершения. Скрипт выполняет объекты в порядке:
   - `Platform/Programmability/` (`fn_*` → `sp_*`)
   - `Modules/*/Programmability/` по алфавиту модулей (`vw_*`)

3. Если выполнение завершилось с ошибкой — покажи полный вывод sqlcmd и **не продолжай** другие действия без указания пользователя.

4. При успехе — кратко сообщи, сколько скриптов выполнено.

## Ограничения

- Сервер всегда `localhost`. Имя базы, логин и пароль берутся из `Database:MdmDb` в `BlazorSvt/appsettings.json`. Поле Server из этой строки не использовать.
- Не изменяй `appsettings.json` и содержимое SQL-скриптов без отдельной просьбы.
- Структурные скрипты (`Structure/`) этой командой **не** накатываются.
