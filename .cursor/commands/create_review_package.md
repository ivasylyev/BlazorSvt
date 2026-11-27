# create_review_package

Собрать zip пакета документов для архитектурного ревью: `docs/output-review-package.zip`.

**Канон:** skill `svt-create-review-package` (`.cursor/skills/svt-create-review-package/SKILL.md`). Прочитай его и выполни.

Кратко (по умолчанию):

```powershell
.\docs\output-review-package\Build-ReviewPackage.ps1
```

Из корня репозитория. Флаги `-NoZip` и `-Destination` — только если пользователь их назвал. Страницы Confluence скрипт не публикует. Zip не коммитить.
