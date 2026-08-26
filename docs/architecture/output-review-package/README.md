# Пакет документов для архитектурного ревью СВТ 2.0

> Плоская копия-снэпшот для передачи на ревью (все файлы в одной папке; ссылки адаптированы).
> Канон в репозитории: `.cursor/skills/svt-architecture/reference.md`,
> `.cursor/skills/svt-architecture/roadmap.md`, `docs/architecture/`.
> Скоуп согласования: **условное одобрение** направления (см. `reference.md` §6.9,
> реестр и приложения митигаций — `roadmap.md` §2б).

Текстовое обоснование и альтернативы — [`reference.md`](reference.md).  
План, Gate A/B/C/D, **§2б отложенная митигация** (включая threat model, SoD, SLI/SLO,
миграции, метрики пилотов, протокол нагрузки) — [`roadmap.md`](roadmap.md).  
Снимок повторного architecture review — [`review-current-state.html`](review-current-state.html).

Отдельные файлы под каждый артефакт митигации **не ведутся** — всё в `roadmap.md` §2б.

## Диаграммы

| № | Файл | Состояние | Статус |
|---|---|---|---|
| 1 | [`01-as-is-legacy.png`](01-as-is-legacy.png) | Легаси (As-Is) | Текущее состояние прод |
| 2 | [`02-to-be-current-program.png`](02-to-be-current-program.png) | Целевая архитектура текущей программы (Gate D) | Согласуется в рамках ревью |
| 3 | [`03-future-containers-out-of-scope.png`](03-future-containers-out-of-scope.png) | Контейнеризация | **Вне скоупа** текущей программы |

Исполнение типа E (прогон 6.9.9, CI 7.0.5, алерты 5.6б, SCA 2.6, калибровка 0.4а, …) —
в бэклоге `roadmap.md`, не выдаётся за закрытое.
