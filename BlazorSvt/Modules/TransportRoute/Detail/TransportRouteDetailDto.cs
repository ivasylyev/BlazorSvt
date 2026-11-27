using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportRoute.Detail;

[DetailSource("v2.vw_TransportRoute_Detail", "TransportRouteId")]
public class TransportRouteDetailDto
{
    public long TransportRouteId { get; set; }
    public required long Code { get; set; }
    public string? CodeSAP { get; set; }
    public bool IsActive { get; set; }
    public string? Route { get; set; }

    public string? StartNodeNameRu { get; set; }
    public string? StartNodeNameEn { get; set; }
    public string? StartNodeCode { get; set; }

    public string? RegionFromNameRu { get; set; }
    public string? RegionFromNameEn { get; set; }
    public string? RegionFromCode { get; set; }

    public string? ProxyNodeNameRu { get; set; }
    public string? ProxyNodeNameEn { get; set; }
    public string? ProxyNodeCode { get; set; }

    public string? EndNodeNameRu { get; set; }
    public string? EndNodeNameEn { get; set; }
    public string? EndNodeCode { get; set; }

    public string? RegionToNameRu { get; set; }
    public string? RegionToNameEn { get; set; }
    public string? RegionToCode { get; set; }

    public required TransportKindRu RouteTransportKindIdRu { get; set; }
    public required TransportKindEn RouteTransportKindIdEn { get; set; }

    public TransportTypeLevel3Ru? Leg1_TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? Leg1_TransportTypeIdEn { get; set; }

    public TransportTypeLevel3Ru? Leg2_TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? Leg2_TransportTypeIdEn { get; set; }

    public string? ShortNameRu { get; set; }
    public string? ShortNameEN { get; set; }
    public string? NameEN { get; set; }
    public string? ChainNodes { get; set; }

    public string? Leg_01Code { get; set; }
    public string? Leg_02Code { get; set; }
    public string? Leg_03Code { get; set; }

    public bool IsArchive { get; set; }
    public string? CreationUserName { get; set; }
    public DateTime CreationDate { get; set; }
    public string? LastChangeUserName { get; set; }
    public DateTime LastChangeDate { get; set; }
}
