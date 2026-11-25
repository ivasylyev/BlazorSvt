using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Region.List;

[GridSnapshot("v2.Region_Snapshot")]
public class RegionDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long RegionId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? RegionCode { get; set; }

    [GridColumn]
    public string? Code_NSI { get; set; }

    [GridColumn]
    public string? RegionCodePlanning { get; set; }

    [GridColumn]
    public string? RegionCodeRF { get; set; }

    [GridColumn]
    public string? NameRu { get; set; }

    [GridColumn]
    public string? NameEn { get; set; }

    [GridColumn]
    public string? CountryNameRu { get; set; }

    [GridColumn]
    public string? CountryNameEn { get; set; }

    [GridColumn]
    public string? Market { get; set; }

    [GridColumn(SqlColumn = "FederalDistrictsId")]
    public FederalDistrictsRu? FederalDistrictsIdRu { get; set; }

    [GridColumn(SqlColumn = "FederalDistrictsId")]
    public FederalDistrictsEn? FederalDistrictsIdEn { get; set; }

    [GridColumn]
    public string? CreationUserName { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public string? LastChangeUserName { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
