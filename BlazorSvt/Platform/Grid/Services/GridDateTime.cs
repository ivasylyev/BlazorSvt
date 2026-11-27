using System.Globalization;
using BlazorSvt.Platform.Infrastructure;

namespace BlazorSvt.Platform.Grid.Services;

internal static class GridDateTime
{
    private static readonly (string Parse, string Emit)[] TimeFormats =
    [
        ("yyyy-MM-dd'T'HH:mm:ss.FFFFFFF", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd'T'HH:mm", "yyyy-MM-dd'T'HH:mm"),
        ("yyyy-MM-dd'T'HH", "yyyy-MM-dd'T'HH"),
        ("yyyy-MM-dd HH:mm:ss.FFFFFFF", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd HH:mm", "yyyy-MM-dd'T'HH:mm"),
        ("yyyy-MM-dd HH", "yyyy-MM-dd'T'HH"),
        ("yyyy-MM-dd", "yyyy-MM-dd'T'HH:mm")
    ];

    public static DateTime ToDisplay(DateTime utc) => MoscowTime.FromUtc(utc);

    public static string NormalizeFilterValue(string value)
    {
        foreach (var (parse, emit) in TimeFormats)
        {
            if (!DateTime.TryParseExact(
                    value,
                    parse,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var moscow))
            {
                continue;
            }

            var utc = MoscowTime.ToUtc(moscow);
            // Blazor всегда дописывает ':00' к datetime-local (blazor.server.js getInputValue):
            // ровная минута — выбор пользователя, секундную точность оставляем только при ненулевых секундах
            var format = emit.EndsWith(":ss") && utc.Ticks % TimeSpan.TicksPerMinute == 0
                ? "yyyy-MM-dd'T'HH:mm"
                : emit;
            return utc.ToString(format, CultureInfo.InvariantCulture);
        }

        return value;
    }

    /// <summary>
    /// Точность фильтра по формату входа — шаг диапазона в SP:
    /// <c>yyyy-MM-dd</c> → day, <c>…THH</c> → hour, <c>…THH:mm[:00]</c> → minute, с ненулевыми секундами → second.
    /// </summary>
    public static string GetFilterPrecision(string value)
    {
        var v = value.Trim();
        var timeSep = v.IndexOf('T');
        if (timeSep < 0) timeSep = v.IndexOf(' ');
        if (timeSep < 0) return "day";
        return v[(timeSep + 1)..].Count(c => c == ':') switch
        {
            0 => "hour",
            1 => "minute",
            // ':00' от Blazor и ':00.000' — минутная точность, секундная — только при ненулевых секундах
            _ => HasNonZeroSeconds(v) ? "second" : "minute"
        };
    }

    private static bool HasNonZeroSeconds(string value) =>
        value[(value.LastIndexOf(':') + 1)..].Trim('0', '.').Length > 0;
}
