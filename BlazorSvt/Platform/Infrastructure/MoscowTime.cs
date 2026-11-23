namespace BlazorSvt.Platform.Infrastructure;

public static class MoscowTime
{
    public const string TimeZoneId = "Russian Standard Time";

    public static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public static DateTime FromUtc(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    public static DateTime ToUtc(DateTime moscow) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(moscow, DateTimeKind.Unspecified), Zone);
}
