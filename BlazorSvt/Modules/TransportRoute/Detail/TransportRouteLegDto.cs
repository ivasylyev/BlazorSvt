using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportRoute.Detail;

[DetailChildSource("v2.vw_TransportRouteLeg_Detail", "TransportRouteId")]
public class TransportRouteLegDto
{
    public long TransportRouteId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public decimal? Step { get; set; }
    public string? LegCode { get; set; }
    public string? NodeFromNameRu { get; set; }
    public string? NodeFromNameEn { get; set; }
    public string? ProxyNodeNameRu { get; set; }
    public string? ProxyNodeNameEn { get; set; }
    public string? NodeToNameRu { get; set; }
    public string? NodeToNameEn { get; set; }
    public TransportKindRu? TransportKindIdRu { get; set; }
    public TransportKindEn? TransportKindIdEn { get; set; }
    public TransportTypeLevel3Ru? Leg1_TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? Leg1_TransportTypeIdEn { get; set; }
    public TransportTypeLevel3Ru? Leg2_TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? Leg2_TransportTypeIdEn { get; set; }
}
