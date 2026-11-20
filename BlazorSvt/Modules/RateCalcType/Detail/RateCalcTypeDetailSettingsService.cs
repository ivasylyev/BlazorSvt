using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateCalcType.Detail;

public class RateCalcTypeDetailSettingsService(
    IStringLocalizer<Resources.RateCalcType> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RateCalcTypeDetailDto>
{
    public DetailSettingsCollection<RateCalcTypeDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<RateCalcTypeDetailDto>(platform);
        var group = L["RateCalcTypeDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RateCalcTypeDetailDto.Code"]);
        b.Add(group, x => x.CreationDate, L["RateCalcTypeDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["RateCalcTypeDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RateCalcTypeDetailDto.IsArchive"]);
        b.Add(group, x => x.Name, L["RateCalcTypeDetailDto.Name"]);
        b.Add(group, x => x.SearchCode, L["RateCalcTypeDetailDto.SearchCode"]);

        return b.Build();
    }
}
