using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.MTR.Detail;

[DetailSource("v2.vw_MTR_Detail", "MTRId")]
public class MTRDetailDto
{
    public long MTRId { get; set; }
    public bool IsArchive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public required string Code { get; set; }
    public string? NameFullRu { get; set; }
    public string? NameShortRu { get; set; }
    public string? NameFullEn { get; set; }
    public string? NameShortEn { get; set; }
    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameRu { get; set; }
    public string? ProductGroupNameEn { get; set; }
    public string? EtsngNameRu { get; set; }
    public string? EtsngNameEn { get; set; }
    public string? GngCode { get; set; }
    public UnitRu? UnitIdRu { get; set; }
    public UnitEn? UnitIdEn { get; set; }
    public string? DpoCode { get; set; }
    public string? MtrType { get; set; }
    public DangerClassRu? DangerClassIdRu { get; set; }
    public DangerClassEn? DangerClassIdEn { get; set; }
    public StatusElementRu? StatusIdRu { get; set; }
    public StatusElementEn? StatusIdEn { get; set; }
    public string? ProductCode { get; set; }
}
