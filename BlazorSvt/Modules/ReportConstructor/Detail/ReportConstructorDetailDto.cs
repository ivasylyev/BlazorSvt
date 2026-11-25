using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.ReportConstructor.Detail;

[DetailSource("v2.vw_ReportConstructor_Detail", "ReportConstructorId")]
public class ReportConstructorDetailDto
{
    public long ReportConstructorId { get; set; }
    public required long Code { get; set; }
    public required ReportDictionaryTypesRu ReportTypeIdRu { get; set; }
    public required ReportDictionaryTypesEn ReportTypeIdEn { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public string? NodeFromCode { get; set; }
    public string? NodeFromNameRu { get; set; }
    public string? NodeFromNameEn { get; set; }
    public string? NodeToCode { get; set; }
    public string? NodeToNameRu { get; set; }
    public string? NodeToNameEn { get; set; }
    public TransportKindRu? TransportKindIdRu { get; set; }
    public TransportKindEn? TransportKindIdEn { get; set; }
    public string? TransportKindCode { get; set; }
    public TransportTypeLevel3Ru? TransportTypeIdRu { get; set; }
    public TransportTypeLevel3En? TransportTypeIdEn { get; set; }
    public string? TransportTypeCode { get; set; }
    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public bool IsArchive { get; set; }
}
