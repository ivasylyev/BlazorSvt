using BlazorBootstrap;
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


    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        return
        [
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.Code),
                Header = L["RateDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
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
                Name = isRu 
                    ? nameof(RateDto.RateTypeCodeRu)
                    : nameof(RateDto.RateTypeCodeEn),
                Header = L["RateDto.RateTypeCode"],
                DisplaySelector = isRu
                    ? dto => typeof(RateTypeRu).GetDisplayName(dto.RateTypeCodeRu.ToString()) ?? string.Empty
                    : dto => typeof(RateTypeEn).GetDisplayName(dto.RateTypeCodeEn.ToString()) ?? string.Empty,
                SortSelector = isRu
                    ? dto => dto.RateTypeCodeRu
                    : dto => dto.RateTypeCodeEn,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportKindCode),
                Header = L["RateDto.TransportKindCode"],
                DisplaySelector = dto => dto.TransportKindCode,
                SortSelector = dto => dto.TransportKindCode,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportKindName),
                Header = L["RateDto.TransportKindName"],
                DisplaySelector = dto => dto.TransportKindName,
                SortSelector = dto => dto.TransportKindName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportTypeCode),
                Header = L["RateDto.TransportTypeCode"],
                DisplaySelector = dto => dto.TransportTypeCode,
                SortSelector = dto => dto.TransportTypeCode,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportTypeName),
                Header = L["RateDto.TransportTypeName"],
                DisplaySelector = dto => dto.TransportTypeName,
                SortSelector = dto => dto.TransportTypeName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeFromCode),
                Header = L["RateDto.NodeFromCode"] ,
                DisplaySelector = dto => dto.NodeFromCode,
                SortSelector = dto => dto.NodeFromCode,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeFromName),
                Header = L["RateDto.NodeFromName"] ,
                DisplaySelector = dto => dto.NodeFromName,
                SortSelector = dto => dto.NodeFromName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProxyNodeCode),
                Header = L["RateDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                SortSelector = dto => dto.ProxyNodeCode!,
                Filterable = true,
                Visible = false
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
                Name = nameof(RateDto.NodeToCode),
                Header = L["RateDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                SortSelector = dto => dto.NodeToCode,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeToName),
                Header = L["RateDto.NodeToName"],
                DisplaySelector = dto => dto.NodeToName,
                SortSelector = dto => dto.NodeToName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProductGroupCode),
                Header = L["RateDto.ProductGroupCode"],
                DisplaySelector = dto => dto.ProductGroupCode!,
                SortSelector = dto => dto.ProductGroupCode!,
                Filterable = true,
                Visible = false
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
                Name = nameof(RateDto.ProductCode),
                Header = L["RateDto.ProductCode"],
                DisplaySelector = dto => dto.ProductCode!,
                SortSelector = dto => dto.ProductCode!,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProductName),
                Header = L["RateDto.ProductName"],
                DisplaySelector = dto => dto.ProductName!,
                SortSelector = dto => dto.ProductName!,
                Filterable = true,
                Visible = false
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
                DisplaySelector = dto => dto.CurrencyCode,
                SortSelector = dto => dto.CurrencyCode,
                Filterable = true,
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
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
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