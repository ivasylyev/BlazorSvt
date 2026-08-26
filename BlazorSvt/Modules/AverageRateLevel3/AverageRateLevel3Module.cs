using BlazorSvt.Modules.AverageRateLevel3.Detail;
using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.AverageRateLevel3.Sync;
using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.AverageRateLevel3;

public static class AverageRateLevel3Module
{
    public static IServiceCollection AddAverageRateLevel3Module(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<AverageRateLevel3Dto>, AverageRateLevel3GridSettingsService>();
        services.AddScoped<IDetailSettingsService<AverageRateLevel3DetailDto>, AverageRateLevel3DetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, AverageRateLevel3SyncJob>();

        return services;
    }
}
