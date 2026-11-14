# Пакет документов для архитектурного ревью СВТ 2.0

Плоская копия для передачи на ревью. **Не источник истины.** Канон — `.cursor/skills/svt-architecture/` (`architecture.md`, `roadmap.md`, `risks.md`, `glossary.md`, `delivery-ring.md`, `backlog.md`, `catalogs-scope.md`, диаграммы `01-as-is-legacy.png`, `02-to-be-current-program.png`, `03-future-containers-out-of-scope.png`) и `docs/docs/svt-v-atlas/SVT-idealnyy-process-2.7.html`.

Страницы Confluence из таблицы ниже обновляются из этого пакета (скрипт копирует канон навыка), не наоборот.

В git в этой папке лежат только этот README, `Build-ReviewPackage.ps1` и снимок заключения [`review-current-state.html`](review-current-state.html) (07.08.2026, статус прода не обновляется). Скрипт собирает `docs/output-review-package.zip` и после успешного zip удаляет из этой папки сгенерированные markdown, диаграммы и HTML идеального процесса. Zip в git не коммитится. `-NoZip` оставляет файлы в `-Destination`.

## Сборка

Из корня репозитория:

```powershell
.\docs\output-review-package\Build-ReviewPackage.ps1
```

Скрипт:

1. Копирует markdown и три диаграммы из `.cursor/skills/svt-architecture/`.
2. Заменяет ссылки и явные упоминания четырёх файлов из таблицы (включая самоссылки) на Confluence. Таблица ниже — и инструкция, и вход скрипта: ровно четыре строки. `risks.md`, `glossary.md` и `delivery-ring.md` копируются рядом и на Confluence не публикуются.

| Файл канона | Страница Confluence |
|---|---|
| `architecture.md` | [01. Архитектура трансформации СВТ: проблемы и решение](https://confluence.sibur.local/pages/viewpage.action?pageId=573815762) |
| `roadmap.md` | [02. План трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573815766) |
| `backlog.md` | [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778) |
| `catalogs-scope.md` | [04. Список справочников для трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573822390) |

3. Копирует `docs/docs/svt-v-atlas/SVT-idealnyy-process-2.7.html` в эту папку как есть. Ссылки на соседние файлы Атласа внутри пакета не открываются: в пакет входит только этот HTML.
4. Относительные ссылки вида `../../../…` из навыка переписывает в `../../…`: пакет на одну папку ближе к корню репозитория.
5. `review-current-state.html`, этот README и skills не меняет.
6. Кладёт zip рядом с папкой: `docs/output-review-package.zip`. В корне архива — папка `output-review-package/` без скрипта сборки. Старый zip перезаписывается.
7. Удаляет из этой папки скопированные markdown, три диаграммы и `SVT-idealnyy-process-2.7.html`. README, скрипт и `review-current-state.html` остаются. При `-Destination` вне этой папки файлы там сохраняются, а сгенерированные копии в этой папке всё равно удаляются.

Скоуп согласования: **условное одобрение** направления (см. [01. Архитектура трансформации СВТ: проблемы и решение](https://confluence.sibur.local/pages/viewpage.action?pageId=573815762) §6.9; реестр и приложения митигаций — `risks.md` в zip).

## Состав zip

Текстовое обоснование и альтернативы — `architecture.md`.
Порядок доменов и Gate A/B/C/D — `roadmap.md`.
Риски, меры и приложения D-* — `risks.md`.
Шаги поставки — `delivery-ring.md`. Термины — `glossary.md`.
Чеклист работ, таблица вех и оценки — `backlog.md`.
Реестр справочников — `catalogs-scope.md`.
Идеальный процесс СВТ, версия 2.7 — `SVT-idealnyy-process-2.7.html`.
Снимок повторного architecture review — `review-current-state.html` (в папке пакета он остаётся и после сборки).

Отдельный файл на каждый артефакт митигации **не ведётся** — приложения D-* в `risks.md`.

## Диаграммы

| № | Файл | Состояние | Статус |
|---|---|---|---|
| 1 | `01-as-is-legacy.png` | Легаси (As-Is) | Текущее состояние прод |
| 2 | `02-to-be-current-program.png` | Целевая архитектура текущей программы (Gate D) | Согласуется в рамках ревью |
| 3 | `03-future-containers-out-of-scope.png` | Контейнеризация | **Вне скоупа** текущей программы |

Исполнение типа E (прогон 3.1.10, CI 3.2.6, алерты 2.7.8, SCA 2.4.6, калибровка 1.7, …) — в [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778), не выдаётся за закрытое.
