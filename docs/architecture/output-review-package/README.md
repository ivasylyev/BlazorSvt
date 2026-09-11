# Пакет документов для архитектурного ревью СВТ 2.0

> Плоская копия-снэпшот для передачи на ревью (все файлы в одной папке).
> Канон в репозитории: `.cursor/skills/svt-architecture/` (`reference.md`, `roadmap.md`,
> `backlog.md`, `catalogs-scope.md`) и `docs/architecture/`.
> В копии пакета перекрёстные ссылки между этими четырьмя документами заменены на страницы
> Confluence (ревьюер ходит по опубликованным страницам, а не по соседним файлам папки).

**Как готовить пакет**

1. Скопировать актуальные `reference.md`, `roadmap.md`, `backlog.md`, `catalogs-scope.md`
   из `.cursor/skills/svt-architecture/` в эту папку.
2. Заменить ссылки и явные упоминания этих файлов (включая самоссылки) на Confluence:

| Файл канона | Страница Confluence |
|---|---|
| `reference.md` | [01. Архитектура трансформации СВТ: проблемы и решение](https://confluence.sibur.local/pages/viewpage.action?pageId=573815762) |
| `roadmap.md` | [02. План трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573815766) |
| `backlog.md` | [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778) |
| `catalogs-scope.md` | [04. Список справочников для трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573822390) |

3. Диаграммы, `review-current-state.html` и это оглавление не переписывать на Confluence.
   Skills в `.cursor/skills/` не менять.
4. Заархивировать папку `output-review-package` в `docs/architecture/output-review-package.zip`
   (рядом с папкой, не внутри неё). В корне архива — сама папка `output-review-package/`.
   Старый zip перезаписать.

Скоуп согласования: **условное одобрение** направления (см. [01. Архитектура трансформации СВТ: проблемы и решение](https://confluence.sibur.local/pages/viewpage.action?pageId=573815762) §6.9;
реестр и приложения митигаций — [02. План трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573815766) §2б).

## Файлы в этой папке

Текстовое обоснование и альтернативы — [`reference.md`](reference.md).  
План, Gate A/B/C/D, **§2б отложенная митигация** (включая threat model, SoD, SLI/SLO,
миграции, метрики пилотов, протокол нагрузки) — [`roadmap.md`](roadmap.md).  
Чеклист работ, таблица этапов и оценки — [`backlog.md`](backlog.md).  
Реестр справочников (скоуп, домены, RO/E/L, меню, приоритеты) — [`catalogs-scope.md`](catalogs-scope.md).  
Снимок повторного architecture review — [`review-current-state.html`](review-current-state.html).

Отдельные файлы под каждый артефакт митигации **не ведутся** — всё в [02. План трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=573815766) §2б.

## Диаграммы

| № | Файл | Состояние | Статус |
|---|---|---|---|
| 1 | [`01-as-is-legacy.png`](01-as-is-legacy.png) | Легаси (As-Is) | Текущее состояние прод |
| 2 | [`02-to-be-current-program.png`](02-to-be-current-program.png) | Целевая архитектура текущей программы (Gate D) | Согласуется в рамках ревью |
| 3 | [`03-future-containers-out-of-scope.png`](03-future-containers-out-of-scope.png) | Контейнеризация | **Вне скоупа** текущей программы |

Исполнение типа E (прогон 0.2.1.10, CI 0.2.2.6, алерты 0.1.7.8, SCA 0.1.4.6, калибровка 0.0.5, …) —
в [03. Бэклог трансформации СВТ](https://confluence.sibur.local/pages/viewpage.action?pageId=583080778), не выдаётся за закрытое.
