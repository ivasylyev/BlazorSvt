# publish_all_sql

Опубликовать все SQL-скрипты деплоя (Structure + Programmability) в плоскую папку `C:\publish\v2` с трёхзначными префиксами.

## Действия

1. Запусти скрипт из корня репозитория:

```powershell
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1"
```

2. Скрипт копирует 17 файлов в порядке из `BlazorSvt/SqlScripts/README.md`:
   - Platform (Structure → Programmability)
   - TransportRate, TransportLeg, LocationsNodes (у каждого: Structure → Programmability)

3. Имена в `C:\publish\v2`: `001.00.Create_Schema.sql`, `002.01.Create_Fulltext_Catalog.sql`, …

4. Перед копированием удаляются старые `*.sql` в целевой папке.

5. При успехе — кратко сообщи количество опубликованных файлов и путь назначения.

## Ограничения

- Только копирование; исходники в `SqlScripts/` не изменять.
- Runner для наката в `C:\publish\v2` не создавать — накат вручную, скрипт за скриптом.
- Сценарий: полный re-deploy v2 (CreateTable пересоздаёт snapshot-таблицы).

## При добавлении справочника

Обновить манифест в `Publish-AllSql.ps1` и порядок в `SqlScripts/README.md`.
