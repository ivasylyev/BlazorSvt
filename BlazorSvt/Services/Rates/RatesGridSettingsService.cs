using Blazored.LocalStorage;
using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

public class RatesGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Svt> L, ILogger<RatesGridSettingsService> logger) : BaseGridSettingsService<RateDto>(localStorage, L, logger)
{
    protected override string StorageKey => "RatesGridColumnSettings";




    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings()
    {
        return
        [
            new GridColumnSetting<RateDto>
            {
                Name = "Code",
                Header = L["RateDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "IsDefRate",
                Header = L["RateDto.IsDefRate"],
                DisplaySelector = dto => dto.IsDefRate ? "Да" : "Нет",
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "RateTypeName",
                Header = L["RateDto.RateTypeName"],
                DisplaySelector = dto => dto.RateTypeName,
                SortSelector = dto => dto.RateTypeName,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameRu",
                Header = L["RateDto.NodeFromName"] ,
                DisplaySelector = dto => dto.NodeFromNameRu,
                SortSelector = dto => dto.NodeFromNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameEn",
                Header = L["RateDto.NodeFromName"],
                DisplaySelector = dto => dto.NodeFromNameEn,
                SortSelector = dto => dto.NodeFromNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameRu",
                Header = L["RateDto.ProxyNodeName"],
                DisplaySelector = dto => dto.ProxyNodeNameRu,
                SortSelector = dto => dto.ProxyNodeNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameEn",
                Header = L["RateDto.ProxyNodeName"],
                DisplaySelector = dto => dto.ProxyNodeNameEn,
                SortSelector = dto => dto.ProxyNodeNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameRu",
                Header = L["RateDto.NodeToName"],
                DisplaySelector = dto => dto.NodeToNameRu,
                SortSelector = dto => dto.NodeToNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameEn",
                Header = L["RateDto.NodeToName"],
                DisplaySelector = dto => dto.NodeToNameEn,
                SortSelector = dto => dto.NodeToNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProductGroupName",
                Header = L["RateDto.ProductGroupName"],
                DisplaySelector = dto => dto.ProductGroupName,
                SortSelector = dto => dto.ProductGroupName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "StartDate",
                Header = L["RateDto.StartDate"],
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "EndDate",
                Header = L["RateDto.EndDate"],
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CurrencyCode",
                Header = L["RateDto.CurrencyCode"],
                DisplaySelector = dto => dto.CurrencyCode,
                SortSelector = dto => dto.CurrencyCode,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTon",
                Header = L["RateDto.TotalCostTon"],
                DisplaySelector = dto => dto.TotalCostTon,
                SortSelector = dto => dto.TotalCostTon,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTransport",
                Header = L["RateDto.TotalCostTransport"],
                DisplaySelector = dto => dto.TotalCostTransport,
                SortSelector = dto => dto.TotalCostTransport,
                Filterable = false,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CreationDate",
                Header = L["RateDto.CreationDate"],
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "LastChangeDate",
                Header = L["RateDto.LastChangeDate"],
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "IsArchive",
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