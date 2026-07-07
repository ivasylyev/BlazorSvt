using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Modules.TransportRate.Sync;
using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportRate;

public static class TransportRateModule
{
    public static IServiceCollection AddTransportRateModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportRateDto>, TransportRateGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportRateDetailDto>, TransportRateDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, TransportRateSyncJob>();

        return services;
    }
}

