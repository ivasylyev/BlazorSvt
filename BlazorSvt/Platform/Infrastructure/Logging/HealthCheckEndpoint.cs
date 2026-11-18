using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.IIS;
using Serilog.Context;

namespace BlazorSvt.Platform.Infrastructure.Logging;

/// <summary>
/// Вызов /hc. ShortCircuit выполняет маршрут до журнала запросов, поэтому строка пишется здесь.
/// </summary>
public static class HealthCheckEndpoint
{
    public static async Task InvokeAsync(HttpContext context)
    {
        var started = Stopwatch.GetTimestamp();
        var account = await ReadAccountAsync(context);

        // Чтение схемы IIS не должно оставлять 401: ответ проверки — healthy.
        // Тело пишем сами: Results.Text добавляет свою строку Information.
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("healthy");
        }

        Write(context, Stopwatch.GetElapsedTime(started).TotalMilliseconds, account);
    }

    internal static void Write(HttpContext context, double elapsedMs, string account)
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(HealthCheckEndpoint));

        var statusCode = context.Response.StatusCode;
        var level = statusCode == StatusCodes.Status200OK ? LogLevel.Debug : LogLevel.Information;
        var correlationId = context.Request.Headers[MiddlewareDecorator.CorrelationHeaderName].FirstOrDefault();

        using (string.IsNullOrWhiteSpace(correlationId)
                   ? null
                   : LogContext.PushProperty("ShortCorrelationId", correlationId))
        {
            logger.Log(
                level,
                "{Method:l} {Path:l} {StatusCode} {ElapsedMs} ms {ClientAddress:l} {UserAgent:l} {Account:l}",
                context.Request.Method,
                (context.Request.PathBase + context.Request.Path).Value ?? "",
                statusCode,
                (long)elapsedMs,
                context.Connection.RemoteIpAddress?.ToString() ?? "",
                context.Request.Headers.UserAgent.ToString(),
                account);
        }
    }

    private static async Task<string> ReadAccountAsync(HttpContext context)
    {
        try
        {
            var schemes = context.RequestServices.GetService<IAuthenticationSchemeProvider>();
            if (schemes is null)
                return "";

            if (await schemes.GetSchemeAsync(IISServerDefaults.AuthenticationScheme) is null)
                return "";

            var result = await context.AuthenticateAsync(IISServerDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return "";

            return result.Principal.Identity?.Name ?? "";
        }
        catch (Exception)
        {
            return "";
        }
    }
}
