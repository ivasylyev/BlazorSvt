using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.Detail;

/// <summary>«Ставки плеча» — дочерний примитив LegRate (PEI 2044).</summary>
public class LegRateSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportLeg> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportLegDetailDto, LegRateDto>(dataService)
{
    public override string Title => L["LegRateDto.Title"];

    protected override object ParentKey(TransportLegDetailDto parent) => parent.TransportLegId;

    protected override bool ParentIsArchive(TransportLegDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<LegRateDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<LegRateDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["LegRateDto.Code"]);
        b.Add(group, x => x.RateCode, L["LegRateDto.RateCode"]);
        b.AddYesNo(group, x => x.IsDefRate, L["LegRateDto.IsDefRate"]);
        b.AddEnum(isRu, group, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["LegRateDto.RateTypeName"]);
        b.AddEnum(isRu, group, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["LegRateDto.TransportTypeName"]);
        b.AddLocalized(isRu, group, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["LegRateDto.ProductGroupName"]);
        b.AddLocalized(isRu, group, x => x.ProductNameRu, x => x.ProductNameEn, L["LegRateDto.ProductName"]);
        b.Add(group, x => x.StartDate, L["LegRateDto.StartDate"]);
        b.Add(group, x => x.EndDate, L["LegRateDto.EndDate"]);
        b.Add(group, x => x.TotalCostTon, L["LegRateDto.TotalCostTon"]);
        b.Add(group, x => x.TotalCostTransport, L["LegRateDto.TotalCostTransport"]);
        b.AddEnum(isRu, group, x => x.CurrencyId, x => x.CurrencyId, L["LegRateDto.Currency"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["LegRateDto.IsArchive"]);

        return b.Build();
    }
}
