# SQL Migrations (релизные артефакты)

История **накатов на стенды**. SoT разработки по-прежнему в `Platform/`, `Modules/`, `Sync/`.
Эта папка — то, что реально уходило / будет уходить на БД в составе релиза приложения.

## Структура

```
Migrations/
├── README.md          ← этот файл
├── 2.0.0/             ← baseline (immutable после выката)
│   └── 001.…037.…
├── 2.0.1/             ← upgrade с 2.0.0 (immutable после выката)
│   └── 038.…054.…
└── 2.3.46/            ← текущий VersionPrefix (ещё не архив)
    └── 001.…005.…
```

Публикация наружу (для DBA):

```powershell
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode All
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode Release -Version 2.0.1
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode Programmability
.\BlazorSvt\SqlScripts\Publish-AllSql.ps1 -Mode FromSource   # legacy: плоский снимок из SoT
```

По умолчанию целевая папка: `C:\publish\v2` (можно `-TargetPath`).

## Два контура

| Контур | Содержимое | Свойство | Где живёт |
|--------|------------|----------|-----------|
| **Schema / Migrations** | CREATE TABLE, ALTER, индексы, one-shot data fix | один раз на релиз | `Migrations/{VersionPrefix}/` |
| **Programmability** | views, SP, fn (`CREATE OR ALTER`) | идемпотентно, всегда latest | SoT для локального наката; в папке версии — последний файл `All_Programmability.sql` |

Накат на **уже развёрнутую** БД: скрипты ещё не применённой папки версии по порядку номеров (снимок programmability — последний).
Greenfield: `2.0.0` → `2.0.1` → `2.3.46`.

Рекомендуется (пока вручную) вести учёт: какая версия уже на стенде (хотя бы запись в wiki / будущая `v2.SchemaVersion`).

## Правила текущего релиза

Имя папки — `VersionPrefix` из `BlazorSvt/BlazorSvt.csproj` (сейчас `2.3.46`), без суффикса числа коммитов. Это не «следующий номер после 2.0.1» и не пункт бэклога `2.0.2`.

1. **Папка уже выкаченного релиза — read-only.** Пока `VersionPrefix` не сменился, скрипты дописываются в текущую папку. После смены версии старую папку не правят: баг уходит новым скриптом в папку новой версии.
2. В папке версии — дельта схемы (Structure, `ALTER`, индексы, data fix), затем **последним файлом полный снимок** всей programmability `v2` (`All_Programmability.sql`). В текущем релизе это `2.3.46/005`. В `2.0.1` такой пакет стоял дважды: `045` и `052`.
3. Нумерация **внутри релиза** с `001`. Исторические `038…` в `2.0.1` не переименовываем.
4. SoT остаётся в `Platform/` и `Modules/`: локальный `Create-Programmability.ps1` читает его, а не `Migrations/`. В папку версии попадает копия дельты и снимок programmability.
5. Публикация в `C:\publish` этим правилом не требуется.

## Архив 2.0.0 / 2.0.1 (факт выкладки)

Скопировано из `C:\publish\v2` **как есть**, без правок содержимого.

| Релиз | Файлы | Что это |
|-------|-------|---------|
| **2.0.0** | `001`–`037` | Baseline: schema + programmability Platform + TransportRate, AverageRateLevel3, TransportLeg, LocationsNodes |
| **2.0.1** | `038`–`054` | ParityRates, повторный Legacy_AddRowVersion, повтор TransportLeg, два `All_Programmability`, data-fix |

Особенности `2.0.1` (не идеал, но история):

- `038` — повтор `Legacy_AddRowVersion` (идемпотентный/дополняющий)
- `039`–`044` — ParityRates
- `045`, `052` — пакеты `All_Programmability` (накат SP/view пачкой)
- `046`–`051` — повторный набор TransportLeg (эволюция snapshot после baseline)
- `053`–`054` — точечные data-fix

Полный снимок programmability в конце папки версии — штатный артефакт, не повторное переиздание одного модуля.

## Текущий релиз 2.3.46

Папка ещё пишется. После выката и смены `VersionPrefix` она становится архивом: файлы не правят, правки уходят новым скриптом в папку следующей версии.

| Файл | Что это |
|------|---------|
| `001.01.RateType_CreateTable.sql` | Первое появление `v2.RateType_Snapshot`: partition function/scheme, sequence, таблица |
| `002.vw_RateType_SnapshotSource.sql` | Проекция из `dbo.vw_RateType`; выполняется до заливки |
| `003.02.RateType_Insert.sql` | Разовая заливка из этой проекции. `SyncState` не создаётся |
| `004.03.RateType_CreateIndexes.sql` | Уникальный индекс, фильтрующие индексы, full-text по `Name` и `Comment` |
| `005.All_Programmability.sql` | Полный снимок programmability, 26 скриптов (2026-09-25) |

RateType — стабильный словарь с enum. В миграции нет `sp_RateType_PopulateAffectedKeys`, `RowVer` и `SyncState`. В снимке `005` у RateType только `vw_RateType_Detail` и `vw_RateType_SnapshotSource`. Карточка отдельным файлом не вынесена: заливка её не требует, она попадает в пакете. Предыдущий пакет `2.0.1/052` содержал 24 скрипта; два новых — эти view. Проекция `002` повторяется внутри `005` (`CREATE OR ALTER`) — так же, как у остальных модулей.

## Чеклист нового релиза

1. Создать `Migrations/{VersionPrefix}/`. Нумерация с `001`: дельта схемы, последним — `All_Programmability.sql`.
2. Обновить этот README, если сменился `VersionPrefix`.
3. После выката и смены `VersionPrefix` файлы старой папки не трогать.
