using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Coefficient.List;

[GridSnapshot("v2.Coefficient_Snapshot")]
public class CoefficientDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long CoefficientId { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn(SqlColumn = "CoefficientTypeId")]
    public required CoefficientTypeRu CoefficientTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "CoefficientTypeId")]
    public required CoefficientTypeEn CoefficientTypeIdEn { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public DateOnly StartDate { get; set; }

    [GridColumn]
    public DateOnly FinishDate { get; set; }

    [GridColumn]
    public bool Active { get; set; }

    [GridColumn]
    public bool OverallValue { get; set; }

    [GridColumn(SqlColumn = "RateTypeId")]
    public required RateTypeRu RateTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "RateTypeId")]
    public required RateTypeEn RateTypeIdEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
