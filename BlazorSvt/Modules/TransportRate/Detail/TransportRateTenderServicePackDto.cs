using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportRate.Detail;

[DetailChildSource("v2.vw_TransportRateTenderServicePack_Detail", "TransportRateId")]
public class TransportRateTenderServicePackDto
{
    public long TransportRateId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? TenderServicePack { get; set; }
    public decimal? LotRate { get; set; }
    public CurrencyRuEn? LotCurrencyId { get; set; }
}
