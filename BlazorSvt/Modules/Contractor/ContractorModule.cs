using BlazorSvt.Modules.Contractor.Detail;
using BlazorSvt.Modules.Contractor.List;
using BlazorSvt.Modules.Contractor.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.Contractor;

public static class ContractorModule
{
    public static IServiceCollection AddContractorModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<ContractorDto>, ContractorGridSettingsService>();
        services.AddScoped<IDetailSettingsService<ContractorDetailDto>, ContractorDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, ContractorSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "contractor",
            "HeaderMenu.Contractor",
            VisibleToEditor: true));
        services.AddCatalogApi<ContractorDto, ContractorDetailDto>(
            "contractors",
            CatalogDomain.Rates,
            "HeaderMenu.Contractor");

        return services;
    }
}
