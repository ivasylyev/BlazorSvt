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
    public long? Digital { get; set; }

    [GridColumn]
    public long? Code_NSI { get; set; }

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
