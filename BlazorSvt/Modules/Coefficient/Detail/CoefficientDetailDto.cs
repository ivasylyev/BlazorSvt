using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Coefficient.Detail;

[DetailSource("v2.vw_Coefficient_Detail", "CoefficientId")]
public class CoefficientDetailDto
{
    public long CoefficientId { get; set; }
    public bool IsArchive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public required string Code { get; set; }
    public required CoefficientTypeRu CoefficientTypeIdRu { get; set; }
    public required CoefficientTypeEn CoefficientTypeIdEn { get; set; }
    public string? Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? FinishDate { get; set; }
    public bool Active { get; set; }
    public bool OverallValue { get; set; }
    public required RateTypeRu RateTypeIdRu { get; set; }
    public required RateTypeEn RateTypeIdEn { get; set; }
}
