using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Coefficient.Detail;

public class CoefficientDetailSettingsService(
    IStringLocalizer<Resources.Coefficient> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<CoefficientDetailDto>
{
    public DetailSettingsCollection<CoefficientDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<CoefficientDetailDto>(platform);
        var group = L["CoefficientDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["CoefficientDetailDto.Code"]);
        b.AddEnum(isRu, group, x => x.CoefficientTypeIdRu, x => x.CoefficientTypeIdEn, L["CoefficientDetailDto.CoefficientTypeName"]);
        b.Add(group, x => x.Name, L["CoefficientDetailDto.Name"]);
        b.Add(group, x => x.StartDate, L["CoefficientDetailDto.StartDate"]);
        b.Add(group, x => x.FinishDate, L["CoefficientDetailDto.FinishDate"]);
        b.AddYesNo(group, x => x.Active, L["CoefficientDetailDto.Active"]);
        b.AddYesNo(group, x => x.OverallValue, L["CoefficientDetailDto.OverallValue"]);
        b.AddEnum(isRu, group, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["CoefficientDetailDto.RateTypeName"]);
        b.AddCreationUser(group, x => x.CreationUserName);
        b.Add(group, x => x.CreationDate, L["CoefficientDetailDto.CreationDate"]);
        b.AddLastChangeUser(group, x => x.LastChangeUserName);
        b.Add(group, x => x.LastChangeDate, L["CoefficientDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["CoefficientDetailDto.IsArchive"]);

        return b.Build();
    }
}
