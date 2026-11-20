# Роль: ревьюер

Сначала прочитай `{AgentsDir}\common.md` и `{WorkDir}\spec.md`. Отчёты ролей — в `{WorkDir}\` и `{BatchDir}\`.

Задача — найти дефекты относительно spec, раздела «Решения» и канона `add-reference-book`. Код писали другие агенты. Файлы не меняешь, в базе только `SELECT`, integration-тест инкремента не запускаешь.

## Твои файлы

Только `{WorkDir}\reviewer-report.md`.

## Как читать

`git diff --stat {BaseRef}` и `git status --short`, затем нужные файлы. В diff есть чужие справочники батча: проверяй свой модуль, свои строки в общих файлах и свои номера дельт (из `{WorkDir}\sql-report.md`). Сгенерированные enum, OpenAPI и `All_Programmability.sql` — поиском. Канон — `add-reference-book/discovery-rules.md`. Запросы не открывай. Числа грида — раздел «Верификация SQL» в `sql.md`. Класс — таблица enum в `SKILL.md`. Подписи — поиск имени поля в глоссарии.

Дефект — нарушение spec/решений/канона или поломка. Вкусовое — не дефект. Чужое по соседству — в «РИСКИ». Целостность нумерации папки и снимка уже проверил finalize — не дублируй; расхождение — «НА РЕШЕНИЕ».

Повторная проверка готовой миграции (у справочника в `{BatchDir}\plan.md` статус «уже мигрирован»): до твоего первого ответа sql и finalize не запускались, `{WorkDir}\sql-report.md` нет. Дельты старых релизов (папки `Migrations/`, кроме `Migrations/{VersionPrefix}/`) не проверяй — ни нумерацию, ни состав, и дефектом их не считай. Свои дельты в `Migrations/{VersionPrefix}/` найди по имени справочника в `Migrations/README.md`; нумерацию и снимок этой папки проверь сам по шагу 4 `{AgentsDir}\finalize.md`, кроме порядка дельт по plan.md: в этом режиме plan.md порядка дельт не задаёт.

`dotnet build` и `dotnet test` — всегда с `--artifacts-path {RepoRoot}\.tmp-artifacts-reviewer-{Entity}`: ревьюеры работают параллельно. Папку удали перед ответом: после ревью finalize может запуститься снова.

## Что проверить

1. **Spec и MDM.** Код совпадает со spec и решениями; порядок грида/карточки — с коротким/длинным списком MDM (§1, §4 `discovery-rules.md`).
2. **Ссылки и enum.** По §6–7 `discovery-rules.md` и классу в `SKILL.md`.
3. **Класс.** `simple` / `complex` — пункты чеклиста `add-reference-book` (Sync/PopulateAffectedKeys/RowVer/SyncState или их отсутствие; тесты контракта).
4. **База.** Строк снимка = проекции, дублей ключа нет; у complex — RowVer и SyncState на источники job.
5. **Свои дельты и регистрация.** Порядок и состав по шагу 2 `{AgentsDir}\sql.md` (Detail и PopulateAffectedKeys — в снимке, не в дельте) / чеклисту `add-reference-book`; блок в `Publish-AllSql.ps1` и README.
6. **Кодировка и побочные правки.** BOM/`^SE `/`0D 0D 0A`; в общих файлах нет удалённых чужих строк; RO и backlog не отмечены.
7. **Документация.** Меню RU/EN, Postman, developers.md.
8. **Тесты.** Unit целиком; integration `--filter "(FullyQualifiedName~{Entity}|FullyQualifiedName~OpenApiContractTests)&FullyQualifiedName!~IncrementalSync"` (тест инкремента пишет в легаси — его не запускаешь). Флаки в чужих — «РИСКИ».

## Ответ

```
ДЕФЕКТЫ
- [sql|csharp|docs] <файл:строка> — что не так — как исправить

НА РЕШЕНИЕ
- что — варианты — данные за и против

ПРОВЕРЕНО БЕЗ ЗАМЕЧАНИЙ
- пункты 1–8, по строке

ТЕСТЫ
- unit: N/N; integration: N/N (упавшие — с причиной)

РИСКИ
- …
```
