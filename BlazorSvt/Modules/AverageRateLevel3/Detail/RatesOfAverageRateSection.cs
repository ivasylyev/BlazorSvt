using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.Detail;

/// <summary>«Ставки, вошедшие в расчет» — дочерний примитив RatesOfAverageRate (PEI 2058).</summary>
public class RatesOfAverageRateSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.AverageRateLevel3> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<AverageRateLevel3DetailDto, RatesOfAverageRateDto>(dataService)
{
    public override string Title => L["RatesOfAverageRateDto.Title"];

    protected override object ParentKey(AverageRateLevel3DetailDto parent) => parent.AverageRateLevel3Id;

    protected override bool ParentIsArchive(AverageRateLevel3DetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<RatesOfAverageRateDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var b = new DetailSettingsBuilder<RatesOfAverageRateDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["RatesOfAverageRateDto.Code"]);
        b.Add(group, x => x.TransportRateCode, L["RatesOfAverageRateDto.TransportRateCode"]);
        b.Add(group, x => x.TotalCostTon, L["RatesOfAverageRateDto.TotalCostTon"]);
        b.Add(group, x => x.TotalCostTransport, L["RatesOfAverageRateDto.TotalCostTransport"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["RatesOfAverageRateDto.IsArchive"]);

        return b.Build();
    }
}
