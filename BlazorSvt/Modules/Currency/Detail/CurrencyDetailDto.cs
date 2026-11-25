namespace BlazorSvt.Modules.Currency.Detail;

[DetailSource("v2.vw_Currency_Detail", "CurrencyId")]
public class CurrencyDetailDto
{
    public long CurrencyId { get; set; }

    public required string Code { get; set; }

    public string? Name { get; set; }

    public string? Symbolic { get; set; }

    public long? Digital { get; set; }

    public long? Code_NSI { get; set; }

    public bool IsArchive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }
}
