using BlazorSvt.Modules.TransportKind.Detail;
using BlazorSvt.Modules.TransportKind.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.TransportKind;

public static class TransportKindModule
{
    public static IServiceCollection AddTransportKindModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportKindDto>, TransportKindGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportKindDetailDto>, TransportKindDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "transportkind",
            "HeaderMenu.TransportKind",
            VisibleToEditor: true));
        services.AddCatalogApi<TransportKindDto, TransportKindDetailDto>(
            "transport-kinds",
            CatalogDomain.Routes,
            "HeaderMenu.TransportKind");

        return services;
    }
}
