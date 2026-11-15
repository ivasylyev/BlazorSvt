# svt-architecture

## Что делает

`svt-architecture` держит канон СВТ 2.0: что на проде, целевое состояние, состав справочников, порядок вех, правила Gate, риски и чеклист работ. Агент открывает его при разработке, рефакторинге, выборе решения и когда функцию ещё предстоит занести в реестр и бэклог.

Таблица «вопрос → файл» — в [`SKILL.md`](../../skills/svt-architecture/SKILL.md), раздел «Канон». Эта страница её не повторяет.

## Когда брать

| Ситуация | Куда |
|---|---|
| Статус, скоуп, веха, Gate, риск, термин, чеклист, функция вне реестра | Этот скил, файл из таблицы «Канон» |
| Новый read-only справочник | [`add-reference-book`](add-reference-book.md) |
| Накат SP на localhost или публикация SQL | [`create-programmability`](create-programmability.md) или [`publish-all-sql`](publish-all-sql.md) |
| Эталон, кольцо, роли, план-график | [`atlas`](../atlas/atlas.md) или [`plan-grafik-md-html`](../atlas/plan-grafik-md-html.md) |
| Как писать текущий код модуля | правило `svt-development-patterns` |
| Обязательные ограничения архитектуры | правило `svt-architecture-principles` |

## Что нужно до старта

Разворачивать ничего не нужно. Постановки `docs/tasks`, транскрипты, бюджет и план для руководства — снимки. При расхождении со скилом берётся файл из таблицы «Канон».

Порядок первого чтения пакета на ревью — [`README.md`](../../skills/svt-architecture/README.md) папки скила. Диаграммы `01-as-is-legacy.png`, `02-to-be-current-program.png`, `03-future-containers-out-of-scope.png` лежат там же.

## Как устроено

`SKILL.md` — указатель для агента. Предметные файлы (`status.md`, `architecture.md`, `catalogs-scope.md`, `roadmap.md`, `backlog.md` и остальные из таблицы) остаются в папке скила.

Канон скоупа при изменении — `catalogs-scope.md` в репозитории. Excel на Confluence — исходный дамп легаси.

Пакет для передачи собирает `docs/output-review-package/Build-ReviewPackage.ps1`. В zip попадают markdown навыка, диаграммы из папки скила и `SVT-idealnyy-process-2.7.html`; скрипт удаляет эти копии из папки пакета. Канон — файлы навыка. Zip в git не коммитится.

## Сработало, если

- Ответ ссылается на файл из раздела «Канон».
- Функция, которой нет в реестре, разбирается по `outside-registry.md`.
- Перед кодом открыт `pre-implementation-checklist.md`.

## Куда встаёт

Этот скил — контекст, который процедуры СВТ читают до работы. [`add-reference-book`](add-reference-book.md) сверяет справочник с `catalogs-scope.md`. Правила `svt-architecture-principles` и `svt-development-patterns` держат ограничения и паттерны кода. Трек эталона идёт через [`atlas`](../atlas/atlas.md).
