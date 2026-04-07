
namespace BlazorSvt.Models.Dto;

public class RateDto
{
    public long Id { get; set; }
    public bool IsArchive { get; set; }
    public bool IsDefRate { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }

    public decimal TotalCostTon { get; set; }
    public decimal TotalCostTransport { get; set; }

    public long NodeFromId { get; set; }
    public long? ProxyNodeId { get; set; }
    public long NodeToId { get; set; }
    public long TransportTypeId { get; set; }
    public long? ProductGroupId { get; set; }
    public long? ProductId { get; set; }

    public required string Code { get; set; }
    public required string CurrencyCode { get; set; }
    public required RateTypeRu RateTypeIdRu { get; set; }
    public required RateTypeEn RateTypeIdEn { get; set; }
    public required string RateTypeName { get; set; }

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
    public required string TransportKindName { get; set; }
    public required string TransportTypeCode { get; set; }
    public required TransportTypeLevel3Ru TransportTypeIdRu { get; set; }
    public required TransportTypeLevel3En TransportTypeIdEn { get; set; }
    public required string TransportTypeName { get; set; }

    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }
    public string? ContractorCode { get; set; }
    // ReSharper disable once InconsistentNaming
    public string? ContractorEGRUL { get; set; }
}