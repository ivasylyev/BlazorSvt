namespace BlazorSvt.Modules.RateSyncClass.Detail;

[DetailSource("v2.vw_RateSyncClass_Detail", "RateSyncClassId")]
public class RateSyncClassDetailDto
{
    public long RateSyncClassId { get; set; }

    public required string Code { get; set; }

    public string? Name { get; set; }

    public bool IsArchive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }
}
