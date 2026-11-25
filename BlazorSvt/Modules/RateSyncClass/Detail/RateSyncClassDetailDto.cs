namespace BlazorSvt.Modules.RateSyncClass.Detail;

[DetailSource("v2.vw_RateSyncClass_Detail", "RateSyncClassId")]
public class RateSyncClassDetailDto
{
    public long RateSyncClassId { get; set; }

    public required long Code { get; set; }

    public string? Name { get; set; }

    public bool IsArchive { get; set; }

    public string? CreationUserName { get; set; }

    public DateTime CreationDate { get; set; }

    public string? LastChangeUserName { get; set; }

    public DateTime LastChangeDate { get; set; }
}
