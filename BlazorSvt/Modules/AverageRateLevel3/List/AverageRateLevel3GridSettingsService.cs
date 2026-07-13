using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.List;

// ReSharper disable once InconsistentNaming
public class AverageRateLevel3GridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.AverageRateLevel3> L,
    ILogger<AverageRateLevel3GridSettingsService> logger)
    : BaseGridSettingsService<AverageRateLevel3Dto>(localStorage, logger)
{
    protected override string StorageKey => "AverageRateLevel3GridColumnSettings";

    protected override List<GridColumnSetting<AverageRateLevel3Dto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<GridColumnSetting<AverageRateLevel3Dto>>();
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
        AddRateLevel3Settings(results);
        AddCurrencySettings(results);
        AddCreationChangeDateSettings(results);
        AddIsArchiveSettings(results);

        return results;
    }

    private void AddCodeSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.Code),
            Header = L["AverageRateLevel3Dto.Code"],
            DisplaySelector = dto => dto.Code,
            SortSelector = dto => dto.Code,
            Filterable = true,
            Visible = true
        });
    }

    private void AddIsDefRateSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.IsDefRate),
            Header = L["AverageRateLevel3Dto.IsDefRate"],
            DisplaySelector = dto => dto.IsDefRate
                ? L["AverageRateLevel3Dto.Yes"]
                : L["AverageRateLevel3Dto.No"],
            SortSelector = dto => dto.IsDefRate,
            Filterable = true,
            Visible = true
        });
    }

    private void AddRateTypeSettings(bool isRu, List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.RateTypeIdRu),
                Header = L["AverageRateLevel3Dto.RateTypeName"],
                DisplaySelector = dto => typeof(RateTypeRu).GetDisplayName(dto.RateTypeIdRu.ToString()) ?? string.Empty,
                SortSelector = dto => dto.RateTypeIdRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.RateTypeIdEn),
                Header = L["AverageRateLevel3Dto.RateTypeName"],
                DisplaySelector = dto => typeof(RateTypeEn).GetDisplayName(dto.RateTypeIdEn.ToString()) ?? string.Empty,
                SortSelector = dto => dto.RateTypeIdEn,
                Filterable = true,
                Visible = true
            });
    }

    private void AddTransportKindSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.TransportKindCode),
            Header = L["AverageRateLevel3Dto.TransportKindCode"],
            DisplaySelector = dto => dto.TransportKindCode,
            SortSelector = dto => dto.TransportKindCode,
            Filterable = false,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.TransportKindIdRu),
                Header = L["AverageRateLevel3Dto.TransportKindName"],
                DisplaySelector = dto => typeof(TransportKindRu).GetDisplayName(dto.TransportKindIdRu.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TransportKindIdRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.TransportKindIdEn),
                Header = L["AverageRateLevel3Dto.TransportKindName"],
                DisplaySelector = dto => typeof(TransportKindEn).GetDisplayName(dto.TransportKindIdEn.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TransportKindIdEn,
                Filterable = true,
                Visible = true
            });
    }

    private void AddTransportTypeSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.TransportTypeCode),
            Header = L["AverageRateLevel3Dto.TransportTypeCode"],
            DisplaySelector = dto => dto.TransportTypeCode,
            SortSelector = dto => dto.TransportTypeCode,
            Filterable = false,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.TransportTypeIdRu),
                Header = L["AverageRateLevel3Dto.TransportTypeName"],
                DisplaySelector = dto => typeof(TransportTypeLevel3Ru).GetDisplayName(dto.TransportTypeIdRu.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TransportTypeIdRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.TransportTypeIdEn),
                Header = L["AverageRateLevel3Dto.TransportTypeName"],
                DisplaySelector = dto => typeof(TransportTypeLevel3En).GetDisplayName(dto.TransportTypeIdEn.ToString()) ?? string.Empty,
                SortSelector = dto => dto.TransportTypeIdEn,
                Filterable = true,
                Visible = true
            });
    }

    private void AddNodeFromSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.NodeFromCode),
            Header = L["AverageRateLevel3Dto.NodeFromCode"],
            DisplaySelector = dto => dto.NodeFromCode,
            SortSelector = dto => dto.NodeFromCode,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.NodeFromNameRu),
                Header = L["AverageRateLevel3Dto.NodeFromName"],
                DisplaySelector = dto => dto.NodeFromNameRu,
                SortSelector = dto => dto.NodeFromNameRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.NodeFromNameEn),
                Header = L["AverageRateLevel3Dto.NodeFromName"],
                DisplaySelector = dto => dto.NodeFromNameEn,
                SortSelector = dto => dto.NodeFromNameEn,
                Filterable = true,
                Visible = true
            });
    }

    private void AddProxyNodeSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.ProxyNodeCode),
            Header = L["AverageRateLevel3Dto.ProxyNodeCode"],
            DisplaySelector = dto => dto.ProxyNodeCode!,
            SortSelector = dto => dto.ProxyNodeCode!,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProxyNodeNameRu),
                Header = L["AverageRateLevel3Dto.ProxyNodeName"],
                DisplaySelector = dto => dto.ProxyNodeNameRu!,
                SortSelector = dto => dto.ProxyNodeNameRu!,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProxyNodeNameEn),
                Header = L["AverageRateLevel3Dto.ProxyNodeName"],
                DisplaySelector = dto => dto.ProxyNodeNameEn!,
                SortSelector = dto => dto.ProxyNodeNameEn!,
                Filterable = true,
                Visible = true
            });
    }

    private void AddNodeToSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.NodeToCode),
            Header = L["AverageRateLevel3Dto.NodeToCode"],
            DisplaySelector = dto => dto.NodeToCode,
            SortSelector = dto => dto.NodeToCode,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.NodeToNameRu),
                Header = L["AverageRateLevel3Dto.NodeToName"],
                DisplaySelector = dto => dto.NodeToNameRu,
                SortSelector = dto => dto.NodeToNameRu,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.NodeToNameEn),
                Header = L["AverageRateLevel3Dto.NodeToName"],
                DisplaySelector = dto => dto.NodeToNameEn,
                SortSelector = dto => dto.NodeToNameEn,
                Filterable = true,
                Visible = true
            });
    }

    private void AddProductGroupSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.ProductGroupCode),
            Header = L["AverageRateLevel3Dto.ProductGroupCode"],
            DisplaySelector = dto => dto.ProductGroupCode!,
            SortSelector = dto => dto.ProductGroupCode!,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProductGroupNameRu),
                Header = L["AverageRateLevel3Dto.ProductGroupName"],
                DisplaySelector = dto => dto.ProductGroupNameRu!,
                SortSelector = dto => dto.ProductGroupNameRu!,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProductGroupNameEn),
                Header = L["AverageRateLevel3Dto.ProductGroupName"],
                DisplaySelector = dto => dto.ProductGroupNameEn!,
                SortSelector = dto => dto.ProductGroupNameEn!,
                Filterable = true,
                Visible = true
            });
    }

    private void AddProductCodeSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results, bool isRu)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.ProductCode),
            Header = L["AverageRateLevel3Dto.ProductCode"],
            DisplaySelector = dto => dto.ProductCode!,
            SortSelector = dto => dto.ProductCode!,
            Filterable = true,
            Visible = false
        });

        if (isRu)
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProductNameRu),
                Header = L["AverageRateLevel3Dto.ProductName"],
                DisplaySelector = dto => dto.ProductNameRu!,
                SortSelector = dto => dto.ProductNameRu!,
                Filterable = true,
                Visible = true
            });
        else
            results.Add(new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.ProductNameEn),
                Header = L["AverageRateLevel3Dto.ProductName"],
                DisplaySelector = dto => dto.ProductNameEn!,
                SortSelector = dto => dto.ProductNameEn!,
                Filterable = true,
                Visible = true
            });
    }

    private void AddStartEndDateSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.AddRange(
        [
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.StartDate),
                Header = L["AverageRateLevel3Dto.StartDate"],
                DisplaySelector = dto => dto.StartDate.ToShortDateString(),
                SortSelector = dto => dto.StartDate,
                Filterable = true,
                Visible = true
            },
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.EndDate),
                Header = L["AverageRateLevel3Dto.EndDate"],
                DisplaySelector = dto => dto.EndDate.ToShortDateString(),
                SortSelector = dto => dto.EndDate,
                Filterable = true,
                Visible = true
            }
        ]);
    }

    private void AddRateLevel3Settings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.AddRange(
        [
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.RateLevel3),
                Header = L["AverageRateLevel3Dto.RateLevel3"],
                DisplaySelector = dto => dto.RateLevel3,
                SortSelector = dto => dto.RateLevel3,
                Filterable = false,
                Visible = true
            },
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.EffectiveLoadOfTransportType),
                Header = L["AverageRateLevel3Dto.EffectiveLoadOfTransportType"],
                DisplaySelector = dto => dto.EffectiveLoadOfTransportType,
                SortSelector = dto => dto.EffectiveLoadOfTransportType,
                Filterable = false,
                Visible = true
            }
        ]);
    }

    private void AddCurrencySettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.CurrencyId),
            Header = L["AverageRateLevel3Dto.Currency"],
            DisplaySelector = dto => typeof(Currency).GetDisplayName(dto.CurrencyId.ToString()) ?? string.Empty,
            SortSelector = dto => dto.CurrencyId,
            Filterable = true,
            Visible = true
        });
    }

    private void AddCreationChangeDateSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.AddRange(
        [
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.CreationDate),
                Header = L["AverageRateLevel3Dto.CreationDate"],
                DisplaySelector = dto => dto.CreationDate,
                SortSelector = dto => dto.CreationDate,
                Filterable = true,
                Visible = false
            },
            new GridColumnSetting<AverageRateLevel3Dto>
            {
                Name = nameof(AverageRateLevel3Dto.LastChangeDate),
                Header = L["AverageRateLevel3Dto.LastChangeDate"],
                DisplaySelector = dto => dto.LastChangeDate,
                SortSelector = dto => dto.LastChangeDate,
                Filterable = true,
                Visible = false
            }
        ]);
    }

    private void AddIsArchiveSettings(List<GridColumnSetting<AverageRateLevel3Dto>> results)
    {
        results.Add(new GridColumnSetting<AverageRateLevel3Dto>
        {
            Name = nameof(AverageRateLevel3Dto.IsArchive),
            Header = L["AverageRateLevel3Dto.IsArchive"],
            DisplaySelector = dto => dto.IsArchive
                ? L["AverageRateLevel3Dto.Archive"]
                : L["AverageRateLevel3Dto.Active"],
            SortSelector = dto => dto.IsArchive,
            Filterable = true,
            Visible = false,
            FilterValue = "False"
        });
    }
}
