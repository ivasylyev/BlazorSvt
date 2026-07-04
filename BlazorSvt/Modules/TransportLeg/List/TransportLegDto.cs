
using BlazorSvt.Modules.TransportLeg.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Infrastructure.Config;

namespace BlazorSvt.Modules.TransportLeg.List;

[GridSnapshot("mdm.v2.TransportLeg_Snapshot")]
public class TransportLegDto
{
    [GridColumn(GridColumnType.Id, Order = 10)]
    public long Id { get; set; }

    [GridColumn(GridColumnType.Id, IsEntityKey = true, Order = 20)]
    public long TransportLegId { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 30)]
    public required string Code { get; set; }

    [GridColumn(GridColumnType.Bit, Order = 40)]
    public bool IsArchive { get; set; }

    [GridColumn(GridColumnType.Bit, Order = 50)]
    public bool CanBeUsed { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "ShipmentTypeId", Order = 60)]
    public required ShipmentTypeRu ShipmentTypeIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "ShipmentTypeId", Order = 70)]
    public required ShipmentTypeEn ShipmentTypeIdEn { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportKindId", Order = 80)]
    public required TransportKindRu TransportKindIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportKindId", Order = 90)]
    public required TransportKindEn TransportKindIdEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 100)]
    public required string TransportKindCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 110)]
    public required string SearchTimeT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 120)]
    public required string LoadTimeT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 130)]
    public required string TravelTimeT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 140)]
    public required string DaysWaitingT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 150)]
    public required string UnLoadTimeT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 160)]
    public required string TransportationTimeT { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 170)]
    public required string NodeFromCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 180)]
    public required string NodeFromNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 190)]
    public required string NodeFromNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 200)]
    public required string RegionFromCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 210)]
    public required string RegionFromNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 220)]
    public required string RegionFromNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 230)]
    public string? ProxyNodeCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 240)]
    public string? ProxyNodeNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 250)]
    public string? ProxyNodeNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 260)]
    public string? ProxyRegionCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 270)]
    public string? ProxyRegionNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 280)]
    public string? ProxyRegionNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 290)]
    public required string NodeToCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 300)]
    public required string NodeToNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 310)]
    public required string NodeToNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 320)]
    public required string RegionToCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 330)]
    public required string RegionToNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 340)]
    public required string RegionToNameEn { get; set; }

    [GridColumn(GridColumnType.Date, Order = 350)]
    public DateTime CreationDate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 360)]
    public DateTime LastChangeDate { get; set; }
}
