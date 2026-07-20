using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.ParityRates.List;

[GridSnapshot("v2.ParityRates_Snapshot")]
public class ParityRatesDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long ParityRatesId { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateOnly StartDate { get; set; }

    [GridColumn]
    public DateOnly EndDate { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn(SqlColumn = "RelevanceId")]
    public required RelevanceRu RelevanceIdRu { get; set; }

    [GridColumn(SqlColumn = "RelevanceId")]
    public required RelevanceEn RelevanceIdEn { get; set; }

    [GridColumn]
    public required string NodeFromCode { get; set; }

    [GridColumn]
    public required string NodeFromNameRu { get; set; }

    [GridColumn]
    public required string NodeFromNameEn { get; set; }

    [GridColumn]
    public string? ProxyNode1Code { get; set; }

    [GridColumn]
    public string? ProxyNode1NameRu { get; set; }

    [GridColumn]
    public string? ProxyNode1NameEn { get; set; }

    [GridColumn]
    public string? ProxyNode2Code { get; set; }

    [GridColumn]
    public string? ProxyNode2NameRu { get; set; }

    [GridColumn]
    public string? ProxyNode2NameEn { get; set; }

    [GridColumn]
    public required string NodeToCode { get; set; }

    [GridColumn]
    public required string NodeToNameRu { get; set; }

    [GridColumn]
    public required string NodeToNameEn { get; set; }

    [GridColumn(Filterable = false)]
    public required string TransportTypeCode { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeLevel3Ru TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeLevel3En TransportTypeIdEn { get; set; }

    [GridColumn]
    public required string ProductGroupCode { get; set; }

    [GridColumn]
    public required string ProductGroupNameRu { get; set; }

    [GridColumn]
    public required string ProductGroupNameEn { get; set; }

    [GridColumn]
    public long? ProductCode { get; set; }

    [GridColumn]
    public string? ProductNameRu { get; set; }

    [GridColumn]
    public string? ProductNameEn { get; set; }

    [GridColumn(Filterable = false)]
    public decimal? Level_Danger_Product { get; set; }

    [GridColumn(Filterable = false)]
    public decimal TotalCostTransport { get; set; }

    [GridColumn(Filterable = false)]
    public decimal LoadOfTransport { get; set; }

    [GridColumn(Filterable = false)]
    public decimal TotalCostTon { get; set; }

    [GridColumn]
    public required Currency CurrencyId { get; set; }

    [GridColumn]
    public string? Comment { get; set; }

    [GridColumn]
    public string? DataSource { get; set; }

    [GridColumn(Filterable = false)]
    public decimal? FactRate { get; set; }

    [GridColumn(Filterable = false)]
    public decimal? BusinessPlanningRate { get; set; }

    [GridColumn]
    public string? DepartmentResponsibilityArea { get; set; }

    [GridColumn]
    public string? EmployeeResponsibilityArea { get; set; }

    [GridColumn]
    public string? Methodology { get; set; }

    [GridColumn]
    public string? PriorityText { get; set; }
}
