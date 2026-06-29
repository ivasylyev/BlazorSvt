using BlazorSvt.Modules.Legs.Detail;
using BlazorSvt.Modules.Legs.List;
using BlazorSvt.Platform.Grid.Services;

namespace BlazorSvt.Modules.Legs;

public static class LegsModule
{
    public static IServiceCollection AddLegsModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<LegDto>, LegsGridSettingsService>();
        services.AddScoped<IDetailSettingsService<LegDetailDto>, LegsDetailSettingsService>();

        return services;
    }
}
