namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Сборка расписания blackout из <see cref="BlackoutInterval"/> и проверка попадания во время.
/// </summary>
public static class BlackoutScheduleBuilder
{
    public static IReadOnlyDictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>> Build(
        IEnumerable<BlackoutInterval>? intervals,
        ILogger? logger = null)
    {
        var result = new Dictionary<DayOfWeek, List<(TimeSpan From, TimeSpan To)>>();

        if (intervals is null)
        {
            return new Dictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>>();
        }

        foreach (var interval in intervals)
        {
            if (interval.DaysOfWeek is null || interval.DaysOfWeek.Count == 0)
            {
                throw new InvalidOperationException(
                    "Sync:BlackoutIntervals: DaysOfWeek не может быть пустым.");
            }

            var from = ParseTime(interval.StartTime);
            var to = ParseTime(interval.EndTime);

            if (from is null || to is null || from >= to)
            {
                logger?.LogWarning(
                    "SnapshotSync: некорректное blackout-окно '{Start}'-'{End}' пропущено.",
                    interval.StartTime,
                    interval.EndTime);
                continue;
            }

            foreach (var dayName in interval.DaysOfWeek)
            {
                var day = ParseDayOfWeek(dayName);

                if (!result.TryGetValue(day, out var windows))
                {
                    windows = new List<(TimeSpan From, TimeSpan To)>();
                    result[day] = windows;
                }

                windows.Add((from.Value, to.Value));
            }
        }

        return result.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<(TimeSpan From, TimeSpan To)>)kvp.Value);
    }

    public static bool IsInBlackout(
        DateTime localTime,
        IReadOnlyDictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>> blackout)
    {
        if (!blackout.TryGetValue(localTime.DayOfWeek, out var windows))
        {
            return false;
        }

        var time = localTime.TimeOfDay;
        foreach (var (from, to) in windows)
        {
            if (time >= from && time < to)
            {
                return true;
            }
        }

        return false;
    }

    public static TimeSpan? ParseTime(string value) =>
        TimeSpan.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;

    private static DayOfWeek ParseDayOfWeek(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                "Sync:BlackoutIntervals: имя дня недели не может быть пустым.");
        }

        if (Enum.TryParse<DayOfWeek>(value, ignoreCase: true, out var day))
        {
            return day;
        }

        throw new InvalidOperationException(
            $"Sync:BlackoutIntervals: неизвестный день недели '{value}'.");
    }
}
