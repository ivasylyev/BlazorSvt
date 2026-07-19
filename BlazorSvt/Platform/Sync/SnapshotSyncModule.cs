namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Регистрация каркаса фоновой синхронизации legacy -> v2 snapshot.
/// Конкретные справочники добавляются модулями через
/// <c>services.AddSingleton&lt;ISnapshotSyncJob, {Entity}SyncJob&gt;()</c>.
/// </summary>
public static class SnapshotSyncModule
{
    public static IServiceCollection AddSnapshotSync(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SnapshotSyncOptions>(configuration.GetSection("Sync"));

        services.AddSingleton<IV2SyncFeatureToggle, V2SyncFeatureToggle>();
        services.AddSingleton<SyncStateStore>();
        services.AddSingleton<SnapshotSyncExecutor>();
        services.AddHostedService<SnapshotSyncScheduler>();

        return services;
    }
}
