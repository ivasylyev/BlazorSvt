using BlazorSvt.Modules.ParityRates.Detail;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Modules.ParityRates.Sync;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.ParityRates;

public static class ParityRatesModule
{
    public static IServiceCollection AddParityRatesModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<ParityRatesDto>, ParityRatesGridSettingsService>();
        services.AddScoped<IDetailSettingsService<ParityRatesDetailDto>, ParityRatesDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, ParityRatesSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "parityrates",
            "HeaderMenu.ParityRates",
            VisibleToEditor: true));

        return services;
    }
}
