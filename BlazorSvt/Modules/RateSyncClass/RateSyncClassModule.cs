using BlazorSvt.Modules.RateSyncClass.Detail;
using BlazorSvt.Modules.RateSyncClass.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.RateSyncClass;

public static class RateSyncClassModule
{
    public static IServiceCollection AddRateSyncClassModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RateSyncClassDto>, RateSyncClassGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RateSyncClassDetailDto>, RateSyncClassDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "ratesyncclass",
            "HeaderMenu.RateSyncClass",
            VisibleToEditor: true));
        services.AddCatalogApi<RateSyncClassDto, RateSyncClassDetailDto>(
            "rate-sync-classes",
            CatalogDomain.Rates,
            "HeaderMenu.RateSyncClass");

        return services;
    }
}
