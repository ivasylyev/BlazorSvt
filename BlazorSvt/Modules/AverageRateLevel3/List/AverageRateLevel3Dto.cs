using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.AverageRateLevel3.List;

[GridSnapshot("v2.AverageRateLevel3_Snapshot")]
public class AverageRateLevel3Dto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long AverageRateLevel3Id { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public bool IsDefRate { get; set; }

    [GridColumn]
    public DateOnly StartDate { get; set; }

    [GridColumn]
    public DateOnly EndDate { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }

    [GridColumn]
    public decimal RateLevel3 { get; set; }

    [GridColumn]
    public decimal EffectiveLoadOfTransportType { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public required Currency CurrencyId { get; set; }

    [GridColumn(SqlColumn = "RateTypeId")]
    public required RateTypeRu RateTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "RateTypeId")]
    public required RateTypeEn RateTypeIdEn { get; set; }

    [GridColumn]
    public required string NodeFromCode { get; set; }

    [GridColumn]
    public required string NodeFromNameRu { get; set; }

    [GridColumn]
    public required string NodeFromNameEn { get; set; }

    [GridColumn]
    public string? ProxyNodeCode { get; set; }

    [GridColumn]
    public string? ProxyNodeNameRu { get; set; }

    [GridColumn]
    public string? ProxyNodeNameEn { get; set; }

    [GridColumn]
    public required string NodeToCode { get; set; }

    [GridColumn]
    public required string NodeToNameRu { get; set; }

    [GridColumn]
    public required string NodeToNameEn { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindRu TransportKindIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindEn TransportKindIdEn { get; set; }

    [GridColumn]
    public required string TransportKindCode { get; set; }

    [GridColumn]
    public required string TransportTypeCode { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeLevel3Ru TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeLevel3En TransportTypeIdEn { get; set; }

    [GridColumn]
    public string? ProductGroupCode { get; set; }

    [GridColumn]
    public string? ProductGroupNameRu { get; set; }

    [GridColumn]
    public string? ProductGroupNameEn { get; set; }

    [GridColumn]
    public long? ProductCode { get; set; }

    [GridColumn]
    public string? ProductNameRu { get; set; }

    [GridColumn]
    public string? ProductNameEn { get; set; }
}
