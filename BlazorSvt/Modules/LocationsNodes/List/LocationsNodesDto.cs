using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using BlazorSvt.Platform.Infrastructure.Config;

namespace BlazorSvt.Modules.LocationsNodes.List;

[GridSnapshot("v2.LocationsNodes_Snapshot")]
public class LocationsNodesDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long LocationsNodesId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? NameRu { get; set; }

    [GridColumn]
    public string? NameEn { get; set; }

    [GridColumn(SqlColumn = "LocationTypeId")]
    public required LocationTypeRu LocationTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "LocationTypeId")]
    public required LocationTypeEn LocationTypeIdEn { get; set; }

    [GridColumn(SqlColumn = "TypeNodeId")]
    public required TypeNodeRu TypeNodeIdRu { get; set; }

    [GridColumn(SqlColumn = "TypeNodeId")]
    public required TypeNodeEn TypeNodeIdEn { get; set; }

    [GridColumn]
    public string? RegionCode { get; set; }

    [GridColumn]
    public string? RegionNameRu { get; set; }

    [GridColumn]
    public string? RegionNameEn { get; set; }

    [GridColumn]
    public string? CountryNameRu { get; set; }

    [GridColumn]
    public string? CountryNameEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
