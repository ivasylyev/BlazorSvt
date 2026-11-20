using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Region.Detail;

[DetailSource("v2.vw_Region_Detail", "RegionId")]
public class RegionDetailDto
{
    public long RegionId { get; set; }
    public required string Code { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public bool IsArchive { get; set; }
    public string? NameRu { get; set; }
    public string? RegionCode { get; set; }
    public string? CountryNameRu { get; set; }
    public string? CountryNameEn { get; set; }
    public required StatusElementRu StatusIdRu { get; set; }
    public required StatusElementEn StatusIdEn { get; set; }
    public string? NameEn { get; set; }
    public string? RegionCodeRF { get; set; }
    public string? RegionCodePlanning { get; set; }
    public string? Market { get; set; }
    public string? Code_NSI { get; set; }
    public FederalDistrictsRu? FederalDistrictsIdRu { get; set; }
    public FederalDistrictsEn? FederalDistrictsIdEn { get; set; }
    public long? CountryIdRu { get; set; }
    public long? CountryIdEn { get; set; }
}
