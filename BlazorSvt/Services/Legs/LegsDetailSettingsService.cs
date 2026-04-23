using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Dto.IdsEnum;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Legs;

// ReSharper disable once InconsistentNaming
public class LegsDetailSettingsService(IStringLocalizer<Svt> L, ILogger<LegsDetailSettingsService> logger) : IDetailSettingsService<LegDetailDto>
{
    public DetailSettingsCollection<LegDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<DetailSetting<LegDetailDto>>();

        AddCodeSettings(results);
        AddCanBeUsedSettings(results);
        AddCreationChangeDateSettings(results);
        AddIsArchiveSettings(results);

        AddNodeFromSettings(results, isRu);
        AddRegionFromSettings(results, isRu);

        AddProxyNodeSettings(results, isRu);
        AddProxyRegionSettings(results, isRu);

        AddNodeToSettings(results, isRu);
        AddRegionToSettings(results, isRu);

        AddTransportKindSettings(results, isRu);

        AddLegTypeSettings(isRu, results);
        AddLeadtimeSettings(results);

        AddLeadtimeLeg1Settings(results);

        AddLeadtimeLeg2Settings(results);

        return new DetailSettingsCollection<LegDetailDto>(results);
    }


    private void AddCodeSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.Add(new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.Code),
                Header = L["LegDetailDto.Code"],
                GroupHeader = L["LegDetailDto.Group1Parameters"], 
                DisplaySelector = dto => dto.Code,
                VisibleSelector = _ => true
            }
        );
    }

    private void AddCanBeUsedSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.Add(new DetailSetting<LegDetailDto>
        {
            Name = nameof(LegDetailDto.CanBeUsed),
            Header = L["LegDetailDto.CanBeUsed"],
            GroupHeader = L["LegDetailDto.Group1Parameters"],
            DisplaySelector = dto => dto.CanBeUsed
                ? L["LegDetailDto.Yes"]
                : L["LegDetailDto.No"],
            VisibleSelector = _ => true
        }
        );
    }


    private void AddCreationChangeDateSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.CreationDate),
                    Header = L["LegDetailDto.CreationDate"],
                    GroupHeader = L["LegDetailDto.Group1Parameters"],
                    DisplaySelector = dto => dto.CreationDate,
                    VisibleSelector = _ => true,
                    HasMargin = true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.LastChangeDate),
                    Header = L["LegDetailDto.LastChangeDate"],
                    GroupHeader = L["LegDetailDto.Group1Parameters"],
                    DisplaySelector = dto => dto.LastChangeDate,
                    VisibleSelector = _ => true
                }
            ]
        );
    }


    private void AddIsArchiveSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.IsArchive),
                Header = L["LegDetailDto.IsArchive"],
                GroupHeader = L["LegDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["LegDetailDto.Archive"]
                    : L["LegDetailDto.Active"],
                VisibleSelector = _ => true,
                HasMargin = true
            }
        );
    }
    private void AddNodeFromSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.NodeFromCode),
                Header = L["LegDetailDto.NodeFromCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeFromCode,
                VisibleSelector = _ => true
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeFromNameRu),
                    Header = L["LegDetailDto.NodeFromName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeFromNameEn),
                    Header = L["LegDetailDto.NodeFromName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddRegionFromSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        // RegionFromCode
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.RegionFromCode),
                Header = L["LegDetailDto.RegionFromCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionFromCode,
                VisibleSelector = _ => true,
            }
        );

        // RegionFromName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionFromNameRu),
                    Header = L["LegDetailDto.RegionFromName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.RegionFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionFromNameEn),
                    Header = L["LegDetailDto.RegionFromName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.RegionFromNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddProxyNodeSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.ProxyNodeCode),
                Header = L["LegDetailDto.ProxyNodeCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyNodeNameRu),
                    Header = L["LegDetailDto.ProxyNodeName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyNodeNameEn),
                    Header = L["LegDetailDto.ProxyNodeName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
    }

    private void AddProxyRegionSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.ProxyRegionCode),
                Header = L["LegDetailDto.ProxyRegionCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyRegionCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
            }
        );

        // ProxyRegionName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyRegionNameRu),
                    Header = L["LegDetailDto.ProxyRegionName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.ProxyRegionNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyRegionNameEn),
                    Header = L["LegDetailDto.ProxyRegionName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.ProxyRegionNameEn!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
    }

    private void AddNodeToSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.NodeToCode),
                Header = L["LegDetailDto.NodeToCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeToCode,
                VisibleSelector = _ => true,
                HasMargin = true
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeToNameRu),
                    Header = L["LegDetailDto.NodeToName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeToNameEn),
                    Header = L["LegDetailDto.NodeToName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddRegionToSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.RegionToCode),
                Header = L["LegDetailDto.RegionToCode"],
                GroupHeader = L["LegDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionToCode,
                VisibleSelector = _ => true
            }
        );


        // RegionToName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionToNameRu),
                    Header = L["LegDetailDto.RegionToName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.RegionToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionToNameEn),
                    Header = L["LegDetailDto.RegionToName"],
                    GroupHeader = L["LegDetailDto.Group2FromTo"],
                    DisplaySelector = dto => dto.RegionToNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddTransportKindSettings(List<DetailSetting<LegDetailDto>> results, bool isRu)
    {
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TransportKindIdRu),
                    Header = L["LegDetailDto.TransportKindName"],
                    GroupHeader = L["LegDetailDto.Group3Transport"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
            }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TransportKindIdEn),
                    Header = L["LegDetailDto.TransportKindName"],
                    GroupHeader = L["LegDetailDto.Group3Transport"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddLegTypeSettings(bool isRu, List<DetailSetting<LegDetailDto>> results)
    {
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ShipmentTypeIdRu),
                    Header = L["LegDetailDto.ShipmentTypeName"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => typeof(ShipmentTypeRu).GetDisplayName(dto.ShipmentTypeIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ShipmentTypeIdEn),
                    Header = L["LegDetailDto.ShipmentTypeName"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => typeof(ShipmentTypeEn).GetDisplayName(dto.ShipmentTypeIdEn.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
    }

   
    private void AddLeadtimeSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.SearchTimeT),
                    Header = L["LegDetailDto.SearchTime"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.SearchTimeT,
                    VisibleSelector = _ => true,
                    HasMargin = true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.LoadTimeT),
                    Header = L["LegDetailDto.LoadTime"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.LoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.DaysWaitingT),
                    Header = L["LegDetailDto.DaysWaiting"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.DaysWaitingT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TravelTimeT),
                    Header = L["LegDetailDto.TravelTime"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.TravelTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.UnLoadTimeT),
                    Header = L["LegDetailDto.UnLoadTime"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.UnLoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TransportationTimeT),
                    Header = L["LegDetailDto.TransportationTime"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.TransportationTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Distance),
                    Header = L["LegDetailDto.Distance"],
                    GroupHeader = L["LegDetailDto.Group4Leadtimes"],
                    DisplaySelector = dto => dto.Distance,
                    VisibleSelector = _ => true,
                    HasMargin = true
                }

            ]
        );
    }

    private void AddLeadtimeLeg1Settings(List<DetailSetting<LegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_SearchTime),
                    Header = L["LegDetailDto.Leg1_SearchTime"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_SearchTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_LoadTime),
                    Header = L["LegDetailDto.Leg1_LoadTime"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_LoadTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_DaysWaiting),
                    Header = L["LegDetailDto.Leg1_DaysWaiting"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_DaysWaiting,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_TravelTime),
                    Header = L["LegDetailDto.Leg1_TravelTime"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_TravelTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_TransportationTime),
                    Header = L["LegDetailDto.Leg1_TransportationTime"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_TransportationTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg1_Distance),
                    Header = L["LegDetailDto.Leg1_Distance"],
                    GroupHeader = L["LegDetailDto.Group41LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_Distance,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null,
                    HasMargin = true
                }
            ]
        );
    }

    private void AddLeadtimeLeg2Settings(List<DetailSetting<LegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg2_UpLoadTime),
                    Header = L["LegDetailDto.Leg2_UpLoadTime"],
                    GroupHeader = L["LegDetailDto.Group42LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_UpLoadTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg2_DaysWaiting),
                    Header = L["LegDetailDto.Leg2_DaysWaiting"],
                    GroupHeader = L["LegDetailDto.Group42LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_DaysWaiting,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg2_TravelTime),
                    Header = L["LegDetailDto.Leg2_TravelTime"],
                    GroupHeader = L["LegDetailDto.Group42LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_TravelTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg2_TransportationTime),
                    Header = L["LegDetailDto.Leg2_TransportationTime"],
                    GroupHeader = L["LegDetailDto.Group42LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_TransportationTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.Leg2_Distance),
                    Header = L["LegDetailDto.Leg2_Distance"],
                    GroupHeader = L["LegDetailDto.Group42LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_Distance,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null,
                    HasMargin = true
                }
            ]
        );
    }

}