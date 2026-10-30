# API СВТ 2.0 — руководство для разработчиков Атласа

Пилот **только чтение**. Контракт — снимок OpenAPI в репозитории СВТ: [`svt-api-v1.openapi.json`](svt-api-v1.openapi.json). Бизнес-описание: [`svt-api-v1-business.md`](svt-api-v1-business.md).

Неизвестные поля в JSON-ответах **игнорируйте** (форвард-совместимость при тираже справочников).

Изменения контракта и вопросы по стендам — **почта архитектору СВТ**.

## Базовый URL

На стендах СВТ 2.0 приложение смонтировано с `PathBase` **`/v2`**:

```text
https://{host}/v2/api/v1/...
```

| Стенд | OpenAPI (живой) | Примечание |
|-------|-----------------|------------|
| **DEV** | `https://s001itd-0085.dev002.local/v2/api/openapi/v1.json` | Отдельный контур; интеграция **не обязательна** (решение команды Атласа) |
| **TEST** | `https://s001tst-as-svt.sibur.local/v2/api/openapi/v1.json` | Защищённый контур; интеграция **обязательна** |
| **PROD** | `https://s001as-svt.sibur.local/v2/api/openapi/v1.json` | Защищённый контур |

| Ресурс | Путь |
|--------|------|
| OpenAPI (живой) | `GET /v2/api/openapi/v1.json` |
| Swagger UI (если включён) | `/v2/api/swagger` |
| Health | `GET /v2/hc` (без аутентификации API) |

Локально у разработчиков СВТ `PathBase` часто `/` — тогда префикс `/v2` не нужен.

Версия в пути: **`v1`**. В ответах есть заголовок `Sunset` (плановая дата вывода версии).

**Деплой конфигурации API:** DEV и TEST — архитектор СВТ; PROD — заявка во [ВКУС](https://vkus.sibur.local/portal/serviceCall.html?serviceCall=serviceCall$2198770158). Нет сетевого доступа между контурами Атласа и СВТ — тоже заявка во ВКУС.

## Аутентификация и права

Два уровня:

1. **Вызывающий (клиент API)** — Windows Authentication IIS. Для пилота достаточно **NTLM** (Kerberos — по согласованию с инфраструктурой). Учётка процесса Атласа (рекомендуется **gMSA**) должна быть в `Api:TrustedClients` на стороне СВТ (`DOMAIN\svc`). Иначе **403** `client_not_trusted` на весь `/api`, включая OpenAPI.
2. **Конечный пользователь** — заголовок **`X-On-Behalf-Of: DOMAIN\user`** на все операции **`/api/v1/*`**. Права — **только** роли этого пользователя в СВТ (`dbo.User` / `dbo.UserRole`). Права служебной учётки в API не используются. В пилоте любая роль = **Read** на все справочники API и на smart-lookup.

### Служебная УЗ (gMSA)

1. Команда Атласа заказывает gMSA у инфраструктуры: отдельно для **TEST** и **PROD** (DEV — по необходимости).
2. Сообщает имя архитектору СВТ **или** указывает в заявке ВКУС (PROD).
3. Архитектор вносит УЗ в `Api:TrustedClients` на DEV/TEST; на PROD — через ВКУС.

### Конечные пользователи

Тот же процесс, что доступ к **легаси СВТ**:

- **TEST / PROD** — портал [ЕСУИД](https://idmoam.sibur.local/)
- **DEV** — через архитектора СВТ

Для **smoke на TEST** в `X-On-Behalf-Of` можно указать ту же TEST gMSA, что и в TrustedClients. Тогда gMSA должна быть **также** заведена в СВТ как пользователь с ролями (не путать с PROD gMSA).

| Ситуация | HTTP | `code` |
|----------|------|--------|
| Нет Windows-идентичности | 401 | `unauthenticated` |
| Учётка не в TrustedClients | 403 | `client_not_trusted` |
| Нет `X-On-Behalf-Of` | 400 | `on_behalf_of_required` |
| Неверный формат (нужен `DOMAIN\user`) | 400 | `on_behalf_of_invalid` |
| Пользователь не в СВТ | 403 | `user_not_registered` |
| Пользователь без ролей | 403 | `user_has_no_roles` |
| Нет права на операцию | 403 | `access_denied` |

OpenAPI/Swagger UI: TrustedClients обязателен; **`X-On-Behalf-Of` для них не нужен**.

## Общие заголовки

| Заголовок | Обязателен | Назначение |
|----------|------------|------------|
| `X-On-Behalf-Of` | да на `/api/v1/*` | `DOMAIN\user` — пользователь Атласа |
| `X-Correlation-ID` | нет | Корреляция; возвращается в ответе; если не передан — генерируется |
| `Accept-Language` | нет | `ru-RU` (по умолчанию) или `en-US` — язык заголовков полей и текстов ошибок. Данные RU/EN в полях приходят как есть |
| `If-None-Match` | нет | Для списка справочников и `/schema` — возможен **304** при совпадении ETag |

На ответах с данными: `Cache-Control: no-store` (у схемы/списка каталогов — `no-cache` + ETag).

## JSON-контракт

- **camelCase**
- **`null` не опускается**
- даты: `yyyy-MM-dd`
- дата-время: ISO 8601 UTC с суффиксом `Z`
- enum: **строковые коды** (имена членов C#, напр. `RUB`, `Tender`, `Short` / `Full`)
- ошибки: Problem Details + поля **`code`**, **`traceId`**; при валидации — **`errors`** по полям

Ветвиться в клиенте нужно по **`code`** (и по кодам в `errors`), не по тексту сообщения.

## Операции

### Справочники

Один набор маршрутов на каждый зарегистрированный справочник `{code}`:

| Метод | Путь | Назначение |
|-------|------|------------|
| `GET` | `/api/v1/catalogs` | Список доступных справочников (код, названия, домен, capabilities), ETag |
| `GET` | `/api/v1/catalogs/{code}/schema` | Поля списка и карточки, операторы фильтров, enum, ссылки, период, `entityKey`, ETag |
| `POST` | `/api/v1/catalogs/{code}/items/search` | Поиск с фильтрами и пагинацией |
| `GET` | `/api/v1/catalogs/{code}/items/{id}` | Карточка; `{id}` — значение поля `entityKey` из schema |
| `POST` | `/api/v1/catalogs/{code}/export` | Excel `.xlsx` в теле ответа |

#### `capabilities` в `GET /catalogs`

| Значение | Смысл |
|----------|--------|
| `schema` | Есть `GET .../schema` |
| `search` | Есть `POST .../items/search` |
| `item` | Есть `GET .../items/{id}` |
| `export` | Есть `POST .../export` |
| `asOf` | У справочника есть период действия; в search/export можно передать `asOf`. Без `asOf` в списке capabilities параметр `asOf` → 400, в `errors.asOf` — `not_supported` |

Коды на момент пилота:

| code | Справочник |
|------|------------|
| `transport-rates` | Транспортные ставки |
| `average-rates-level3` | Средневзвешенные ставки 3 ур. |
| `parity-rates` | Паритетные ставки |
| `rate-types` | Типы ставок |
| `currencies` | Валюты |
| `locations-nodes` | Узлы / локации |
| `transport-legs` | Транспортные плечи |

Актуальный список всегда берите из `GET /catalogs` или из OpenAPI. Новый справочник появляется после регистрации в СВТ 2.0 — отдельный «второй» контракт не нужен.

**Очередь RO для кодов smart-lookup:** `Currency` доступен как `currencies`. Ещё не перенесены `MTR`, `ProductGroup`, `TransportType_level_3`. До появления этих модулей в API — разовые выгрузки от архитектора СВТ **по запросу** (для первого smoke на TEST обычно не нужны, если коды уже есть у Атласа).

Неизвестный `{code}` → **404** `catalog_not_found`. Нет записи → **404** `item_not_found`.

#### Поиск (`POST .../items/search`)

Тело (пример):

```json
{
  "filters": [
    { "field": "nodeFromCode", "operator": "contains", "value": "MSK" }
  ],
  "sort": { "field": "startDate", "direction": "desc" },
  "page": 1,
  "pageSize": 50,
  "asOf": "2026-10-22",
  "includeArchived": false
}
```

Правила:

- фильтры только через **И**
- имена полей и допустимые `operator` — из `/schema` (`operators` у поля)
- текст: `contains` (после нормализации как в СВТ — не короче 3 символов)
- числа / даты / ключи: `eq`, `ne`, `lt`, `le`, `gt`, `ge`
- флаги: `eq`
- enum: `eq` / `ne` по **коду** из schema
- сортировка — **одно** поле; `direction`: `asc` (по умолчанию) / `desc`
- `page` с 1; `pageSize` **1–1000** (по умолчанию 100; больше → 400)
- по умолчанию архивные не возвращаются (`includeArchived: false`)
- `asOf` — только у справочников с периодом: записи, где начало ≤ дата ≤ конец

Ответ:

```json
{
  "items": [ /* строки как в гриде СВТ */ ],
  "totalCount": 123,
  "dataAsOf": "2026-10-22T10:15:00.000Z"
}
```

`dataAsOf` — UTC-время последней синхронизации snapshot с легаси (может быть `null`). Возможна **краткая задержка** относительно CRM сразу после правок в легаси — этого достаточно учитывать в UI Атласа.

#### Выгрузка Excel (`POST .../export`)

```json
{
  "kind": "Short",
  "filters": [],
  "sort": null,
  "asOf": null,
  "includeArchived": false
}
```

- `kind`: **`Short`** — колонки списка; **`Full`** — поля карточки (как в OpenAPI enum `CatalogExportKind`)
- фильтры/сортировка/`asOf` — как у поиска
- ответ — файл `.xlsx` (синхронно, **без лимита строк** в пилоте)

Для больших выборок задайте **увеличенный read timeout** на стороне Атласа. Выгрузка делит процесс и пул соединений с UI СВТ.

### Умный подбор ставки

`POST /api/v1/rates/smart-lookup`

Поведение как у продовой CRM-процедуры `CalcTransportRateCRM`: одна наиболее подходящая средневзвешенная ставка 3-го уровня с обобщением (уровни 1–8), затем доп. узлы отправления организации DPO.

Доступ: как у чтения справочников (любая роль пользователя).

Запрос:

```json
{
  "nodeFromCode": "...",
  "nodeToCode": "...",
  "productCode": "...",
  "transportTypeCode": "...",
  "currencyCode": "RUB",
  "date": "2026-10-22",
  "profile": null
}
```

| Поле | Обязательно | Смысл |
|------|-------------|--------|
| `nodeFromCode` / `nodeToCode` | да | Коды узлов |
| `productCode` | да | Код продукта (МТР) |
| `transportTypeCode` | да | Тип транспорта 3-го уровня |
| `currencyCode` | да | Валюта ответа |
| `date` | нет | Дата действия ставки и курса; по умолчанию — сегодня (сервер, Москва) |
| `profile` | нет | В пилоте **не передавать** (или `null`): сервер подставит «Импорт в CRM» |

Неизвестный код узла/продукта/транспорта/валюты/профиля → **400** `validation_failed`, в `errors.{field}` — **`not_found`** (это не HTTP 404).

Ответ:

- `found: false` — ставка не найдена (**200**, не ошибка); `rate` и `match` будут `null`
- `found: true` — заполнены `rate` (сумма в запрошенной валюте, оригинал, курс, период, тип ставки, узлы, продукт…) и `match` (уровень 1–8, флаги обобщения, текст для UI)
- `request` — разрешённые/обогащённые параметры запроса

Нет курса для пересчёта → **422** `currency_rate_missing`.

Осознанное отличие пилота: ставки — из snapshot СВТ 2.0; часть справочников подбора пока читается из легаси-view (техдолг до переноса на снимки).

## Коды ошибок

### Верхний уровень (поле `code` в Problem Details)

| code | Типичный HTTP |
|------|----------------|
| `invalid_request` | 400 |
| `validation_failed` | 400 |
| `unauthenticated` | 401 |
| `client_not_trusted` | 403 |
| `on_behalf_of_required` / `on_behalf_of_invalid` | 400 |
| `user_not_registered` / `user_has_no_roles` / `access_denied` | 403 |
| `not_found` / `catalog_not_found` / `item_not_found` | 404 |
| `currency_rate_missing` | 422 |
| `service_unavailable` | 503 |
| `internal_error` | 500 |

При остановке приложения возможен 503 с `Retry-After`. Отмена запроса клиентом уважается (в логах СВТ — 499).

### Коды в `errors` (при `validation_failed`)

| code | Когда |
|------|--------|
| `required` | Пустое обязательное поле; нет `field` / `operator` / `value` в фильтре |
| `unknown_field` | Поле не из `/schema` |
| `not_filterable` / `not_sortable` | Поле есть, но фильтр или сортировка запрещены |
| `operator_not_allowed` | Оператор не из `operators` поля в schema |
| `invalid_value` | Неверный тип, формат, enum или `sort.direction` |
| `value_too_short` | Текст для `contains` после очистки короче 3 символов |
| `out_of_range` | `page` &lt; 1 или `pageSize` вне 1–1000 |
| `not_supported` | `asOf` у справочника без периода |
| `not_found` | **Только smart-lookup:** код в поле запроса не найден в справочниках подбора |

## Рекомендуемый порядок интеграции

1. Заказать gMSA у инфраструктуры (TEST, PROD; DEV по необходимости) → сообщить архитектору СВТ / указать в заявке ВКУС → внести в `Api:TrustedClients`. Проверить Windows Auth (**NTLM**).
2. Забрать [`svt-api-v1.openapi.json`](svt-api-v1.openapi.json), сгенерировать клиент (у операций есть стабильные `operationId`).
3. Smoke на **TEST**: `GET /v2/api/openapi/v1.json` под gMSA → `GET /v2/api/v1/catalogs` с `X-On-Behalf-Of` (TEST gMSA или пользователь с ролями в СВТ).
4. Для UI справочника: `schema` → `search` / `items/{id}` → при необходимости `export`.
5. Для подбора ставки: `POST .../rates/smart-lookup` без `profile` (или `null`); обрабатывать `found: false`, `currency_rate_missing` и field-`not_found`.

Ручной smoke (локальный стенд разработчика СВТ, не контракт): Postman-коллекция [`svt-api-v1.vasilevivv-local-tests.postman.json`](svt-api-v1.vasilevivv-local-tests.postman.json).

## Ограничения пилота (важно для клиента)

- Только чтение; запись через API — отдельная постановка после контура записи СВТ.
- Нет rate limit — не долбите стенд циклами без паузы.
- Синхронный Excel без потолка строк — нужен большой timeout и осознанный объём выборки.
- Список справочников растёт по мере тиража RO в СВТ 2.0; ориентируйтесь на `GET /catalogs` и обновления OpenAPI (уведомление — почта архитектору СВТ).
- OIDC / отдельный IdP — вне текущего пилота.
