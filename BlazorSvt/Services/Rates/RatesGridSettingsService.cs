using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

// ReSharper disable once InconsistentNaming
public class RatesGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Svt> L, ILogger<RatesGridSettingsService> logger)
    : BaseGridSettingsService<RateDto>(localStorage, logger)
{
    protected override string StorageKey => "RatesGridColumnSettings";


    protected override List<GridColumnSetting<RateDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<GridColumnSetting<RateDto>>();
        AddCodeSettings(results);
        AddIsDefRateSettings(results);
        AddRateTypeSettings(isRu, results);
        AddTransportKindSettings(results, isRu);
        AddTransportTypeSettings(results, isRu);
        AddNodeFromSettings(results, isRu);
        AddProxyNodeSettings(results, isRu);
        AddNodeToSettings(results, isRu);
        AddProductGroupSettings(results, isRu);
        AddProductCodeSettings(results, isRu);
        AddStartEndDateSettings(results);
        AddTotalCostSettings(results);
        AddCurrencySettings(results);
        AddCreationChangeDateSettings(results);
        AddIsArchiveSettings(results);

        return results;
    }


    private void AddCodeSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.Add(new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.Code),
                Header = L["RateDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddIsDefRateSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.Add(new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.IsDefRate),
                Header = L["RateDto.IsDefRate"],
                DisplaySelector = dto => dto.IsDefRate
                    ? L["RateDto.Yes"]
                    : L["RateDto.No"],
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddRateTypeSettings(bool isRu, List<GridColumnSetting<RateDto>> results)
    {
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.RateTypeIdRu),
                    Header = L["RateDto.RateTypeName"],
                    DisplaySelector = dto => typeof(RateTypeRu).GetDisplayName(dto.RateTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.RateTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.RateTypeIdEn),
                    Header = L["RateDto.RateTypeName"],
                    DisplaySelector = dto => typeof(RateTypeEn).GetDisplayName(dto.RateTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.RateTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportKindSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportKindCode),
                Header = L["RateDto.TransportKindCode"],
                DisplaySelector = dto => dto.TransportKindCode,
                SortSelector = dto => dto.TransportKindCode,
                Filterable = true,
                Visible = false
            }
        );

        //TransportKind
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.TransportKindIdRu),
                    Header = L["RateDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.TransportKindIdEn),
                    Header = L["RateDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportTypeSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.TransportTypeCode),
                Header = L["RateDto.TransportTypeCode"],
                DisplaySelector = dto => dto.TransportTypeCode,
                SortSelector = dto => dto.TransportTypeCode,
                Filterable = true,
                Visible = false
            }
        );

        // TransportType
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.TransportTypeIdRu),
                    Header = L["RateDto.TransportTypeName"],
                    DisplaySelector = dto => typeof(TransportTypeLevel3Ru).GetDisplayName(dto.TransportTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.TransportTypeIdEn),
                    Header = L["RateDto.TransportTypeName"],
                    DisplaySelector = dto => typeof(TransportTypeLevel3En).GetDisplayName(dto.TransportTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddNodeFromSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeFromCode),
                Header = L["RateDto.NodeFromCode"],
                DisplaySelector = dto => dto.NodeFromCode,
                SortSelector = dto => dto.NodeFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.NodeFromNameRu),
                    Header = L["RateDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    SortSelector = dto => dto.NodeFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.NodeFromNameEn),
                    Header = L["RateDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    SortSelector = dto => dto.NodeFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyNodeSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProxyNodeCode),
                Header = L["RateDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                SortSelector = dto => dto.ProxyNodeCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProxyNodeNameRu),
                    Header = L["RateDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    SortSelector = dto => dto.ProxyNodeNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProxyNodeNameEn),
                    Header = L["RateDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    SortSelector = dto => dto.ProxyNodeNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddNodeToSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.NodeToCode),
                Header = L["RateDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                SortSelector = dto => dto.NodeToCode,
                Filterable = true,
                Visible = false
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.NodeToNameRu),
                    Header = L["RateDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    SortSelector = dto => dto.NodeToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.NodeToNameEn),
                    Header = L["RateDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    SortSelector = dto => dto.NodeToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProductGroupSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProductGroupCode),
                Header = L["RateDto.ProductGroupCode"],
                DisplaySelector = dto => dto.ProductGroupCode!,
                SortSelector = dto => dto.ProductGroupCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProductGroupName
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProductGroupNameRu),
                    Header = L["RateDto.ProductGroupName"],
                    DisplaySelector = dto => dto.ProductGroupNameRu!,
                    SortSelector = dto => dto.ProductGroupNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProductGroupNameEn),
                    Header = L["RateDto.ProductGroupName"],
                    DisplaySelector = dto => dto.ProductGroupNameEn!,
                    SortSelector = dto => dto.ProductGroupNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProductCodeSettings(List<GridColumnSetting<RateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.ProductCode),
                Header = L["RateDto.ProductCode"],
                DisplaySelector = dto => dto.ProductCode!,
                SortSelector = dto => dto.ProductCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProductName
        if (isRu)
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProductNameRu),
                    Header = L["RateDto.ProductName"],
                    DisplaySelector = dto => dto.ProductNameRu!,
                    SortSelector = dto => dto.ProductNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<RateDto>
                {
                    Name = nameof(RateDto.ProductNameEn),
                    Header = L["RateDto.ProductName"],
                    DisplaySelector = dto => dto.ProductNameEn!,
                    SortSelector = dto => dto.ProductNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddStartEndDateSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.AddRange(
            new[]
            {
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
                }
            }
        );
    }

    private void AddTotalCostSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.AddRange(
            new[]
            {
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
                }
            }
        );
    }

    private void AddCurrencySettings(List<GridColumnSetting<RateDto>> results)
    {
        results.Add(new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.CurrencyId),
                Header = L["RateDto.Currency"],
                DisplaySelector = dto => typeof(Currency).GetDisplayName(dto.CurrencyId.ToString()) ?? string.Empty,
                SortSelector = dto => dto.CurrencyId,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddCreationChangeDateSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.AddRange(
            new[]
            {
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
                }
            }
        );
    }


    private void AddIsArchiveSettings(List<GridColumnSetting<RateDto>> results)
    {
        results.Add(
            new GridColumnSetting<RateDto>
            {
                Name = nameof(RateDto.IsArchive),
                Header = L["RateDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["RateDto.Archive"]
                    : L["RateDto.Active"],
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        );
    }
}