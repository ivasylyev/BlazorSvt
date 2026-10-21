using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace BlazorSvt.Platform.Api.OpenApi;

/// <summary>Документ OpenAPI: только <c>/api/v1</c>, общие заголовки, описание контракта.</summary>
public static class SvtOpenApi
{
    public static void Configure(OpenApiOptions options, IConfiguration configuration)
    {
        options.ShouldInclude = description =>
            description.RelativePath?.StartsWith("api/v1/", StringComparison.OrdinalIgnoreCase) == true;

        var serverUrl = NormalizePathBase(configuration["PathBase"]);

        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "СВТ API",
                Version = "v1",
                Description =
                    "Read-only API СВТ для внешних фронтендов (Атлас). " +
                    "Вызывающий — доверенная служебная учётка (Windows Authentication); " +
                    "права — по пользователю из заголовка X-On-Behalf-Of. " +
                    "JSON camelCase; даты yyyy-MM-dd; дата-время ISO 8601 UTC (Z); enum — строковые коды; null не опускается. " +
                    "Ошибки — ProblemDetails с полями code и traceId. Заголовок Sunset — плановая дата вывода версии."
            };
            // Без PathBase Swagger UI бьёт в https://host/api/... вместо https://host/v2/api/...
            document.Servers.Clear();
            if (serverUrl is not null)
            {
                document.Servers.Add(new OpenApiServer { Url = serverUrl });
            }

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, _, _) =>
        {
            operation.Parameters ??= [];
            operation.Parameters.Add(Header(
                ApiAccessMiddleware.OnBehalfOfHeader,
                required: true,
                "Пользователь, от имени которого выполняется запрос: DOMAIN\\user. Права определяются его ролями в СВТ."));
            operation.Parameters.Add(Header(
                "X-Correlation-ID",
                required: false,
                "Идентификатор корреляции; возвращается в ответе. Если не передан — генерируется."));
            operation.Parameters.Add(Header(
                "Accept-Language",
                required: false,
                "ru-RU (по умолчанию) или en-US: язык заголовков полей и текстов ошибок. Данные RU/EN возвращаются как есть.",
                example: "ru-RU"));
            return Task.CompletedTask;
        });
    }

    /// <summary>Относительный server URL для Swagger UI; <c>null</c> — корень хоста (локальный PathBase <c>/</c>).</summary>
    private static string? NormalizePathBase(string? pathBase)
    {
        if (string.IsNullOrWhiteSpace(pathBase) || pathBase == "/")
        {
            return null;
        }

        return pathBase.TrimEnd('/');
    }

    private static OpenApiParameter Header(string name, bool required, string description, string? example = null) =>
        new()
        {
            Name = name,
            In = ParameterLocation.Header,
            Required = required,
            Description = description,
            Schema = new OpenApiSchema { Type = "string" },
            Example = example is null ? null : new OpenApiString(example)
        };
}
