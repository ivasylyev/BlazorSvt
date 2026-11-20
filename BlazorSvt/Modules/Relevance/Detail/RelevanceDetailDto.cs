namespace BlazorSvt.Modules.Relevance.Detail;

[DetailSource("v2.vw_Relevance_Detail", "RelevanceId")]
public class RelevanceDetailDto
{
    public long RelevanceId { get; set; }

    public required string Code { get; set; }

    public string? Name { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }

    public bool IsArchive { get; set; }
}
