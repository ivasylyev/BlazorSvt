namespace BlazorSvt.Platform.Infrastructure.Logging;

public static class MiddlewareDecorator
{
    public const string CorrelationHeaderName = "X-Correlation-ID";

    public static async Task MiddlewareShortCorrelationId(HttpContext context, Func<Task> next)
    {
        var shortCorrelationId = context.Request.Headers[CorrelationHeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString("N").Substring(0, 8);

        context.Response.Headers[CorrelationHeaderName] = shortCorrelationId;

        var userLogin = context.User.Identity?.Name;
        using (Serilog.Context.LogContext.PushProperty("ShortCorrelationId", shortCorrelationId))
        using (Serilog.Context.LogContext.PushProperty("UserLogin", userLogin))
        using (Serilog.Context.LogContext.PushProperty("Channel", LogChannels.For(context.Request.Path)))
        {
            await next();
        }
    }
}
