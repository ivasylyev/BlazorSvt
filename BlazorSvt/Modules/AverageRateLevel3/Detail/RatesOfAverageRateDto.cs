namespace BlazorSvt.Modules.AverageRateLevel3.Detail;

[DetailChildSource("v2.vw_RatesOfAverageRate_Detail", "AverageRateLevel3Id")]
public class RatesOfAverageRateDto
{
    public long AverageRateLevel3Id { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? TransportRateCode { get; set; }
    public decimal? TotalCostTon { get; set; }
    public decimal? TotalCostTransport { get; set; }
}
