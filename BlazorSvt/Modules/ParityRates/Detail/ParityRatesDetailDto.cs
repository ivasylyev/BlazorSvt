namespace BlazorSvt.Modules.ParityRates.Detail;

[DetailSource("v2.vw_ParityRates_Detail", "ParityRatesId")]
public class ParityRatesDetailDto
{
    public long ParityRatesId { get; set; }
    public required string Code { get; set; }
    public bool IsArchive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? LastChangeDate { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public string? RelevanceCode { get; set; }
    public string? RelevanceName { get; set; }

    public string? NodeFromCode { get; set; }
    public string? NodeFromNameRu { get; set; }
    public string? NodeFromNameEn { get; set; }

    public string? ProxyNode1Code { get; set; }
    public string? ProxyNode1NameRu { get; set; }
    public string? ProxyNode1NameEn { get; set; }

    public string? ProxyNode2Code { get; set; }
    public string? ProxyNode2NameRu { get; set; }
    public string? ProxyNode2NameEn { get; set; }

    public string? NodeToCode { get; set; }
    public string? NodeToNameRu { get; set; }
    public string? NodeToNameEn { get; set; }

    public string? TransportTypeCode { get; set; }
    public string? TransportTypeNameRu { get; set; }
    public string? TransportTypeNameEn { get; set; }

    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }

    public long? ProductCode { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }

    public decimal? Level_Danger_Product { get; set; }
    public bool Dangerous_Cargo { get; set; }

    public decimal? TotalCostTransport { get; set; }
    public decimal? LoadOfTransport { get; set; }
    public decimal? TotalCostTon { get; set; }
    public string? CurrencyStandard { get; set; }

    public string? Comment { get; set; }
    public string? DataSource { get; set; }
    public decimal? FactRate { get; set; }
    public decimal? BusinessPlanningRate { get; set; }
    public string? DepartmentResponsibilityArea { get; set; }
    public string? EmployeeResponsibilityArea { get; set; }
    public string? Methodology { get; set; }
    public string? PriorityText { get; set; }
}
