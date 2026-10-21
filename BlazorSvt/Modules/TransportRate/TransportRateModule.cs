using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Modules.TransportRate.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.TransportRate;

public static class TransportRateModule
{
    public static IServiceCollection AddTransportRateModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportRateDto>, TransportRateGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportRateDetailDto>, TransportRateDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, TransportRateSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "transportrate",
            "HeaderMenu.TransportRate",
            VisibleToEditor: true));
        services.AddCatalogApi<TransportRateDto, TransportRateDetailDto>(
            "transport-rates",
            CatalogDomain.Rates,
            "HeaderMenu.TransportRate",
            new CatalogPeriod(nameof(TransportRateDto.StartDate), nameof(TransportRateDto.EndDate)));

        return services;
    }
}

