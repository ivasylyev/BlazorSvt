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
└── 2.0.2/             ← следующий релиз (только дельты; см. правила ниже)
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
| **Schema / Migrations** | CREATE TABLE, ALTER, индексы, one-shot data fix | один раз, immutable | `Migrations/{version}/` |
| **Programmability** | views, SP, fn (`CREATE OR ALTER`) | идемпотентно, всегда latest | SoT → publish `-Mode Programmability` или `/create_programmability` на dev |

Накат на **уже развёрнутую** БД: скрипты из ещё не применённых папок релиза → затем весь Programmability.
Greenfield: `2.0.0` → `2.0.1` → … → Programmability.

Рекомендуется (пока вручную) вести учёт: какая версия уже на стенде (хотя бы запись в wiki / будущая `v2.SchemaVersion`).

## Правила с релиза 2.0.2+

1. **Папка выпущенного релиза — read-only.** Баг в уже выкаченном DDL → новый скрипт в следующем релизе, не правка старого файла.
2. В релиз кладём **только schema-дельты** (новый модуль Structure, `ALTER`, индексы, data fix). Не копируем заново все SP/view.
3. Нумерация **внутри релиза** с `001` (глобальный порядок = версия папки + номер). Исторические `038…` в `2.0.1` не переименовываем.
4. Programmability **не дублируем** в папке релиза — накат отдельным шагом из SoT.
5. SoT `Platform/` / `Modules/*/Structure/` можно обновлять для документации greenfield; на прод уходит только файл из `Migrations/`.

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

С **2.0.2** такие «полные переиздания» модуля в миграциях не делаем: дельта Structure + отдельный Programmability.

## Чеклист нового релиза

1. Создать `Migrations/X.Y.Z/` с `001.…`, `002.…` (только нужные DDL/fix).
2. Обновить этот README (строка в таблице релизов).
3. `Publish-AllSql.ps1 -Mode Release -Version X.Y.Z` → отдать DBA вместе с `-Mode Programmability` при изменении views/SP.
4. После выката — не трогать файлы в `Migrations/X.Y.Z/`.
