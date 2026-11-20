using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Coefficient.Detail;

[DetailChildSource("v2.vw_CoefficientConditionValue_Detail", "CoefficientId")]
public class CoefficientConditionValueDto
{
    public long CoefficientId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public bool? Except { get; set; }
    public TransportKindRu? TransportKindIdRu { get; set; }
    public TransportKindEn? TransportKindIdEn { get; set; }
    public TransportTypeRu? TransportTypeLevel2IdRu { get; set; }
    public TransportTypeEn? TransportTypeLevel2IdEn { get; set; }
    public string? VehicleTypeName { get; set; }
    public string? FromLocationNameRu { get; set; }
    public string? FromLocationNameEn { get; set; }
    public string? ToLocationNameRu { get; set; }
    public string? ToLocationNameEn { get; set; }
}
