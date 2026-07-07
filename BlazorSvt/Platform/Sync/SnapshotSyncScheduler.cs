using BlazorSvt.Platform.Infrastructure;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Один фоновый воркер на все справочники. Периодически (каждые
/// <see cref="SnapshotSyncOptions.IntervalSeconds"/>) прогоняет инкрементальную
/// синхронизацию по каждому <see cref="ISnapshotSyncJob"/>, а один раз в сутки в
/// <see cref="SnapshotSyncOptions.ReconcileAtTime"/> — reconciliation.
///
/// Инкремент не запускается в blackout-окнах (<see cref="SnapshotSyncOptions.BlackoutIntervals"/>);
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
        reconcileAt = BlackoutScheduleBuilder.ParseTime(this.options.ReconcileAtTime)
                      ?? throw new InvalidOperationException(
                          $"Sync:ReconcileAtTime '{this.options.ReconcileAtTime}' не является временем в формате HH:mm.");
        blackout = BlackoutScheduleBuilder.Build(this.options.BlackoutIntervals, logger);
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
        var inBlackout = BlackoutScheduleBuilder.IsInBlackout(nowLocal, blackout);
        var shouldReconcile = CrossedReconcileBoundary(previousLocal, nowLocal, reconcileAt);

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
    /// True, если между предыдущим и текущим тиком пересекли ежедневную отметку reconcile.
    /// Без catch-up: если приложение было выключено во время отметки, пересечение
    /// не наблюдается — reconcile ждёт следующих суток.
    /// </summary>
    internal static bool CrossedReconcileBoundary(
        DateTime previousLocal,
        DateTime nowLocal,
        TimeSpan reconcileAt)
    {
        var scheduled = nowLocal.Date + reconcileAt;
        return previousLocal < scheduled && scheduled <= nowLocal;
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
}
