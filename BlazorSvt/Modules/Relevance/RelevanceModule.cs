using BlazorSvt.Modules.Relevance.Detail;
using BlazorSvt.Modules.Relevance.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Relevance;

public static class RelevanceModule
{
    public static IServiceCollection AddRelevanceModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RelevanceDto>, RelevanceGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RelevanceDetailDto>, RelevanceDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Parities,
            "relevance",
            "HeaderMenu.Relevance",
            VisibleToEditor: true));
        services.AddCatalogApi<RelevanceDto, RelevanceDetailDto>(
            "relevances",
            CatalogDomain.Parities,
            "HeaderMenu.Relevance");

        return services;
    }
}
