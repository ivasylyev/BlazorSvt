---
name: publish-all-sql
description: >-
  Публикация SQL-скриптов деплоя BlazorSVT из Migrations/ и SoT в C:\publish\v2.
  Используй когда пользователь просит publish_all_sql, опубликовать SQL для деплоя
  или обновить C:\publish\v2.
---

# publish_all_sql

## Команда

Запускать из корня репозитория `c:\Work\BlazorSVT`.

```powershell
# По умолчанию: все версии Migrations → C:\publish\v2\{version}\
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1" -Mode All

# Один релиз
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1" -Mode Release -Version 2.0.1

# Latest Programmability из SoT
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1" -Mode Programmability

# Legacy flat snapshot из SoT (greenfield, не история релизов)
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1" -Mode FromSource
```

Опционально: `-TargetPath D:\deploy\v2`.

## Режимы

| Mode | Источник | Куда |
|------|----------|------|
| `All` (default) | `SqlScripts/Migrations/{x.y.z}/` | `TargetPath\{x.y.z}\` |
| `Release` | `Migrations/{Version}/` | `TargetPath\{Version}\` |
| `Programmability` | SoT `**/Programmability/*.sql` (манифест) | `TargetPath\programmability\` |
| `FromSource` | SoT Structure+Programmability (манифест) | плоский `TargetPath\` с префиксами `001.`… |

Перед копированием в целевую папку режима удаляются старые `*.sql` в этой папке.
Исходники в репозитории не изменяет.

## Конвенция

Подробно: `BlazorSvt/SqlScripts/Migrations/README.md`.

- **Migrations/** — immutable релизные артефакты (после выката не править).
- С **2.0.2+** в папку релиза класть только schema-дельты; Programmability — отдельно (`-Mode Programmability`).
- `2.0.0` / `2.0.1` — архив факта из старого `C:\publish\v2` (001–037 / 038–054).

## Не копируется

- `Create-Programmability.ps1`, `Publish-AllSql.ps1`
- `README.md`, `Translations_info.txt`, `Migrations/README.md`

## После запуска

- Ошибка (нет версии / нет исходного файла) → показать сообщение, не продолжать
- Успех → число файлов, `Mode=…`, путь `TargetPath`

## Добавление нового справочника

1. SoT: `Modules/{Name}/Structure` + `Programmability` + таблицы в `SqlScripts/README.md`
2. Релиз: DDL-дельта в `Migrations/X.Y.Z/`
3. Манифест `FromSource` / `Programmability` в `Publish-AllSql.ps1`
