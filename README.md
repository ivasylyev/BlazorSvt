# BlazorSVT

Веб-приложение **Системы Ведения Тарифов (СВТ)** — замещающий контур legacy-системы.  
Проект реализован как **модульный монолит** на Blazor Server с read-моделью в MS SQL.

На текущем этапе доступны:
- просмотр справочников (режим только чтения);
- фильтрация, сортировка, пагинация;
- детальный просмотр записи;
- выгрузка краткого и полного отчёта в Excel;
- пилот read-only API `/api/v1` и MCP `/api/mcp` (на проде MCP выключен).

Редактирование записей и интеграции в новом контуре ещё не сделаны. Прототип загрузки Excel есть в `Import/`; staging не подключён, на проде Import закрыт.

Актуальный статус — `.cursor/skills/svt-architecture/status.md`. Карта документов — [`docs/README.md`](docs/README.md).

---

## Стек

| Компонент | Технология |
|-----------|------------|
| UI | Blazor Server (.NET 9), [Blazor Bootstrap](blazorbootstrap/) |
| Доступ к данным | Dapper |
| БД | Microsoft SQL Server |
| Логирование | Serilog |
| Локализация | ru-RU, en-US |
| Экспорт | ClosedXML |

---

## Структура репозитория

```
BlazorSvt/                  # Основное приложение
├── Host/                    # Точка входа, layout, общие страницы
├── Platform/                # Общий фреймворк (grid, sync, отчёты, инфраструктура)
├── Modules/                 # Доменные модули (вертикальные срезы)
│   ├── TransportRate/       # Транспортные тарифы
│   ├── AverageRateLevel3/   # Средневзвешенные ставки L3
│   ├── ParityRates/         # Паритетные ставки
│   ├── TransportLeg/        # Транспортные плечи
│   ├── LocationsNodes/      # Локации / узлы
│   ├── RateType/            # Типы ставок
│   ├── Currency/            # Валюты
│   ├── ProductGroup/        # Транспортные группы
│   ├── MTR/                 # Продукты (МТР)
│   └── TransportTypeLevel3/ # Типы транспорта, 3-й уровень
├── Import/                  # Контур загрузки данных (прототип)
└── SqlScripts/              # DDL и SP (Platform + Modules, схема v2)
    ├── Platform/            # Схема, full-text, GetBlazorGridData, Sync_*
    └── Modules/             # Structure + Programmability по справочникам

blazorbootstrap/             # Локальная сборка UI-компонентов
docs/                        # Карта — docs/README.md
├── user-guide/              # Руководство пользователя СВТ 2.0 (RU/EN, Confluence)
├── api/                     # OpenAPI-снимок и описание API
├── estimation/              # Оценки трудозатрат трансформации
└── …
tests/                       # Unit + integration (read-only SQL)
```

### Слои приложения

**Host** — `Program.cs`, маршрутизация, layout, локализация, контроллеры.

**Platform** — переиспользуемый код, не зависящий от конкретного справочника:
- `Grid` — `GenericGrid`, `GridDataService`, атрибуты list/detail DTO;
- `Sync` — оркестрация legacy → snapshot (`ISnapshotSyncJob`, scheduler);
- `Reporting` — экспорт в Excel, подтверждение больших выгрузок;
- `Infrastructure` — конфигурация, логирование, работа с SQL;
- `Domain/IdsEnum` — стабильные legacy ItemId (не sync-cascade);
- `UI` — базовые компоненты (`SvtComponentBase`, меню, ошибки).

**Modules** — изолированные доменные модули. Каждый модуль содержит:
- `List/` — DTO списка, страница grid, настройки колонок;
- `Detail/` — DTO детализации, настройки detail view;
- `Sync/` — `{Entity}SyncJob` (декларация источников RowVer); у стабильного словаря с enum этой папки нет;
- `{Module}Module.cs` — регистрация сервисов в DI.

**Import** — загрузка данных из Excel (отдельный контур, не смешивается с read-модулями).

---

## Справочники на проде (read-only)

Список и оговорки (стабильные словари без watermark-sync, легаси-имена) — в `.cursor/skills/svt-architecture/status.md`. Сейчас это:

| Маршрут | Модуль | Описание |
|---------|--------|----------|
| `/transportrate` | TransportRate | Транспортные тарифы |
| `/averageratelevel3` | AverageRateLevel3 | Средневзвешенные ставки L3 |
| `/parityrates` | ParityRates | Паритетные ставки |
| `/transportleg` | TransportLeg | Транспортные плечи |
| `/locationsnodes` | LocationsNodes | Локации / узлы |
| `/ratetype` | RateType | Типы ставок |
| `/currency` | Currency | Валюты |
| `/productgroup` | ProductGroup | Транспортные группы |
| `/mtr` | MTR | Продукты (МТР) |
| `/transporttypelevel3` | TransportTypeLevel3 | Типы транспорта, 3-й уровень |

Имена `Rates` / `TransportLegs` / `TransportType_level_3` в реестре легаси относятся к модулям `TransportRate`, `TransportLeg` и `TransportTypeLevel3`.

Рядом в коде, без отдельной строки меню справочника: `HomeRatePivot`, `SmartRateLookup`.

---

## Архитектурные принципы

- **Модульный монолит** — один deployable, чёткие границы между модулями.
- **Типизированная read-модель** — snapshot-таблицы (`v2.*_Snapshot`) вместо метамодели.
- **CQRS (read-сторона)** — snapshot-таблицы и представления для чтения; write-контур планируется отдельно.
- **SQL как единственный источник** — фильтрация, сортировка, полнотекстовый поиск через `v2.GetBlazorGridData`.
- **Strangler Fig** — переходный период с общей БД `mdm`, детальные view могут ссылаться на legacy-объекты `dbo.vw_*`.
- **Инкрементальный sync** — RowVer → `PopulateAffectedKeys` → upsert snapshot; детали в `SqlScripts/README.md`.

Подробнее: `.cursor/skills/svt-architecture/` (`architecture.md` — архитектура, `roadmap.md` — порядок и сводка вех, `cutover.md` — критерии Gate, `backlog.md` — чеклист работ), правило `svt-development-patterns`.

---

## Требования

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MS SQL Server с БД `mdm` и развёрнутыми скриптами из `BlazorSvt/SqlScripts/`

---

## Запуск

```powershell
# из корня репозитория
dotnet restore
dotnet run --project BlazorSvt/BlazorSvt.csproj
```

Приложение по умолчанию: `http://localhost:5126` (профиль `http` в `launchSettings.json`).

### Конфигурация

Основные параметры — в `BlazorSvt/appsettings.json`:

| Секция | Назначение |
|--------|------------|
| `PathBase` | Базовый путь приложения (например `/v2`) |
| `Database:MdmDb` | Строка подключения к БД |
| `Database:DefaultQueryTimeoutSeconds` | Таймаут запросов grid |
| `Database:ReportQueryTimeoutSeconds` | Таймаут запросов отчётов |
| `Reports:*ReportConfirmationThreshold` | Порог подтверждения перед выгрузкой |
| `Sync` | Интервал / blackout / reconcile для snapshot sync jobs |
| `Serilog` | Настройки логирования (файл `logs/SVT_Blazor-*.txt`) |

> Для локальной разработки рекомендуется хранить строку подключения в User Secrets, а не в репозитории.

---

## SQL-скрипты

Структура `BlazorSvt/SqlScripts/` повторяет модульный монолит:

1. `Platform/` — схема `v2`, full-text catalog, `GetBlazorGridData`, generic Sync
2. `Modules/{Name}/` — snapshot-таблицы, индексы, SnapshotSource / PopulateAffectedKeys / Detail

Порядок выполнения и модель sync — в [`BlazorSvt/SqlScripts/README.md`](BlazorSvt/SqlScripts/README.md).

---

## Добавление нового справочника

Алгоритм целиком — skill `.cursor/skills/add-reference-book/` (`SKILL.md`, затем фазы `discovery.md`, `sql.md`, `csharp.md`). Образец обычного справочника — `Modules/TransportRate/`. Стабильный словарь с enum (без `SyncJob`) описан в том же навыке.

---

## Сборка

```powershell
dotnet build BlazorSvt/BlazorSvt.csproj
```

```powershell
dotnet test --filter "Category=Unit"   # без БД
```

---

## Статус разработки

| Область | Статус |
|---------|--------|
| Фреймворк grid (read-only) | Готово |
| Отчёты Excel | Готово |
| 10 справочников read-only (см. таблицу выше) | На проде |
| Sync legacy → snapshot | Готово (инкремент + reconcile; стабильные словари — разовый снимок) |
| Пилот read-only API `/api/v1` | Готово |
| Редактирование записей | Не реализовано |
| Массовая загрузка | Прототип в `Import/`; на проде закрыт |
| Интеграции | Не реализовано |
| Write-модель / CQRS (command side) | Не реализовано |
