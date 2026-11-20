namespace BlazorSvt.Modules.Coefficient.Detail;

[DetailChildSource("v2.vw_CoefficientComponentValue_Detail", "CoefficientId")]
public class CoefficientComponentValueDto
{
    public long CoefficientId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? ComponentName { get; set; }
    public decimal? Value { get; set; }
}
