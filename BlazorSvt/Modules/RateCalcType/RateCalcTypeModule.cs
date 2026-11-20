using BlazorSvt.Modules.RateCalcType.Detail;
using BlazorSvt.Modules.RateCalcType.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.RateCalcType;

public static class RateCalcTypeModule
{
    public static IServiceCollection AddRateCalcTypeModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RateCalcTypeDto>, RateCalcTypeGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RateCalcTypeDetailDto>, RateCalcTypeDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "ratecalctype",
            "HeaderMenu.RateCalcType",
            VisibleToEditor: true));
        services.AddCatalogApi<RateCalcTypeDto, RateCalcTypeDetailDto>(
            "rate-calc-types",
            CatalogDomain.Rates,
            "HeaderMenu.RateCalcType");

        return services;
    }
}
