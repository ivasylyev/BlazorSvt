# Краткий контекст

**СВТ** — MDM логистических маршрутов, тарифов, складских ограничений. Замещение легаси (Angular, распределённый монолит, Elastic, RabbitMQ, метамодель справочников, логика в SP/SQL Agent).

**Целевой стек**: Blazor Server + MS SQL. **Стиль**: модульный монолит. **Миграция**: Strangler Fig с общей БД. До Gate C источник истины домена — легаси-таблицы. На Gate C новый контур объявляется источником истины строк с редактором (колонка E не `—`); запись в легаси-таблицы продолжается до Gate D. Exit dual-run волнами → инкрементальный SoT flip по доменам справочников → единый финальный демонтаж легаси (детали — в [cutover.md](cutover.md), раздел «Критерии Gate»).

Корпоративный паспорт действующей ИС (критичность, ИСПДн, СУБД, обновление) — [architecture.md](architecture.md) §4.6.

**Текущий статус**

| | |
|---|---|
| Контур | Read-only справочники, грид, карточка, выгрузка Excel. Сквозные отчёты (веха 5.1) и дашборды (веха 12) ещё не сделаны |
| На проде | TransportRate, AverageRateLevel3, LocationsNodes, ParityRates, TransportLeg, RateType, Currency, ProductGroup, MTR, TransportTypeLevel3 |
| Имена в реестре | `Rates` → `TransportRate`, `TransportLegs` → `TransportLeg`, `TransportType_level_3` → `TransportTypeLevel3` |
| Без watermark-sync | RateType, Currency, TransportTypeLevel3: разовый снимок |
| Очередь Атласа | Релиз 2.3.48, все четыре на проде. Currency и TransportTypeLevel3 — разовый снимок. ProductGroup и MTR — watermark-sync |
| Ещё в коде | `HomeRatePivot`, `SmartRateLookup`. `HomeRatePivot` читает снимки AverageRateLevel3 и LocationsNodes; отбор ставки — в тексте SQL |
| Доступ и sync | Windows Auth, роли `dbo.Role` (любая роль = read). Watermark-sync для остальных справочников на проде |
| API | Пилот `/api/v1`, только чтение: все зарегистрированные модули и умный подбор. Код едет с приложением. Вызов Атласом с PROD не открыт, пока не закрыты [backlog.md](backlog.md) 5.2.7 и 5.2.8. [architecture.md](architecture.md) §«API для внешних фронтендов» |
| MCP | `/api/mcp`. Флаг `Api:McpEnabled` по умолчанию выключен; на PROD — после T6 |
| Import | Прототип Excel, валидаторы и UI. Staging не подключён. На проде Import закрыт |
| Тесты | Каркас unit и integration в `tests/` |

