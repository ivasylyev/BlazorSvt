using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.ProductGroup.List;

[GridSnapshot("v2.ProductGroup_Snapshot")]
public class ProductGroupDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long ProductGroupId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public string? ShortName { get; set; }

    [GridColumn]
    public string? GroupCode { get; set; }

    [GridColumn]
    public string? ProductGroupCBD { get; set; }

    [GridColumn]
    public string? NameEnCode { get; set; }

    [GridColumn]
    public string? NameEnRu { get; set; }

    [GridColumn]
    public string? Code_NSI { get; set; }

    [GridColumn]
    public bool SVTUseProductGroupWhenNoProductRateExists { get; set; }

    [GridColumn]
    public string? NameEn { get; set; }

    [GridColumn(SqlColumn = "RateSyncClassId")]
    public required RateSyncClassRu RateSyncClassIdRu { get; set; }

    [GridColumn(SqlColumn = "RateSyncClassId")]
    public required RateSyncClassEn RateSyncClassIdEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
