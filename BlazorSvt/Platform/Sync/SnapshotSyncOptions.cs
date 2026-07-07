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
    /// <see cref="BlackoutIntervals"/> (Windows TZ id, напр. "Russian Standard Time").
    /// При неизвестном id используется локальный пояс сервера.</summary>
    public string TimeZone { get; set; } = "Russian Standard Time";

    /// <summary>Таймаут SQL-команд синхронизации (пачки до десятков тыс. строк).</summary>
    public int CommandTimeoutSeconds { get; set; } = 300;

    /// <summary>Окна, когда инкрементальная синхронизация не запускается
    /// (reconciliation в них выполняется — окна задуманы под тяжёлые операции).</summary>
    public List<BlackoutInterval> BlackoutIntervals { get; set; } = new();
}

/// <summary>
/// Одно blackout-окно: время суток и явный список дней недели, к которым оно применяется.
/// Границы — полуоткрытый интервал [StartTime; EndTime).
/// </summary>
public class BlackoutInterval
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public List<string> DaysOfWeek { get; set; } = new();
}
