using BlazorSvt.Modules.RateType.Detail;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.RateType;

public static class RateTypeModule
{
    public static IServiceCollection AddRateTypeModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RateTypeDto>, RateTypeGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RateTypeDetailDto>, RateTypeDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "ratetype",
            "HeaderMenu.RateType",
            VisibleToEditor: true));

        return services;
    }
}
