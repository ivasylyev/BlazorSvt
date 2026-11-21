using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.Detail;

/// <summary>«Средневзвешенные ставки 3 уровня» — дочерний примитив LegAverageRateLevel3 (PEI 2059).</summary>
public class LegAverageRateLevel3Section(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportLeg> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportLegDetailDto, LegAverageRateLevel3Dto>(dataService)
{
    public override string Title => L["LegAverageRateLevel3Dto.Title"];

    protected override object ParentKey(TransportLegDetailDto parent) => parent.TransportLegId;

    protected override bool ParentIsArchive(TransportLegDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<LegAverageRateLevel3Dto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<LegAverageRateLevel3Dto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["LegAverageRateLevel3Dto.Code"]);
        b.Add(group, x => x.AverageRateCode, L["LegAverageRateLevel3Dto.AverageRateCode"]);
        b.AddYesNo(group, x => x.IsDefRate, L["LegAverageRateLevel3Dto.IsDefRate"]);
        b.AddEnum(isRu, group, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["LegAverageRateLevel3Dto.RateTypeName"]);
        b.AddEnum(isRu, group, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["LegAverageRateLevel3Dto.TransportTypeName"]);
        b.AddLocalized(isRu, group, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["LegAverageRateLevel3Dto.ProductGroupName"]);
        b.AddLocalized(isRu, group, x => x.ProductNameRu, x => x.ProductNameEn, L["LegAverageRateLevel3Dto.ProductName"]);
        b.Add(group, x => x.StartDate, L["LegAverageRateLevel3Dto.StartDate"]);
        b.Add(group, x => x.EndDate, L["LegAverageRateLevel3Dto.EndDate"]);
        b.Add(group, x => x.RateLevel3, L["LegAverageRateLevel3Dto.RateLevel3"]);
        b.Add(group, x => x.EffectiveLoadOfTransportType, L["LegAverageRateLevel3Dto.EffectiveLoadOfTransportType"]);
        b.AddEnum(isRu, group, x => x.CurrencyId, x => x.CurrencyId, L["LegAverageRateLevel3Dto.Currency"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["LegAverageRateLevel3Dto.IsArchive"]);

        return b.Build();
    }
}
