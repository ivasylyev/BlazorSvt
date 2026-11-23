using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.Detail;

/// <summary>«Применённые коэффициенты» — дочерний примитив TransportRateCoefficient (PEI 2106).</summary>
public class TransportRateCoefficientSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportRate> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportRateDetailDto, TransportRateCoefficientDto>(dataService)
{
    public override string Title => L["TransportRateCoefficientDto.Title"];

    protected override object ParentKey(TransportRateDetailDto parent) => parent.TransportRateId;

    protected override bool ParentIsArchive(TransportRateDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<TransportRateCoefficientDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var b = new DetailSettingsBuilder<TransportRateCoefficientDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["TransportRateCoefficientDto.Code"]);
        b.Add(group, x => x.CoefficientRateCode, L["TransportRateCoefficientDto.CoefficientRateCode"]);
        b.Add(group, x => x.StartDate, L["TransportRateCoefficientDto.StartDate"]);
        b.Add(group, x => x.EndDate, L["TransportRateCoefficientDto.EndDate"]);
        b.Add(group, x => x.CoefficientName, L["TransportRateCoefficientDto.CoefficientName"]);
        b.Add(group, x => x.SourceRateCode, L["TransportRateCoefficientDto.SourceRateCode"]);
        b.Add(group, x => x.SourceRateKindName, L["TransportRateCoefficientDto.SourceRateKindName"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["TransportRateCoefficientDto.IsArchive"]);

        return b.Build();
    }
}
