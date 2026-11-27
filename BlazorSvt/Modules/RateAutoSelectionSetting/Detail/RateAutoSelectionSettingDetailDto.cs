namespace BlazorSvt.Modules.RateAutoSelectionSetting.Detail;

[DetailSource("v2.vw_RateAutoSelectionSetting_Detail", "RateAutoSelectionSettingId")]
public class RateAutoSelectionSettingDetailDto
{
    public long RateAutoSelectionSettingId { get; set; }

    public required long Code { get; set; }

    public bool? DestinationNodeToRegion { get; set; }

    public bool? DestinationNodeToCountry { get; set; }

    public bool? ProductToProductGroup { get; set; }

    public bool? AnyTransportKind { get; set; }

    public string? CreationUserName { get; set; }

    public DateTime CreationDate { get; set; }

    public string? LastChangeUserName { get; set; }

    public DateTime? LastChangeDate { get; set; }

    public bool IsArchive { get; set; }
}
