using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Coefficient.Detail;

/// <summary>«Условие по коэффициенту» — дочерний примитив CoefficientConditionValue (PEI 2080).</summary>
public class CoefficientConditionValueSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.Coefficient> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<CoefficientDetailDto, CoefficientConditionValueDto>(dataService)
{
    public override string Title => L["CoefficientConditionValueDto.Title"];

    protected override object ParentKey(CoefficientDetailDto parent) => parent.CoefficientId;

    protected override bool ParentIsArchive(CoefficientDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<CoefficientConditionValueDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<CoefficientConditionValueDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["CoefficientConditionValueDto.Code"]);
        b.AddYesNo(group, x => x.Except, L["CoefficientConditionValueDto.Except"]);
        b.AddEnum(isRu, group, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["CoefficientConditionValueDto.TransportKindName"]);
        b.AddEnum(isRu, group, x => x.TransportTypeLevel2IdRu, x => x.TransportTypeLevel2IdEn, L["CoefficientConditionValueDto.TransportTypeLevel2Name"]);
        b.Add(group, x => x.VehicleTypeName, L["CoefficientConditionValueDto.VehicleTypeName"]);
        b.AddLocalized(isRu, group, x => x.FromLocationNameRu, x => x.FromLocationNameEn, L["CoefficientConditionValueDto.FromLocationName"]);
        b.AddLocalized(isRu, group, x => x.ToLocationNameRu, x => x.ToLocationNameEn, L["CoefficientConditionValueDto.ToLocationName"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["CoefficientConditionValueDto.IsArchive"]);

        return b.Build();
    }
}
