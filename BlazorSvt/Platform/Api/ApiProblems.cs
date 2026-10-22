using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.Api;

/// <summary>
/// ProblemDetails для API: стабильный <c>code</c>, <c>traceId</c>, локализованный <c>title</c>
/// (<c>Api.Error.{code}</c>, язык — по <c>Accept-Language</c>).
/// </summary>
public static class ApiProblems
{
    public const string CodeKey = "code";
    public const string TraceIdKey = "traceId";
    public const string ErrorsKey = "errors";

    public static ProblemDetails Create(
        HttpContext context,
        int statusCode,
        string code,
        IReadOnlyDictionary<string, string[]>? errors = null,
        params object[] messageArgs)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = Localize(context, code, messageArgs),
            Instance = context.Request.Path
        };

        problem.Extensions[CodeKey] = code;
        problem.Extensions[TraceIdKey] = TraceId(context);
        if (errors is { Count: > 0 })
        {
            problem.Extensions[ErrorsKey] = errors;
        }

        return problem;
    }

    public static Task WriteAsync(
        HttpContext context,
        int statusCode,
        string code,
        IReadOnlyDictionary<string, string[]>? errors = null,
        params object[] messageArgs)
    {
        var problem = Create(context, statusCode, code, errors, messageArgs);
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(
            problem,
            ApiJson.Options(context),
            contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }

    public static Task WriteAsync(HttpContext context, ApiException exception) =>
        WriteAsync(context, exception.StatusCode, exception.Code, exception.Errors, exception.MessageArgs);

    /// <summary>Дополняет ProblemDetails, созданные фреймворком (ошибки биндинга тела и т.п.).</summary>
    public static void Enrich(ProblemDetailsContext context)
    {
        var extensions = context.ProblemDetails.Extensions;
        if (!extensions.ContainsKey(CodeKey))
        {
            var code = context.ProblemDetails.Status switch
            {
                StatusCodes.Status400BadRequest => ApiErrorCodes.InvalidRequest,
                StatusCodes.Status404NotFound => ApiErrorCodes.NotFound,
                StatusCodes.Status503ServiceUnavailable => ApiErrorCodes.ServiceUnavailable,
                _ => ApiErrorCodes.InternalError
            };
            extensions[CodeKey] = code;
            context.ProblemDetails.Title = Localize(context.HttpContext, code, []);
        }

        extensions[TraceIdKey] = TraceId(context.HttpContext);
    }

    private static string TraceId(HttpContext context) =>
        Activity.Current?.Id ?? context.TraceIdentifier;

    private static string Localize(HttpContext context, string code, object[] messageArgs)
    {
        var localizer = context.RequestServices.GetService<IStringLocalizer<PlatformResources>>();
        if (localizer is null)
        {
            return code;
        }

        var text = localizer[$"Api.Error.{code}", messageArgs];
        return text.ResourceNotFound ? code : text.Value;
    }
}
