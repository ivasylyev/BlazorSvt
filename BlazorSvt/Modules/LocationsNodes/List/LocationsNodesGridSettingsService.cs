using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.List;

public class LocationsNodesGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.LocationsNodes> L,
    ILogger<LocationsNodesGridSettingsService> logger)
    : BaseGridSettingsService<LocationsNodesDto>(localStorage, logger)
{
    protected override string StorageKey => "LocationsNodesGridColumnSettings";

    protected override List<GridColumnSetting<LocationsNodesDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var results = new List<GridColumnSetting<LocationsNodesDto>>();

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.Code),
            Header = L["LocationsNodesDto.Code"],
            DisplaySelector = dto => dto.Code,
            SortSelector = dto => dto.Code,
            Filterable = true,
            Visible = true
        });

        if (isRu)
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.NameRu),
                Header = L["LocationsNodesDto.Name"],
                DisplaySelector = dto => dto.NameRu ?? string.Empty,
                SortSelector = dto => dto.NameRu ?? string.Empty,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.NameEn),
                Header = L["LocationsNodesDto.Name"],
                DisplaySelector = dto => dto.NameEn ?? string.Empty,
                SortSelector = dto => dto.NameEn ?? string.Empty,
                Filterable = true,
                Visible = true
            });

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.LocationTypeCode),
            Header = L["LocationsNodesDto.LocationTypeCode"],
            DisplaySelector = dto => dto.LocationTypeCode ?? string.Empty,
            SortSelector = dto => dto.LocationTypeCode ?? string.Empty,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.LocationTypeIdRu),
                Header = L["LocationsNodesDto.LocationTypeName"],
                DisplaySelector = dto => typeof(LocationTypeRu).GetDisplayName(dto.LocationTypeIdRu.ToString()) ?? string.Empty,
                SortSelector = dto => dto.LocationTypeIdRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.LocationTypeIdEn),
                Header = L["LocationsNodesDto.LocationTypeName"],
                DisplaySelector = dto => typeof(LocationTypeEn).GetDisplayName(dto.LocationTypeIdEn.ToString()) ?? string.Empty,
                SortSelector = dto => dto.LocationTypeIdEn,
                Filterable = true,
                Visible = true
            });

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.TypeNodeCode),
            Header = L["LocationsNodesDto.TypeNodeCode"],
            DisplaySelector = dto => dto.TypeNodeCode ?? string.Empty,
            SortSelector = dto => dto.TypeNodeCode ?? string.Empty,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.TypeNodeIdRu),
                Header = L["LocationsNodesDto.TypeNodeName"],
                DisplaySelector = dto => typeof(TypeNodeRu).GetDisplayName(dto.TypeNodeIdRu.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TypeNodeIdRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.TypeNodeIdEn),
                Header = L["LocationsNodesDto.TypeNodeName"],
                DisplaySelector = dto => typeof(TypeNodeEn).GetDisplayName(dto.TypeNodeIdEn.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TypeNodeIdEn,
                Filterable = true,
                Visible = true
            });

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.RegionCode),
            Header = L["LocationsNodesDto.RegionCode"],
            DisplaySelector = dto => dto.RegionCode ?? string.Empty,
            SortSelector = dto => dto.RegionCode ?? string.Empty,
            Filterable = true,
            Visible = false
        });

        if (isRu)
        {
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.RegionIdRu),
                Header = L["LocationsNodesDto.RegionName"],
                DisplaySelector = dto => dto.RegionNameRu ?? string.Empty,
                SortSelector = dto => dto.RegionIdRu,
                Filterable = true,
                Visible = true
            });
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.RegionNameRu),
                Header = L["LocationsNodesDto.RegionName"],
                DisplaySelector = dto => dto.RegionNameRu ?? string.Empty,
                SortSelector = dto => dto.RegionNameRu ?? string.Empty,
                Filterable = true,
                Visible = false
            });
        }
        else
        {
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.RegionIdEn),
                Header = L["LocationsNodesDto.RegionName"],
                DisplaySelector = dto => dto.RegionNameEn ?? string.Empty,
                SortSelector = dto => dto.RegionIdEn,
                Filterable = true,
                Visible = true
            });
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.RegionNameEn),
                Header = L["LocationsNodesDto.RegionName"],
                DisplaySelector = dto => dto.RegionNameEn ?? string.Empty,
                SortSelector = dto => dto.RegionNameEn ?? string.Empty,
                Filterable = true,
                Visible = false
            });
        }

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.RegionRU),
            Header = L["LocationsNodesDto.RegionRU"],
            DisplaySelector = dto => dto.RegionRU ?? string.Empty,
            SortSelector = dto => dto.RegionRU ?? string.Empty,
            Filterable = true,
            Visible = true
        });

        results.Add(new GridColumnSetting<LocationsNodesDto>
        {
            Name = nameof(LocationsNodesDto.CountryCode),
            Header = L["LocationsNodesDto.CountryCode"],
            DisplaySelector = dto => dto.CountryCode ?? string.Empty,
            SortSelector = dto => dto.CountryCode ?? string.Empty,
            Filterable = true,
            Visible = false
        });

        if (isRu)
        {
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.CountryIdRu),
                Header = L["LocationsNodesDto.CountryName"],
                DisplaySelector = dto => dto.CountryNameRu ?? string.Empty,
                SortSelector = dto => dto.CountryIdRu,
                Filterable = true,
                Visible = true
            });
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.CountryNameRu),
                Header = L["LocationsNodesDto.CountryName"],
                DisplaySelector = dto => dto.CountryNameRu ?? string.Empty,
                SortSelector = dto => dto.CountryNameRu ?? string.Empty,
                Filterable = true,
                Visible = false
            });
        }
        else
        {
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.CountryIdEn),
                Header = L["LocationsNodesDto.CountryName"],
                DisplaySelector = dto => dto.CountryNameEn ?? string.Empty,
                SortSelector = dto => dto.CountryIdEn,
                Filterable = true,
                Visible = true
            });
            results.Add(new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.CountryNameEn),
                Header = L["LocationsNodesDto.CountryName"],
                DisplaySelector = dto => dto.CountryNameEn ?? string.Empty,
                SortSelector = dto => dto.CountryNameEn ?? string.Empty,
                Filterable = true,
                Visible = false
            });
        }

        results.AddRange(
        [
            new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.CreationDate),
                Header = L["LocationsNodesDto.CreationDate"],
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.LastChangeDate),
                Header = L["LocationsNodesDto.LastChangeDate"],
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<LocationsNodesDto>
            {
                Name = nameof(LocationsNodesDto.IsArchive),
                Header = L["LocationsNodesDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["LocationsNodesDto.Archive"]
                    : L["LocationsNodesDto.Active"],
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        ]);

        return results;
    }
}
