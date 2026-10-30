namespace BlazorSvt.Modules.Currency.List;

[GridSnapshot("v2.Currency_Snapshot")]
public class CurrencyDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long CurrencyId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public string? Symbolic { get; set; }

    [GridColumn]
    public decimal? Digital { get; set; }

    [GridColumn]
    public string? Code_NSI { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
