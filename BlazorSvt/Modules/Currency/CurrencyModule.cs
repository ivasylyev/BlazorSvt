using BlazorSvt.Modules.Currency.Detail;
using BlazorSvt.Modules.Currency.List;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Currency;

public static class CurrencyModule
{
    public static IServiceCollection AddCurrencyModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<CurrencyDto>, CurrencyGridSettingsService>();
        services.AddScoped<IDetailSettingsService<CurrencyDetailDto>, CurrencyDetailSettingsService>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Other,
            "currency",
            "HeaderMenu.Currency",
            VisibleToEditor: false));
        services.AddCatalogApi<CurrencyDto, CurrencyDetailDto>(
            "currencies",
            CatalogDomain.Other,
            "HeaderMenu.Currency");

        return services;
    }
}
