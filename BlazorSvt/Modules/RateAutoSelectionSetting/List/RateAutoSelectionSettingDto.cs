namespace BlazorSvt.Modules.RateAutoSelectionSetting.List;

[GridSnapshot("v2.RateAutoSelectionSetting_Snapshot")]
public class RateAutoSelectionSettingDto
{
    [GridColumn]
    public long Id { get; set; }

    [GridColumn(IsEntityKey = true)]
    public long RateAutoSelectionSettingId { get; set; }

    [GridColumn]
    public bool IsArchive { get; set; }

    [GridColumn]
    public required long Code { get; set; }

    [GridColumn]
    public bool? DestinationNodeToRegion { get; set; }

    [GridColumn]
    public bool? DestinationNodeToCountry { get; set; }

    [GridColumn]
    public bool? ProductToProductGroup { get; set; }

    [GridColumn]
    public bool? AnyTransportKind { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? CreationUserName { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn(WithoutFullText = true)]
    public string? LastChangeUserName { get; set; }

    [GridColumn]
    public DateTime? LastChangeDate { get; set; }
}
