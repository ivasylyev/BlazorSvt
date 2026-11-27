---
name: svt-create-review-package
description: >-
  Сборка zip пакета документов для архитектурного ревью СВТ:
  Build-ReviewPackage.ps1 и docs/output-review-package.zip. Используй когда
  пользователь просит svt-create-review-package, create_review_package, собери
  пакет на ревью или пакет для архитектурного ревью. Ревью навыков и документов
  агента — skill-review. Пакет версии приложения в C:\publish — svt-publish.
---

# svt-create-review-package

## Команда

Запускать из корня репозитория `c:\Work\BlazorSVT`.

```powershell
.\docs\output-review-package\Build-ReviewPackage.ps1
```

Без флагов, пока пользователь не назвал их. `-NoZip` оставляет файлы в `-Destination` и zip не пишет. `-Destination` задаёт папку пакета; без него папка — `docs/output-review-package/`.

## Что делает скрипт

Канон содержимого — файлы skill `svt-architecture` и `docs/docs/svt-v-atlas/SVT-idealnyy-process-2.7.html`. Этот навык их не редактирует и скрипт не переписывает.

Вход скрипта — таблица в `docs/output-review-package/README.md`, заголовок которой содержит «Страница Confluence». Строки и адреса в этот навык не копировать.

- Файл `.md` берётся из `.cursor/skills/svt-architecture/`.
- `SVT-idealnyy-process-2.7.html` берётся из `docs/docs/svt-v-atlas/` как есть. Соседние файлы Атласа в пакет не входят.
- `review-current-state.html` — снимок, который уже лежит в папке пакета. Текст снимка, README пакета и skills скрипт не меняет.
- Строка со ссылкой: ссылки и явные упоминания этого файла, включая самоссылки, заменяются на страницу Confluence. Относительные ссылки `../../../` из навыка переписываются в `../../`.
- Пустая ячейка ссылки: файл копируется как есть и на Confluence не публикуется.

Три диаграммы копируются из `.cursor/skills/svt-architecture/`: `01-as-is-legacy.png`, `02-to-be-current-program.png`, `03-future-containers-out-of-scope.png`.

Без `-NoZip` скрипт пишет `docs/output-review-package.zip`. В корне архива — папка `output-review-package/` без скрипта сборки. Прежний zip перезаписывается. После успешного zip из папки пакета удаляются скопированные markdown, три диаграммы и `SVT-idealnyy-process-2.7.html`. README, скрипт и `review-current-state.html` остаются. При `-Destination` вне этой папки файлы там сохраняются, а сгенерированные копии в папке пакета всё равно удаляются.

Страницы Confluence навык не публикует. Zip не коммитить.

## После запуска

- Ошибка скрипта → показать вывод как есть, повторно не запускать
- Успех → путь zip и что сгенерированные копии убраны из папки пакета
- `-NoZip` → путь папки и что zip не писался
