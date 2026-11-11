# Постановка: MCP-надстройка над API СВТ (только чтение)

Закрывает бэклог **0.3.2.17**. Статус: утверждено и реализовано. На стендах флаг выключен; на PROD не включать до закрытия T6 (раздел 12).

## 1. Цель

Дать Атласу, который подключает внешние инструменты только по MCP, чтение СВТ через те же сервисы, что `/api/v1`. Надстройка не ходит в SQL сама, не дублирует правила и не вводит своих прав.

Потребитель MCP — агентный рантайм Атласа. Основным контрактом остаётся HTTP API, MCP его не заменяет.

## 2. Границы

**В скоупе:**

- один адрес `POST /api/mcp` в том же приложении (на стендах с `PathBase` — `/v2/api/mcp`);
- пять инструментов: список справочников, схема, поиск, карточка, умный подбор;
- те же проверки доступа, что у `/api/v1`;
- флаг включения, по умолчанию выключен;
- журнал вызовов, отличимый от REST;
- тесты, правила проекта и руководство разработчика API.

**Вне скоупа:**

- запись и загрузка файлов;
- выгрузка Excel;
- ресурсы и промпты MCP;
- отдельный инструмент на каждый справочник;
- ограничения частоты запросов (это 0.3.2.9) и глубины постраничной выборки;
- замена Windows-аутентификации на OAuth;
- отдельная модель прав для ИИ;
- изменение `v2.GetBlazorGridData` и снимка OpenAPI.

## 3. Транспорт

- Официальный SDK `ModelContextProtocol.AspNetCore` **2.4.0**. Своя реализация JSON-RPC не пишется.
- Режим Streamable HTTP **без сессий** (`Stateless = true`): каждый POST самодостаточен, сервер ничего не хранит между вызовами. GET-поток и `DELETE` сессии не поддерживаются.
- Ответ на POST может прийти как `application/json` или как `text/event-stream` с одним сообщением, оба допустимы. Клиент шлёт `Accept: application/json, text/event-stream`.
- Протокол: поддерживаются клиенты 2025-06-18 и новее с рукопожатием `initialize` (проверено тестами на 2025-06-18 и 2025-11-25), а также 2026-07-28 (без `initialize`, с `server/discover`). Нижняя граница — 2025-06-18, потому что с неё есть `structuredContent` и `outputSchema`.
- **Пакеты JSON-RPC запрещены.** Тело-массив отклоняется до SDK: HTTP 400 `invalid_request`. Журнал и `ApiCallLog` рассчитаны на один вызов на запрос.
- В OpenAPI маршрут не попадает (`ExcludeFromDescription`), снимок `docs/api/svt-api-v1.openapi.json` не меняется.

**Флаг `Api:McpEnabled`** (по умолчанию `false`):

- читается при старте, после смены нужен перезапуск пула;
- при `false` маршрут SDK не регистрируется, а `ApiAccessMiddleware` отвечает на `/api/mcp` **404 ProblemDetails с кодом `mcp_disabled`** и текстом «MCP отключён на этом стенде (флаг Api:McpEnabled)». По коду понятно, что дело во флаге, а не в маршрутизации;
- порядок проверок: Windows-учётка → доверенный клиент → флаг → `X-On-Behalf-Of`. Недоверенный клиент по-прежнему получает 403 `client_not_trusted`, состояние флага ему не раскрывается.

## 4. Доступ

Всё делает `ApiAccessMiddleware`, до разбора JSON-RPC:

| Условие | Ответ |
|---|---|
| Нет Windows-учётки | 401 `unauthenticated` |
| Учётка не в `Api:TrustedClients` | 403 `client_not_trusted` |
| Флаг выключен | 404 `mcp_disabled` |
| Нет `X-On-Behalf-Of` | 400 `on_behalf_of_required` |
| Формат не `DOMAIN\user` | 400 `on_behalf_of_invalid` |
| Тело — пакет JSON-RPC | 400 `invalid_request` |
| Пользователя нет в СВТ | 403 `user_not_registered` |
| Нет ролей | 403 `user_has_no_roles` |
| Приложение останавливается | 503 `service_unavailable` |

- Заголовок `X-On-Behalf-Of` и синхронизация пользователя обязательны на **каждом** POST, включая `initialize` и `tools/list`. Так рантайм узнаёт об отказе сразу, а не на первом вызове инструмента. Цена — один короткий запрос к БД на POST.
- Имя пользователя в аргументах инструментов не передаётся. Права — только роли человека из заголовка; права служебной учётки не используются.
- Инструменты берут сервисы из области HTTP-запроса (`HttpContext.RequestServices`), той же, где middleware заполнил `CurrentUser`. Тест проверяет, что инструмент работает с правами пользователя из заголовка.
- Отказ по правам на операцию (`access_denied`) и недоступность справочника пользователей (`service_unavailable`) возникают уже внутри инструмента и возвращаются как ошибка инструмента (раздел 6).
- `X-Correlation-ID` и `Accept-Language` работают как в `/api/v1`. От языка зависят названия в схеме и тексты ошибок.

**Предусловия со стороны Атласа** (блокируют включение на стенде, не код):

- MCP-клиент рантайма умеет Windows-аутентификацию (NTLM или Kerberos);
- рантайм выставляет `X-On-Behalf-Of` на каждый запрос из аутентифицированной сессии человека, а не из ответа модели; одно MCP-соединение не используется для разных пользователей;
- клиент поддерживает протокол 2025-06-18 или новее.

## 5. Инструменты

Имена стабильны, описания и подсказки — на русском. Справочник передаётся кодом (`transport-rates`, `locations-nodes`), как в URL API.

Каждый справочник, зарегистрированный через `AddCatalogApi`, сразу виден в `list_catalogs` без правок MCP. Отдельный инструмент нужен только для операции, которая не является справочником. Умный подбор — такой случай: инструмент лежит в `Modules/SmartRateLookup/Api`, модуль регистрирует его через `services.AddMcpTool<T>()`, а сервер в `Platform/Api/Mcp` собирает все зарегистрированные инструменты. `Platform` на модуль не ссылается.

| Инструмент | Аргументы | Результат |
|---|---|---|
| `list_catalogs` | нет | Как `GET /api/v1/catalogs`: код, название RU/EN, домен. В `capabilities` нет `export`: остаются `schema`, `search`, `item` и `asOf` (только у справочников с периодом) |
| `get_catalog_schema` | `catalog` | Как `GET .../schema` |
| `search_catalog` | `catalog`, `filters`, `sort`, `page`, `pageSize`, `asOf`, `includeArchived`, `fields` | Как ответ поиска API: `items`, `totalCount`, `dataAsOf` |
| `get_catalog_item` | `catalog`, `id`, `fields` | Карточка, как `GET .../items/{id}` |
| `smart_rate_lookup` | обязательные `nodeFromCode`, `nodeToCode`, `productCode`, `transportTypeCode`, `currencyCode`; необязательный `date` (`yyyy-MM-dd`) | Как `SmartRateLookupResponse`: `found`, `rate`, `match`, `request` |

**Правила поиска** — те же, что в API, через общий `CatalogQueryTranslator`: фильтры только через И, операторы и поля из схемы, одна сортировка, `asOf` только при периоде, архив по умолчанию скрыт.

**Отличия `search_catalog` от REST:**

- `page` по умолчанию 1. `pageSize` по умолчанию 25, допустимо 1–25. Предел передаётся в переводчик параметром `maxPageSize`: REST передаёт 1000, MCP — 25. Значение вне диапазона даёт `validation_failed` с `errors.pageSize` = `out_of_range`, вместе с остальными ошибками запроса за один проход. Предел REST не меняется.
- `fields` необязателен. Без него элементы совпадают с ответом поиска API.

**Отбор полей `fields`** (в `search_catalog` и `get_catalog_item`):

- имена — из `schema.fields[].name` для поиска и из `schema.detailFields[].name` для карточки;
- регистр не важен, повторы схлопываются, порядок свойств — как в DTO, а не как в запросе;
- ключ записи (`entityKey` из схемы) добавляется всегда;
- неизвестное имя — `validation_failed`, `errors["fields[i]"]` = `unknown_field`; пустой список — `errors.fields` = `required`;
- отбор делается над уже полученными данными, запрос к БД не меняется. Ошибки `fields` поиска возвращаются вместе с ошибками фильтров; при ошибке данные не читаются.

**Карточка.** `id` — значение `entityKey`, целое (`long`).

**Умный подбор.** Поля `profile` во входной схеме нет: инструмент передаёт в сервис `Profile = null`, сервис подставляет «Импорт в CRM», как для HTTP-клиента. Лишний аргумент `profile` отклоняется как `unknown_field`. Ставка не найдена — успешный результат с `found=false`. Нет курса — ошибка `currency_rate_missing` (статус 422). Инструмент вызывает тот же `SmartRateLookupService.LookupAsync`, что HTTP-маршрут.

**Пометки инструментов:** `readOnlyHint=true`, `destructiveHint=false`, `idempotentHint=true`, `openWorldHint=false`.

**Инструкции сервера** (`instructions` в ответе рукопожатия) объясняют агенту: только чтение; порядок работы `list_catalogs` → `get_catalog_schema` → поиск или карточка; страница не больше 25 строк, при большом `totalCount` уточнять фильтры; `fields` сокращает ответ; одну ставку подбирает `smart_rate_lookup`; выгрузки Excel нет.

## 6. Результаты и ошибки инструментов

**Успех.** Каждый инструмент объявляет `outputSchema`. Результат кладётся в `structuredContent` и тем же JSON дублируется в `content[0].text` для клиентов без поддержки структурированного вывода. Сериализация — `ApiJson`: camelCase, `null` не опускается, даты `yyyy-MM-dd`, дата-время UTC с `Z`, enum строковыми кодами. У `search_catalog` элементы `items` в схеме — объект с произвольными свойствами: состав полей зависит от справочника и описан в `get_catalog_schema`.

**Ошибка.** HTTP-ответ `tools/call` остаётся 200 по JSON-RPC. Ошибка приходит как результат инструмента с `isError: true`; в `content[0].text` лежит JSON:

```json
{
  "code": "validation_failed",
  "message": "Ошибка в параметрах запроса. См. errors.",
  "status": 400,
  "traceId": "00-…",
  "errors": { "pageSize": ["out_of_range"] }
}
```

- `code` — тот же код, что в API; `message` — локализованный текст `Api.Error.{code}` по `Accept-Language`; `status` — HTTP-статус, который вернул бы REST; `traceId` — тот же идентификатор, что в ProblemDetails; `errors` — только при ошибке проверки.
- У ошибки нет `structuredContent`: клиент, сверяющий ответ с `outputSchema`, не споткнётся.

Все инструменты наследуют общий базовый класс `SvtMcpTool`, который сопоставляет исключения так:

| Исключение | `code` |
|---|---|
| `ApiException` | его `Code`, статус и `Errors` |
| `AccessDeniedException` | `access_denied` (403) |
| `DirectoryUnavailableException` | `service_unavailable` (503) |
| любое другое | `internal_error` (500); текст исключения наружу не уходит, пишется `LogError` |

Отмена запроса клиентом в результат не превращается; в журнале — 499, как в API.

**Ошибки аргументов.** У входных схем `additionalProperties: false`. Пропущенный, лишний или неверно типизированный аргумент — это результат инструмента с `validation_failed`, а не ошибка протокола: модель видит ошибку и может исправиться. Коды в `errors` по имени аргумента: `required`, `unknown_field`, `invalid_value`. Целые числа принимаются и строкой с числом. Ошибкой JSON-RPC остаётся только неизвестное имя инструмента.

## 7. Общий слой чтения

HTTP-маршруты и инструменты читают через один scoped `CatalogApiReader` в `Platform/Api/Catalogs`: список, схема, поиск, карточка по коду справочника. Типизированную часть делает адаптер `CatalogApiSource<TItem,TDetail>`, который `AddCatalogApi` регистрирует на каждый справочник (без рефлексии и `MakeGenericType`). Сигнатура `AddCatalogApi` и его вызовы в модулях не меняются.

Выгрузка Excel остаётся только в HTTP-маршруте и в reader не входит.

## 8. Журнал

- Для `/api/mcp` свойство `Channel` равно `Mcp` (`LogChannels`).
- Файловый шаблон выводит только текст сообщения, поэтому канал виден в самой строке: она начинается с `MCP` вместо `API`.
- В строку добавлен код исхода `{ApiErrorCode}`: `-` при успехе, `code` из конверта при ошибке инструмента, код ProblemDetails при отказе middleware. Это же поле получил и REST: ошибка инструмента приходит с HTTP 200, и без кода исход не различить.

```text
{ApiChannel} {ApiOperation} {Catalog} for {UserLogin} via {ApiClient}: {StatusCode} {ApiErrorCode}, rows {ApiRowCount}, {ElapsedMs} ms
```

**Операция** — имя инструмента, поэтому вызовы через MCP не смешиваются с REST (`catalogs`, `schema`, `search`, `item`, `smart_lookup`):

| Запрос | Операция |
|---|---|
| `tools/call` | `list_catalogs`, `get_catalog_schema`, `search_catalog`, `get_catalog_item`, `smart_rate_lookup` |
| `tools/call` неизвестного инструмента | `mcp_tools_call` |
| `initialize` | `mcp_initialize` |
| `server/discover` | `mcp_discover` |
| `tools/list` | `mcp_tools_list` |
| `ping` | `mcp_ping` |
| `notifications/*` | `mcp_notification` |
| пакет JSON-RPC | `mcp_batch` |
| прочее, не POST | `mcp_other` |

Служебные операции проставляет middleware по полю `method` тела, инструмент перезаписывает операцию своим именем и добавляет справочник и число строк.

Пример:

```text
MCP search_catalog transport-rates for DOMAIN\ivanov via DOMAIN\svc-atlas: 200 validation_failed, rows null, 71 ms
```

Тела запросов и ответов в журнал не пишутся.

## 9. Состав изменений

```text
BlazorSvt/BlazorSvt.csproj                                   — пакет ModelContextProtocol.AspNetCore 2.4.0
BlazorSvt/Platform/Api/ApiOptions.cs                         — McpEnabled, по умолчанию false
BlazorSvt/Platform/Api/ApiErrorCodes.cs                      — mcp_disabled
BlazorSvt/Platform/Resources/Platform(.ru-RU).resx           — Api.Error.mcp_disabled
BlazorSvt/Platform/Api/ApiAccessMiddleware.cs                — флаг, X-On-Behalf-Of на /api/mcp, запрет пакетов, операции, строка журнала
BlazorSvt/Platform/Api/ApiCallLog.cs                         — код исхода
BlazorSvt/Platform/Api/ApiProblems.cs                        — код исхода в журнал, общий текст ошибки
BlazorSvt/Platform/Api/ApiModule.cs                          — /catalogs через reader
BlazorSvt/Platform/Api/Catalogs/CatalogApiSource.cs          — типизированный адаптер справочника
BlazorSvt/Platform/Api/Catalogs/CatalogApiReader.cs          — список, схема, поиск, карточка по коду
BlazorSvt/Platform/Api/Catalogs/CatalogApiEndpoints.cs       — маршруты вызывают reader
BlazorSvt/Platform/Api/Catalogs/CatalogApiServiceCollectionExtensions.cs — регистрация адаптера
BlazorSvt/Platform/Api/Catalogs/CatalogQueryTranslator.cs    — maxPageSize, priorErrors
BlazorSvt/Platform/Api/Mcp/McpModule.cs                      — транспорт, флаг, сбор инструментов, AddMcpTool
BlazorSvt/Platform/Api/Mcp/SvtMcpTool.cs                     — базовый инструмент: результат, конверт ошибки, журнал
BlazorSvt/Platform/Api/Mcp/McpArguments.cs                   — разбор и проверка аргументов
BlazorSvt/Platform/Api/Mcp/McpFieldSelection.cs              — отбор полей
BlazorSvt/Platform/Api/Mcp/McpRequestInspector.cs            — пакет и метод JSON-RPC для журнала
BlazorSvt/Platform/Api/Mcp/CatalogMcpTools.cs                — четыре инструмента справочников
BlazorSvt/Platform/Infrastructure/Logging/LogChannels.cs     — канал Mcp
BlazorSvt/Modules/SmartRateLookup/Api/SmartRateLookupMcpTool.cs
BlazorSvt/Modules/SmartRateLookup/SmartRateLookupModule.cs   — регистрация инструмента
BlazorSvt/Host/Program.cs                                    — AddSvtMcp / MapSvtMcp рядом с API
docs/api/svt-api-v1-developers.md                            — раздел MCP
```

Обновлены правило `svt-development-patterns` (раздел API), текущее состояние в `svt-project-context` и skill `svt-architecture`. SQL, DTO справочников и снимок OpenAPI не менялись.

## 10. Тесты

**Unit, без БД** (`CatalogQueryTranslatorTests`, `McpToolTests`):

- `pageSize` 0 и 26 отклоняются при пределе 25, 25 проходит;
- ошибки, найденные до переводчика, возвращаются вместе с ошибками запроса;
- `fields` оставляет перечисленные свойства в порядке DTO и всегда ключ; без `fields` отбора нет; неизвестное имя и пустой список дают свои коды;
- неизвестный, пропущенный и неверный аргумент собираются в одну ошибку;
- `ApiException` превращается в ошибку инструмента с тем же `code`, статусом, `traceId` и `errors`; `AccessDeniedException` — в `access_denied`; неожиданное исключение — в `internal_error` без текста;
- успех: `structuredContent` совпадает с текстом в `ApiJson`;
- пометки только для чтения;
- методы протокола сопоставляются с операциями журнала.

**Через `WebApplicationFactory`** (`McpHttpTests`; фабрики `McpTestFactory` с включённым флагом и `ApiTestFactory` с выключенным):

- флаг выключен: доверенный клиент получает 404 `mcp_disabled`, недоверенный — 403;
- без пользователя, с неверным форматом и недоверенной учёткой — те же ProblemDetails, что в `/api/v1`;
- пакет — 400 `invalid_request`;
- `tools/list` возвращает ровно пять инструментов только для чтения, в умном подборе нет `profile`; инструкции сервера на месте;
- клиенты с рукопожатием на 2025-06-18 и 2025-11-25 обслуживаются без сессий;
- `list_catalogs` совпадает со списком API, но без `export`;
- поиск по умолчанию отдаёт 25 строк, фильтр и `fields` работают, ключ сохраняется;
- `pageSize` 26 и неизвестное поле дают обе ошибки, грид не вызывается;
- неизвестный аргумент отклоняется;
- карточка: неизвестный справочник и отсутствующая запись дают коды API;
- умный подбор вызывает тот же сервис с правами пользователя из заголовка; нет курса — `currency_rate_missing` (422); `profile` отклоняется.

Существующий `OpenApiContractTests` проходит без перегенерации снимка. `CatalogApiCoverageTests` по-прежнему требует `AddCatalogApi` для каждого справочника; отдельной регистрации в MCP нет.

Формат строки журнала проверен вручную на тестовом хосте; автотестом не закреплён: статический логгер Serilog общий для всех фабрик в прогоне.

## 11. Приёмка

- При включённом флаге клиент с учёткой из `TrustedClients` и заголовком `X-On-Behalf-Of` вызывает пять инструментов и получает те же данные, что соответствующие операции `/api/v1`, с пределом страницы 25 и отбором `fields`.
- Новый справочник после `AddCatalogApi` виден в `list_catalogs` без правок инструментов.
- При выключенном флаге доверенный клиент получает 404 `mcp_disabled`.
- Снимок OpenAPI не изменён.
- Вызов виден в журнале с префиксом `MCP`, именем инструмента, кодом исхода, пользователем и служебной учёткой.

## 12. Включение на стенде

- Флаг по умолчанию `false`. Включение — `Api:McpEnabled = true` в конфигурации стенда и перезапуск пула. DEV и TEST — архитектор СВТ.
- **На PROD не включать**, пока не согласованы класс данных и то, где ответы живут в контексте модели. Это пробел T6; постановка его не закрывает.
- Данные справочников попадают в контекст модели. Риск prompt injection через текстовые поля — зона ответственности Атласа.
- Ограничений частоты и глубины постраничной выборки по-прежнему нет: перебор страниц агентом нагружает БД так же, как перебор через REST.
- Ломающие изменения без версии в пути не вносятся: имена инструментов стабильны, новые аргументы — только необязательные.
