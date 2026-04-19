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

        AddLegTypeSettings(isRu, results);
        AddTransportKindSettings(results, isRu);

        AddNodeFromSettings(results, isRu);
        AddRegionFromSettings(results, isRu);

        AddProxyNodeSettings(results, isRu);
        AddProxyRegionSettings(results, isRu);

        AddNodeToSettings(results, isRu);
        AddRegionToSettings(results, isRu);

        AddLeadtimeSettings(results);

        AddCreationChangeDateSettings(results);
        AddIsArchiveSettings(results);
        
        return new DetailSettingsCollection<LegDetailDto>(results);
    }


    private void AddCodeSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.Add(new DetailSetting<LegDetailDto>
            {
                Name = nameof(LegDetailDto.Code),
                Header = L["LegDto.Code"],
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
            Header = L["LegDto.CanBeUsed"],
            DisplaySelector = dto => dto.CanBeUsed
                ? L["LegDto.Yes"]
                : L["LegDto.No"],
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
                    Header = L["LegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeRu).GetDisplayName(dto.ShipmentTypeIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ShipmentTypeIdEn),
                    Header = L["LegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeEn).GetDisplayName(dto.ShipmentTypeIdEn.ToString()) ?? string.Empty,
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
                    Header = L["LegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TransportKindIdEn),
                    Header = L["LegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    VisibleSelector = _ => true
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
                Header = L["LegDto.NodeFromCode"],
                DisplaySelector = dto => dto.NodeFromCode,
                VisibleSelector = _ => true
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeFromNameRu),
                    Header = L["LegDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeFromNameEn),
                    Header = L["LegDto.NodeFromName"],
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
                Header = L["LegDto.RegionFromCode"],
                DisplaySelector = dto => dto.RegionFromCode,
                VisibleSelector = _ => true
            }
        );

        // RegionFromName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionFromNameRu),
                    Header = L["LegDto.RegionFromName"],
                    DisplaySelector = dto => dto.RegionFromNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionFromNameEn),
                    Header = L["LegDto.RegionFromName"],
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
                Header = L["LegDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyNodeNameRu),
                    Header = L["LegDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyNodeNameEn),
                    Header = L["LegDto.ProxyNodeName"],
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
                Header = L["LegDto.ProxyRegionCode"],
                DisplaySelector = dto => dto.ProxyRegionCode!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        );

        // ProxyRegionName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyRegionNameRu),
                    Header = L["LegDto.ProxyRegionName"],
                    DisplaySelector = dto => dto.ProxyRegionNameRu!,
                    VisibleSelector = dto => dto.ProxyNodeCode is not null
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.ProxyRegionNameEn),
                    Header = L["LegDto.ProxyRegionName"],
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
                Header = L["LegDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                VisibleSelector = _ => true
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeToNameRu),
                    Header = L["LegDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.NodeToNameEn),
                    Header = L["LegDto.NodeToName"],
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
                Header = L["LegDto.RegionToCode"],
                DisplaySelector = dto => dto.RegionToCode,
                VisibleSelector = _ => true
            }
        );


        // RegionToName
        if (isRu)
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionToNameRu),
                    Header = L["LegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameRu,
                    VisibleSelector = _ => true
                }
            );
        else
            results.Add(new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.RegionToNameEn),
                    Header = L["LegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameEn,
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
                    Header = L["LegDto.SearchTime"],
                    DisplaySelector = dto => dto.SearchTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.LoadTimeT),
                    Header = L["LegDto.LoadTime"],
                    DisplaySelector = dto => dto.LoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.DaysWaitingT),
                    Header = L["LegDto.DaysWaiting"],
                    DisplaySelector = dto => dto.DaysWaitingT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TravelTimeT),
                    Header = L["LegDto.TravelTime"],
                    DisplaySelector = dto => dto.TravelTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.UnLoadTimeT),
                    Header = L["LegDto.UnLoadTime"],
                    DisplaySelector = dto => dto.UnLoadTimeT,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.TransportationTimeT),
                    Header = L["LegDto.TransportationTime"],
                    DisplaySelector = dto => dto.TransportationTimeT,
                    VisibleSelector = _ => true
                }
            ]
        );
    }

    private void AddCreationChangeDateSettings(List<DetailSetting<LegDetailDto>> results)
    {
        results.AddRange(
            [
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.CreationDate),
                    Header = L["LegDto.CreationDate"],
                    DisplaySelector = dto => dto.CreationDate,
                    VisibleSelector = _ => true
                },
                new DetailSetting<LegDetailDto>
                {
                    Name = nameof(LegDetailDto.LastChangeDate),
                    Header = L["LegDto.LastChangeDate"],
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
                Header = L["LegDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["LegDto.Archive"]
                    : L["LegDto.Active"],
                VisibleSelector = _ => true
            }
        );
    }
}