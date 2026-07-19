using Serilog.Events;

namespace BlazorSvt.Platform.Infrastructure.Logging;

/// <summary>
/// Уровень для Serilog request logging: шумные статика/Blazor-пути → Verbose.
/// </summary>
public static class SerilogRequestLogLevel
{
    public static LogEventLevel GetLevel(HttpContext httpContext, double elapsedMs, Exception? ex)
    {
        if (ex is not null)
            return LogEventLevel.Error;

        var statusCode = httpContext.Response.StatusCode;
        if (statusCode >= 500)
            return LogEventLevel.Error;
        if (statusCode >= 400)
            return LogEventLevel.Warning;

        if (IsNoisyPath(httpContext.Request.Path))
            return LogEventLevel.Verbose;

        return LogEventLevel.Information;
    }

    private static bool IsNoisyPath(PathString path)
    {
        var value = path.Value;
        if (string.IsNullOrEmpty(value))
            return false;

        return value.StartsWith("/_content", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/_blazor", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
            || value.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase);
    }
}
