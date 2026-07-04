using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Modules.TransportLeg.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.List;

// ReSharper disable once InconsistentNaming
public class TransportLegGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Resources.TransportLeg> L, ILogger<TransportLegGridSettingsService> logger)
    : BaseGridSettingsService<TransportLegDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportLegGridColumnSettings";


    protected override List<GridColumnSetting<TransportLegDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<GridColumnSetting<TransportLegDto>>();
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


    private void AddCodeSettings(List<GridColumnSetting<TransportLegDto>> results)
    {
        results.Add(new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.Code),
                Header = L["TransportLegDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddCanBeUsedSettings(List<GridColumnSetting<TransportLegDto>> results)
    {
        results.Add(new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.CanBeUsed),
                Header = L["TransportLegDto.CanBeUsed"],
                DisplaySelector = dto => dto.CanBeUsed
                    ? L["TransportLegDto.Yes"]
                    : L["TransportLegDto.No"],
                SortSelector = dto => dto.CanBeUsed,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddLegTypeSettings(bool isRu, List<GridColumnSetting<TransportLegDto>> results)
    {
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ShipmentTypeIdRu),
                    Header = L["TransportLegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeRu).GetDisplayName(dto.ShipmentTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.ShipmentTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ShipmentTypeIdEn),
                    Header = L["TransportLegDto.ShipmentTypeName"],
                    DisplaySelector = dto => typeof(ShipmentTypeEn).GetDisplayName(dto.ShipmentTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.ShipmentTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportKindSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.TransportKindCode),
                Header = L["TransportLegDto.TransportKindCode"],
                DisplaySelector = dto => dto.TransportKindCode,
                SortSelector = dto => dto.TransportKindCode,
                Filterable = false,
                Visible = false
            }
        );

        //TransportKind
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.TransportKindIdRu),
                    Header = L["TransportLegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.TransportKindIdEn),
                    Header = L["TransportLegDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddNodeFromSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.NodeFromCode),
                Header = L["TransportLegDto.NodeFromCode"],
                DisplaySelector = dto => dto.NodeFromCode,
                SortSelector = dto => dto.NodeFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.NodeFromNameRu),
                    Header = L["TransportLegDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    SortSelector = dto => dto.NodeFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.NodeFromNameEn),
                    Header = L["TransportLegDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    SortSelector = dto => dto.NodeFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddRegionFromSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        // RegionFromCode
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.RegionFromCode),
                Header = L["TransportLegDto.RegionFromCode"],
                DisplaySelector = dto => dto.RegionFromCode,
                SortSelector = dto => dto.RegionFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // RegionFromName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.RegionFromNameRu),
                    Header = L["TransportLegDto.RegionFromName"],
                    DisplaySelector = dto => dto.RegionFromNameRu,
                    SortSelector = dto => dto.RegionFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.RegionFromNameEn),
                    Header = L["TransportLegDto.RegionFromName"],
                    DisplaySelector = dto => dto.RegionFromNameEn,
                    SortSelector = dto => dto.RegionFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyNodeSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.ProxyNodeCode),
                Header = L["TransportLegDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                SortSelector = dto => dto.ProxyNodeCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ProxyNodeNameRu),
                    Header = L["TransportLegDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    SortSelector = dto => dto.ProxyNodeNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ProxyNodeNameEn),
                    Header = L["TransportLegDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    SortSelector = dto => dto.ProxyNodeNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyRegionSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.ProxyRegionCode),
                Header = L["TransportLegDto.ProxyRegionCode"],
                DisplaySelector = dto => dto.ProxyRegionCode!,
                SortSelector = dto => dto.ProxyRegionCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyRegionName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ProxyRegionNameRu),
                    Header = L["TransportLegDto.ProxyRegionName"],
                    DisplaySelector = dto => dto.ProxyRegionNameRu!,
                    SortSelector = dto => dto.ProxyRegionNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.ProxyRegionNameEn),
                    Header = L["TransportLegDto.ProxyRegionName"],
                    DisplaySelector = dto => dto.ProxyRegionNameEn!,
                    SortSelector = dto => dto.ProxyRegionNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddNodeToSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.NodeToCode),
                Header = L["TransportLegDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                SortSelector = dto => dto.NodeToCode,
                Filterable = true,
                Visible = false
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.NodeToNameRu),
                    Header = L["TransportLegDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    SortSelector = dto => dto.NodeToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.NodeToNameEn),
                    Header = L["TransportLegDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    SortSelector = dto => dto.NodeToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddRegionToSettings(List<GridColumnSetting<TransportLegDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.RegionToCode),
                Header = L["TransportLegDto.RegionToCode"],
                DisplaySelector = dto => dto.RegionToCode,
                SortSelector = dto => dto.RegionToCode,
                Filterable = true,
                Visible = false
            }
        );


        // RegionToName
        if (isRu)
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.RegionToNameRu),
                    Header = L["TransportLegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameRu,
                    SortSelector = dto => dto.RegionToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.RegionToNameEn),
                    Header = L["TransportLegDto.RegionToName"],
                    DisplaySelector = dto => dto.RegionToNameEn,
                    SortSelector = dto => dto.RegionToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddLeadtimeSettings(List<GridColumnSetting<TransportLegDto>> results)
    {
        // Колонки leadtime скрыты по требованию бизнеса: они редко нужны,
        // а при включении всех колонок грид становится слишком широким.
        results.AddRange(
            [
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.SearchTimeT),
                    Header = L["TransportLegDto.SearchTime"],
                    DisplaySelector = dto => dto.SearchTimeT,
                    SortSelector = dto => dto.SearchTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.LoadTimeT),
                    Header = L["TransportLegDto.LoadTime"],
                    DisplaySelector = dto => dto.LoadTimeT,
                    SortSelector = dto => dto.LoadTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.DaysWaitingT),
                    Header = L["TransportLegDto.DaysWaiting"],
                    DisplaySelector = dto => dto.DaysWaitingT,
                    SortSelector = dto => dto.DaysWaitingT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.TravelTimeT),
                    Header = L["TransportLegDto.TravelTime"],
                    DisplaySelector = dto => dto.TravelTimeT,
                    SortSelector = dto => dto.TravelTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.UnLoadTimeT),
                    Header = L["TransportLegDto.UnLoadTime"],
                    DisplaySelector = dto => dto.UnLoadTimeT,
                    SortSelector = dto => dto.UnLoadTimeT,
                    Filterable = false,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.TransportationTimeT),
                    Header = L["TransportLegDto.TransportationTime"],
                    DisplaySelector = dto => dto.TransportationTimeT,
                    SortSelector = dto => dto.TransportationTimeT,
                    Filterable = false,
                    Visible = true
                }
            ]
        );
    }

    private void AddCreationChangeDateSettings(List<GridColumnSetting<TransportLegDto>> results)
    {
        results.AddRange(
            [
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.CreationDate),
                    Header = L["TransportLegDto.CreationDate"],
                    DisplaySelector = dto => dto.CreationDate,
                    SortSelector = dto => dto.CreationDate,
                    Filterable = true,
                    Visible = false
                },
                new GridColumnSetting<TransportLegDto>
                {
                    Name = nameof(TransportLegDto.LastChangeDate),
                    Header = L["TransportLegDto.LastChangeDate"],
                    DisplaySelector = dto => dto.LastChangeDate,
                    SortSelector = dto => dto.LastChangeDate,
                    Filterable = true,
                    Visible = false
                }
            ]
        );
    }


    private void AddIsArchiveSettings(List<GridColumnSetting<TransportLegDto>> results)
    {
        results.Add(
            new GridColumnSetting<TransportLegDto>
            {
                Name = nameof(TransportLegDto.IsArchive),
                Header = L["TransportLegDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["TransportLegDto.Archive"]
                    : L["TransportLegDto.Active"],
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        );
    }
}
