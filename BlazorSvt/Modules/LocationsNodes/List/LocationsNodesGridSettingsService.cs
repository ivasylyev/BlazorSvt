using Blazored.LocalStorage;
using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.List;

public class LocationsNodesGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.LocationsNodes> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<LocationsNodesGridSettingsService> logger)
    : BaseGridSettingsService<LocationsNodesDto>(localStorage, logger)
{
    protected override string StorageKey => "LocationsNodesGridColumnSettings";

    protected override List<GridColumnSetting<LocationsNodesDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<LocationsNodesDto>(platform);

        b.Add(x => x.Code, L["LocationsNodesDto.Code"]);
        b.AddLocalized(isRu, x => x.NameRu, x => x.NameEn, L["LocationsNodesDto.Name"]);
        b.AddEnum(isRu, x => x.LocationTypeIdRu, x => x.LocationTypeIdEn, L["LocationsNodesDto.LocationTypeName"]);
        b.AddEnum(isRu, x => x.TypeNodeIdRu, x => x.TypeNodeIdEn, L["LocationsNodesDto.TypeNodeName"]);
        b.Add(x => x.RegionCode, L["LocationsNodesDto.RegionCode"], visible: false);
        b.AddLocalized(isRu, x => x.RegionNameRu, x => x.RegionNameEn, L["LocationsNodesDto.RegionName"]);
        b.AddLocalized(isRu, x => x.CountryNameRu, x => x.CountryNameEn, L["LocationsNodesDto.CountryName"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["LocationsNodesDto.CreationDate"],
            L["LocationsNodesDto.LastChangeDate"],
            L["LocationsNodesDto.IsArchive"]);

        return b.Build();
    }
}
