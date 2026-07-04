using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.Detail;

public class LocationsNodesDetailSettingsService(IStringLocalizer<Resources.LocationsNodes> L, ILogger<LocationsNodesDetailSettingsService> logger)
    : IDetailSettingsService<LocationsNodesDetailDto>
{
    public DetailSettingsCollection<LocationsNodesDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var results = new List<DetailSetting<LocationsNodesDetailDto>>();
        var group = L["LocationsNodesDetailDto.Group.0.Default"];

        AddSystemSettings(results, group);
        AddField(results, group, nameof(LocationsNodesDetailDto.NameZD));
        AddField(results, group, nameof(LocationsNodesDetailDto.Seaport));
        AddField(results, group, nameof(LocationsNodesDetailDto.AutoNode));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASRegion));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASDistrict));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASCity));
        AddField(results, group, nameof(LocationsNodesDetailDto.Terminal));
        AddField(results, group, nameof(LocationsNodesDetailDto.OpenStreetMap));
        AddField(results, group, nameof(LocationsNodesDetailDto.Virtual));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsFactory));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsPortShip));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsPortStore));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsStore));
        AddField(results, group, nameof(LocationsNodesDetailDto.CoordinateW));
        AddField(results, group, nameof(LocationsNodesDetailDto.CoordinateL));
        AddField(results, group, nameof(LocationsNodesDetailDto.Code_NSI));
        AddField(results, group, nameof(LocationsNodesDetailDto.Status));
        AddField(results, group, nameof(LocationsNodesDetailDto.PointTube));
        AddField(results, group, nameof(LocationsNodesDetailDto.Street));
        AddField(results, group, nameof(LocationsNodesDetailDto.House));
        AddField(results, group, nameof(LocationsNodesDetailDto.OfficeApart));
        AddField(results, group, nameof(LocationsNodesDetailDto.ZDCodeN));
        AddField(results, group, nameof(LocationsNodesDetailDto.NodesCode));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.NameRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.NameEn));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeIdRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeIdEn));
        AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeCode));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeNameRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeNameEn));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeIdRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeIdEn));
        AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeCode));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeNameRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeNameEn));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.RegionIdRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.RegionIdEn));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionCode));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.RegionNameRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.RegionNameEn));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.CountryIdRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.CountryIdEn));
        AddField(results, group, nameof(LocationsNodesDetailDto.CountryCode));
        if (isRu) AddField(results, group, nameof(LocationsNodesDetailDto.CountryNameRu));
        else AddField(results, group, nameof(LocationsNodesDetailDto.CountryNameEn));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.City));
        AddField(results, group, nameof(LocationsNodesDetailDto.FullAddress));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderCrossing));
        AddField(results, group, nameof(LocationsNodesDetailDto.LocationTypeCodeNSI));
        AddField(results, group, nameof(LocationsNodesDetailDto.TypeNodeCodeNSI));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionNSIEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionCodeNSI));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionCodeDPO));
        AddField(results, group, nameof(LocationsNodesDetailDto.CountryRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameCountryEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.CountryISO2));
        AddField(results, group, nameof(LocationsNodesDetailDto.CountryISO3));
        AddField(results, group, nameof(LocationsNodesDetailDto.CountryCodeDPO));
        AddField(results, group, nameof(LocationsNodesDetailDto.MarketNameRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.MarketNameEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.MarketCode));
        AddField(results, group, nameof(LocationsNodesDetailDto.MarketCodeDPO));
        AddField(results, group, nameof(LocationsNodesDetailDto.CodeZDroad));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameZDroadRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameZDroadEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameZDEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressCountryISO2));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressCountryISO3));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressCountryCodeDPO));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressNameCountryRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressNameCountryEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.Pobox));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameFederalDistrictRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameDistrictRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameCityRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.NameCityDistrictRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsKladr));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressLanguage));
        AddField(results, group, nameof(LocationsNodesDetailDto.RegionCodeRF));
        AddField(results, group, nameof(LocationsNodesDetailDto.AddressRegionISO));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASCodeCity));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASStreet));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASHouse));
        AddField(results, group, nameof(LocationsNodesDetailDto.OKTMOCode));
        AddField(results, group, nameof(LocationsNodesDetailDto.FIASCodeAddress));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsDadata));
        AddField(results, group, nameof(LocationsNodesDetailDto.CannotDeliver));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderCountryISO2));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderCountryISO3));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderCountryCodeDPO));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderNameCountryRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.BorderNameCountryEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.StatusNSI));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsPlanning));
        AddField(results, group, nameof(LocationsNodesDetailDto.IsPlan));
        AddField(results, group, nameof(LocationsNodesDetailDto.Level4CityRU));
        AddField(results, group, nameof(LocationsNodesDetailDto.Level4CityEN));
        AddField(results, group, nameof(LocationsNodesDetailDto.Level4CityFias));

        return new DetailSettingsCollection<LocationsNodesDetailDto>(results);
    }

    private void AddSystemSettings(List<DetailSetting<LocationsNodesDetailDto>> results, string group)
    {
        AddField(results, group, nameof(LocationsNodesDetailDto.Code));
        AddField(results, group, nameof(LocationsNodesDetailDto.CreationDate));
        AddField(results, group, nameof(LocationsNodesDetailDto.LastChangeDate));
        AddBoolField(results, group, nameof(LocationsNodesDetailDto.IsArchive),
            dto => dto.IsArchive ? L["LocationsNodesDetailDto.Archive"] : L["LocationsNodesDetailDto.Active"]);
    }

    private void AddField(List<DetailSetting<LocationsNodesDetailDto>> results, string group, string name)
    {
        var resxKey = name switch
        {
            nameof(LocationsNodesDetailDto.LocationTypeIdRu) => "LocationsNodesDetailDto.LocationTypeName",
            nameof(LocationsNodesDetailDto.LocationTypeIdEn) => "LocationsNodesDetailDto.LocationTypeName",
            nameof(LocationsNodesDetailDto.TypeNodeIdRu) => "LocationsNodesDetailDto.TypeNodeName",
            nameof(LocationsNodesDetailDto.TypeNodeIdEn) => "LocationsNodesDetailDto.TypeNodeName",
            nameof(LocationsNodesDetailDto.RegionIdRu) => "LocationsNodesDetailDto.RegionName",
            nameof(LocationsNodesDetailDto.RegionIdEn) => "LocationsNodesDetailDto.RegionName",
            nameof(LocationsNodesDetailDto.CountryIdRu) => "LocationsNodesDetailDto.CountryName",
            nameof(LocationsNodesDetailDto.CountryIdEn) => "LocationsNodesDetailDto.CountryName",
            nameof(LocationsNodesDetailDto.LocationTypeNameRu) => "LocationsNodesDetailDto.LocationTypeName",
            nameof(LocationsNodesDetailDto.LocationTypeNameEn) => "LocationsNodesDetailDto.LocationTypeName",
            nameof(LocationsNodesDetailDto.TypeNodeNameRu) => "LocationsNodesDetailDto.TypeNodeName",
            nameof(LocationsNodesDetailDto.TypeNodeNameEn) => "LocationsNodesDetailDto.TypeNodeName",
            nameof(LocationsNodesDetailDto.RegionNameRu) => "LocationsNodesDetailDto.RegionName",
            nameof(LocationsNodesDetailDto.RegionNameEn) => "LocationsNodesDetailDto.RegionName",
            nameof(LocationsNodesDetailDto.CountryNameRu) => "LocationsNodesDetailDto.CountryName",
            nameof(LocationsNodesDetailDto.CountryNameEn) => "LocationsNodesDetailDto.CountryName",
            _ => $"LocationsNodesDetailDto.{name}"
        };
        results.Add(new DetailSetting<LocationsNodesDetailDto>
        {
            Name = name,
            Header = L[resxKey],
            GroupHeader = group,
            DisplaySelector = dto => GetValue(dto, name),
            VisibleSelector = _ => true
        });
    }

    private void AddBoolField(
        List<DetailSetting<LocationsNodesDetailDto>> results,
        string group,
        string name,
        Func<LocationsNodesDetailDto, string> display)
    {
        results.Add(new DetailSetting<LocationsNodesDetailDto>
        {
            Name = name,
            Header = L[$"LocationsNodesDetailDto.{name}"],
            GroupHeader = group,
            DisplaySelector = dto => display(dto),
            VisibleSelector = _ => true
        });
    }

    private static object GetValue(LocationsNodesDetailDto dto, string name) =>
        typeof(LocationsNodesDetailDto).GetProperty(name)?.GetValue(dto) ?? string.Empty;
}
