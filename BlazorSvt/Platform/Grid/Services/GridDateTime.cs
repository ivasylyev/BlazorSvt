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
        ("yyyy-MM-dd HH:mm:ss.FFFFFFF", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss"),
        ("yyyy-MM-dd HH:mm", "yyyy-MM-dd'T'HH:mm")
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

            return MoscowTime.ToUtc(moscow).ToString(emit, CultureInfo.InvariantCulture);
        }

        return value;
    }
}
