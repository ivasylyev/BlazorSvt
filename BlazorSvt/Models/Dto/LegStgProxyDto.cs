namespace BlazorSvt.Models.Dto;

public class LegStgProxyDto
{
    public string? Code { get; set; }

    public string? NodeFrom { get; set; }
    public string? RegionFrom { get; set; }
    public string? ProxyNode { get; set; }
    public string? ProxyRegion { get; set; }
    public string? NodeTo { get; set; }
    public string? RegionTo { get; set; }

    public string? TransportKind { get; set; }

    public decimal? SearchTime { get; set; }
    public decimal? LoadTime { get; set; }
    public decimal? TravelTime { get; set; }
    public decimal? UnLoadTime { get; set; }
    public decimal? DaysWaiting { get; set; }
    public decimal? TransportationTime { get; set; }
    public decimal? Distance { get; set; }

    public string? Leg1_TransportType { get; set; }
    public decimal? Leg1_SearchTime { get; set; }
    public decimal? Leg1_LoadTime { get; set; }
    public decimal? Leg1_TravelTime { get; set; }
    public decimal? Leg1_DaysWaiting { get; set; }
    public decimal? Leg1_TransportationTime { get; set; }
    public decimal? Leg1_Distance { get; set; }

    public string? Leg2_TransportType { get; set; }
    public decimal? Leg2_DaysWaiting { get; set; }
    public decimal? Leg2_TravelTime { get; set; }
    public decimal? Leg2_UploadTime { get; set; }
    public decimal? Leg2_TransportationTime { get; set; }
    public decimal? Leg2_Distance { get; set; }
}
