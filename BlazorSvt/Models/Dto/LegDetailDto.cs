
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Dto.IdsEnum;

namespace BlazorSvt.Models.Dto;

[TableFunction("v2.fn_GetTransportLegDetail", "LegId")]
public class LegDetailDto
{
    public long Id { get; set; }
    public long LegId { get; set; }
    public required string Code { get; set; }

    public bool IsArchive { get; set; }
    public bool CanBeUsed { get; set; }

    public required ShipmentTypeRu ShipmentTypeIdRu { get; set; }
    public required ShipmentTypeEn ShipmentTypeIdEn { get; set; }

    public required string TransportKindCode { get; set; }
    public required TransportKindRu TransportKindIdRu { get; set; }
    public required TransportKindEn TransportKindIdEn { get; set; }


    public required string SearchTimeT { get; set; }
    public required string LoadTimeT { get; set; }
    public required string DaysWaitingT { get; set; }
    public required string TravelTimeT { get; set; }
    public required string UnLoadTimeT { get; set; }
    public required string TransportationTimeT { get; set; }
    public required int Distance { get; set; }

    public required decimal Leg1_SearchTime { get; set; }
    public required decimal Leg1_LoadTime { get; set; }
    public required decimal Leg1_TravelTime { get; set; }
    public required decimal Leg1_DaysWaiting { get; set; }
    public required decimal Leg1_TransportationTime { get; set; }
    public required int Leg1_Distance { get; set; }


    public required decimal Leg2_UpLoadTime { get; set; }
    public required decimal Leg2_TravelTime { get; set; }
    public required decimal Leg2_DaysWaiting { get; set; }
    public required decimal Leg2_TransportationTime { get; set; }
    public required int Leg2_Distance { get; set; }


    public required string NodeFromCode { get; set; }
    public required string NodeFromNameRu { get; set; }
    public required string NodeFromNameEn { get; set; }
    public required string RegionFromCode { get; set; }
    public required string RegionFromNameRu { get; set; }
    public required string RegionFromNameEn { get; set; }

    public string? ProxyNodeCode { get; set; }
    public string? ProxyNodeNameRu { get; set; }
    public string? ProxyNodeNameEn { get; set; }
    public string? ProxyRegionCode { get; set; }
    public string? ProxyRegionNameRu { get; set; }
    public string? ProxyRegionNameEn { get; set; }

    public required string NodeToCode { get; set; }
    public required string NodeToNameRu { get; set; }
    public required string NodeToNameEn { get; set; }
    public required string RegionToCode { get; set; }
    public required string RegionToNameRu { get; set; }
    public required string RegionToNameEn { get; set; }


    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
}