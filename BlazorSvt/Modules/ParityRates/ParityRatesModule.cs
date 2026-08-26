using BlazorSvt.Modules.ParityRates.Detail;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.ParityRates.Sync;
using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.ParityRates;

public static class ParityRatesModule
{
    public static IServiceCollection AddParityRatesModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<ParityRatesDto>, ParityRatesGridSettingsService>();
        services.AddScoped<IDetailSettingsService<ParityRatesDetailDto>, ParityRatesDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, ParityRatesSyncJob>();

        return services;
    }
}
