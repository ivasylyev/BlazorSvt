using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateAutoSelectionSetting.Detail;

public class RateAutoSelectionSettingDetailSettingsService(
    IStringLocalizer<Resources.RateAutoSelectionSetting> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RateAutoSelectionSettingDetailDto>
{
    public DetailSettingsCollection<RateAutoSelectionSettingDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<RateAutoSelectionSettingDetailDto>(platform);
        var group = L["RateAutoSelectionSettingDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RateAutoSelectionSettingDetailDto.Code"]);
        b.AddYesNo(group, x => x.DestinationNodeToRegion, L["RateAutoSelectionSettingDetailDto.DestinationNodeToRegion"]);
        b.AddYesNo(group, x => x.DestinationNodeToCountry, L["RateAutoSelectionSettingDetailDto.DestinationNodeToCountry"]);
        b.AddYesNo(group, x => x.ProductToProductGroup, L["RateAutoSelectionSettingDetailDto.ProductToProductGroup"]);
        b.AddYesNo(group, x => x.AnyTransportKind, L["RateAutoSelectionSettingDetailDto.AnyTransportKind"]);
        b.Add(group, x => x.CreationDate, L["RateAutoSelectionSettingDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["RateAutoSelectionSettingDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RateAutoSelectionSettingDetailDto.IsArchive"]);

        return b.Build();
    }
}
