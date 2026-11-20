using BlazorSvt.Modules.TransportRoute.Detail;
using BlazorSvt.Modules.TransportRoute.List;
using BlazorSvt.Modules.TransportRoute.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.TransportRoute;

public static class TransportRouteModule
{
    public static IServiceCollection AddTransportRouteModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportRouteDto>, TransportRouteGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportRouteDetailDto>, TransportRouteDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, TransportRouteSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "transportroute",
            "HeaderMenu.TransportRoute",
            VisibleToEditor: true));
        services.AddCatalogApi<TransportRouteDto, TransportRouteDetailDto>(
            "transport-routes",
            CatalogDomain.Routes,
            "HeaderMenu.TransportRoute");

        return services;
    }
}
