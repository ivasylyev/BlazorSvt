using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary>Флаг <see cref="ApiOptions.McpEnabled"/>, прочитанный при старте: от него зависит, зарегистрирован ли маршрут.</summary>
public sealed record McpEndpointState(bool Enabled);

/// <summary>
/// MCP-надстройка над API СВТ (только чтение): <c>POST /api/mcp</c>, Streamable HTTP без сессий.
/// Второй протокол поверх тех же сервисов и проверок доступа, что <c>/api/v1</c>; основной контракт — HTTP API.
/// </summary>
public static class McpModule
{
    public const string Path = "/api/mcp";

    /// <summary>Предел страницы поиска для агента; у REST свой (1000) и не меняется.</summary>
    public const int MaxPageSize = 25;

    public const string Instructions =
        "Чтение справочников СВТ (Система ведения тарифов): ставки, маршруты, узлы, продукты и др. Только чтение. " +
        "Порядок работы: list_catalogs — какие справочники есть; get_catalog_schema — поля, операторы фильтра, " +
        "значения enum и ключ записи; затем search_catalog или get_catalog_item. " +
        "Страница поиска — не больше 25 строк; при большом totalCount уточните фильтры. " +
        "Аргумент fields сокращает ответ до нужных полей. " +
        "Одну ставку по маршруту подбирает smart_rate_lookup. Выгрузки Excel здесь нет.";

    public static IServiceCollection AddSvtMcp(this IServiceCollection services, IConfiguration configuration)
    {
        var enabled = configuration.GetSection(ApiOptions.SectionName).GetValue<bool>(nameof(ApiOptions.McpEnabled));
        services.AddSingleton(new McpEndpointState(enabled));
        services.AddHttpContextAccessor();

        services.AddMcpTool<ListCatalogsTool>();
        services.AddMcpTool<GetCatalogSchemaTool>();
        services.AddMcpTool<SearchCatalogTool>();
        services.AddMcpTool<GetCatalogItemTool>();

        if (!enabled)
        {
            return services;
        }

        services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = "svt",
                    Title = "СВТ — справочники (только чтение)",
                    Version = typeof(McpModule).Assembly.GetName().Version?.ToString() ?? "0.0.0"
                };
                options.ServerInstructions = Instructions;
            })
            .WithHttpTransport(options => options.Stateless = true);

        return services;
    }

    /// <summary>
    /// Инструмент модуля (операция, которая не является справочником). Платформа на модуль не ссылается:
    /// сервер собирает все зарегистрированные инструменты сам.
    /// </summary>
    public static IServiceCollection AddMcpTool<TTool>(this IServiceCollection services)
        where TTool : SvtMcpTool
    {
        services.AddSingleton<McpServerTool, TTool>();
        return services;
    }

    /// <summary>Маршрут <c>/api/mcp</c> — только при включённом флаге. При выключенном отвечает <c>ApiAccessMiddleware</c>: 404 <c>mcp_disabled</c>.</summary>
    public static IEndpointRouteBuilder MapSvtMcp(this IEndpointRouteBuilder endpoints)
    {
        if (endpoints.ServiceProvider.GetRequiredService<McpEndpointState>().Enabled)
        {
            endpoints.MapMcp(Path).ExcludeFromDescription();
        }

        return endpoints;
    }

    public static bool IsMcp(PathString path) => path.StartsWithSegments(Path, StringComparison.OrdinalIgnoreCase);
}
