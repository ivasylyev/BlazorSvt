> Рабочая постановка SVT-5290. При расхождении с кодом и skill верны код и skill.

## Задача: read-only справочник Contractor (Контрагенты)

### Контекст

**СВТ 2.0** — переписывание MDM тарифов/маршрутов с легаси на **Blazor Server + MS SQL**. Эта задача переносит справочник **только для чтения** (грид, карточка, Excel, API). Редактор и загрузчик не входят в задачу.

Источник истины по составу: `.cursor\skills\svt-architecture\catalogs-scope.md`.  
Чеклист: `.cursor\skills\svt-architecture\backlog.md` → пункт **3.3.3**.

### Что сделать

Перенести сущность реестра **Contractor** (EntityInfo **2054**, «Контрагенты») как read-only модуль `Contractor`.

Примитив — `LegalType` (2028), таблица `dbo.PrimitiveEntityData_2028`, представление `dbo.vw_LegalType`, словарь `pei/LegalType/ai`. `vw_Contractor` устарело и не используется.

- грид + карточка + выгрузка Excel
- пункт меню домена **Ставки**, меню ЕО = да
- watermark-sync (снимок, SyncJob, RowVer, SyncState)
- регистрация в API, код `contractors`

На экран не выводить: примитив `Address`, атрибут `Status`, пустые `LegalForm` и `LevelOrg`. Расхождение состояния записи и Status на localhost не учитывать: на проде его нет.

Порядок колонок грида: `Code`, `NameSearch`, `Consignee`, `IsTrader`, `IsProcessing`, `IsSender`, `IsDebtor`, `IsCreditor`, `INN`. Подписи — `grid-column-glossary.md`, раздел Contractor.

### Как делать

1. Skill `.cursor\skills\add-reference-book\SKILL.md` целиком.
2. Строка `Contractor` в `catalogs-scope.md`.
3. Эталон — `Modules/MTR` и `SqlScripts/Modules/MTR`: обычный RO со снимком и SyncJob. Не копировать разовый снимок RateType/Currency.
4. C# только после наката и проверки SQL на `localhost` / `mdm`.
5. Unit contract-тест DTO и SyncJob; integration smoke грида, карточки и FTS; снимок OpenAPI обновить по skill.

### Критерии готовности

- [x] Модуль `BlazorSvt\Modules\Contractor\`
- [x] SQL в `BlazorSvt\SqlScripts\Modules\Contractor\`
- [x] SyncJob зарегистрирован, снимок заполнен
- [x] Грид, карточка и выгрузка читают локальную базу
- [x] `AddCatalogApi` (`contractors`)
- [x] Тесты по skill
- [ ] Пункт 3.3.3 в backlog не отмечать до ревью
