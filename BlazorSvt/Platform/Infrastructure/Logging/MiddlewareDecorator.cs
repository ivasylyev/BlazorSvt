namespace BlazorSvt.Platform.Infrastructure.Logging;

public static class MiddlewareDecorator
{
    public static async Task MiddlewareShortCorrelationId(HttpContext context, Func<Task> next)
    {
        const string headerName = "X-Correlation-ID";

        var shortCorrelationId = context.Request.Headers[headerName].FirstOrDefault() ?? Guid.NewGuid().ToString("N").Substring(0, 8);

        context.Response.Headers[headerName] = shortCorrelationId;

        using (Serilog.Context.LogContext.PushProperty("ShortCorrelationId", shortCorrelationId))
        {
            await next();
        }
    }
}