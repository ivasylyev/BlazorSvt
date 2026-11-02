using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.MTR.List;

[GridSnapshot("v2.MTR_Snapshot")]
public class MTRDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long MTRId { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public string? NameFullRu { get; set; }

    [GridColumn]
    public string? NameShortEn { get; set; }

    [GridColumn, ApiReference("product-groups", "code")]
    public string? ProductGroupCode { get; set; }

    [GridColumn]
    public string? ProductGroupNameRu { get; set; }

    [GridColumn]
    public string? ProductGroupNameEn { get; set; }

    [GridColumn]
    public string? EtsngNameRu { get; set; }

    [GridColumn]
    public string? EtsngNameEn { get; set; }

    [GridColumn]
    public string? GngCode { get; set; }

    [GridColumn(SqlColumn = "UnitId")]
    public UnitRu? UnitIdRu { get; set; }

    [GridColumn(SqlColumn = "UnitId")]
    public UnitEn? UnitIdEn { get; set; }

    [GridColumn]
    public string? DpoCode { get; set; }

    [GridColumn]
    public string? MtrType { get; set; }

    [GridColumn(SqlColumn = "DangerClassId")]
    public DangerClassRu? DangerClassIdRu { get; set; }

    [GridColumn(SqlColumn = "DangerClassId")]
    public DangerClassEn? DangerClassIdEn { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
