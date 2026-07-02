using BlazorSvt.Modules.LocationsNodes.Detail;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Platform.Grid.Services;

namespace BlazorSvt.Modules.LocationsNodes;

public static class LocationsNodesModule
{
    public static IServiceCollection AddLocationsNodesModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<LocationsNodesDto>, LocationsNodesGridSettingsService>();
        services.AddScoped<IDetailSettingsService<LocationsNodesDetailDto>, LocationsNodesDetailSettingsService>();

        return services;
    }
}
