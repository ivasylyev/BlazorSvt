namespace BlazorSvt.Modules.TransportRate.Detail;

[DetailChildSource("v2.vw_TransportRateCoefficient_Detail", "TransportRateId")]
public class TransportRateCoefficientDto
{
    public long TransportRateId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? CoefficientRateCode { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? CoefficientName { get; set; }
    public string? SourceRateCode { get; set; }
    public string? SourceRateKindName { get; set; }
}
