using Blazored.LocalStorage;
using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

// ReSharper disable once InconsistentNaming
public class RatesGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Svt> L, ILogger<RatesGridSettingsService> logger) : BaseGridSettingsService<RateDto>(localStorage, logger)
{
    protected override string StorageKey => "RatesGridColumnSettings";


    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings()
    {
        return
        [
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.Code),
                Header = L["RateDto.Code"],
                DisplaySelector = dto => dto.Code!,
                SortSelector = dto => dto.Code!,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.IsDefRate),
                Header = L["RateDto.IsDefRate"],
                DisplaySelector = dto => dto.IsDefRate ? "Да" : "Нет",
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.RateTypeName),
                Header = L["RateDto.RateTypeName"],
                DisplaySelector = dto => dto.RateTypeName!,
                SortSelector = dto => dto.RateTypeName!,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeFromName),
                Header = L["RateDto.NodeFromName"] ,
                DisplaySelector = dto => dto.NodeFromName!,
                SortSelector = dto => dto.NodeFromName!,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProxyNodeName),
                Header = L["RateDto.ProxyNodeName"],
                DisplaySelector = dto => dto.ProxyNodeName!,
                SortSelector = dto => dto.ProxyNodeName!,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeToName),
                Header = L["RateDto.NodeToName"],
                DisplaySelector = dto => dto.NodeToName!,
                SortSelector = dto => dto.NodeToName!,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProductGroupName),
                Header = L["RateDto.ProductGroupName"],
                DisplaySelector = dto => dto.ProductGroupName!,
                SortSelector = dto => dto.ProductGroupName!,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.StartDate),
                Header = L["RateDto.StartDate"],
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.EndDate),
                Header = L["RateDto.EndDate"],
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.CurrencyCode),
                Header = L["RateDto.CurrencyCode"],
                DisplaySelector = dto => dto.CurrencyCode!,
                SortSelector = dto => dto.CurrencyCode!,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TotalCostTon),
                Header = L["RateDto.TotalCostTon"],
                DisplaySelector = dto => dto.TotalCostTon,
                SortSelector = dto => dto.TotalCostTon,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TotalCostTransport),
                Header = L["RateDto.TotalCostTransport"],
                DisplaySelector = dto => dto.TotalCostTransport,
                SortSelector = dto => dto.TotalCostTransport,
                Filterable = false,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.CreationDate),
                Header = L["RateDto.CreationDate"],
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.LastChangeDate),
                Header = L["RateDto.LastChangeDate"],
                DisplaySelector = dto => dto.LastChangeDate!,
                SortSelector = dto => dto.LastChangeDate!,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.IsArchive),
                Header = L["RateDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive ? "Архив" : "Актив",
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        ];
    }
}