using BlazorSvt.Modules.Region.Detail;
using BlazorSvt.Modules.Region.List;
using BlazorSvt.Modules.Region.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Region;

public static class RegionModule
{
    public static IServiceCollection AddRegionModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RegionDto>, RegionGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RegionDetailDto>, RegionDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, RegionSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "region",
            "HeaderMenu.Region",
            VisibleToEditor: true));
        services.AddCatalogApi<RegionDto, RegionDetailDto>(
            "regions",
            CatalogDomain.Routes,
            "HeaderMenu.Region");

        return services;
    }
}
