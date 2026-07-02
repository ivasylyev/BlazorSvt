using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using BlazorSvt.Platform.Infrastructure.Config;

namespace BlazorSvt.Modules.LocationsNodes.List;

[StoredProcedure("v2.LocationsNodes_Get")]
public class LocationsNodesDto
{
    public long Id { get; set; }
    public long LocationsNodesId { get; set; }
    public required string Code { get; set; }

    public bool IsArchive { get; set; }

    public string? NameRu { get; set; }
    public string? NameEn { get; set; }

    public required LocationTypeRu LocationTypeIdRu { get; set; }
    public required LocationTypeEn LocationTypeIdEn { get; set; }

    public required TypeNodeRu TypeNodeIdRu { get; set; }
    public required TypeNodeEn TypeNodeIdEn { get; set; }

    public string? RegionCode { get; set; }
    public string? RegionNameRu { get; set; }
    public string? RegionNameEn { get; set; }
    public string? RegionRU { get; set; }

    public string? CountryNameRu { get; set; }
    public string? CountryNameEn { get; set; }

    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
}
