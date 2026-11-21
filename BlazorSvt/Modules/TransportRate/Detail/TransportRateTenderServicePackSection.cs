using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.Detail;

/// <summary>«Тендерный пакет услуг» — дочерний примитив TransportRateTenderServicePack (PEI 2089).</summary>
public class TransportRateTenderServicePackSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportRate> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportRateDetailDto, TransportRateTenderServicePackDto>(dataService)
{
    public override string Title => L["TransportRateTenderServicePackDto.Title"];

    protected override object ParentKey(TransportRateDetailDto parent) => parent.TransportRateId;

    protected override bool ParentIsArchive(TransportRateDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<TransportRateTenderServicePackDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportRateTenderServicePackDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["TransportRateTenderServicePackDto.Code"]);
        b.Add(group, x => x.TenderServicePack, L["TransportRateTenderServicePackDto.TenderServicePack"]);
        b.Add(group, x => x.LotRate, L["TransportRateTenderServicePackDto.LotRate"]);
        b.AddEnum(isRu, group, x => x.LotCurrencyId, x => x.LotCurrencyId, L["TransportRateTenderServicePackDto.LotCurrency"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["TransportRateTenderServicePackDto.IsArchive"]);

        return b.Build();
    }
}
