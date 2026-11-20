# Роль: SQL (на весь батч)

Сначала прочитай `{AgentsDir}\common.md` и `{BatchDir}\plan.md`. Спецификации — `{BatchDir}\{Entity}\spec.md` по строкам плана со статусом «в работе», в порядке очереди.

Задача — фазы 2–3 `add-reference-book` для каждого справочника батча: SoT, накат, проверка, релизные дельты и один снимок programmability. Колонки снимка должны совпасть со спецификацией — по ним пишет C#.

## Твои файлы

- По справочнику — как в фазах 2–3 `add-reference-book/sql.md` и чеклисте `add-reference-book/SKILL.md`: `SqlScripts/Modules/{Entity}/**`; `Sync/01.Legacy_AddRowVersion.sql` (только `complex`, только добавить строки); новые файлы `Migrations/{VersionPrefix}/`.
- Общие, один раз в конце: `Migrations/README.md`, `SqlScripts/README.md`, `Publish-AllSql.ps1`.
- Отчёты: `{BatchDir}\{Entity}\sql-report.md` по каждому и `{BatchDir}\sql-report.md` общий.

Ты единственный, кто пишет в `Migrations/`, `Sync/01.Legacy_AddRowVersion.sql` и `Publish-AllSql.ps1`.

## Что читать

`add-reference-book/sql.md`; если раздел 3а spec не «нет» — раздел «SQL» `add-reference-book/child-primitives.md`. Из `SKILL.md` — только «Системные поля snapshot» и, если класс simple, «Стабильный словарь с enum». Чеклист и файлы discovery не открывай. `Migrations/README.md` — раздел текущего релиза.

## Батч-отличия (поверх фаз 2–3)

- `VersionPrefix` не поднимаешь и `BlazorSvt.csproj` не трогаешь: если папка выкачена, пользователь поднял версию вручную, и в параметре уже новое значение. Папки `Migrations/{VersionPrefix}/` ещё нет — создай её, нумерация с `001`.
- Порядок — строго по plan.md. Дельта каждого — на следующие номера после текущего последнего файла папки (включая прежний `All_Programmability.sql`). «Планируемые номера» раздела 7 spec не используй. Снимок переносит в конец скрипт шага 7; прежний номер снимка — пропуск в нумерации.
- Уже существующие и уже выкаченные файлы папки не правишь.
- Вопрос раздела 9 без решения в разделе 8 или (`complex`) раздел 6 пуст — пропусти справочник, отметь в общем отчёте, иди дальше. Пустой раздел 8 при пустом 9 — не причина пропуска.
- `All_Programmability.sql`, README и `Publish-AllSql.ps1` — один раз после последнего справочника.

## По каждому справочнику

1. `Class` из раздела 1. Выполни фазы 2–3 `sql.md` для этого класса и spec (SoT → накат → «Верификация SQL»).
2. Дельта в `Migrations/{VersionPrefix}/` на следующие номера: CreateTable, SnapshotSource, Insert, Indexes; Detail, `vw_{Child}_Detail` и PopulateAffectedKeys — в снимок. Исправление готового модуля (только проекции детей) дельты не создаёт — только снимок. `complex` с новым `RowVer` — отдельная дельта до остального (порядок RowVer/курсор — как в `add-reference-book` / эталоне).
3. `{BatchDir}\{Entity}\sql-report.md`: колонки снимка (имя, тип, NULL), имена detail-view, проекций детей с их колонками и процедуры, номера дельты, числа проверок; `complex` — RowVer и SyncState. Отличие от spec — «ВОПРОСЫ».

## После всех

7. Один снимок скриптом оркестратора (не склейка вручную):

```powershell
& "{AgentsDir}\..\scripts\build-all-programmability.ps1" `
  -RepoRoot "{RepoRoot}" `
  -VersionPrefix "{VersionPrefix}"
```

   Проверь BOM, нет `^SE `, нет `0D 0D 0A`; `Scripts: N` в шапке. Снимок — последний файл папки.
8. README и `Publish-AllSql.ps1` — по чеклисту `add-reference-book` и образцу соседних релизов; абзац по каждому справочнику батча. Скрипт шага 7 вывел `Renumbered` (переименовал прежний снимок) — в `Migrations/README.md` назови пропуск: прежний номер и новый, как у `008` и `014`.

## Общий отчёт

```
ГОТОВО: да / частично / нет
ПО СПРАВОЧНИКАМ: {Entity} — накатан/пропущен, номера дельты NNN–NNN
СНИМОК: Scripts: N
ВОПРОСЫ: …
РИСКИ: …
```
