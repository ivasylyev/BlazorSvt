using BlazorSvt.Modules.LocationsNodes.Detail;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.LocationsNodes.Sync;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.LocationsNodes;

public static class LocationsNodesModule
{
    public static IServiceCollection AddLocationsNodesModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<LocationsNodesDto>, LocationsNodesGridSettingsService>();
        services.AddScoped<IDetailSettingsService<LocationsNodesDetailDto>, LocationsNodesDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, LocationsNodesSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "locationsnodes",
            "HeaderMenu.LocationsNodes",
            VisibleToEditor: true));

        return services;
    }
}
