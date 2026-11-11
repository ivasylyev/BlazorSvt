namespace BlazorSvt.Platform.Infrastructure.Logging;

/// <summary>Значения свойства лога <c>Channel</c>: откуда пришёл запрос.</summary>
public static class LogChannels
{
    public const string Blazor = "Blazor";
    public const string Api = "Api";
    public const string Mcp = "Mcp";

    public static string For(PathString path) =>
        path.StartsWithSegments("/api/mcp", StringComparison.OrdinalIgnoreCase) ? Mcp
        : path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) ? Api
        : Blazor;
}
