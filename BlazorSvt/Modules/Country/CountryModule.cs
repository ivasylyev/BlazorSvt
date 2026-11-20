using BlazorSvt.Modules.Country.Detail;
using BlazorSvt.Modules.Country.List;
using BlazorSvt.Modules.Country.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Country;

public static class CountryModule
{
    public static IServiceCollection AddCountryModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<CountryDto>, CountryGridSettingsService>();
        services.AddScoped<IDetailSettingsService<CountryDetailDto>, CountryDetailSettingsService>();
        services.AddSingleton<ISnapshotSyncJob, CountrySyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Routes,
            "country",
            "HeaderMenu.Country",
            VisibleToEditor: true));
        services.AddCatalogApi<CountryDto, CountryDetailDto>(
            "countries",
            CatalogDomain.Routes,
            "HeaderMenu.Country");

        return services;
    }
}
