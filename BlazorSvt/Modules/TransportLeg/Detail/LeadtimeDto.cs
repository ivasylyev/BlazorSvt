namespace BlazorSvt.Modules.TransportLeg.Detail;

[DetailChildSource("v2.vw_Leadtime_Detail", "TransportLegId")]
public class LeadtimeDto
{
    public long TransportLegId { get; set; }
    public long Id { get; set; }
    public int? PrimitiveEntityDataStateId { get; set; }
    public bool IsArchive => PrimitiveEntityDataStateId == DetailChildRowStates.Archival;
    public string? Code { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ShipmentTypeNameRu { get; set; }
    public string? ShipmentTypeNameEn { get; set; }
    public bool? LeadTimeIsActive { get; set; }
    public decimal? CommercialLT { get; set; }
    public decimal? SearchTime { get; set; }
    public decimal? LoadTime { get; set; }
    public decimal? TravelTime { get; set; }
    public decimal? DaysWaiting { get; set; }
    public decimal? UnLoadTime { get; set; }
    public decimal? TransportationTime { get; set; }
    public decimal? Distance { get; set; }
    public decimal? Leg1_SearchTime { get; set; }
    public decimal? Leg1_LoadTime { get; set; }
    public decimal? Leg1_TravelTime { get; set; }
    public decimal? Leg1_DaysWaiting { get; set; }
    public decimal? Leg1_TransportationTime { get; set; }
    public decimal? Leg1_Distance { get; set; }
    public decimal? Leg2_TravelTime { get; set; }
    public decimal? Leg2_DaysWaiting { get; set; }
    public decimal? Leg2_UploadTime { get; set; }
    public decimal? Leg2_TransportationTime { get; set; }
    public decimal? Leg2_Distance { get; set; }
}
