namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Настройки фоновой синхронизации legacy -> v2 snapshot (секция "Sync" в appsettings).
/// </summary>
public class SnapshotSyncOptions
{
    /// <summary>Включена ли фоновая синхронизация. По умолчанию выключена.</summary>
    public bool Enabled { get; set; }

    /// <summary>Интервал между циклами инкрементальной синхронизации.
    /// Держим заметно ниже SLA (5 мин, p99), чтобы иметь запас на длинные циклы.</summary>
    public int IntervalSeconds { get; set; } = 90;

    /// <summary>Время суток (HH:mm) ежедневного запуска reconciliation.
    /// Тяжёлая сверка идёт в тихое окно; допустимо existence фантомов до суток.</summary>
    public string ReconcileAtTime { get; set; } = "02:00";

    /// <summary>Часовой пояс для <see cref="ReconcileAtTime"/> и окон
    /// <see cref="Blackout"/> (Windows TZ id, напр. "Russian Standard Time").
    /// При неизвестном id используется локальный пояс сервера.</summary>
    public string TimeZone { get; set; } = "Russian Standard Time";

    /// <summary>Таймаут SQL-команд синхронизации (пачки до десятков тыс. строк).</summary>
    public int CommandTimeoutSeconds { get; set; } = 300;

    /// <summary>Окна, когда инкрементальная синхронизация не запускается
    /// (reconciliation в них выполняется — окна задуманы под тяжёлые операции).</summary>
    public BlackoutSchedule Blackout { get; set; } = new();
}

/// <summary>
/// Расписание blackout-окон по дням недели. Семантика — override: если для дня
/// задан свой список окон, он используется вместо <see cref="Default"/>; иначе
/// применяется <see cref="Default"/>.
/// </summary>
public class BlackoutSchedule
{
    public List<BlackoutWindow> Default { get; set; } = new();
    public List<BlackoutWindow>? Monday { get; set; }
    public List<BlackoutWindow>? Tuesday { get; set; }
    public List<BlackoutWindow>? Wednesday { get; set; }
    public List<BlackoutWindow>? Thursday { get; set; }
    public List<BlackoutWindow>? Friday { get; set; }
    public List<BlackoutWindow>? Saturday { get; set; }
    public List<BlackoutWindow>? Sunday { get; set; }

    /// <summary>Список окон для указанного дня (override дня либо Default).</summary>
    public IReadOnlyList<BlackoutWindow> ForDay(DayOfWeek day) =>
        (day switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => null
        }) ?? Default;
}

/// <summary>Одно blackout-окно внутри суток (границы в формате HH:mm, [From; To)).</summary>
public class BlackoutWindow
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}
