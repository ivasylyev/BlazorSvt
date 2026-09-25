namespace BlazorSvt.Modules.RateType.Detail;

[DetailSource("v2.vw_RateType_Detail", "RateTypeId")]
public class RateTypeDetailDto
{
    public long RateTypeId { get; set; }

    public required string Code { get; set; }

    public string? Name { get; set; }

    public bool? IsUseNomination { get; set; }

    public bool? IsDeflated { get; set; }

    public string? Comment { get; set; }

    public bool? IsIntoSyncClass { get; set; }

    public bool IsArchive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }
}
