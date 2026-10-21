using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Api.OpenApi;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Api;

/// <summary>
/// HTTP API СВТ для внешних фронтендов (Атлас): <c>/api/v1</c>, OpenAPI, Swagger UI.
/// </summary>
public static class ApiModule
{
    public const string ApiPrefix = "/api";
    public const string V1Prefix = "/api/v1";
    public const string OpenApiDocumentName = "v1";
    public const string OpenApiRoute = "/api/openapi/{documentName}.json";
    public const string SwaggerUiRoutePrefix = "api/swagger";

    public static IServiceCollection AddSvtApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        services.ConfigureHttpJsonOptions(o => ApiJson.Configure(o.SerializerOptions));
        // Иначе вне Development битый JSON/параметр — голый 400 без тела и кода ошибки.
        services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);

        services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
        {
            if (IsApi(ctx.HttpContext.Request.Path))
            {
                ApiProblems.Enrich(ctx);
            }
        });

        services.AddResponseCompression(o =>
        {
            o.EnableForHttps = true;
            o.Providers.Add<BrotliCompressionProvider>();
            o.Providers.Add<GzipCompressionProvider>();
            o.MimeTypes = ["application/json", "application/problem+json"];
        });

        services.AddScoped<ICatalogFreshnessReader, CatalogFreshnessReader>();

        services.AddOpenApi(OpenApiDocumentName, SvtOpenApi.Configure);

        return services;
    }

    /// <summary>Middleware для <c>/api</c>: сжатие, доступ, Swagger UI. Ставить после аутентификации и request logging.</summary>
    public static IApplicationBuilder UseSvtApi(this IApplicationBuilder app)
    {
        var swaggerUiEnabled = app.ApplicationServices.GetRequiredService<IOptions<ApiOptions>>().Value.SwaggerUiEnabled;

        return app.UseWhen(
            context => IsApi(context.Request.Path),
            api =>
            {
                api.UseResponseCompression();
                api.UseMiddleware<ApiAccessMiddleware>();
                if (swaggerUiEnabled)
                {
                    api.UseSwaggerUI(o =>
                    {
                        o.RoutePrefix = SwaggerUiRoutePrefix;
                        o.DocumentTitle = "СВТ API";
                        o.SwaggerEndpoint($"../openapi/{OpenApiDocumentName}.json", "СВТ API v1");
                    });
                }
            });
    }

    /// <summary>Маршруты <c>/api/v1</c>: список справочников, вклады модулей, 404 по умолчанию, OpenAPI.</summary>
    public static IEndpointRouteBuilder MapSvtApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOpenApi(OpenApiRoute);

        var api = endpoints.MapGroup(V1Prefix);

        api.MapGet("/catalogs", (HttpContext context, IEnumerable<CatalogApiDescriptor> catalogs) =>
            {
                ApiCallLog.SetOperation(context, "catalogs");
                var items = catalogs
                    .OrderBy(c => c.Domain)
                    .ThenBy(c => c.Code, StringComparer.Ordinal)
                    .Select(c => new CatalogInfo(
                        c.Code,
                        CatalogTitles.GetRu(c.TitleResourceKey),
                        CatalogTitles.GetEn(c.TitleResourceKey),
                        ApiJson.EnumCode(c.Domain.ToString()),
                        Capabilities(c)))
                    .ToList();
                return ApiHttp.JsonWithETag(context, new CatalogListResponse(items));
            })
            .WithTags("catalogs")
            .WithName("listCatalogs")
            .WithSummary("Список справочников")
            .WithDescription("Справочники, доступные через API: код, заголовки RU/EN, домен, возможности. Поддерживает ETag.")
            .Produces<CatalogListResponse>()
            .Produces(StatusCodes.Status304NotModified);

        foreach (var contribution in endpoints.ServiceProvider.GetServices<IApiEndpointContribution>())
        {
            contribution.Map(api);
        }

        api.Map("/{**path}", (string? path) =>
            {
                var code = path?.StartsWith("catalogs/", StringComparison.OrdinalIgnoreCase) == true
                    ? ApiErrorCodes.CatalogNotFound
                    : ApiErrorCodes.NotFound;
                throw ApiException.NotFound(code);
            })
            .ExcludeFromDescription();

        return endpoints;
    }

    public static bool IsApi(PathString path) => path.StartsWithSegments(ApiPrefix, StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyList<string> Capabilities(CatalogApiDescriptor descriptor) =>
        descriptor.Period is null
            ? ["schema", "search", "item", "export"]
            : ["schema", "search", "item", "export", "asOf"];
}
