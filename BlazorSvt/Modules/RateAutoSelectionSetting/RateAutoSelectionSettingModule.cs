using BlazorSvt.Modules.RateAutoSelectionSetting.Detail;
using BlazorSvt.Modules.RateAutoSelectionSetting.List;
using BlazorSvt.Modules.RateAutoSelectionSetting.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.RateAutoSelectionSetting;

public static class RateAutoSelectionSettingModule
{
    public static IServiceCollection AddRateAutoSelectionSettingModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<RateAutoSelectionSettingDto>, RateAutoSelectionSettingGridSettingsService>();
        services.AddScoped<IDetailSettingsService<RateAutoSelectionSettingDetailDto>, RateAutoSelectionSettingDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, RateAutoSelectionSettingSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "rateautoselectionsetting",
            "HeaderMenu.RateAutoSelectionSetting",
            VisibleToEditor: true));
        services.AddCatalogApi<RateAutoSelectionSettingDto, RateAutoSelectionSettingDetailDto>(
            "rate-auto-selection-settings",
            CatalogDomain.Rates,
            "HeaderMenu.RateAutoSelectionSetting");

        return services;
    }
}
