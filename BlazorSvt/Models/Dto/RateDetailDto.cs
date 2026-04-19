
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Dto.IdsEnum;

namespace BlazorSvt.Models.Dto;


[StoredProcedure("dbo.GetTransportRateDetail")]
public class RateDetailDto
{
    public long Id { get; set; }
    public long RateId { get; set; }
    public bool IsArchive { get; set; }
    public bool IsDefRate { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }

    public decimal TotalCostTon { get; set; }
    public decimal TotalCostTransport { get; set; }

    public long TransportTypeId { get; set; }
    public long? ProductGroupId { get; set; }
    public long? ProductId { get; set; }

    public required long Code { get; set; }
    public required Currency CurrencyId { get; set; }
    public required RateTypeRu RateTypeIdRu { get; set; }
    public required RateTypeEn RateTypeIdEn { get; set; }
    public required string NodeFromCode { get; set; }
    public required string NodeFromNameRu { get; set; }
    public required string NodeFromNameEn { get; set; }
    public string? ProxyNodeCode { get; set; }
    public string? ProxyNodeNameRu { get; set; }
    public string? ProxyNodeNameEn { get; set; }
    public required string NodeToCode { get; set; }
    public required string NodeToNameRu { get; set; }
    public required string NodeToNameEn { get; set; }
    public required TransportKindRu TransportKindIdRu { get; set; }
    public required TransportKindEn TransportKindIdEn { get; set; }
    public required string TransportKindCode { get; set; }
    public required string TransportTypeCode { get; set; }
    public required TransportTypeLevel3Ru TransportTypeIdRu { get; set; }
    public required TransportTypeLevel3En TransportTypeIdEn { get; set; }
    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }
    public long? ProductCode { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }

}