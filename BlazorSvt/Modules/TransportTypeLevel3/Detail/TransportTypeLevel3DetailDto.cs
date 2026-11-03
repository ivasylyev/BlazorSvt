using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportTypeLevel3.Detail;

[DetailSource("v2.vw_TransportTypeLevel3_Detail", "TransportTypeLevel3Id")]
public class TransportTypeLevel3DetailDto
{
    public long TransportTypeLevel3Id { get; set; }

    public bool? IsBasic_forCBD { get; set; }

    public required string Code { get; set; }

    public required TransportKindRu TransportKindIdRu { get; set; }

    public required TransportKindEn TransportKindIdEn { get; set; }

    public required TransportTypeRu TransportTypeIdRu { get; set; }

    public required TransportTypeEn TransportTypeIdEn { get; set; }

    public string? Name { get; set; }

    public string? NameEnRu { get; set; }

    public string? TypeCode { get; set; }

    public string? SAPCode { get; set; }

    public string? OrionCode { get; set; }

    public StatusElementRu? StatusIdRu { get; set; }

    public StatusElementEn? StatusIdEn { get; set; }

    public bool IsArchive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastChangeDate { get; set; }
}
