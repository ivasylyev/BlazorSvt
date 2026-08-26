using System.Diagnostics;
using BlazorSvt.Platform.Infrastructure;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Один фоновый воркер на все справочники. Периодически (каждые
/// <see cref="SnapshotSyncOptions.IntervalSeconds"/>) прогоняет инкрементальную
/// синхронизацию по каждому <see cref="ISnapshotSyncJob"/>, а один раз в сутки в
/// <see cref="SnapshotSyncOptions.ReconcileAtTime"/> — reconciliation.
///
/// Два выключателя: <see cref="SnapshotSyncOptions.Enabled"/> (деплой, до рестарта)
/// и <see cref="IV2SyncFeatureToggle"/> / <c>V2SyncEnabled</c> (оперативный, без рестарта).
/// Инкремент не запускается в blackout-окнах (<see cref="SnapshotSyncOptions.BlackoutIntervals"/>);
/// reconciliation в них выполняется — окна как раз задуманы под тяжёлые операции.
/// Все времена трактуются в поясе <see cref="SnapshotSyncOptions.TimeZone"/>.
/// Ошибка одного справочника не останавливает остальные и следующий цикл.
/// </summary>
public sealed class SnapshotSyncScheduler : BackgroundService
{
    private readonly List<ISnapshotSyncJob> jobs;
    private readonly SnapshotSyncExecutor executor;
    private readonly IV2SyncFeatureToggle featureToggle;
    private readonly SnapshotSyncOptions options;
    private readonly ILogger<SnapshotSyncScheduler> logger;

    private readonly TimeZoneInfo timeZone;
    private readonly TimeSpan reconcileAt;
    private readonly IReadOnlyDictionary<DayOfWeek, IReadOnlyList<(TimeSpan From, TimeSpan To)>> blackout;

    public SnapshotSyncScheduler(
        IEnumerable<ISnapshotSyncJob> jobs,
        SnapshotSyncExecutor executor,
        IV2SyncFeatureToggle featureToggle,
        IOptions<SnapshotSyncOptions> options,
        ILogger<SnapshotSyncScheduler> logger)
    {
        this.jobs = jobs.ToList();
        this.executor = executor;
        this.featureToggle = featureToggle;
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
            logger.LogInformation(
                "SnapshotSync выключен. Sync:Enabled=false (деплой-выключатель: воркер не запускается до смены конфига и рестарта). " +
                "V2SyncEnabled не опрашивается (оперативный kill switch из dbo.vw_FeatureToggle).");
            return;
        }

        if (jobs.Count == 0)
        {
            logger.LogInformation("SnapshotSync: нет зарегистрированных справочников.");
            return;
        }

        var v2SyncEnabled = await featureToggle.IsEnabledAsync(stoppingToken);
        logger.LogInformation(
            "SnapshotSync запущен: {Count} справочник(ов), интервал {Interval}с, reconcile в {ReconcileAt} ({Tz}). " +
            "Sync:Enabled=true (деплой-выключатель: false останавливает воркер до рестарта). " +
            "V2SyncEnabled={V2SyncEnabled} (оперативный kill switch из dbo.vw_FeatureToggle; смена без рестарта; нет строки/ошибка чтения = выкл).",
            jobs.Count, options.IntervalSeconds, options.ReconcileAtTime, timeZone.Id, v2SyncEnabled);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.IntervalSeconds));
        var skipTracker = new V2SyncSkipTracker(initialEnabled: v2SyncEnabled);

        // На старте не догоняем пропущенный reconcile: точка отсчёта = "сейчас".
        var previousLocal = NowLocal();

        do
        {
            var nowLocal = NowLocal();

            if (!await ShouldRunCycleAsync(skipTracker, stoppingToken))
            {
                previousLocal = nowLocal;
                continue;
            }

            await RunCycleAsync(previousLocal, nowLocal, stoppingToken);
            previousLocal = nowLocal;
        }
        while (await WaitForNextTickAsync(timer, stoppingToken));
    }

    private async Task<bool> ShouldRunCycleAsync(V2SyncSkipTracker skipTracker, CancellationToken stoppingToken)
    {
        var enabled = await featureToggle.IsEnabledAsync(stoppingToken);
        var decision = skipTracker.Observe(enabled);

        if (decision.LogTransition)
        {
            logger.LogWarning(
                decision.Enabled
                    ? "SnapshotSync: V2SyncEnabled включён — синхронизация возобновлена."
                    : "SnapshotSync: V2SyncEnabled выключен — синхронизация приостановлена.");
        }

        if (decision.LogHeartbeat)
        {
            logger.LogWarning(
                "SnapshotSync: V2SyncEnabled выключен, работа пропущена (heartbeat, пропущено тиков: {SkippedTicks}).",
                decision.SkippedTicks);
        }

        return decision.ShouldRun;
    }

    private async Task RunCycleAsync(DateTime previousLocal, DateTime nowLocal, CancellationToken stoppingToken)
    {
        var cycleId = Guid.NewGuid().ToString("N")[..8];
        var stopwatch = Stopwatch.StartNew();

        using (LogContext.PushProperty("ShortCorrelationId", cycleId))
        using (LogContext.PushProperty("SyncCycleId", cycleId))
        {
            var inBlackout = BlackoutScheduleBuilder.IsInBlackout(nowLocal, blackout);
            var shouldReconcile = CrossedReconcileBoundary(previousLocal, nowLocal, reconcileAt);

            logger.LogDebug(
                "SnapshotSync cycle {SyncCycleId} started (blackout={InBlackout}, reconcile={ShouldReconcile}).",
                cycleId, inBlackout, shouldReconcile);

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

            logger.LogDebug(
                "SnapshotSync cycle {SyncCycleId} finished in {ElapsedMs} ms.",
                cycleId, stopwatch.ElapsedMilliseconds);
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
