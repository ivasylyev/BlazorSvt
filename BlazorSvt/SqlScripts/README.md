# SQL-скрипты BlazorSVT

Структура повторяет модульный монолит C#: общие объекты в `Platform/`, объекты справочников — в `Modules/{Name}/`.

```
SqlScripts/
├── Platform/
│   ├── Structure/          # Схема v2, full-text catalog
│   └── Programmability/    # Универсальный grid engine
├── Modules/
│   ├── Rates/
│   │   ├── Structure/      # Snapshot-таблица, индексы, seed
│   │   └── Programmability/ # Grid-обёртки, detail, export
│   ├── Legs/
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

**Rates**

| # | Скрипт |
|---|--------|
| 1 | `Modules/Rates/Structure/01.CreateTable_TransportRateSnapshot.sql` |
| 2 | `Modules/Rates/Structure/02.CreateIndexes_TransportRateSnapshot.sql` |
| 3 | `Modules/Rates/Structure/03.Insert_TransportRateSnapshot.sql` |
| 4 | `Modules/Rates/Programmability/fn_GetTransportRateDetail.sql` |
| 5 | `Modules/Rates/Programmability/sp_GetTransportRates.sql` |
| 6 | `Modules/Rates/Programmability/sp_ExportTransportRatesFull.sql` |

**Legs**

| # | Скрипт |
|---|--------|
| 1 | `Modules/Legs/Structure/01.CreateTable_TransportLegSnapshot.sql` |
| 2 | `Modules/Legs/Structure/02.CreateIndexes_TransportLegSnapshot.sql` |
| 3 | `Modules/Legs/Structure/03.Insert_TransportLegSnapshot.sql` |
| 4 | `Modules/Legs/Programmability/fn_GetTransportLegDetail.sql` |
| 5 | `Modules/Legs/Programmability/sp_GetTransportLegs.sql` |
| 6 | `Modules/Legs/Programmability/sp_ExportTransportLegsFull.sql` |

**LocationNodes** (опционально)

| # | Скрипт |
|---|--------|
| 1 | `Modules/LocationNodes/Structure/01.CreateTable_LocationNodesSnapshot.sql` |

## Добавление нового справочника

Создать `Modules/{Name}/Structure/` и `Modules/{Name}/Programmability/` по образцу `Rates/` или `Legs/`.
Параллельно — C#-модуль в `BlazorSvt/Modules/{Name}/`.

Ключевая процедура `v2.GetBlazorGridData` (Platform) — универсальный query engine: whitelist колонок, фильтры, FTS, пагинация.
