using System.Reflection;
using System.Text.Json;
using BlazorSvt.Platform.Api.Catalogs;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary><c>list_catalogs</c> — тот же состав, что <c>GET /api/v1/catalogs</c>, но без <c>export</c> в <c>capabilities</c>.</summary>
public sealed class ListCatalogsTool(IHttpContextAccessor httpContextAccessor) : SvtMcpTool(
    httpContextAccessor,
    "list_catalogs",
    "Список справочников СВТ",
    "Справочники СВТ, доступные для чтения: код (передаётся в остальные инструменты), названия RU/EN, домен, " +
    "возможности (schema, search, item; asOf — у справочников с периодом действия). Вызывайте первым.",
    """
    { "type": "object", "properties": {}, "additionalProperties": false }
    """,
    """
    {
      "type": "object",
      "properties": {
        "items": {
          "type": "array",
          "items": {
            "type": "object",
            "properties": {
              "code": { "type": "string", "description": "Код справочника, напр. transport-rates" },
              "titleRu": { "type": "string" },
              "titleEn": { "type": "string" },
              "domain": { "type": "string" },
              "capabilities": { "type": "array", "items": { "type": "string" } }
            },
            "required": ["code", "titleRu", "titleEn", "domain", "capabilities"]
          }
        }
      },
      "required": ["items"]
    }
    """)
{
    protected override Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken)
    {
        context.Arguments.ThrowIfInvalid();
        var list = context.GetRequiredService<CatalogApiReader>().List(includeExport: false);
        ApiCallLog.SetRowCount(context.HttpContext, list.Items.Count);
        return Task.FromResult<object>(list);
    }
}

/// <summary><c>get_catalog_schema</c> — тот же ответ, что <c>GET /api/v1/catalogs/{code}/schema</c>.</summary>
public sealed class GetCatalogSchemaTool(IHttpContextAccessor httpContextAccessor) : SvtMcpTool(
    httpContextAccessor,
    "get_catalog_schema",
    "Описание полей справочника",
    "Поля справочника: имя, тип, название (язык — по Accept-Language), допустимые операторы фильтра, " +
    "значения enum (фильтровать по code), ссылки на другие справочники, поля периода, ключ записи (entityKey). " +
    "fields — поля строк поиска, detailFields — поля карточки. Вызывайте перед search_catalog.",
    """
    {
      "type": "object",
      "properties": {
        "catalog": { "type": "string", "description": "Код справочника из list_catalogs, напр. transport-rates" }
      },
      "required": ["catalog"],
      "additionalProperties": false
    }
    """,
    """
    {
      "type": "object",
      "properties": {
        "code": { "type": "string" },
        "title": { "type": "string" },
        "entityKey": { "type": "string", "description": "Поле-ключ записи; его значение — id для get_catalog_item" },
        "period": {
          "type": ["object", "null"],
          "properties": { "from": { "type": "string" }, "to": { "type": "string" } }
        },
        "fields": { "type": "array", "items": { "type": "object" } },
        "detailFields": { "type": "array", "items": { "type": "object" } }
      },
      "required": ["code", "title", "entityKey", "period", "fields", "detailFields"]
    }
    """)
{
    protected override Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken)
    {
        var code = context.Arguments.GetString("catalog", required: true);
        context.Arguments.ThrowIfInvalid();

        var reader = context.GetRequiredService<CatalogApiReader>();
        var descriptor = reader.Find(code);
        ApiCallLog.SetOperation(context.HttpContext, Name, descriptor.Code);
        return Task.FromResult<object>(reader.GetSchema(descriptor.Code));
    }
}

/// <summary>
/// <c>search_catalog</c> — правила поиска <c>POST /api/v1/catalogs/{code}/items/search</c>,
/// но страница не больше <see cref="McpModule.MaxPageSize"/> и есть отбор полей <c>fields</c>.
/// </summary>
public sealed class SearchCatalogTool(IHttpContextAccessor httpContextAccessor) : SvtMcpTool(
    httpContextAccessor,
    "search_catalog",
    "Поиск записей справочника",
    "Страница записей справочника. Фильтры объединяются только через И; поля и операторы — из get_catalog_schema " +
    "(текст — contains, не короче 3 символов; числа и даты — eq ne lt le gt ge; флаги — eq; enum — eq ne по коду). " +
    "Сортировка по одному полю. pageSize 1–25 (по умолчанию 25). asOf — записи, действующие на дату " +
    "(только у справочников с периодом). Архивные записи по умолчанию скрыты. " +
    "fields — оставить в строках только эти поля (ключ записи добавляется всегда).",
    """
    {
      "type": "object",
      "properties": {
        "catalog": { "type": "string", "description": "Код справочника из list_catalogs" },
        "filters": {
          "type": "array",
          "description": "Условия, объединяются через И",
          "items": {
            "type": "object",
            "properties": {
              "field": { "type": "string", "description": "Имя поля из fields схемы" },
              "operator": { "type": "string", "enum": ["contains", "eq", "ne", "lt", "le", "gt", "ge"] },
              "value": { "type": ["string", "number", "boolean"], "description": "Даты — yyyy-MM-dd, enum — code из схемы" }
            },
            "required": ["field", "operator", "value"],
            "additionalProperties": false
          }
        },
        "sort": {
          "type": "object",
          "properties": {
            "field": { "type": "string" },
            "direction": { "type": "string", "enum": ["asc", "desc"] }
          },
          "required": ["field"],
          "additionalProperties": false
        },
        "page": { "type": "integer", "minimum": 1, "default": 1 },
        "pageSize": { "type": "integer", "minimum": 1, "maximum": 25, "default": 25 },
        "asOf": { "type": "string", "format": "date", "description": "yyyy-MM-dd" },
        "includeArchived": { "type": "boolean", "default": false },
        "fields": {
          "type": "array",
          "items": { "type": "string" },
          "minItems": 1,
          "description": "Имена полей из fields схемы; ключ записи добавляется всегда"
        }
      },
      "required": ["catalog"],
      "additionalProperties": false
    }
    """,
    """
    {
      "type": "object",
      "properties": {
        "items": { "type": "array", "items": { "type": "object" }, "description": "Строки; поля — как в fields схемы" },
        "totalCount": { "type": "integer", "description": "Число записей под фильтром" },
        "dataAsOf": { "type": ["string", "null"], "description": "Время последней синхронизации с легаси, UTC" }
      },
      "required": ["items", "totalCount", "dataAsOf"]
    }
    """)
{
    protected override async Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken)
    {
        var args = context.Arguments;
        var options = context.JsonOptions;
        var code = args.GetString("catalog", required: true);
        var filters = args.GetObject<List<CatalogFilterRequest>>("filters", options);
        var sort = args.GetObject<CatalogSortRequest>("sort", options);
        var page = args.GetInt32("page") ?? 1;
        var pageSize = args.GetInt32("pageSize") ?? McpModule.MaxPageSize;
        var asOf = args.GetDate("asOf");
        var includeArchived = args.GetBoolean("includeArchived") ?? false;
        var fields = args.GetStringList("fields");
        args.ThrowIfInvalid();

        var reader = context.GetRequiredService<CatalogApiReader>();
        var descriptor = reader.Find(code);
        ApiCallLog.SetOperation(context.HttpContext, Name, descriptor.Code);

        var fieldErrors = new Dictionary<string, string[]>();
        var selected = McpFieldSelection.Resolve(
            fields,
            CatalogFields.For(descriptor.ItemType).Select(f => f.Name).ToList(),
            CatalogMcpFields.EntityKey(descriptor),
            "fields",
            fieldErrors);

        var request = new CatalogSearchRequest
        {
            Filters = filters,
            Sort = sort,
            Page = page,
            PageSize = pageSize,
            AsOf = asOf,
            IncludeArchived = includeArchived
        };
        var response = await reader.SearchAsync(
            descriptor.Code, request, page, pageSize, cancellationToken, McpModule.MaxPageSize, fieldErrors);

        ApiCallLog.SetRowCount(context.HttpContext, response.Items.Count);
        if (selected is null)
        {
            return response;
        }

        var items = response.Items
            .Select(item => (object)McpFieldSelection.Project(JsonSerializer.SerializeToElement(item, options), selected))
            .ToList();
        return response with { Items = items };
    }
}

/// <summary><c>get_catalog_item</c> — карточка, как <c>GET /api/v1/catalogs/{code}/items/{id}</c>, с отбором полей <c>fields</c>.</summary>
public sealed class GetCatalogItemTool(IHttpContextAccessor httpContextAccessor) : SvtMcpTool(
    httpContextAccessor,
    "get_catalog_item",
    "Карточка записи справочника",
    "Все поля карточки записи (detailFields схемы). id — значение ключа записи (entityKey) из строки search_catalog. " +
    "fields — оставить только эти поля карточки.",
    """
    {
      "type": "object",
      "properties": {
        "catalog": { "type": "string", "description": "Код справочника из list_catalogs" },
        "id": { "type": "integer", "description": "Значение ключа записи (entityKey из схемы)" },
        "fields": {
          "type": "array",
          "items": { "type": "string" },
          "minItems": 1,
          "description": "Имена полей из detailFields схемы"
        }
      },
      "required": ["catalog", "id"],
      "additionalProperties": false
    }
    """,
    """
    { "type": "object", "description": "Поля карточки — как в detailFields схемы" }
    """)
{
    protected override async Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken)
    {
        var args = context.Arguments;
        var code = args.GetString("catalog", required: true);
        var id = args.GetInt64("id", required: true);
        var fields = args.GetStringList("fields");
        args.ThrowIfInvalid();

        var reader = context.GetRequiredService<CatalogApiReader>();
        var descriptor = reader.Find(code);
        ApiCallLog.SetOperation(context.HttpContext, Name, descriptor.Code);

        var fieldErrors = new Dictionary<string, string[]>();
        var selected = McpFieldSelection.Resolve(
            fields,
            CatalogMcpFields.DetailFields(descriptor),
            CatalogMcpFields.EntityKey(descriptor),
            "fields",
            fieldErrors);
        if (fieldErrors.Count > 0)
        {
            throw ApiException.Validation(fieldErrors);
        }

        var item = await reader.GetItemAsync(descriptor.Code, id!.Value, cancellationToken);
        ApiCallLog.SetRowCount(context.HttpContext, 1);

        return selected is null
            ? item
            : McpFieldSelection.Project(JsonSerializer.SerializeToElement(item, context.JsonOptions), selected);
    }
}

/// <summary>Имена полей контракта для отбора <c>fields</c>: те же, что в схеме справочника.</summary>
internal static class CatalogMcpFields
{
    public static string? EntityKey(CatalogApiDescriptor descriptor) =>
        GridColumnMetadataBuilder.GetMetadata(descriptor.ItemType).EntityKeyPropertyName is { } key
            ? ApiJson.FieldName(key)
            : null;

    public static IReadOnlyList<string> DetailFields(CatalogApiDescriptor descriptor) =>
        descriptor.DetailType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => ApiJson.FieldName(p.Name))
            .ToList();
}
