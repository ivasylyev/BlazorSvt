namespace BlazorSvt.Modules.RateSyncClass.List;

[GridSnapshot("v2.RateSyncClass_Snapshot")]
public class RateSyncClassDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long RateSyncClassId { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? CreationUserName { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? LastChangeUserName { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
