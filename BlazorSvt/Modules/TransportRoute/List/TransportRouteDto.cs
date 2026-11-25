using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportRoute.List;

[GridSnapshot("v2.TransportRoute_Snapshot")]
public class TransportRouteDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long TransportRouteId { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public string? CodeSAP { get; set; }

    [GridColumn]
    public string? Route { get; set; }

    [GridColumn, ApiReference("locations-nodes", "code")]
    public string? StartNodeCode { get; set; }

    [GridColumn]
    public string? StartNodeNameRu { get; set; }

    [GridColumn]
    public string? StartNodeNameEn { get; set; }

    [GridColumn, ApiReference("regions", "code")]
    public string? RegionFromCode { get; set; }

    [GridColumn]
    public string? RegionFromNameRu { get; set; }

    [GridColumn]
    public string? RegionFromNameEn { get; set; }

    [GridColumn, ApiReference("locations-nodes", "code")]
    public string? ProxyNodeCode { get; set; }

    [GridColumn]
    public string? ProxyNodeNameRu { get; set; }

    [GridColumn]
    public string? ProxyNodeNameEn { get; set; }

    [GridColumn, ApiReference("locations-nodes", "code")]
    public string? EndNodeCode { get; set; }

    [GridColumn]
    public string? EndNodeNameRu { get; set; }

    [GridColumn]
    public string? EndNodeNameEn { get; set; }

    [GridColumn, ApiReference("regions", "code")]
    public string? RegionToCode { get; set; }

    [GridColumn]
    public string? RegionToNameRu { get; set; }

    [GridColumn]
    public string? RegionToNameEn { get; set; }

    [GridColumn(SqlColumn = "RouteTransportKindId")]
    public required TransportKindRu RouteTransportKindIdRu { get; set; }

    [GridColumn(SqlColumn = "RouteTransportKindId")]
    public required TransportKindEn RouteTransportKindIdEn { get; set; }

    [GridColumn]
    public bool IsActive { get; set; }

    [GridColumn(SqlColumn = "Leg1_TransportTypeId")]
    public TransportTypeLevel3Ru? Leg1_TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "Leg1_TransportTypeId")]
    public TransportTypeLevel3En? Leg1_TransportTypeIdEn { get; set; }

    [GridColumn(SqlColumn = "Leg2_TransportTypeId")]
    public TransportTypeLevel3Ru? Leg2_TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "Leg2_TransportTypeId")]
    public TransportTypeLevel3En? Leg2_TransportTypeIdEn { get; set; }

    [GridColumn]
    public string? ChainNodes { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
