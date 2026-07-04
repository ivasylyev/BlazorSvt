
using BlazorSvt.Modules.TransportLeg.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportLeg.List;

[GridSnapshot("v2.TransportLeg_Snapshot")]
public class TransportLegDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long TransportLegId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public bool CanBeUsed { get; set; }

    [GridColumn(SqlColumn = "ShipmentTypeId")]
    public required ShipmentTypeRu ShipmentTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "ShipmentTypeId")]
    public required ShipmentTypeEn ShipmentTypeIdEn { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindRu TransportKindIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindEn TransportKindIdEn { get; set; }

    [GridColumn(Filterable = false)]
    public required string TransportKindCode { get; set; }

    [GridColumn(Filterable = false)]
    public required string SearchTimeT { get; set; }

    [GridColumn(Filterable = false)]
    public required string LoadTimeT { get; set; }

    [GridColumn(Filterable = false)]
    public required string TravelTimeT { get; set; }

    [GridColumn(Filterable = false)]
    public required string DaysWaitingT { get; set; }

    [GridColumn(Filterable = false)]
    public required string UnLoadTimeT { get; set; }

    [GridColumn(Filterable = false)]
    public required string TransportationTimeT { get; set; }

    [GridColumn]
    public required string NodeFromCode { get; set; }

    [GridColumn]
    public required string NodeFromNameRu { get; set; }

    [GridColumn]
    public required string NodeFromNameEn { get; set; }

    [GridColumn]
    public required string RegionFromCode { get; set; }

    [GridColumn]
    public required string RegionFromNameRu { get; set; }

    [GridColumn]
    public required string RegionFromNameEn { get; set; }

    [GridColumn]
    public string? ProxyNodeCode { get; set; }

    [GridColumn]
    public string? ProxyNodeNameRu { get; set; }

    [GridColumn]
    public string? ProxyNodeNameEn { get; set; }

    [GridColumn]
    public string? ProxyRegionCode { get; set; }

    [GridColumn]
    public string? ProxyRegionNameRu { get; set; }

    [GridColumn]
    public string? ProxyRegionNameEn { get; set; }

    [GridColumn]
    public required string NodeToCode { get; set; }

    [GridColumn]
    public required string NodeToNameRu { get; set; }

    [GridColumn]
    public required string NodeToNameEn { get; set; }

    [GridColumn]
    public required string RegionToCode { get; set; }

    [GridColumn]
    public required string RegionToNameRu { get; set; }

    [GridColumn]
    public required string RegionToNameEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
