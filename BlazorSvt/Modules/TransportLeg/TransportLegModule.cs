using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportLeg.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.TransportLeg;

public static class TransportLegModule
{
    public static IServiceCollection AddTransportLegModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportLegDto>, TransportLegGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportLegDetailDto>, TransportLegDetailSettingsService>();
        services.AddScoped<IDetailChildSection<TransportLegDetailDto>, LegRateSection>();
        services.AddScoped<IDetailChildSection<TransportLegDetailDto>, LegAverageRateLevel3Section>();
        services.AddScoped<IDetailChildSection<TransportLegDetailDto>, LeadtimeSection>();

        services.AddSingleton<ISnapshotSyncJob, TransportLegSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "transportleg",
            "HeaderMenu.TransportLeg",
            VisibleToEditor: true));
        services.AddCatalogApi<TransportLegDto, TransportLegDetailDto>(
            "transport-legs",
            CatalogDomain.Routes,
            "HeaderMenu.TransportLeg");

        return services;
    }
}
