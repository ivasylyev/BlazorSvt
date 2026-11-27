---
name: svt-publish
description: >-
  Публикация пакета BlazorSVT в C:\publish\{VersionPrefix}: приложение, склейка
  миграции и zip. Режим local дополнительно накатывает склейку на localhost/mdm.
  Используй когда пользователь просит publish, svt-publish, опубликовать пакет,
  собрать версию или обновить C:\publish.
---

# svt-publish

## Команда

Запускать из корня репозитория `c:\Work\BlazorSVT`.

```powershell
# app + склейка миграции + zip. На базу не накатывает.
& ".\BlazorSvt\SqlScripts\svt-publish.ps1" -Mode all

# Только приложение
& ".\BlazorSvt\SqlScripts\svt-publish.ps1" -Mode app

# Только склейка скриптов
& ".\BlazorSvt\SqlScripts\svt-publish.ps1" -Mode sql

# То же, что all, затем sqlcmd на localhost / mdm
& ".\BlazorSvt\SqlScripts\svt-publish.ps1" -Mode local
```

Без `-Mode` скрипт берёт `all`.

## Версия

`VersionPrefix` из `BlazorSvt/BlazorSvt.csproj` (вид `x.y.z`, без числа коммитов). Максимальная папка — каталог `SqlScripts/Migrations/{x.y.z}` с наибольшей версией. Исходники в `SqlScripts/` скрипт не меняет. `appsettings.json` не менять.

Версии совпали:

| Mode | Куда | Что внутри |
|------|------|------------|
| `app` | `C:\publish\{VersionPrefix}` | `v2\` — содержимое `C:\publish\Blazor` |
| `sql` | `C:\publish\{VersionPrefix}` | `{VersionPrefix}.sql` |
| `all`, `local` | `C:\publish\{VersionPrefix}` | `v2\` и `{VersionPrefix}.sql` |

Zip: `C:\publish\{VersionPrefix}.zip`, внутри папка версии. `C:\publish\Blazor` в zip не входит.

## Версии не совпали

`all` и `local` недоступны. Показать пользователю вывод скрипта как есть и ждать. Код выхода `2`. Ответ «да» выбором не является. Нужна цифра `1`, `2` или `3`.

1. Отменить запуск. Повторно скрипт не вызывать.
2. Только приложение: `-Mode app -AllowMismatch`. Пакет в `C:\publish\{VersionPrefix}`.
3. Только скрипты: `-Mode sql -AllowMismatch`. Скрипты из максимальной папки, пакет в `C:\publish\{имя этой папки}`.

`-AllowMismatch` не открывает `all` и `local`.

## Как собирается пакет

Новые файлы собираются в `C:\publish\_svt-publish-staging`. Живые `C:\publish\{версия}` и `C:\publish\{версия}.zip` заменяются вместе, когда сборка уже готова. Ошибка `dotnet publish`, чтения скриптов, sqlcmd или подмены оставляет прежнюю папку и прежний zip.

`dotnet publish` — Release, framework-dependent, каталог `C:\publish\Blazor`.

Склейка — все `*.sql` папки версии по имени файла, файл UTF-8 with BOM. Перед телом каждого файла отдельный батч: `PRINT 'Starting {имя файла}';`, затем `SYSDATETIME()` в переменную и `EXEC sys.sp_set_session_context N'svt_publish_started', @переменная;`. Прямой вызов с `SYSDATETIME()` в аргументе процедура не принимает. После тела каждого файла, включая последний, печатается `Finished {имя файла} in {мин}:{сек}.{тысячные}`. Время — `DATEDIFF_BIG(MILLISECOND, метка, SYSDATETIME())` на первой команде этого батча; дробная доля миллисекунды отбрасывается. Минуты целиком, без ведущего нуля и без часов; секунды — две цифры; тысячные — три (`0:00.000`, `1:05.007`, `75:00.000`). На стыке двух файлов окончание предыдущего, старт следующего и новая метка — один батч. Между батчами строка `GO`. Одинарная кавычка в имени удваивается.

`local` выполняет этот файл до подмены пакета: `sqlcmd -S localhost -d mdm -b -f 65001`. Сервер всегда `localhost`, база `mdm`. Строка `Database:MdmDb` читается вместе с комментариями в `appsettings.json`. При `User ID` sqlcmd берёт этот логин, при `Trusted_Connection` — `-E`. Поле Server из строки не используется. `appsettings.json` не менять. Первая ошибка останавливает следующие батчи: в выводе остаётся последний `Starting` без `Finished`. Уже выполненные батчи на базе остаются. Zip при ошибке не пишется.

Накат SoT — skill `svt-create-programmability`. Этот скрипт его не вызывает.

## После запуска

- Код `2` → вопрос пользователю, пакет не трогать, пока нет выбора `2` или `3`
- Другая ошибка → показать вывод, не продолжать
- Успех → `Mode`, `Version`, `Package`, `Zip`; для скриптов ещё `SqlFiles`; для `local` ещё `LocalApply: yes`
