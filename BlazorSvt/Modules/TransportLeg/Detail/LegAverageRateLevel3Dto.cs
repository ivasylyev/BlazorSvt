using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportLeg.Detail;

[DetailChildSource("v2.vw_LegAverageRateLevel3_Detail", "TransportLegId")]
public class LegAverageRateLevel3Dto
{
    public long TransportLegId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? AverageRateCode { get; set; }
    public bool? IsDefRate { get; set; }
    public RateTypeRu? RateTypeIdRu { get; set; }
    public RateTypeEn? RateTypeIdEn { get; set; }
    public TransportTypeLevel3Ru? TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? TransportTypeIdEn { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? RateLevel3 { get; set; }
    public decimal? EffectiveLoadOfTransportType { get; set; }
    public CurrencyRuEn? CurrencyId { get; set; }
}
