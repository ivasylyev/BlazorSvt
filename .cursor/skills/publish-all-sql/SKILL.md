---
name: publish-all-sql
description: >-
  Публикация всех SQL-скриптов деплоя BlazorSVT в плоскую папку C:\publish\v2
  с трёхзначными префиксами. Используй когда пользователь просит
  publish_all_sql, опубликовать SQL для деплоя или обновить C:\publish\v2.
---

# publish_all_sql

## Команда

```powershell
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1"
```

Запускать из корня репозитория `c:\Work\BlazorSVT`.

## Что делает

- Копирует все deploy SQL-скрипты (Structure + Programmability) в `C:\publish\v2`
- Имена файлов: `001.{оригинальное_имя}.sql`, `002....`, и т.д.
- Порядок — как в `BlazorSvt/SqlScripts/README.md` (полный re-deploy v2)
- Перед копированием удаляет старые `*.sql` в целевой папке
- Исходники в репозитории не изменяет

## Не копируется

- `Create-Programmability.ps1`, `Publish-AllSql.ps1`
- `README.md`, `Translations_info.txt`

## Манифест (17 скриптов)

| # | Исходный путь |
|---|---------------|
| 001 | `Platform/Structure/00.Create_Schema.sql` |
| 002 | `Platform/Structure/01.Create_Fulltext_Catalog.sql` |
| 003 | `Platform/Programmability/fn_GetDateSqlOperator.sql` |
| 004 | `Platform/Programmability/sp_GetBlazorGridData.sql` |
| 005 | `Platform/Programmability/sp_ExportBlazorGridDetail.sql` |
| 006–009 | TransportRate: Structure → Programmability |
| 010–013 | TransportLeg: Structure → Programmability |
| 014–017 | LocationsNodes: Structure → Programmability |

## После запуска

- Ошибка (отсутствует исходный файл) → показать сообщение, не продолжать
- Успех → сообщить количество скопированных файлов и путь `C:\publish\v2`

## Добавление нового справочника

Обновить манифест в `Publish-AllSql.ps1` и таблицу порядка в `SqlScripts/README.md`.
