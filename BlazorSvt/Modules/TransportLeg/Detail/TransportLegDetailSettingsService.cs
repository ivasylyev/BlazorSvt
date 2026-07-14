using BlazorSvt.Modules.TransportLeg.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.Detail;

// ReSharper disable once InconsistentNaming
public class TransportLegDetailSettingsService(
    IStringLocalizer<Resources.TransportLeg> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<TransportLegDetailDto>
{
    public DetailSettingsCollection<TransportLegDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportLegDetailDto>(platform);
        var g1 = L["TransportLegDetailDto.Group.1.Parameters"];
        var g2 = L["TransportLegDetailDto.Group.2.FromTo"];
        var g3 = L["TransportLegDetailDto.Group.3.Transport"];
        var g4 = L["TransportLegDetailDto.Group.4.Leadtimes"];
        var g41 = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"];
        var g42 = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"];
        Func<TransportLegDetailDto, bool> hasProxy = dto => dto.ProxyNodeCode is not null;

        b.Add(g1, x => x.Code, L["TransportLegDetailDto.Code"]);
        b.AddYesNo(g1, x => x.CanBeUsed, L["TransportLegDetailDto.CanBeUsed"]);
        b.Add(g1, x => x.CreationDate, L["TransportLegDetailDto.CreationDate"], hasMargin: true);
        b.Add(g1, x => x.LastChangeDate, L["TransportLegDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(g1, x => x.IsArchive, L["TransportLegDetailDto.IsArchive"], hasMargin: true);

        b.Add(g2, x => x.NodeFromCode, L["TransportLegDetailDto.NodeFromCode"]);
        b.AddLocalized(isRu, g2, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["TransportLegDetailDto.NodeFromName"]);
        b.Add(g2, x => x.RegionFromCode, L["TransportLegDetailDto.RegionFromCode"]);
        b.AddLocalized(isRu, g2, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["TransportLegDetailDto.RegionFromName"]);
        b.Add(g2, x => x.ProxyNodeCode, L["TransportLegDetailDto.ProxyNodeCode"], visible: hasProxy, hasMargin: true);
        b.AddLocalized(isRu, g2, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportLegDetailDto.ProxyNodeName"], visible: hasProxy);
        b.Add(g2, x => x.ProxyRegionCode, L["TransportLegDetailDto.ProxyRegionCode"], visible: hasProxy);
        b.AddLocalized(isRu, g2, x => x.ProxyRegionNameRu, x => x.ProxyRegionNameEn, L["TransportLegDetailDto.ProxyRegionName"], visible: hasProxy);
        b.Add(g2, x => x.NodeToCode, L["TransportLegDetailDto.NodeToCode"], hasMargin: true);
        b.AddLocalized(isRu, g2, x => x.NodeToNameRu, x => x.NodeToNameEn, L["TransportLegDetailDto.NodeToName"]);
        b.Add(g2, x => x.RegionToCode, L["TransportLegDetailDto.RegionToCode"]);
        b.AddLocalized(isRu, g2, x => x.RegionToNameRu, x => x.RegionToNameEn, L["TransportLegDetailDto.RegionToName"]);

        b.AddEnum(isRu, g3, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportLegDetailDto.TransportKindName"]);
        b.AddEnum(isRu, g4, x => x.ShipmentTypeIdRu, x => x.ShipmentTypeIdEn, L["TransportLegDetailDto.ShipmentTypeName"]);

        b.Add(g4, x => x.SearchTimeT, L["TransportLegDetailDto.SearchTime"], hasMargin: true);
        b.Add(g4, x => x.LoadTimeT, L["TransportLegDetailDto.LoadTime"]);
        b.Add(g4, x => x.DaysWaitingT, L["TransportLegDetailDto.DaysWaiting"]);
        b.Add(g4, x => x.TravelTimeT, L["TransportLegDetailDto.TravelTime"]);
        b.Add(g4, x => x.UnLoadTimeT, L["TransportLegDetailDto.UnLoadTime"]);
        b.Add(g4, x => x.TransportationTimeT, L["TransportLegDetailDto.TransportationTime"]);
        b.Add(g4, x => x.Distance, L["TransportLegDetailDto.Distance"], hasMargin: true);

        b.Add(g41, x => x.Leg1_SearchTime, L["TransportLegDetailDto.Leg1_SearchTime"], visible: hasProxy);
        b.Add(g41, x => x.Leg1_LoadTime, L["TransportLegDetailDto.Leg1_LoadTime"], visible: hasProxy);
        b.Add(g41, x => x.Leg1_DaysWaiting, L["TransportLegDetailDto.Leg1_DaysWaiting"], visible: hasProxy);
        b.Add(g41, x => x.Leg1_TravelTime, L["TransportLegDetailDto.Leg1_TravelTime"], visible: hasProxy);
        b.Add(g41, x => x.Leg1_TransportationTime, L["TransportLegDetailDto.Leg1_TransportationTime"], visible: hasProxy);
        b.Add(g41, x => x.Leg1_Distance, L["TransportLegDetailDto.Leg1_Distance"], visible: hasProxy, hasMargin: true);

        b.Add(g42, x => x.Leg2_UpLoadTime, L["TransportLegDetailDto.Leg2_UpLoadTime"], visible: hasProxy);
        b.Add(g42, x => x.Leg2_DaysWaiting, L["TransportLegDetailDto.Leg2_DaysWaiting"], visible: hasProxy);
        b.Add(g42, x => x.Leg2_TravelTime, L["TransportLegDetailDto.Leg2_TravelTime"], visible: hasProxy);
        b.Add(g42, x => x.Leg2_TransportationTime, L["TransportLegDetailDto.Leg2_TransportationTime"], visible: hasProxy);
        b.Add(g42, x => x.Leg2_Distance, L["TransportLegDetailDto.Leg2_Distance"], visible: hasProxy, hasMargin: true);

        return b.Build();
    }
}
