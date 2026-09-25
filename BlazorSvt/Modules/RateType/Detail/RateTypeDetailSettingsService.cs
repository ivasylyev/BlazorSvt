using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateType.Detail;

public class RateTypeDetailSettingsService(
    IStringLocalizer<Resources.RateType> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RateTypeDetailDto>
{
    public DetailSettingsCollection<RateTypeDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<RateTypeDetailDto>(platform);
        var group = L["RateTypeDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RateTypeDetailDto.Code"]);
        b.Add(group, x => x.Name, L["RateTypeDetailDto.Name"]);
        b.AddYesNo(group, x => x.IsUseNomination, L["RateTypeDetailDto.IsUseNomination"]);
        b.AddYesNo(group, x => x.IsDeflated, L["RateTypeDetailDto.IsDeflated"]);
        b.Add(group, x => x.Comment, L["RateTypeDetailDto.Comment"]);
        b.AddYesNo(group, x => x.IsIntoSyncClass, L["RateTypeDetailDto.IsIntoSyncClass"]);
        b.Add(group, x => x.CreationDate, L["RateTypeDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["RateTypeDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RateTypeDetailDto.IsArchive"]);

        return b.Build();
    }
}
