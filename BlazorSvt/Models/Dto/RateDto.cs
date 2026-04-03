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
    public long TransportKindId { get; set; }
    public long TransportTypeId { get; set; }
    public long? ProductGroupId { get; set; }
    public long? ProductId { get; set; }
    public long RateTypeId { get; set; }

    public required string Code { get; set; }
    public required string CurrencyCode { get; set; }
    public required string RateTypeCode { get; set; }
    public required string RateTypeName { get; set; }

    public required string NodeFromCode { get; set; }
    public required string NodeFromName { get; set; }

    public string? ProxyNodeCode { get; set; }
    public string? ProxyNodeName { get; set; }

    public required string NodeToCode { get; set; }
    public required string NodeToName { get; set; }

    public required string TransportKindCode { get; set; }
    public required string TransportKindName { get; set; }
    public required string TransportTypeCode { get; set; }
    public required string TransportTypeName { get; set; }

    public string? ProductGroupCode { get; set; }
    public string? ProductGroupName { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? ContractorCode { get; set; }
    // ReSharper disable once InconsistentNaming
    public string? ContractorEGRUL { get; set; }
}