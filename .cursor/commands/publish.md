# publish

Собрать пакет версии в `C:\publish\{VersionPrefix}` и zip рядом. По умолчанию приложение и склейка скриптов миграции. На базу не накатывает, пока режим не `local`.

**Канон:** skill `svt-publish` (`.cursor/skills/svt-publish/SKILL.md`). Прочитай его и выполни.

Кратко (по умолчанию):

```powershell
& ".\BlazorSvt\SqlScripts\svt-publish.ps1" -Mode all
```

Из корня репозитория. Режимы `app` / `sql` / `all` / `local` — в skill. Исходники в `SqlScripts/` не менять. Код выхода `2` — вопрос пользователю, без `-AllowMismatch`, пока он не выбрал пункт 2 или 3.
