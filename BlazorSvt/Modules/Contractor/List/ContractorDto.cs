namespace BlazorSvt.Modules.Contractor.List;

[GridSnapshot("v2.Contractor_Snapshot")]
public class ContractorDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long ContractorId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? NameSearch { get; set; }

    [GridColumn]
    public bool Consignee { get; set; }

    [GridColumn]
    public bool IsTrader { get; set; }

    [GridColumn]
    public bool IsProcessing { get; set; }

    [GridColumn]
    public bool IsSender { get; set; }

    [GridColumn]
    public bool IsDebtor { get; set; }

    [GridColumn]
    public bool IsCreditor { get; set; }

    [GridColumn]
    public string? INN { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
