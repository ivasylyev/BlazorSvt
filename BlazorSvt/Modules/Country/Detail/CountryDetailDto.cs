using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Country.Detail;

[DetailSource("v2.vw_Country_Detail", "CountryId")]
public class CountryDetailDto
{
    public long CountryId { get; set; }
    public required string Code { get; set; }
    public string? NameRu { get; set; }
    public decimal? ISOCode { get; set; }
    public string? NameEn { get; set; }
    public string? PlanCode { get; set; }
    public string? Market { get; set; }
    public string? ISOCode3 { get; set; }
    public StatusElementRu? StatusIdRu { get; set; }
    public StatusElementEn? StatusIdEn { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? LastChangeDate { get; set; }
    public bool IsArchive { get; set; }
}
