using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.ReportConstructor.List;

[GridSnapshot("v2.ReportConstructor_Snapshot")]
public class ReportConstructorDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long ReportConstructorId { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn(SqlColumn = "ReportTypeId")]
    public required ReportDictionaryTypesRu ReportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "ReportTypeId")]
    public required ReportDictionaryTypesEn ReportTypeIdEn { get; set; }

    [GridColumn]
    public DateOnly DateFrom { get; set; }

    [GridColumn]
    public DateOnly DateTo { get; set; }

    [GridColumn, ApiReference("locations-nodes", "code")]
    public string? NodeFromCode { get; set; }

    [GridColumn]
    public string? NodeFromNameRu { get; set; }

    [GridColumn]
    public string? NodeFromNameEn { get; set; }

    [GridColumn, ApiReference("locations-nodes", "code")]
    public string? NodeToCode { get; set; }

    [GridColumn]
    public string? NodeToNameRu { get; set; }

    [GridColumn]
    public string? NodeToNameEn { get; set; }

    [GridColumn]
    public string? TransportKindCode { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public TransportKindRu? TransportKindIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public TransportKindEn? TransportKindIdEn { get; set; }

    [GridColumn]
    public string? TransportTypeCode { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public TransportTypeLevel3Ru? TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public TransportTypeLevel3En? TransportTypeIdEn { get; set; }

    [GridColumn, ApiReference("product-groups", "code")]
    public string? ProductGroupCode { get; set; }

    [GridColumn]
    public string? ProductGroupNameRu { get; set; }

    [GridColumn]
    public string? ProductGroupNameEn { get; set; }

    [GridColumn, ApiReference("products", "code")]
    public string? ProductCode { get; set; }

    [GridColumn]
    public string? ProductNameRu { get; set; }

    [GridColumn]
    public string? ProductNameEn { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? CreationUserName { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? LastChangeUserName { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
