# Пакет документов для архитектурного ревью СВТ 2.0

Плоская копия для передачи на ревью. **Не источник истины.** Канон — `.cursor/skills/svt-architecture/` (`architecture.md`, `roadmap.md`, `cutover.md`, `risks.md`, `review-conditions.md`, `mitigations.md`, `glossary.md`, `delivery-steps.md`, `backlog.md`, `catalogs-scope.md`, `status.md`, `outside-registry.md`, `pre-implementation-checklist.md`, диаграммы `01-as-is-legacy.png`, `02-to-be-current-program.png`, `03-future-containers-out-of-scope.png`) и `docs/docs/svt-v-atlas/SVT-idealnyy-process-2.7.html`.

Строки таблицы со ссылкой на Confluence обновляются из этого пакета (скрипт копирует канон навыка), не наоборот. Пустая ячейка ссылки — страница ещё не заведена.

В git в этой папке лежат только этот README, `Build-ReviewPackage.ps1` и снимок заключения [`review-current-state.html`](review-current-state.html) (07.08.2026, статус прода не обновляется). Zip в git не коммитится.

Процедура сборки — skill `svt-create-review-package`. Таблица ниже — вход скрипта `Build-ReviewPackage.ps1`: строка со ссылкой заменяется на страницу Confluence, пустая ячейка копируется как есть.

| № | Файл | Зачем | Страница Confluence |
|---|---|---|---|
| 1 | `glossary.md` | Термины. Открыть до остальных текстов | |
| 2 | `status.md` | Где контур сейчас | |
| 3 | `architecture.md` | Обоснование, альтернативы, целевое состояние. Сразу после него — схемы 1–3 ниже | [01. Архитектура трансформации СВТ: проблемы и решение](https://confluence.sibur.local/pages/viewpage.action?pageId=573815762) |
| 4 | `SVT-idealnyy-process-2.7.html` | Идеальный процесс, версия 2.7 (канон, в пакет копируется скриптом) | |
| 5 | `catalogs-scope.md` | Реестр справочников: скоуп, домены, RO/E/L | [04. Список справочников для трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573822390) |
| 6 | `roadmap.md` | Порядок доменов, сводка вех, критический путь | [02. План трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573815766) |
| 7 | `delivery-steps.md` | Двенадцать шагов поставки. Подписи «Шаг» в бэклоге | |
| 8 | `backlog.md` | Чеклист работ | [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778) |
| 9 | `cutover.md` | Правила перехода и критерии Gate A/B/C/D | |
| 10 | `risks.md` | Риски и меры | |
| 11 | `review-conditions.md` | Условия условного одобрения, реестр D-* | |
| 12 | `mitigations.md` | Приложения: модель угроз, SoD, миграции, SLI/SLO, метрики пилотов, протокол нагрузки | |
| 13 | `outside-registry.md` | Функции вне реестра справочников | |
| 14 | `pre-implementation-checklist.md` | Чеклист перед кодом | |
| 15 | `review-current-state.html` | Снимок повторного architecture review (07.08.2026, статус прода не обновляется) | |

Скоуп согласования: **условное одобрение** направления (см. `review-conditions.md` в zip, раздел §6.9; реестр там же, приложения митигаций — `mitigations.md`).

## Состав zip

Текстовое обоснование и альтернативы — `architecture.md`.
Порядок доменов, сводка вех и критический путь — `roadmap.md`.
Правила перехода и критерии Gate A/B/C/D — `cutover.md`.
Риски и меры — `risks.md`. Условия одобрения — `review-conditions.md`. Приложения D-* — `mitigations.md`.
Шаги поставки — `delivery-steps.md`. Термины — `glossary.md`.
Чеклист работ — `backlog.md`.
Реестр справочников — `catalogs-scope.md`.
Статус контура — `status.md`. Функции вне реестра — `outside-registry.md`. Чеклист перед кодом — `pre-implementation-checklist.md`.
Идеальный процесс СВТ, версия 2.7 — `SVT-idealnyy-process-2.7.html`.
Снимок повторного architecture review — `review-current-state.html` (в папке пакета он остаётся и после сборки).

Отдельный файл на каждый артефакт митигации **не ведётся** — приложения D-* в `mitigations.md`.

## Диаграммы

| № | Файл | Состояние | Статус |
|---|---|---|---|
| 1 | `01-as-is-legacy.png` | Легаси (As-Is) | Текущее состояние прод |
| 2 | `02-to-be-current-program.png` | Целевая архитектура текущей программы (Gate D) | Согласуется в рамках ревью |
| 3 | `03-future-containers-out-of-scope.png` | Контейнеризация | **Вне скоупа** текущей программы |

Исполнение типа E (прогон 4.1.10, CI 4.2.8, алерты 4.5.3, SCA 4.4.5, калибровка 6.17, …) — в [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778), не выдаётся за закрытое.
