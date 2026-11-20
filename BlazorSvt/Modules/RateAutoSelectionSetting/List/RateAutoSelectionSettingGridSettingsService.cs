using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateAutoSelectionSetting.List;

public class RateAutoSelectionSettingGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.RateAutoSelectionSetting> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RateAutoSelectionSettingGridSettingsService> logger)
    : BaseGridSettingsService<RateAutoSelectionSettingDto>(localStorage, logger)
{
    protected override string StorageKey => "RateAutoSelectionSettingGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<RateAutoSelectionSettingDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<RateAutoSelectionSettingDto>(platform);

        b.Add(x => x.Code, L["RateAutoSelectionSettingDto.Code"]);
        b.Add(x => x.DestinationNodeToRegion, L["RateAutoSelectionSettingDto.DestinationNodeToRegion"], display: dto => YesNo(dto.DestinationNodeToRegion));
        b.Add(x => x.DestinationNodeToCountry, L["RateAutoSelectionSettingDto.DestinationNodeToCountry"], display: dto => YesNo(dto.DestinationNodeToCountry));
        b.Add(x => x.ProductToProductGroup, L["RateAutoSelectionSettingDto.ProductToProductGroup"], display: dto => YesNo(dto.ProductToProductGroup));
        b.Add(x => x.AnyTransportKind, L["RateAutoSelectionSettingDto.AnyTransportKind"], display: dto => YesNo(dto.AnyTransportKind));
        b.Add(x => x.CreationDate, L["RateAutoSelectionSettingDto.CreationDate"], visible: false);
        b.Add(x => x.LastChangeDate, L["RateAutoSelectionSettingDto.LastChangeDate"], visible: false);
        b.Add(
            x => x.IsArchive,
            L["RateAutoSelectionSettingDto.IsArchive"],
            visible: false,
            display: dto => dto.IsArchive ? platform["Common.Archive"] : platform["Common.Active"]);

        var settings = b.Build();
        settings.Single(c => c.Name == nameof(RateAutoSelectionSettingDto.IsArchive)).FilterValue = "False";
        return settings;

        string YesNo(bool? value) => value switch
        {
            true => platform["Common.Yes"],
            false => platform["Common.No"],
            _ => string.Empty
        };
    }
}
