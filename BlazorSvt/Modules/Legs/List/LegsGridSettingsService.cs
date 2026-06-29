using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Modules.Legs.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Host.Resources;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Legs.List;

// ReSharper disable once InconsistentNaming
public class LegsGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Svt> L, ILogger<LegsGridSettingsService> logger)
    : BaseGridSettingsService<LegDto>(localStorage, logger)
{
    protected override string StorageKey => "LegsGridColumnSettings";


    protected override List<GridColumnSetting<LegDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<GridColumnSetting<LegDto>>();
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

        return results;
    }


    private void AddCodeSettings(List<GridColumnSetting<LegDto>> results)
    {
        results.Add(new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.Code),
                Header = L["LegDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddCanBeUsedSettings(List<GridColumnSetting<LegDto>> results)
    {
        results.Add(new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.CanBeUsed),
                Header = L["LegDto.CanBeUsed"],
                DisplaySelector = dto => dto.CanBeUsed
                    ? L["LegDto.Yes"]
                    : L["LegDto.No"],
                SortSelector = dto => dto.CanBeUsed,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddLegTypeSettings(bool isRu, List<GridColumnSetting<LegDto>> results)
    {
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ShipmentTypeIdRu),
                    Header = L["LegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeRu).GetDisplayName(dto.ShipmentTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.ShipmentTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ShipmentTypeIdEn),
                    Header = L["LegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeEn).GetDisplayName(dto.ShipmentTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.ShipmentTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportKindSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.TransportKindCode),
                Header = L["LegDto.TransportKindCode"],
                DisplaySelector = dto => dto.TransportKindCode,
                SortSelector = dto => dto.TransportKindCode,
                Filterable = false,
                Visible = false
            }
        );

        //TransportKind
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.TransportKindIdRu),
                    Header = L["LegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.TransportKindIdEn),
                    Header = L["LegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddNodeFromSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.NodeFromCode),
                Header = L["LegDto.NodeFromCode"],
                DisplaySelector = dto => dto.NodeFromCode,
                SortSelector = dto => dto.NodeFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.NodeFromNameRu),
                    Header = L["LegDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    SortSelector = dto => dto.NodeFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.NodeFromNameEn),
                    Header = L["LegDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    SortSelector = dto => dto.NodeFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddRegionFromSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        // RegionFromCode
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.RegionFromCode),
                Header = L["LegDto.RegionFromCode"],
                DisplaySelector = dto => dto.RegionFromCode,
                SortSelector = dto => dto.RegionFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // RegionFromName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.RegionFromNameRu),
                    Header = L["LegDto.RegionFromName"],
                    DisplaySelector = dto => dto.RegionFromNameRu,
                    SortSelector = dto => dto.RegionFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.RegionFromNameEn),
                    Header = L["LegDto.RegionFromName"],
                    DisplaySelector = dto => dto.RegionFromNameEn,
                    SortSelector = dto => dto.RegionFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyNodeSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.ProxyNodeCode),
                Header = L["LegDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                SortSelector = dto => dto.ProxyNodeCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ProxyNodeNameRu),
                    Header = L["LegDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    SortSelector = dto => dto.ProxyNodeNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ProxyNodeNameEn),
                    Header = L["LegDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    SortSelector = dto => dto.ProxyNodeNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyRegionSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.ProxyRegionCode),
                Header = L["LegDto.ProxyRegionCode"],
                DisplaySelector = dto => dto.ProxyRegionCode!,
                SortSelector = dto => dto.ProxyRegionCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyRegionName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ProxyRegionNameRu),
                    Header = L["LegDto.ProxyRegionName"],
                    DisplaySelector = dto => dto.ProxyRegionNameRu!,
                    SortSelector = dto => dto.ProxyRegionNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.ProxyRegionNameEn),
                    Header = L["LegDto.ProxyRegionName"],
                    DisplaySelector = dto => dto.ProxyRegionNameEn!,
                    SortSelector = dto => dto.ProxyRegionNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddNodeToSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.NodeToCode),
                Header = L["LegDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                SortSelector = dto => dto.NodeToCode,
                Filterable = true,
                Visible = false
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.NodeToNameRu),
                    Header = L["LegDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    SortSelector = dto => dto.NodeToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.NodeToNameEn),
                    Header = L["LegDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    SortSelector = dto => dto.NodeToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddRegionToSettings(List<GridColumnSetting<LegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.RegionToCode),
                Header = L["LegDto.RegionToCode"],
                DisplaySelector = dto => dto.RegionToCode,
                SortSelector = dto => dto.RegionToCode,
                Filterable = true,
                Visible = false
            }
        );


        // RegionToName
        if (isRu)
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.RegionToNameRu),
                    Header = L["LegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameRu,
                    SortSelector = dto => dto.RegionToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.RegionToNameEn),
                    Header = L["LegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameEn,
                    SortSelector = dto => dto.RegionToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddLeadtimeSettings(List<GridColumnSetting<LegDto>> results)
    {
        results.AddRange(
            [
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.SearchTimeT),
                    Header = L["LegDto.SearchTime"],
                    DisplaySelector = dto => dto.SearchTimeT,
                    SortSelector = dto => dto.SearchTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.LoadTimeT),
                    Header = L["LegDto.LoadTime"],
                    DisplaySelector = dto => dto.LoadTimeT,
                    SortSelector = dto => dto.LoadTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.DaysWaitingT),
                    Header = L["LegDto.DaysWaiting"],
                    DisplaySelector = dto => dto.DaysWaitingT,
                    SortSelector = dto => dto.DaysWaitingT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.TravelTimeT),
                    Header = L["LegDto.TravelTime"],
                    DisplaySelector = dto => dto.TravelTimeT,
                    SortSelector = dto => dto.TravelTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.UnLoadTimeT),
                    Header = L["LegDto.UnLoadTime"],
                    DisplaySelector = dto => dto.UnLoadTimeT,
                    SortSelector = dto => dto.UnLoadTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.TransportationTimeT),
                    Header = L["LegDto.TransportationTime"],
                    DisplaySelector = dto => dto.TransportationTimeT,
                    SortSelector = dto => dto.TransportationTimeT,
                    Filterable = false,
                    Visible = true
                }
            ]
        );
    }

    private void AddCreationChangeDateSettings(List<GridColumnSetting<LegDto>> results)
    {
        results.AddRange(
            [
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.CreationDate),
                    Header = L["LegDto.CreationDate"],
                    DisplaySelector = dto => dto.CreationDate,
                    SortSelector = dto => dto.CreationDate,
                    Filterable = true,
                    Visible = false
                },
                new GridColumnSetting<LegDto>
                {
                    Name = nameof(LegDto.LastChangeDate),
                    Header = L["LegDto.LastChangeDate"],
                    DisplaySelector = dto => dto.LastChangeDate,
                    SortSelector = dto => dto.LastChangeDate,
                    Filterable = true,
                    Visible = false
                }
            ]
        );
    }


    private void AddIsArchiveSettings(List<GridColumnSetting<LegDto>> results)
    {
        results.Add(
            new GridColumnSetting<LegDto>
            {
                Name = nameof(LegDto.IsArchive),
                Header = L["LegDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["LegDto.Archive"]
                    : L["LegDto.Active"],
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        );
    }
}