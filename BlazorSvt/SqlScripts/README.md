# SQL-скрипты BlazorSVT

Структура повторяет модульный монолит C#: общие объекты в `Platform/`, объекты справочников — в `Modules/{Name}/`.

```
SqlScripts/
├── Platform/
│   ├── Structure/          # Схема v2, full-text catalog
│   └── Programmability/    # Универсальный grid engine
├── Modules/
│   ├── TransportRate/
│   │   ├── Structure/      # Snapshot-таблица, индексы, seed
│   │   └── Programmability/ # Grid-обёртки, detail, export
│   ├── TransportLeg/
│   │   ├── Structure/
│   │   └── Programmability/
│   └── LocationNodes/      # SQL-only (C#-модуль в разработке)
│       └── Structure/
└── Translations_info.txt   # Справочник переводов полей (не для деплоя)
```

## Порядок выполнения

### 1. Platform (обязательно первым)

| # | Скрипт |
|---|--------|
| 1 | `Platform/Structure/00.Create_Schema.sql` |
| 2 | `Platform/Structure/01.Create_Fulltext_Catalog.sql` |
| 3 | `Platform/Programmability/fn_GetDateSqlOperator.sql` |
| 4 | `Platform/Programmability/sp_GetBlazorGridData.sql` |

### 2. Modules (по одному модулю: Structure → Programmability)

**TransportRate**

| # | Скрипт |
|---|--------|
| 1 | `Modules/TransportRate/Structure/01.TransportRate_CreateTable.sql` |
| 2 | `Modules/TransportRate/Structure/02.TransportRate_Insert.sql` |
| 3 | `Modules/TransportRate/Structure/03.TransportRate_CreateIndexes.sql` |
| 4 | `Modules/TransportRate/Programmability/vw_TransportRate_Detail.sql` |
| 5 | `Modules/TransportRate/Programmability/TransportRate_Get.sql` |
| 6 | `Modules/TransportRate/Programmability/TransportRate_ExportFull.sql` |

**TransportLeg**

| # | Скрипт |
|---|--------|
| 1 | `Modules/TransportLeg/Structure/01.TransportLeg_CreateTable.sql` |
| 2 | `Modules/TransportLeg/Structure/02.TransportLeg_Insert.sql` |
| 3 | `Modules/TransportLeg/Structure/03.TransportLeg_CreateIndexes.sql` |
| 4 | `Modules/TransportLeg/Programmability/vw_TransportLeg_Detail.sql` |
| 5 | `Modules/TransportLeg/Programmability/TransportLeg_Get.sql` |
| 6 | `Modules/TransportLeg/Programmability/TransportLeg_ExportFull.sql` |

**LocationNodes** (опционально)

| # | Скрипт |
|---|--------|
| 1 | `Modules/LocationNodes/Structure/01.LocationNodes_CreateTable.sql` |

## Добавление нового справочника

Создать `Modules/{Name}/Structure/` и `Modules/{Name}/Programmability/` по образцу `TransportRate/` или `TransportLeg/`.
Параллельно — C#-модуль в `BlazorSvt/Modules/{Name}/`.

Ключевая процедура `v2.GetBlazorGridData` (Platform) — универсальный query engine: whitelist колонок, фильтры, FTS, пагинация.

## Накат Programmability (dev)

```powershell
.\BlazorSvt\SqlScripts\Create-Programmability.ps1
```

Скрипт читает строку подключения из `appsettings.json`, выполняет Platform и все модули; при ошибке останавливается.
В Cursor: `/create_programmability`.

