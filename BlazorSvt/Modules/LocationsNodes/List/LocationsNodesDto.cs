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

    public LocationTypeRu? LocationTypeIdRu { get; set; }
    public LocationTypeEn? LocationTypeIdEn { get; set; }
    public string? LocationTypeCode { get; set; }
    public string? LocationTypeNameRu { get; set; }
    public string? LocationTypeNameEn { get; set; }

    public TypeNodeRu? TypeNodeIdRu { get; set; }
    public TypeNodeEn? TypeNodeIdEn { get; set; }
    public string? TypeNodeCode { get; set; }
    public string? TypeNodeNameRu { get; set; }
    public string? TypeNodeNameEn { get; set; }

    public long? RegionIdRu { get; set; }
    public long? RegionIdEn { get; set; }
    public string? RegionCode { get; set; }
    public string? RegionNameRu { get; set; }
    public string? RegionNameEn { get; set; }
    public string? RegionRU { get; set; }

    public long? CountryIdRu { get; set; }
    public long? CountryIdEn { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryNameRu { get; set; }
    public string? CountryNameEn { get; set; }

    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
}
