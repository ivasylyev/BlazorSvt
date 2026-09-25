namespace BlazorSvt.Modules.RateType.List;

[GridSnapshot("v2.RateType_Snapshot")]
public class RateTypeDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long RateTypeId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public bool? IsUseNomination { get; set; }

    [GridColumn]
    public bool? IsDeflated { get; set; }

    [GridColumn]
    public string? Comment { get; set; }

    [GridColumn]
    public bool? IsIntoSyncClass { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
