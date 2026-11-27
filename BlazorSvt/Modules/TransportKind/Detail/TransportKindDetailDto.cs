using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportKind.Detail;

[DetailSource("v2.vw_TransportKind_Detail", "TransportKindId")]
public class TransportKindDetailDto
{
    public long TransportKindId { get; set; }

    public required string Code { get; set; }

    public string? Name { get; set; }

    public string? NameEnRu { get; set; }

    public StatusElementRu? StatusIdRu { get; set; }

    public StatusElementEn? StatusIdEn { get; set; }

    public string? Level { get; set; }

    public string? KindCode { get; set; }

    public string? CreationUserName { get; set; }

    public DateTime CreationDate { get; set; }

    public string? LastChangeUserName { get; set; }

    public DateTime LastChangeDate { get; set; }

    public bool IsArchive { get; set; }
}
