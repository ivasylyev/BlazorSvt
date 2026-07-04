using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Infrastructure.Config;

namespace BlazorSvt.Modules.LocationsNodes.List;

[GridSnapshot("mdm.v2.LocationsNodes_Snapshot")]
public class LocationsNodesDto
{
    [GridColumn(GridColumnType.Id, Order = 10)]
    public long Id { get; set; }

    [GridColumn(GridColumnType.Id, IsEntityKey = true, Order = 20)]
    public long LocationsNodesId { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 30)]
    public required string Code { get; set; }

    [GridColumn(GridColumnType.Bit, Order = 40)]
    public bool IsArchive { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 50)]
    public string? NameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 60)]
    public string? NameEn { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "LocationTypeId", Order = 70)]
    public required LocationTypeRu LocationTypeIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "LocationTypeId", Order = 80)]
    public required LocationTypeEn LocationTypeIdEn { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TypeNodeId", Order = 90)]
    public required TypeNodeRu TypeNodeIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TypeNodeId", Order = 100)]
    public required TypeNodeEn TypeNodeIdEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 110)]
    public string? RegionCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 120)]
    public string? RegionNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 130)]
    public string? RegionNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 140)]
    public string? CountryNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 150)]
    public string? CountryNameEn { get; set; }

    [GridColumn(GridColumnType.Date, Order = 160)]
    public DateTime CreationDate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 170)]
    public DateTime LastChangeDate { get; set; }
}
