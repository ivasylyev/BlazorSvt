namespace BlazorSvt.Modules.Relevance.List;

[GridSnapshot("v2.Relevance_Snapshot")]
public class RelevanceDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long RelevanceId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
