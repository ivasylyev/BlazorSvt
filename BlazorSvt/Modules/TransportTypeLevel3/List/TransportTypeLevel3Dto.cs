using BlazorSvt.Platform.Domain.IdsEnum;

namespace BlazorSvt.Modules.TransportTypeLevel3.List;

[GridSnapshot("v2.TransportTypeLevel3_Snapshot")]
public class TransportTypeLevel3Dto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long TransportTypeLevel3Id { get; set; }

    [GridColumn]
    public bool? IsBasic_forCBD { get; set; }

    [GridColumn]
    public required string Code { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindRu TransportKindIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportKindId")]
    public required TransportKindEn TransportKindIdEn { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeRu TransportTypeIdRu { get; set; }

    [GridColumn(SqlColumn = "TransportTypeId")]
    public required TransportTypeEn TransportTypeIdEn { get; set; }

    [GridColumn]
    public string? Name { get; set; }

    [GridColumn]
    public string? NameEnRu { get; set; }

    [GridColumn]
    public string? TypeCode { get; set; }

    [GridColumn]
    public string? SAPCode { get; set; }

    [GridColumn]
    public string? OrionCode { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime LastChangeDate { get; set; }
}
