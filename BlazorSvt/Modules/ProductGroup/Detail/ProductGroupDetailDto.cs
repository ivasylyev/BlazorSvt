using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.ProductGroup.Detail;

[DetailSource("v2.vw_ProductGroup_Detail", "ProductGroupId")]
public class ProductGroupDetailDto
{
    public long ProductGroupId { get; set; }
    public bool IsArchive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? GroupCode { get; set; }
    public string? ProductGroupCBD { get; set; }
    public string? NameEnCode { get; set; }
    public string? NameEnRu { get; set; }
    public string? Code_NSI { get; set; }
    public bool? SVTUseProductGroupWhenNoProductRateExists { get; set; }
    public string? NameEn { get; set; }
    public required RateSyncClassRu RateSyncClassIdRu { get; set; }
    public required RateSyncClassEn RateSyncClassIdEn { get; set; }
}
