using BlazorSvt.Modules.Rates.Detail;
using BlazorSvt.Modules.Rates.List;
using BlazorSvt.Platform.Grid.Services;

namespace BlazorSvt.Modules.Rates;

public static class RatesModule
{
    public static IServiceCollection AddRatesModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RateDto>, RatesGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RateDetailDto>, RatesDetailSettingsService>();

        return services;
    }
}
