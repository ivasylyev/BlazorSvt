# Пакет документов для архитектурного ревью СВТ 2.0

> Плоская копия-снэпшот для передачи на ревью (все файлы в одной папке; ссылки адаптированы).
> Канон в репозитории: `.cursor/skills/svt-architecture/reference.md`,
> `.cursor/skills/svt-architecture/roadmap.md`, `.cursor/skills/svt-architecture/backlog.md`,
> `.cursor/skills/svt-architecture/catalogs-scope.md`, `docs/architecture/`.
> Скоуп согласования: **условное одобрение** направления (см. `reference.md` §6.9,
> реестр и приложения митигаций — `roadmap.md` §2б).

Текстовое обоснование и альтернативы — [`reference.md`](reference.md).  
План, Gate A/B/C/D, **§2б отложенная митигация** (включая threat model, SoD, SLI/SLO,
миграции, метрики пилотов, протокол нагрузки) — [`roadmap.md`](roadmap.md).  
Чеклист работ, таблица этапов и оценки — [`backlog.md`](backlog.md).  
Реестр справочников (скоуп, домены, RO/E/L, меню, приоритеты) — [`catalogs-scope.md`](catalogs-scope.md).  
Снимок повторного architecture review — [`review-current-state.html`](review-current-state.html).

Отдельные файлы под каждый артефакт митигации **не ведутся** — всё в `roadmap.md` §2б.

## Диаграммы

| № | Файл | Состояние | Статус |
|---|---|---|---|
| 1 | [`01-as-is-legacy.png`](01-as-is-legacy.png) | Легаси (As-Is) | Текущее состояние прод |
| 2 | [`02-to-be-current-program.png`](02-to-be-current-program.png) | Целевая архитектура текущей программы (Gate D) | Согласуется в рамках ревью |
| 3 | [`03-future-containers-out-of-scope.png`](03-future-containers-out-of-scope.png) | Контейнеризация | **Вне скоупа** текущей программы |

Исполнение типа E (прогон 0.2.1.10, CI 0.2.2.6, алерты 0.1.7.8, SCA 0.1.4.6, калибровка 0.0.5, …) —
в бэклоге `backlog.md`, не выдаётся за закрытое.
