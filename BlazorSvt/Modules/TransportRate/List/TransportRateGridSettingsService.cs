using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Modules.TransportRate.List.IdsEnum;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.List;

// ReSharper disable once InconsistentNaming
public class TransportRateGridSettingsService(ILocalStorageService localStorage, IStringLocalizer<Resources.TransportRate> L, ILogger<TransportRateGridSettingsService> logger)
    : BaseGridSettingsService<TransportRateDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportRateGridColumnSettings";


    protected override List<GridColumnSetting<TransportRateDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<GridColumnSetting<TransportRateDto>>();
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


    private void AddCodeSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.Add(new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.Code),
                Header = L["TransportRateDto.Code"],
                DisplaySelector = dto => dto.Code,
                SortSelector = dto => dto.Code,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddIsDefRateSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.Add(new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.IsDefRate),
                Header = L["TransportRateDto.IsDefRate"],
                DisplaySelector = dto => dto.IsDefRate
                    ? L["TransportRateDto.Yes"]
                    : L["TransportRateDto.No"],
                SortSelector = dto => dto.IsDefRate,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddRateTypeSettings(bool isRu, List<GridColumnSetting<TransportRateDto>> results)
    {
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.RateTypeIdRu),
                    Header = L["TransportRateDto.RateTypeName"],
                    DisplaySelector = dto => typeof(RateTypeRu).GetDisplayName(dto.RateTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.RateTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.RateTypeIdEn),
                    Header = L["TransportRateDto.RateTypeName"],
                    DisplaySelector = dto => typeof(RateTypeEn).GetDisplayName(dto.RateTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.RateTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportKindSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.TransportKindCode),
                Header = L["TransportRateDto.TransportKindCode"],
                DisplaySelector = dto => dto.TransportKindCode,
                SortSelector = dto => dto.TransportKindCode,
                Filterable = false,
                Visible = false
            }
        );

        //TransportKind
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TransportKindIdRu),
                    Header = L["TransportRateDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TransportKindIdEn),
                    Header = L["TransportRateDto.TransportKindName"],
                    DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportKindIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddTransportTypeSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.TransportTypeCode),
                Header = L["TransportRateDto.TransportTypeCode"],
                DisplaySelector = dto => dto.TransportTypeCode,
                SortSelector = dto => dto.TransportTypeCode,
                Filterable = false,
                Visible = false
            }
        );

        // TransportType
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TransportTypeIdRu),
                    Header = L["TransportRateDto.TransportTypeName"],
                    DisplaySelector = dto => typeof(TransportTypeLevel3Ru).GetDisplayName(dto.TransportTypeIdRu.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportTypeIdRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TransportTypeIdEn),
                    Header = L["TransportRateDto.TransportTypeName"],
                    DisplaySelector = dto => typeof(TransportTypeLevel3En).GetDisplayName(dto.TransportTypeIdEn.ToString()) ?? string.Empty,
                    SortSelector = dto => dto.TransportTypeIdEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddNodeFromSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        // NodeFromCode
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.NodeFromCode),
                Header = L["TransportRateDto.NodeFromCode"],
                DisplaySelector = dto => dto.NodeFromCode,
                SortSelector = dto => dto.NodeFromCode,
                Filterable = true,
                Visible = false
            }
        );

        // NodeFromName
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.NodeFromNameRu),
                    Header = L["TransportRateDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameRu,
                    SortSelector = dto => dto.NodeFromNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.NodeFromNameEn),
                    Header = L["TransportRateDto.NodeFromName"],
                    DisplaySelector = dto => dto.NodeFromNameEn,
                    SortSelector = dto => dto.NodeFromNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProxyNodeSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.ProxyNodeCode),
                Header = L["TransportRateDto.ProxyNodeCode"],
                DisplaySelector = dto => dto.ProxyNodeCode!,
                SortSelector = dto => dto.ProxyNodeCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProxyNodeName
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProxyNodeNameRu),
                    Header = L["TransportRateDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameRu!,
                    SortSelector = dto => dto.ProxyNodeNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProxyNodeNameEn),
                    Header = L["TransportRateDto.ProxyNodeName"],
                    DisplaySelector = dto => dto.ProxyNodeNameEn!,
                    SortSelector = dto => dto.ProxyNodeNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddNodeToSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.NodeToCode),
                Header = L["TransportRateDto.NodeToCode"],
                DisplaySelector = dto => dto.NodeToCode,
                SortSelector = dto => dto.NodeToCode,
                Filterable = true,
                Visible = false
            }
        );


        // NodeToName
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.NodeToNameRu),
                    Header = L["TransportRateDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameRu,
                    SortSelector = dto => dto.NodeToNameRu,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.NodeToNameEn),
                    Header = L["TransportRateDto.NodeToName"],
                    DisplaySelector = dto => dto.NodeToNameEn,
                    SortSelector = dto => dto.NodeToNameEn,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProductGroupSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.ProductGroupCode),
                Header = L["TransportRateDto.ProductGroupCode"],
                DisplaySelector = dto => dto.ProductGroupCode!,
                SortSelector = dto => dto.ProductGroupCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProductGroupName
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProductGroupNameRu),
                    Header = L["TransportRateDto.ProductGroupName"],
                    DisplaySelector = dto => dto.ProductGroupNameRu!,
                    SortSelector = dto => dto.ProductGroupNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProductGroupNameEn),
                    Header = L["TransportRateDto.ProductGroupName"],
                    DisplaySelector = dto => dto.ProductGroupNameEn!,
                    SortSelector = dto => dto.ProductGroupNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }

    private void AddProductCodeSettings(List<GridColumnSetting<TransportRateDto>> results, bool isRu)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.ProductCode),
                Header = L["TransportRateDto.ProductCode"],
                DisplaySelector = dto => dto.ProductCode!,
                SortSelector = dto => dto.ProductCode!,
                Filterable = true,
                Visible = false
            }
        );

        // ProductName
        if (isRu)
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProductNameRu),
                    Header = L["TransportRateDto.ProductName"],
                    DisplaySelector = dto => dto.ProductNameRu!,
                    SortSelector = dto => dto.ProductNameRu!,
                    Filterable = true,
                    Visible = true
                }
            );
        else
            results.Add(new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.ProductNameEn),
                    Header = L["TransportRateDto.ProductName"],
                    DisplaySelector = dto => dto.ProductNameEn!,
                    SortSelector = dto => dto.ProductNameEn!,
                    Filterable = true,
                    Visible = true
                }
            );
    }


    private void AddStartEndDateSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.AddRange(
            new[]
            {
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.StartDate),
                    Header = L["TransportRateDto.StartDate"],
                    DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                    SortSelector = dto => dto.StartDate,
                    Filterable = true,
                    Visible = true
                },
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.EndDate),
                    Header = L["TransportRateDto.EndDate"],
                    DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                    SortSelector = dto => dto.EndDate,
                    Filterable = true,
                    Visible = true
                }
            }
        );
    }

    private void AddTotalCostSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.AddRange(
            new[]
            {
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TotalCostTon),
                    Header = L["TransportRateDto.TotalCostTon"],
                    DisplaySelector = dto => dto.TotalCostTon,
                    SortSelector = dto => dto.TotalCostTon,
                    Filterable = false,
                    Visible = true
                },
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.TotalCostTransport),
                    Header = L["TransportRateDto.TotalCostTransport"],
                    DisplaySelector = dto => dto.TotalCostTransport,
                    SortSelector = dto => dto.TotalCostTransport,
                    Filterable = false,
                    Visible = true
                }
            }
        );
    }

    private void AddCurrencySettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.Add(new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.CurrencyId),
                Header = L["TransportRateDto.Currency"],
                DisplaySelector = dto => typeof(Currency).GetDisplayName(dto.CurrencyId.ToString()) ?? string.Empty,
                SortSelector = dto => dto.CurrencyId,
                Filterable = true,
                Visible = true
            }
        );
    }

    private void AddCreationChangeDateSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.AddRange(
            new[]
            {
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.CreationDate),
                    Header = L["TransportRateDto.CreationDate"],
                    DisplaySelector = dto => dto.CreationDate,
                    SortSelector = dto => dto.CreationDate,
                    Filterable = true,
                    Visible = false
                },
                new GridColumnSetting<TransportRateDto>
                {
                    Name = nameof(TransportRateDto.LastChangeDate),
                    Header = L["TransportRateDto.LastChangeDate"],
                    DisplaySelector = dto => dto.LastChangeDate,
                    SortSelector = dto => dto.LastChangeDate,
                    Filterable = true,
                    Visible = false
                }
            }
        );
    }


    private void AddIsArchiveSettings(List<GridColumnSetting<TransportRateDto>> results)
    {
        results.Add(
            new GridColumnSetting<TransportRateDto>
            {
                Name = nameof(TransportRateDto.IsArchive),
                Header = L["TransportRateDto.IsArchive"],
                DisplaySelector = dto => dto.IsArchive
                    ? L["TransportRateDto.Archive"]
                    : L["TransportRateDto.Active"],
                SortSelector = dto => dto.IsArchive,
                Filterable = true,
                Visible = false,
                FilterValue = "False"
            }
        );
    }
}

