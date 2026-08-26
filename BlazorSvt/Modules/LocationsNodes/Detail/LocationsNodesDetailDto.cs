using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

namespace BlazorSvt.Modules.LocationsNodes.Detail;

[DetailSource("v2.vw_LocationsNodes_Detail", "LocationsNodesId")]
public class LocationsNodesDetailDto
{
    public long LocationsNodesId { get; set; }
    public bool IsArchive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }
    public required string Code { get; set; }
    public string? NameZD { get; set; }
    public string? Seaport { get; set; }
    public string? AutoNode { get; set; }
    public string? FIASRegion { get; set; }
    public string? FIASDistrict { get; set; }
    public string? FIASCity { get; set; }
    public bool? Terminal { get; set; }
    public string? OpenStreetMap { get; set; }
    public bool? Virtual { get; set; }
    public bool? IsFactory { get; set; }
    public bool? IsPortShip { get; set; }
    public bool? IsPortStore { get; set; }
    public bool? IsStore { get; set; }
    public decimal? CoordinateW { get; set; }
    public decimal? CoordinateL { get; set; }
    public string? Code_NSI { get; set; }
    public long? Status { get; set; }
    public string? PointTube { get; set; }
    public string? Street { get; set; }
    public string? House { get; set; }
    public string? OfficeApart { get; set; }
    public string? ZDCodeN { get; set; }
    public string? NodesCode { get; set; }
    public string? NameRu { get; set; }
    public string? NameEn { get; set; }
    public required LocationTypeRu LocationTypeIdRu { get; set; }
    public required LocationTypeEn LocationTypeIdEn { get; set; }
    public string? LocationTypeCode { get; set; }
    public string? LocationTypeNameRu { get; set; }
    public string? LocationTypeNameEn { get; set; }
    public required TypeNodeRu TypeNodeIdRu { get; set; }
    public required TypeNodeEn TypeNodeIdEn { get; set; }
    public string? TypeNodeCode { get; set; }
    public string? TypeNodeNameRu { get; set; }
    public string? TypeNodeNameEn { get; set; }
    public long? RegionIdRu { get; set; }
    public long? RegionIdEn { get; set; }
    public string? RegionCode { get; set; }
    public string? RegionNameRu { get; set; }
    public string? RegionNameEn { get; set; }
    public string? RegionRU { get; set; }
    public long? CountryIdRu { get; set; }
    public long? CountryIdEn { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryNameRu { get; set; }
    public string? CountryNameEn { get; set; }
    public string? City { get; set; }
    public string? FullAddress { get; set; }
    public bool? BorderCrossing { get; set; }
    public string? LocationTypeCodeNSI { get; set; }
    public string? TypeNodeCodeNSI { get; set; }
    public string? RegionNSIEN { get; set; }
    public string? RegionCodeNSI { get; set; }
    public string? RegionCodeDPO { get; set; }
    public string? CountryRU { get; set; }
    public string? NameCountryEN { get; set; }
    public string? CountryISO2 { get; set; }
    public string? CountryISO3 { get; set; }
    public string? CountryCodeDPO { get; set; }
    public string? MarketNameRU { get; set; }
    public string? MarketNameEN { get; set; }
    public string? MarketCode { get; set; }
    public string? MarketCodeDPO { get; set; }
    public string? CodeZDroad { get; set; }
    public string? NameZDroadRU { get; set; }
    public string? NameZDroadEN { get; set; }
    public string? NameZDEN { get; set; }
    public string? AddressCountryISO2 { get; set; }
    public string? AddressCountryISO3 { get; set; }
    public string? AddressCountryCodeDPO { get; set; }
    public string? AddressNameCountryRU { get; set; }
    public string? AddressNameCountryEN { get; set; }
    public string? Pobox { get; set; }
    public string? NameFederalDistrictRU { get; set; }
    public string? NameDistrictRU { get; set; }
    public string? NameCityRU { get; set; }
    public string? NameCityDistrictRU { get; set; }
    public bool? IsKladr { get; set; }
    public string? AddressLanguage { get; set; }
    public string? RegionCodeRF { get; set; }
    public string? AddressRegionISO { get; set; }
    public string? FIASCodeCity { get; set; }
    public string? FIASStreet { get; set; }
    public string? FIASHouse { get; set; }
    public string? OKTMOCode { get; set; }
    public string? FIASCodeAddress { get; set; }
    public bool? IsDadata { get; set; }
    public bool? CannotDeliver { get; set; }
    public string? BorderCountryISO2 { get; set; }
    public string? BorderCountryISO3 { get; set; }
    public string? BorderCountryCodeDPO { get; set; }
    public string? BorderNameCountryRU { get; set; }
    public string? BorderNameCountryEN { get; set; }
    public string? StatusNSI { get; set; }
    public string? IsPlanning { get; set; }
    public bool? IsPlan { get; set; }
    public string? Level4CityRU { get; set; }
    public string? Level4CityEN { get; set; }
    public string? Level4CityFias { get; set; }
}
