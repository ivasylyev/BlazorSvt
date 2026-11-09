using BlazorSvt.Modules.TransportTypeLevel3.Detail;
using BlazorSvt.Modules.TransportTypeLevel3.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.TransportTypeLevel3;

public static class TransportTypeLevel3Module
{
    public static IServiceCollection AddTransportTypeLevel3Module(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportTypeLevel3Dto>, TransportTypeLevel3GridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportTypeLevel3DetailDto>, TransportTypeLevel3DetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "transporttypelevel3",
            "HeaderMenu.TransportTypeLevel3",
            VisibleToEditor: true));
        services.AddCatalogApi<TransportTypeLevel3Dto, TransportTypeLevel3DetailDto>(
            "transport-types-level-3",
            CatalogDomain.Routes,
            "HeaderMenu.TransportTypeLevel3");

        return services;
    }
}
