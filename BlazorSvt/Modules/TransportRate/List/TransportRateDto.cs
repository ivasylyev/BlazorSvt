
using BlazorSvt.Modules.TransportRate.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Infrastructure.Config;

namespace BlazorSvt.Modules.TransportRate.List;

[GridSnapshot("mdm.v2.TransportRate_Snapshot")]
public class TransportRateDto
{
    [GridColumn(GridColumnType.Id, Order = 10)]
    public long Id { get; set; }

    [GridColumn(GridColumnType.Id, IsEntityKey = true, Order = 20)]
    public long TransportRateId { get; set; }

    [GridColumn(GridColumnType.Bit, Order = 30)]
    public bool IsArchive { get; set; }

    [GridColumn(GridColumnType.Bit, Order = 40)]
    public bool IsDefRate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 50)]
    public DateOnly StartDate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 60)]
    public DateOnly EndDate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 70)]
    public DateTime CreationDate { get; set; }

    [GridColumn(GridColumnType.Date, Order = 80)]
    public DateTime LastChangeDate { get; set; }

    [GridColumn(GridColumnType.Id, Filterable = false, Order = 90)]
    public decimal TotalCostTon { get; set; }

    [GridColumn(GridColumnType.Id, Filterable = false, Order = 100)]
    public decimal TotalCostTransport { get; set; }

    [GridColumn(GridColumnType.Id, Order = 110)]
    public required long Code { get; set; }

    [GridColumn(GridColumnType.Id, Order = 120)]
    public required Currency CurrencyId { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "RateTypeId", Order = 130)]
    public required RateTypeRu RateTypeIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "RateTypeId", Order = 140)]
    public required RateTypeEn RateTypeIdEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 150)]
    public required string NodeFromCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 160)]
    public required string NodeFromNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 170)]
    public required string NodeFromNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 180)]
    public string? ProxyNodeCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 190)]
    public string? ProxyNodeNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 200)]
    public string? ProxyNodeNameEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 210)]
    public required string NodeToCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 220)]
    public required string NodeToNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 230)]
    public required string NodeToNameEn { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportKindId", Order = 240)]
    public required TransportKindRu TransportKindIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportKindId", Order = 250)]
    public required TransportKindEn TransportKindIdEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 260)]
    public required string TransportKindCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Filterable = false, Order = 270)]
    public required string TransportTypeCode { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportTypeId", Order = 280)]
    public required TransportTypeLevel3Ru TransportTypeIdRu { get; set; }

    [GridColumn(GridColumnType.Id, SqlColumn = "TransportTypeId", Order = 290)]
    public required TransportTypeLevel3En TransportTypeIdEn { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 300)]
    public string? ProductGroupCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 310)]
    public string? ProductGroupNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 320)]
    public string? ProductGroupNameEn { get; set; }

    [GridColumn(GridColumnType.Id, Order = 330)]
    public long? ProductCode { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 340)]
    public string? ProductNameRu { get; set; }

    [GridColumn(GridColumnType.Nvarchar, Order = 350)]
    public string? ProductNameEn { get; set; }
}
