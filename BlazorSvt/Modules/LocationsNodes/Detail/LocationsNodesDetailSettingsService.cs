using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.Detail;

public class LocationsNodesDetailSettingsService(
    IStringLocalizer<Resources.LocationsNodes> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<LocationsNodesDetailDto>
{
    public DetailSettingsCollection<LocationsNodesDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<LocationsNodesDetailDto>(platform);
        var group = L["LocationsNodesDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["LocationsNodesDetailDto.Code"]);
        b.Add(group, x => x.CreationDate, L["LocationsNodesDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["LocationsNodesDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["LocationsNodesDetailDto.IsArchive"]);

        b.Add(group, x => x.NameZD, L["LocationsNodesDetailDto.NameZD"]);
        b.Add(group, x => x.Seaport, L["LocationsNodesDetailDto.Seaport"]);
        b.Add(group, x => x.AutoNode, L["LocationsNodesDetailDto.AutoNode"]);
        b.Add(group, x => x.FIASRegion, L["LocationsNodesDetailDto.FIASRegion"]);
        b.Add(group, x => x.FIASDistrict, L["LocationsNodesDetailDto.FIASDistrict"]);
        b.Add(group, x => x.FIASCity, L["LocationsNodesDetailDto.FIASCity"]);
        b.Add(group, x => x.Terminal, L["LocationsNodesDetailDto.Terminal"]);
        b.Add(group, x => x.OpenStreetMap, L["LocationsNodesDetailDto.OpenStreetMap"]);
        b.Add(group, x => x.Virtual, L["LocationsNodesDetailDto.Virtual"]);
        b.Add(group, x => x.IsFactory, L["LocationsNodesDetailDto.IsFactory"]);
        b.Add(group, x => x.IsPortShip, L["LocationsNodesDetailDto.IsPortShip"]);
        b.Add(group, x => x.IsPortStore, L["LocationsNodesDetailDto.IsPortStore"]);
        b.Add(group, x => x.IsStore, L["LocationsNodesDetailDto.IsStore"]);
        b.Add(group, x => x.CoordinateW, L["LocationsNodesDetailDto.CoordinateW"]);
        b.Add(group, x => x.CoordinateL, L["LocationsNodesDetailDto.CoordinateL"]);
        b.Add(group, x => x.Code_NSI, L["LocationsNodesDetailDto.Code_NSI"]);
        b.Add(group, x => x.Status, L["LocationsNodesDetailDto.Status"]);
        b.Add(group, x => x.PointTube, L["LocationsNodesDetailDto.PointTube"]);
        b.Add(group, x => x.Street, L["LocationsNodesDetailDto.Street"]);
        b.Add(group, x => x.House, L["LocationsNodesDetailDto.House"]);
        b.Add(group, x => x.OfficeApart, L["LocationsNodesDetailDto.OfficeApart"]);
        b.Add(group, x => x.ZDCodeN, L["LocationsNodesDetailDto.ZDCodeN"]);
        b.Add(group, x => x.NodesCode, L["LocationsNodesDetailDto.NodesCode"]);
        b.AddLocalized(isRu, group, x => x.NameRu, x => x.NameEn,
            isRu ? L["LocationsNodesDetailDto.NameRu"] : L["LocationsNodesDetailDto.NameEn"]);
        b.AddEnum(isRu, group, x => x.LocationTypeIdRu, x => x.LocationTypeIdEn, L["LocationsNodesDetailDto.LocationTypeName"]);
        b.Add(group, x => x.LocationTypeCode, L["LocationsNodesDetailDto.LocationTypeCode"]);
        b.AddLocalized(isRu, group, x => x.LocationTypeNameRu, x => x.LocationTypeNameEn, L["LocationsNodesDetailDto.LocationTypeName"]);
        b.AddEnum(isRu, group, x => x.TypeNodeIdRu, x => x.TypeNodeIdEn, L["LocationsNodesDetailDto.TypeNodeName"]);
        b.Add(group, x => x.TypeNodeCode, L["LocationsNodesDetailDto.TypeNodeCode"]);
        b.AddLocalized(isRu, group, x => x.TypeNodeNameRu, x => x.TypeNodeNameEn, L["LocationsNodesDetailDto.TypeNodeName"]);
        b.AddLocalized(isRu, group, x => x.RegionIdRu, x => x.RegionIdEn, L["LocationsNodesDetailDto.RegionName"]);
        b.Add(group, x => x.RegionCode, L["LocationsNodesDetailDto.RegionCode"]);
        b.AddLocalized(isRu, group, x => x.RegionNameRu, x => x.RegionNameEn, L["LocationsNodesDetailDto.RegionName"]);
        b.AddLocalized(isRu, group, x => x.CountryIdRu, x => x.CountryIdEn, L["LocationsNodesDetailDto.CountryName"]);
        b.Add(group, x => x.CountryCode, L["LocationsNodesDetailDto.CountryCode"]);
        b.AddLocalized(isRu, group, x => x.CountryNameRu, x => x.CountryNameEn, L["LocationsNodesDetailDto.CountryName"]);
        b.Add(group, x => x.RegionRU, L["LocationsNodesDetailDto.RegionRU"]);
        b.Add(group, x => x.City, L["LocationsNodesDetailDto.City"]);
        b.Add(group, x => x.FullAddress, L["LocationsNodesDetailDto.FullAddress"]);
        b.Add(group, x => x.BorderCrossing, L["LocationsNodesDetailDto.BorderCrossing"]);
        b.Add(group, x => x.LocationTypeCodeNSI, L["LocationsNodesDetailDto.LocationTypeCodeNSI"]);
        b.Add(group, x => x.TypeNodeCodeNSI, L["LocationsNodesDetailDto.TypeNodeCodeNSI"]);
        b.Add(group, x => x.RegionNSIEN, L["LocationsNodesDetailDto.RegionNSIEN"]);
        b.Add(group, x => x.RegionCodeNSI, L["LocationsNodesDetailDto.RegionCodeNSI"]);
        b.Add(group, x => x.RegionCodeDPO, L["LocationsNodesDetailDto.RegionCodeDPO"]);
        b.Add(group, x => x.CountryRU, L["LocationsNodesDetailDto.CountryRU"]);
        b.Add(group, x => x.NameCountryEN, L["LocationsNodesDetailDto.NameCountryEN"]);
        b.Add(group, x => x.CountryISO2, L["LocationsNodesDetailDto.CountryISO2"]);
        b.Add(group, x => x.CountryISO3, L["LocationsNodesDetailDto.CountryISO3"]);
        b.Add(group, x => x.CountryCodeDPO, L["LocationsNodesDetailDto.CountryCodeDPO"]);
        b.Add(group, x => x.MarketNameRU, L["LocationsNodesDetailDto.MarketNameRU"]);
        b.Add(group, x => x.MarketNameEN, L["LocationsNodesDetailDto.MarketNameEN"]);
        b.Add(group, x => x.MarketCode, L["LocationsNodesDetailDto.MarketCode"]);
        b.Add(group, x => x.MarketCodeDPO, L["LocationsNodesDetailDto.MarketCodeDPO"]);
        b.Add(group, x => x.CodeZDroad, L["LocationsNodesDetailDto.CodeZDroad"]);
        b.Add(group, x => x.NameZDroadRU, L["LocationsNodesDetailDto.NameZDroadRU"]);
        b.Add(group, x => x.NameZDroadEN, L["LocationsNodesDetailDto.NameZDroadEN"]);
        b.Add(group, x => x.NameZDEN, L["LocationsNodesDetailDto.NameZDEN"]);
        b.Add(group, x => x.AddressCountryISO2, L["LocationsNodesDetailDto.AddressCountryISO2"]);
        b.Add(group, x => x.AddressCountryISO3, L["LocationsNodesDetailDto.AddressCountryISO3"]);
        b.Add(group, x => x.AddressCountryCodeDPO, L["LocationsNodesDetailDto.AddressCountryCodeDPO"]);
        b.Add(group, x => x.AddressNameCountryRU, L["LocationsNodesDetailDto.AddressNameCountryRU"]);
        b.Add(group, x => x.AddressNameCountryEN, L["LocationsNodesDetailDto.AddressNameCountryEN"]);
        b.Add(group, x => x.Pobox, L["LocationsNodesDetailDto.Pobox"]);
        b.Add(group, x => x.NameFederalDistrictRU, L["LocationsNodesDetailDto.NameFederalDistrictRU"]);
        b.Add(group, x => x.NameDistrictRU, L["LocationsNodesDetailDto.NameDistrictRU"]);
        b.Add(group, x => x.NameCityRU, L["LocationsNodesDetailDto.NameCityRU"]);
        b.Add(group, x => x.NameCityDistrictRU, L["LocationsNodesDetailDto.NameCityDistrictRU"]);
        b.Add(group, x => x.IsKladr, L["LocationsNodesDetailDto.IsKladr"]);
        b.Add(group, x => x.AddressLanguage, L["LocationsNodesDetailDto.AddressLanguage"]);
        b.Add(group, x => x.RegionCodeRF, L["LocationsNodesDetailDto.RegionCodeRF"]);
        b.Add(group, x => x.AddressRegionISO, L["LocationsNodesDetailDto.AddressRegionISO"]);
        b.Add(group, x => x.FIASCodeCity, L["LocationsNodesDetailDto.FIASCodeCity"]);
        b.Add(group, x => x.FIASStreet, L["LocationsNodesDetailDto.FIASStreet"]);
        b.Add(group, x => x.FIASHouse, L["LocationsNodesDetailDto.FIASHouse"]);
        b.Add(group, x => x.OKTMOCode, L["LocationsNodesDetailDto.OKTMOCode"]);
        b.Add(group, x => x.FIASCodeAddress, L["LocationsNodesDetailDto.FIASCodeAddress"]);
        b.Add(group, x => x.IsDadata, L["LocationsNodesDetailDto.IsDadata"]);
        b.Add(group, x => x.CannotDeliver, L["LocationsNodesDetailDto.CannotDeliver"]);
        b.Add(group, x => x.BorderCountryISO2, L["LocationsNodesDetailDto.BorderCountryISO2"]);
        b.Add(group, x => x.BorderCountryISO3, L["LocationsNodesDetailDto.BorderCountryISO3"]);
        b.Add(group, x => x.BorderCountryCodeDPO, L["LocationsNodesDetailDto.BorderCountryCodeDPO"]);
        b.Add(group, x => x.BorderNameCountryRU, L["LocationsNodesDetailDto.BorderNameCountryRU"]);
        b.Add(group, x => x.BorderNameCountryEN, L["LocationsNodesDetailDto.BorderNameCountryEN"]);
        b.Add(group, x => x.StatusNSI, L["LocationsNodesDetailDto.StatusNSI"]);
        b.Add(group, x => x.IsPlanning, L["LocationsNodesDetailDto.IsPlanning"]);
        b.Add(group, x => x.IsPlan, L["LocationsNodesDetailDto.IsPlan"]);
        b.Add(group, x => x.Level4CityRU, L["LocationsNodesDetailDto.Level4CityRU"]);
        b.Add(group, x => x.Level4CityEN, L["LocationsNodesDetailDto.Level4CityEN"]);
        b.Add(group, x => x.Level4CityFias, L["LocationsNodesDetailDto.Level4CityFias"]);

        return b.Build();
    }
}
