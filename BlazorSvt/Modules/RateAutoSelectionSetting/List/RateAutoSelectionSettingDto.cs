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
    public required string Code { get; set; }

    [GridColumn]
    public bool? DestinationNodeToRegion { get; set; }

    [GridColumn]
    public bool? DestinationNodeToCountry { get; set; }

    [GridColumn]
    public bool? ProductToProductGroup { get; set; }

    [GridColumn]
    public bool? AnyTransportKind { get; set; }

    [GridColumn]
    public DateTime CreationDate { get; set; }

    [GridColumn]
    public DateTime? LastChangeDate { get; set; }
}
