using BlazorSvt.Platform.Infrastructure;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Один фоновый воркер на все справочники. Периодически (каждые
/// <see cref="SnapshotSyncOptions.IntervalSeconds"/>) прогоняет инкрементальную
/// синхронизацию по каждому <see cref="ISnapshotSyncJob"/>, а один раз в сутки в
/// <see cref="SnapshotSyncOptions.ReconcileAtTime"/> — reconciliation.
///
/// Инкремент не запускается в blackout-окнах (<see cref="SnapshotSyncOptions.Blackout"/>);
/// reconciliation в них выполняется — окна как раз задуманы под тяжёлые операции.
/// Все времена трактуются в поясе <see cref="SnapshotSyncOptions.TimeZone"/>.
/// Ошибка одного справочника не останавливает остальные и следующий цикл.
/// </summary>
public sealed class SnapshotSyncScheduler : BackgroundService
{
    private readonly List<ISnapshotSyncJob> jobs;
    private readonly SnapshotSyncExecutor executor;
    private readonly SnapshotSyncOptions options;
    private readonly ILogger<SnapshotSyncScheduler> logger;

    private readonly TimeZoneInfo timeZone;
    private readonly TimeSpan reconcileAt;
    private readonly IReadOnlyDictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>> blackout;

    public SnapshotSyncScheduler(
        IEnumerable<ISnapshotSyncJob> jobs,
        SnapshotSyncExecutor executor,
        IOptions<SnapshotSyncOptions> options,
        ILogger<SnapshotSyncScheduler> logger)
    {
        this.jobs = jobs.ToList();
        this.executor = executor;
        this.options = options.Value;
        this.logger = logger;

        timeZone = ResolveTimeZone(this.options.TimeZone, logger);
        reconcileAt = ParseTime(this.options.ReconcileAtTime)
                      ?? throw new InvalidOperationException(
                          $"Sync:ReconcileAtTime '{this.options.ReconcileAtTime}' не является временем в формате HH:mm.");
        blackout = BuildBlackout(this.options.Blackout, logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled)
        {
            logger.LogInformation("SnapshotSync выключен (Sync:Enabled=false).");
            return;
        }

        if (jobs.Count == 0)
        {
            logger.LogInformation("SnapshotSync: нет зарегистрированных справочников.");
            return;
        }

        logger.LogInformation(
            "SnapshotSync запущен: {Count} справочник(ов), интервал {Interval}с, reconcile в {ReconcileAt} ({Tz}).",
            jobs.Count, options.IntervalSeconds, options.ReconcileAtTime, timeZone.Id);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.IntervalSeconds));

        // На старте не догоняем пропущенный reconcile: точка отсчёта = "сейчас".
        var previousLocal = NowLocal();

        do
        {
            var nowLocal = NowLocal();
            await RunCycleAsync(previousLocal, nowLocal, stoppingToken);
            previousLocal = nowLocal;
        }
        while (await WaitForNextTickAsync(timer, stoppingToken));
    }

    private async Task RunCycleAsync(DateTime previousLocal, DateTime nowLocal, CancellationToken stoppingToken)
    {
        var inBlackout = IsInBlackout(nowLocal);
        var shouldReconcile = CrossedReconcileBoundary(previousLocal, nowLocal);

        if (inBlackout)
        {
            logger.LogDebug("SnapshotSync: инкремент пропущен (blackout-окно, {Now:HH:mm}).", nowLocal);
        }

        foreach (var job in jobs)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            if (!inBlackout)
            {
                try
                {
                    await executor.RunIncrementalAsync(job, stoppingToken);
                }
                catch (Exception ex) when (!OperationCancellation.IsCancellation(ex, stoppingToken))
                {
                    logger.LogError(ex, "SnapshotSync: ошибка инкремента справочника {Entity}.", job.Entity);
                }
            }

            if (!shouldReconcile)
            {
                continue;
            }

            try
            {
                await executor.RunReconcileAsync(job, stoppingToken);
            }
            catch (Exception ex) when (!OperationCancellation.IsCancellation(ex, stoppingToken))
            {
                logger.LogError(ex, "SnapshotSync: ошибка reconciliation справочника {Entity}.", job.Entity);
            }
        }
    }

    /// <summary>
    /// True, если между предыдущим и текущим тиком пересекли ежедневную отметку
    /// reconcile. Без catch-up: если приложение было выключено во время отметки,
    /// пересечение не наблюдается — reconcile ждёт следующих суток.
    /// </summary>
    private bool CrossedReconcileBoundary(DateTime previousLocal, DateTime nowLocal)
    {
        var scheduled = nowLocal.Date + reconcileAt;
        return previousLocal < scheduled && scheduled <= nowLocal;
    }

    private bool IsInBlackout(DateTime nowLocal)
    {
        if (!blackout.TryGetValue(nowLocal.DayOfWeek, out var windows))
        {
            return false;
        }

        var time = nowLocal.TimeOfDay;
        foreach (var (from, to) in windows)
        {
            if (time >= from && time < to)
            {
                return true;
            }
        }

        return false;
    }

    private DateTime NowLocal() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

    private static async Task<bool> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken stoppingToken)
    {
        try
        {
            return await timer.WaitForNextTickAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return TimeZoneInfo.Local;
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            logger.LogWarning(
                "SnapshotSync: часовой пояс '{Tz}' не найден, используется локальный ({Local}).",
                timeZoneId, TimeZoneInfo.Local.Id);
            return TimeZoneInfo.Local;
        }
    }

    private static IReadOnlyDictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>> BuildBlackout(
        BlackoutSchedule schedule, ILogger logger)
    {
        var result = new Dictionary<DayOfWeek, IReadOnlyList<(TimeSpan, TimeSpan)>>();

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            var windows = new List<(TimeSpan, TimeSpan)>();

            foreach (var window in schedule.ForDay(day))
            {
                var from = ParseTime(window.From);
                var to = ParseTime(window.To);

                if (from is null || to is null || from >= to)
                {
                    logger.LogWarning(
                        "SnapshotSync: некорректное blackout-окно '{From}'-'{To}' ({Day}) пропущено.",
                        window.From, window.To, day);
                    continue;
                }

                windows.Add((from.Value, to.Value));
            }

            if (windows.Count > 0)
            {
                result[day] = windows;
            }
        }

        return result;
    }

    private static TimeSpan? ParseTime(string value) =>
        TimeSpan.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
}
