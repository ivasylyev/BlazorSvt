using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportKind.List;

[GridSnapshot("v2.TransportKind_Snapshot")]
public class TransportKindDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long TransportKindId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public string? NameEnRu { get; set; }

    [GridColumn(SqlColumn = "StatusId")]
    public StatusElementRu? StatusIdRu { get; set; }

    [GridColumn(SqlColumn = "StatusId")]
    public StatusElementEn? StatusIdEn { get; set; }

    [GridColumn]
    public string? Level { get; set; }

    [GridColumn]
    public string? KindCode { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
