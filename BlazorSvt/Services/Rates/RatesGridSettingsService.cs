using Blazored.LocalStorage;
using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Services.Rates;

public class RatesBaseGridSettingsService(ILocalStorageService localStorage) : BaseGridSettingsService<RateDto>(localStorage)
{
    protected override string StorageKey => "RatesGridColumnSettings";


    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings()
    {
        return
        [
            new GridColumnSetting<RateDto>
            {
                Name = "Code",
                Header = "Code",
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "IsDefRate",
                Header = "Дефлятор",
                DisplaySelector = dto => dto.IsDefRate ? "Да" : "Нет",
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "RateTypeName",
                Header = "Тип ставки",
                DisplaySelector = dto => dto.RateTypeName,
                SortSelector = dto => dto.RateTypeName,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameRu",
                Header = "Отправление",
                DisplaySelector = dto => dto.NodeFromNameRu,
                SortSelector = dto => dto.NodeFromNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeFromNameEn",
                Header = "Отправление (En)",
                DisplaySelector = dto => dto.NodeFromNameEn,
                SortSelector = dto => dto.NodeFromNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameRu",
                Header = "Промежуточный",
                DisplaySelector = dto => dto.ProxyNodeNameRu,
                SortSelector = dto => dto.ProxyNodeNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProxyNodeNameEn",
                Header = "Промежуточный (En)",
                DisplaySelector = dto => dto.ProxyNodeNameEn,
                SortSelector = dto => dto.ProxyNodeNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameRu",
                Header = "Назначение",
                DisplaySelector = dto => dto.NodeToNameRu,
                SortSelector = dto => dto.NodeToNameRu,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "NodeToNameEn",
                Header = "Назначение (En)",
                DisplaySelector = dto => dto.NodeToNameEn,
                SortSelector = dto => dto.NodeToNameEn,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "ProductGroupName",
                Header = "Группа продуктов",
                DisplaySelector = dto => dto.ProductGroupName,
                SortSelector = dto => dto.ProductGroupName,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "StartDate",
                Header = "Начало",
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "EndDate",
                Header = "Окончание",
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CurrencyCode",
                Header = "Валюта",
                DisplaySelector = dto => dto.CurrencyCode,
                SortSelector = dto => dto.CurrencyCode,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTon",
                Header = "За тонну",
                DisplaySelector = dto => dto.TotalCostTon,
                SortSelector = dto => dto.TotalCostTon,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<RateDto>
            {
                Name = "TotalCostTransport",
                Header = "За ТС",
                DisplaySelector = dto => dto.TotalCostTransport,
                SortSelector = dto => dto.TotalCostTransport,
                Filterable = false,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "CreationDate",
                Header = "Дата создания",
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "LastChangeDate",
                Header = "Дата изменения",
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<RateDto>
            {
                Name = "IsArchive",
                Header = "Архив",
                DisplaySelector = dto => dto.IsArchive ? "Архив" : "Актив",
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        ];
    }
}