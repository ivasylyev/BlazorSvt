namespace BlazorSvt.Models.Dto;

public class LegStgWithoutProxyDto
{
    public string? Code { get; set; }

    public string? NodeFrom { get; set; }
    public string? RegionFrom { get; set; }
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
}
