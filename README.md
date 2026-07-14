# BlazorSVT

Веб-приложение **Системы Ведения Тарифов (СВТ)** — замещающий контур legacy-системы.  
Проект реализован как **модульный монолит** на Blazor Server с read-моделью в MS SQL.

На текущем этапе доступны:
- просмотр справочников (режим только чтения);
- фильтрация, сортировка, пагинация;
- детальный просмотр записи;
- выгрузка краткого и полного отчёта в Excel.

Редактирование данных, массовая загрузка и интеграции находятся в разработке.

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
│   ├── TransportLeg/        # Транспортные плечи
│   ├── AverageRateLevel3/   # Средние тарифы L3
│   └── LocationsNodes/      # Локации / узлы
├── Import/                  # Контур загрузки данных (прототип)
└── SqlScripts/              # DDL и SP (Platform + Modules, схема v2)
    ├── Platform/            # Схема, full-text, GetBlazorGridData, Sync_*
    └── Modules/             # Structure + Programmability по справочникам

blazorbootstrap/             # Локальная сборка UI-компонентов
docs/                        # Документация Blazor Bootstrap (сторонняя, не СВТ)
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
- `Sync/` — `{Entity}SyncJob` (декларация источников RowVer);
- `{Module}Module.cs` — регистрация сервисов в DI.

**Import** — загрузка данных из Excel (отдельный контур, не смешивается с read-модулями).

---

## Реализованные справочники

| Маршрут | Модуль | Описание |
|---------|--------|----------|
| `/transportrate` | TransportRate | Транспортные тарифы |
| `/transportleg` | TransportLeg | Транспортные плечи |
| `/averageratelevel3` | AverageRateLevel3 | Средние тарифы L3 |
| `/locationsnodes` | LocationsNodes | Локации / узлы |

Имена `Rates` / `Legs` в старых материалах относятся к тем же сущностям до переименования в `Transport*`.

---

## Архитектурные принципы

- **Модульный монолит** — один deployable, чёткие границы между модулями.
- **Типизированная read-модель** — snapshot-таблицы (`v2.*_Snapshot`) вместо метамодели.
- **CQRS (read-сторона)** — snapshot-таблицы и представления для чтения; write-контур планируется отдельно.
- **SQL как единственный источник** — фильтрация, сортировка, полнотекстовый поиск через `v2.GetBlazorGridData`.
- **Strangler Fig** — переходный период с общей БД `mdm`, детальные view могут ссылаться на legacy-объекты `dbo.vw_*`.
- **Инкрементальный sync** — RowVer → `PopulateAffectedKeys` → upsert snapshot; детали в `SqlScripts/README.md`.

Подробнее: `.cursor/skills/svt-architecture/`, правило `svt-development-patterns`.

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

Алгоритм целиком — skill `.cursor/skills/add-reference-book/`. Кратко:

1. Создать модуль `Modules/{Name}/` со структурой `List/`, `Detail/`, `Sync/`.
2. Определить DTO с атрибутами:
   - `[GridSnapshot]`, `[GridColumn]` на list-DTO (метаданные для `v2.GetBlazorGridData`);
   - `[DetailSource(...)]` на detail-DTO.
3. Реализовать `*GridSettingsService` и `*DetailSettingsService`.
4. Добавить Razor-страницу, наследующую `BaseGridPage<TItem, TDetailItem>`.
5. `{Name}SyncJob` → `AddSingleton<ISnapshotSyncJob, …>()` внутри модуля.
6. Создать `{Name}Module.cs` и зарегистрировать в `Host/Program.cs`:

```csharp
builder.Services.Add{Name}Module();
```

7. Добавить SQL в `SqlScripts/Modules/{Name}/`: Structure + Programmability (SnapshotSource, PopulateAffectedKeys, Detail).

Образец — `Modules/TransportRate/` (C#) и `SqlScripts/Modules/TransportRate/` (SQL).

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
| Справочники TransportRate, TransportLeg, AverageRateLevel3, LocationsNodes | Готово (read) |
| Sync legacy → snapshot | Готово (инкремент + reconcile) |
| Редактирование записей | Не реализовано |
| Массовая загрузка | В разработке (`Import/`) |
| Интеграции | Не реализовано |
| Write-модель / CQRS (command side) | Не реализовано |
