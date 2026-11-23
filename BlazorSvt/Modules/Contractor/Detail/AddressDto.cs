using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.Contractor.Detail;

[DetailChildSource("v2.vw_Address_Detail", "ContractorId")]
public class AddressDto
{
    public long ContractorId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public string? AddressType { get; set; }
    public string? City { get; set; }
    public StatusElementRu? StatusIdRu { get; set; }
    public StatusElementEn? StatusIdEn { get; set; }
    public string? AddressKindName { get; set; }
    public string? Pobox { get; set; }
    public string? RegionCode { get; set; }
    public string? District_ru { get; set; }
    public string? Street { get; set; }
    public string? House { get; set; }
    public string? OfficeApart { get; set; }
    public string? FullAddress_ru { get; set; }
    public decimal? CoordinateW { get; set; }
    public decimal? CoordinateL { get; set; }
    public string? FIASCodeRegion { get; set; }
    public string? FIASCodeDistrict { get; set; }
    public string? FIASCodeSettlement { get; set; }
    public string? RegionNameRu { get; set; }
    public string? RegionNameEn { get; set; }
    public string? CountryNameRu { get; set; }
    public string? CountryNameEn { get; set; }
}
