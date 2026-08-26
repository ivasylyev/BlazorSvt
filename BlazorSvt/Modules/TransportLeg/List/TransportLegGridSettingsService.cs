using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.List;

// ReSharper disable once InconsistentNaming
public class TransportLegGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.TransportLeg> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<TransportLegGridSettingsService> logger)
    : BaseGridSettingsService<TransportLegDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportLegGridColumnSettings";

    protected override List<GridColumnSetting<TransportLegDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<TransportLegDto>(platform);

        b.Add(x => x.Code, L["TransportLegDto.Code"]);
        b.AddYesNo(x => x.CanBeUsed, L["TransportLegDto.CanBeUsed"]);
        b.Add(x => x.ShipmentTypeCodeT, L["TransportLegDto.ShipmentTypeCodeT"]);
        b.Add(x => x.TransportKindCode, L["TransportLegDto.TransportKindCode"], visible: false);
        b.AddEnum(isRu, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportLegDto.TransportKindName"]);
        b.Add(x => x.NodeFromCode, L["TransportLegDto.NodeFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["TransportLegDto.NodeFromName"]);
        b.Add(x => x.RegionFromCode, L["TransportLegDto.RegionFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["TransportLegDto.RegionFromName"]);
        b.Add(x => x.ProxyNodeCode, L["TransportLegDto.ProxyNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportLegDto.ProxyNodeName"]);
        b.Add(x => x.ProxyRegionCode, L["TransportLegDto.ProxyRegionCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyRegionNameRu, x => x.ProxyRegionNameEn, L["TransportLegDto.ProxyRegionName"]);
        b.Add(x => x.NodeToCode, L["TransportLegDto.NodeToCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeToNameRu, x => x.NodeToNameEn, L["TransportLegDto.NodeToName"]);
        b.Add(x => x.RegionToCode, L["TransportLegDto.RegionToCode"], visible: false);
        b.AddLocalized(isRu, x => x.RegionToNameRu, x => x.RegionToNameEn, L["TransportLegDto.RegionToName"]);
        // Leadtime columns: rarely needed; default-hidden to keep the grid narrow.
        b.Add(x => x.SearchTimeT, L["TransportLegDto.SearchTime"], visible: false, filterable: false);
        b.Add(x => x.LoadTimeT, L["TransportLegDto.LoadTime"], visible: false, filterable: false);
        b.Add(x => x.DaysWaitingT, L["TransportLegDto.DaysWaiting"], visible: false, filterable: false);
        b.Add(x => x.TravelTimeT, L["TransportLegDto.TravelTime"], visible: false, filterable: false);
        b.Add(x => x.UnLoadTimeT, L["TransportLegDto.UnLoadTime"], visible: false, filterable: false);
        b.Add(x => x.TransportationTimeT, L["TransportLegDto.TransportationTime"], filterable: false);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["TransportLegDto.CreationDate"],
            L["TransportLegDto.LastChangeDate"],
            L["TransportLegDto.IsArchive"]);

        return b.Build();
    }
}
