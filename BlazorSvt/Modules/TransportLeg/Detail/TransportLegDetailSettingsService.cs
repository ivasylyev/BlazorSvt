using BlazorBootstrap;
using BlazorSvt.Modules.TransportLeg.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.Detail;

// ReSharper disable once InconsistentNaming
public class TransportLegDetailSettingsService(IStringLocalizer<Svt> L, ILogger<TransportLegDetailSettingsService> logger) : IDetailSettingsService<TransportLegDetailDto>
{
    public DetailSettingsCollection<TransportLegDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<DetailSetting<TransportLegDetailDto>>();

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

        return new DetailSettingsCollection<TransportLegDetailDto>(results);
    }


    private void AddCodeSettings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.Code),
                Header = L["TransportLegDetailDto.Code"],
                GroupHeader = L["TransportLegDetailDto.Group.1.Parameters"], 
                DisplaySelector = dto => dto.Code,
                VisibleSelector = _ => true
            }
        );
    }

    private void AddCanBeUsedSettings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportLegDetailDto>
        {
            Name = nameof(TransportLegDetailDto.CanBeUsed),
            Header = L["TransportLegDetailDto.CanBeUsed"],
            GroupHeader = L["TransportLegDetailDto.Group.1.Parameters"],
            DisplaySelector = dto => dto.CanBeUsed
                ? L["TransportLegDetailDto.Yes"]
                : L["TransportLegDetailDto.No"],
            VisibleSelector = _ => true
        }
        );
    }


    private void AddCreationChangeDateSettings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.CreationDate),
                    Header = L["TransportLegDetailDto.CreationDate"],
                    GroupHeader = L["TransportLegDetailDto.Group.1.Parameters"],
                    DisplaySelector = dto => dto.CreationDate,
                    VisibleSelector = _ => true,
                    HasMargin = true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.LastChangeDate),
                    Header = L["TransportLegDetailDto.LastChangeDate"],
                    GroupHeader = L["TransportLegDetailDto.Group.1.Parameters"],
                    DisplaySelector = dto => dto.LastChangeDate,
                    VisibleSelector = _ => true
                }
            ]
        );
    }


    private void AddIsArchiveSettings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.IsArchive),
                Header = L["TransportLegDetailDto.IsArchive"],
                GroupHeader = L["TransportLegDetailDto.Group.1.Parameters"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["TransportLegDetailDto.Archive"]
                    : L["TransportLegDetailDto.Active"],
                VisibleSelector = _ => true,
                HasMargin = true
            }
        );
    }
    private void AddNodeFromSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.NodeFromCode),
                Header = L["TransportLegDetailDto.NodeFromCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.NodeFromCode,
                VisibleSelector = _ => true
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.NodeFromNameRu),
                    Header = L["TransportLegDetailDto.NodeFromName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.NodeFromNameEn),
                    Header = L["TransportLegDetailDto.NodeFromName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddRegionFromSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        // RegionFromCode
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.RegionFromCode),
                Header = L["TransportLegDetailDto.RegionFromCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.RegionFromCode,
                VisibleSelector = _ => true,
            }
        );

        // RegionFromName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.RegionFromNameRu),
                    Header = L["TransportLegDetailDto.RegionFromName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.RegionFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.RegionFromNameEn),
                    Header = L["TransportLegDetailDto.RegionFromName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.RegionFromNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddProxyNodeSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.ProxyNodeCode),
                Header = L["TransportLegDetailDto.ProxyNodeCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ProxyNodeNameRu),
                    Header = L["TransportLegDetailDto.ProxyNodeName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ProxyNodeNameEn),
                    Header = L["TransportLegDetailDto.ProxyNodeName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
    }

    private void AddProxyRegionSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.ProxyRegionCode),
                Header = L["TransportLegDetailDto.ProxyRegionCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.ProxyRegionCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
            }
        );

        // ProxyRegionName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ProxyRegionNameRu),
                    Header = L["TransportLegDetailDto.ProxyRegionName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.ProxyRegionNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ProxyRegionNameEn),
                    Header = L["TransportLegDetailDto.ProxyRegionName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.ProxyRegionNameEn!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
    }

    private void AddNodeToSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.NodeToCode),
                Header = L["TransportLegDetailDto.NodeToCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.NodeToCode,
                VisibleSelector = _ => true,
                HasMargin = true
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.NodeToNameRu),
                    Header = L["TransportLegDetailDto.NodeToName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.NodeToNameEn),
                    Header = L["TransportLegDetailDto.NodeToName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddRegionToSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        results.Add(
            new DetailSetting<TransportLegDetailDto>
            {
                Name = nameof(TransportLegDetailDto.RegionToCode),
                Header = L["TransportLegDetailDto.RegionToCode"],
                GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                DisplaySelector = dto => dto.RegionToCode,
                VisibleSelector = _ => true
            }
        );


        // RegionToName
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.RegionToNameRu),
                    Header = L["TransportLegDetailDto.RegionToName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.RegionToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.RegionToNameEn),
                    Header = L["TransportLegDetailDto.RegionToName"],
                    GroupHeader = L["TransportLegDetailDto.Group.2.FromTo"],
                    DisplaySelector = dto => dto.RegionToNameEn,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddTransportKindSettings(List<DetailSetting<TransportLegDetailDto>> results, bool isRu)
    {
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.TransportKindIdRu),
                    Header = L["TransportLegDetailDto.TransportKindName"],
                    GroupHeader = L["TransportLegDetailDto.Group.3.Transport"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
            }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.TransportKindIdEn),
                    Header = L["TransportLegDetailDto.TransportKindName"],
                    GroupHeader = L["TransportLegDetailDto.Group.3.Transport"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
    }

    private void AddLegTypeSettings(bool isRu, List<DetailSetting<TransportLegDetailDto>> results)
    {
        if (isRu)
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ShipmentTypeIdRu),
                    Header = L["TransportLegDetailDto.ShipmentTypeName"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => typeof(ShipmentTypeRu).GetDisplayName(dto.ShipmentTypeIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.ShipmentTypeIdEn),
                    Header = L["TransportLegDetailDto.ShipmentTypeName"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => typeof(ShipmentTypeEn).GetDisplayName(dto.ShipmentTypeIdEn.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
    }

   
    private void AddLeadtimeSettings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.SearchTimeT),
                    Header = L["TransportLegDetailDto.SearchTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.SearchTimeT,
                    VisibleSelector = _ => true,
                    HasMargin = true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.LoadTimeT),
                    Header = L["TransportLegDetailDto.LoadTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.LoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.DaysWaitingT),
                    Header = L["TransportLegDetailDto.DaysWaiting"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.DaysWaitingT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.TravelTimeT),
                    Header = L["TransportLegDetailDto.TravelTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.TravelTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.UnLoadTimeT),
                    Header = L["TransportLegDetailDto.UnLoadTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.UnLoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.TransportationTimeT),
                    Header = L["TransportLegDetailDto.TransportationTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.TransportationTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Distance),
                    Header = L["TransportLegDetailDto.Distance"],
                    GroupHeader = L["TransportLegDetailDto.Group.4.Leadtimes"],
                    DisplaySelector = dto => dto.Distance,
                    VisibleSelector = _ => true,
                    HasMargin = true
                }

            ]
        );
    }

    private void AddLeadtimeLeg1Settings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_SearchTime),
                    Header = L["TransportLegDetailDto.Leg1_SearchTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_SearchTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_LoadTime),
                    Header = L["TransportLegDetailDto.Leg1_LoadTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_LoadTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_DaysWaiting),
                    Header = L["TransportLegDetailDto.Leg1_DaysWaiting"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_DaysWaiting,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_TravelTime),
                    Header = L["TransportLegDetailDto.Leg1_TravelTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_TravelTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_TransportationTime),
                    Header = L["TransportLegDetailDto.Leg1_TransportationTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_TransportationTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg1_Distance),
                    Header = L["TransportLegDetailDto.Leg1_Distance"],
                    GroupHeader = L["TransportLegDetailDto.Group.41.LeadtimesLeg1"],
                    DisplaySelector = dto => dto.Leg1_Distance,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null,
                    HasMargin = true
                }
            ]
        );
    }

    private void AddLeadtimeLeg2Settings(List<DetailSetting<TransportLegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg2_UpLoadTime),
                    Header = L["TransportLegDetailDto.Leg2_UpLoadTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_UpLoadTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg2_DaysWaiting),
                    Header = L["TransportLegDetailDto.Leg2_DaysWaiting"],
                    GroupHeader = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_DaysWaiting,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg2_TravelTime),
                    Header = L["TransportLegDetailDto.Leg2_TravelTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_TravelTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg2_TransportationTime),
                    Header = L["TransportLegDetailDto.Leg2_TransportationTime"],
                    GroupHeader = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_TransportationTime,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                },
                new DetailSetting<TransportLegDetailDto>
                {
                    Name = nameof(TransportLegDetailDto.Leg2_Distance),
                    Header = L["TransportLegDetailDto.Leg2_Distance"],
                    GroupHeader = L["TransportLegDetailDto.Group.42.LeadtimesLeg2"],
                    DisplaySelector = dto => dto.Leg2_Distance,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null,
                    HasMargin = true
                }
            ]
        );
    }

}
