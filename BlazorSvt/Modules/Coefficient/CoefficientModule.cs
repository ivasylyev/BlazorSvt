using BlazorSvt.Modules.Coefficient.Detail;
using BlazorSvt.Modules.Coefficient.List;
using BlazorSvt.Modules.Coefficient.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Coefficient;

public static class CoefficientModule
{
    public static IServiceCollection AddCoefficientModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<CoefficientDto>, CoefficientGridSettingsService>();
        services.AddScoped<IDetailSettingsService<CoefficientDetailDto>, CoefficientDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, CoefficientSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "coefficient",
            "HeaderMenu.Coefficient",
            VisibleToEditor: true));
        services.AddCatalogApi<CoefficientDto, CoefficientDetailDto>(
            "coefficients",
            CatalogDomain.Rates,
            "HeaderMenu.Coefficient",
            new CatalogPeriod(nameof(CoefficientDto.StartDate), nameof(CoefficientDto.FinishDate)));

        return services;
    }
}
