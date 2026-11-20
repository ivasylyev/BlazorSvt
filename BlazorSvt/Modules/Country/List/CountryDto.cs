using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Country.List;

[GridSnapshot("v2.Country_Snapshot")]
public class CountryDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long CountryId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? NameRu { get; set; }

    [GridColumn]
    public decimal? ISOCode { get; set; }

    [GridColumn]
    public string? NameEn { get; set; }

    [GridColumn]
    public string? PlanCode { get; set; }

    [GridColumn]
    public string? Market { get; set; }

    [GridColumn]
    public string? ISOCode3 { get; set; }

    [GridColumn(SqlColumn = "StatusId")]
    public StatusElementRu? StatusIdRu { get; set; }

    [GridColumn(SqlColumn = "StatusId")]
    public StatusElementEn? StatusIdEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime? LastChangeDate { get; set; }
}
