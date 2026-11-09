using BlazorSvt.Modules.ProductGroup.Detail;
using BlazorSvt.Modules.ProductGroup.List;
using BlazorSvt.Modules.ProductGroup.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.ProductGroup;

public static class ProductGroupModule
{
    public static IServiceCollection AddProductGroupModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<ProductGroupDto>, ProductGroupGridSettingsService>();
        services.AddScoped<IDetailSettingsService<ProductGroupDetailDto>, ProductGroupDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, ProductGroupSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Products,
            "productgroup",
            "HeaderMenu.ProductGroup",
            VisibleToEditor: true));
        services.AddCatalogApi<ProductGroupDto, ProductGroupDetailDto>(
            "product-groups",
            CatalogDomain.Products,
            "HeaderMenu.ProductGroup");

        return services;
    }
}
