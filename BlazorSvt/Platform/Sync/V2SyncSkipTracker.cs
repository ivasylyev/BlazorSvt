namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Состояние оперативного kill switch между тиками планировщика:
/// переходы on↔off и heartbeat каждого 10-го пропущенного тика.
/// </summary>
internal sealed class V2SyncSkipTracker
{
    internal const int HeartbeatEveryTicks = 10;

    private bool? lastEnabled;
    private int skippedTicks;

    public V2SyncSkipTracker(bool? initialEnabled = null)
    {
        lastEnabled = initialEnabled;
    }

    public readonly record struct Decision(
        bool ShouldRun,
        bool LogTransition,
        bool LogHeartbeat,
        bool Enabled,
        int SkippedTicks);

    /// <summary>Учитывает текущее значение тогла и возвращает, что делать/логировать.</summary>
    public Decision Observe(bool enabled)
    {
        var logTransition = lastEnabled.HasValue && lastEnabled.Value != enabled;
        lastEnabled = enabled;

        if (enabled)
        {
            skippedTicks = 0;
            return new Decision(
                ShouldRun: true,
                LogTransition: logTransition,
                LogHeartbeat: false,
                Enabled: true,
                SkippedTicks: 0);
        }

        skippedTicks++;
        return new Decision(
            ShouldRun: false,
            LogTransition: logTransition,
            LogHeartbeat: skippedTicks % HeartbeatEveryTicks == 0,
            Enabled: false,
            SkippedTicks: skippedTicks);
    }
}
