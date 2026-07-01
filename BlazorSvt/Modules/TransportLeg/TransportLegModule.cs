using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Platform.Grid.Services;

namespace BlazorSvt.Modules.TransportLeg;

public static class TransportLegModule
{
    public static IServiceCollection AddTransportLegModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportLegDto>, TransportLegGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportLegDetailDto>, TransportLegDetailSettingsService>();

        return services;
    }
}
