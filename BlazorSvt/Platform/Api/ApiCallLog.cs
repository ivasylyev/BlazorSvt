using Serilog;

namespace BlazorSvt.Platform.Api;

/// <summary>
/// Сведения о вызове API для итоговой строки лога и события завершения запроса Serilog.
/// Тела запросов не логируются.
/// </summary>
public static class ApiCallLog
{
    private const string OperationKey = "ApiOperation";
    private const string CatalogKey = "Catalog";
    private const string RowCountKey = "ApiRowCount";
    private const string ErrorCodeKey = "ApiErrorCode";

    public static void SetOperation(HttpContext context, string operation, string? catalog = null)
    {
        Set(context, OperationKey, operation);
        if (catalog is not null)
        {
            Set(context, CatalogKey, catalog);
        }
    }

    public static void SetRowCount(HttpContext context, int rows) => Set(context, RowCountKey, rows);

    /// <summary>
    /// Код исхода вызова. Нужен MCP: ошибка инструмента приходит с HTTP 200,
    /// и без кода строка журнала не отличила бы её от успеха.
    /// </summary>
    public static void SetErrorCode(HttpContext context, string code) => Set(context, ErrorCodeKey, code);

    public static string? Operation(HttpContext context) => context.Items[OperationKey] as string;

    public static string? Catalog(HttpContext context) => context.Items[CatalogKey] as string;

    public static int? RowCount(HttpContext context) => context.Items[RowCountKey] as int?;

    public static string? ErrorCode(HttpContext context) => context.Items[ErrorCodeKey] as string;

    private static void Set(HttpContext context, string key, object value)
    {
        context.Items[key] = value;
        context.RequestServices.GetService<IDiagnosticContext>()?.Set(key, value);
    }
}
