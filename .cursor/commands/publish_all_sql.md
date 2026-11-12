# publish_all_sql

Опубликовать SQL-скрипты деплоя в `C:\publish\v2` (или `-TargetPath`).

**Канон:** skill `publish-all-sql` (`.cursor/skills/publish-all-sql/SKILL.md`). Прочитай его и выполни.

Кратко (по умолчанию):

```powershell
& ".\BlazorSvt\SqlScripts\Publish-AllSql.ps1" -Mode All
```

Из корня репозитория. Режимы `All` / `Release` / `Programmability` / `FromSource` — в skill. Исходники в `SqlScripts/` не менять.
