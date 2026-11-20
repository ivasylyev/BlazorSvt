using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Coefficient.Detail;

/// <summary>«Составляющая по коэффициенту» — дочерний примитив CoefficientComponentValue (PEI 2083).</summary>
public class CoefficientComponentValueSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.Coefficient> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<CoefficientDetailDto, CoefficientComponentValueDto>(dataService)
{
    public override string Title => L["CoefficientComponentValueDto.Title"];

    protected override object ParentKey(CoefficientDetailDto parent) => parent.CoefficientId;

    protected override bool ParentIsArchive(CoefficientDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<CoefficientComponentValueDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var b = new DetailSettingsBuilder<CoefficientComponentValueDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["CoefficientComponentValueDto.Code"]);
        b.Add(group, x => x.ComponentName, L["CoefficientComponentValueDto.ComponentName"]);
        b.Add(group, x => x.Value, L["CoefficientComponentValueDto.Value"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["CoefficientComponentValueDto.IsArchive"]);

        return b.Build();
    }
}
