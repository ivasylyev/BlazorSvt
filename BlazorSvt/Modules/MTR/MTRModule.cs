using BlazorSvt.Modules.MTR.Detail;
using BlazorSvt.Modules.MTR.List;
using BlazorSvt.Modules.MTR.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.MTR;

public static class MTRModule
{
    public static IServiceCollection AddMTRModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<MTRDto>, MTRGridSettingsService>();
        services.AddScoped<IDetailSettingsService<MTRDetailDto>, MTRDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, MTRSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Products,
            "mtr",
            "HeaderMenu.MTR",
            VisibleToEditor: true));
        services.AddCatalogApi<MTRDto, MTRDetailDto>(
            "products",
            CatalogDomain.Products,
            "HeaderMenu.MTR");

        return services;
    }
}
