using BlazorSvt.Modules.HomeRatePivot.Data;

namespace BlazorSvt.Modules.HomeRatePivot;

public static class HomeRatePivotModule
{
    public static IServiceCollection AddHomeRatePivotModule(this IServiceCollection services)
    {
        services.AddScoped<IHomeRatePivotService, HomeRatePivotService>();
        return services;
    }
}
