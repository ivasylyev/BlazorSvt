namespace BlazorSvt.Modules.RateCalcType.Detail;

[DetailSource("v2.vw_RateCalcType_Detail", "RateCalcTypeId")]
public class RateCalcTypeDetailDto
{
    public long RateCalcTypeId { get; set; }

    public required string Code { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }

    public bool IsArchive { get; set; }

    public string? Name { get; set; }

    public string? SearchCode { get; set; }
}
