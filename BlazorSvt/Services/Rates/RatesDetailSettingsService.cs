using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

// ReSharper disable once InconsistentNaming
public class RatesDetailSettingsService(IStringLocalizer<Svt> L, ILogger<RatesDetailSettingsService> logger) : IDetailSettingsService<RateDetailDto>
{
    public DetailSettingsCollection<RateDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<DetailSetting<RateDetailDto>>();

        AddCodeSettings(results);
        AddIsDefRateSettings(results);
        AddCreationChangeDateSettings(results);
        AddIsArchiveSettings(results);
        AddStartEndDateSettings(results);
        AddAverageRateAndCostSettings(results);
        AddCalcCurrencyAndTypeSettings(results);
        AddMultiCurrencyCostSettings(results);
        AddFeeComponentSettings(results);

        AddNodeFromSettings(results, isRu);
        AddRegionFromSettings(results, isRu);
        AddProxyNodeSettings(results, isRu);
        AddProxyRegionSettings(results, isRu);
        AddNodeToSettings(results, isRu);
        AddRegionToSettings(results, isRu);
        AddBasisSettings(results, isRu);

        AddTransportKindSettings(results, isRu);
        AddTransportTypeSettings(results, isRu);

        AddProductSettings(results, isRu);
        AddContractorSettings(results);
        AddTenderAndCommentSettings(results);

        AddLegReferenceSettings(results);
        AddLeadtimeMetaSettings(results);
        AddLeadtimeSettings(results);

        AddLeg1TransportAndCostSettings(results, isRu);
        AddLeadtimeLeg1Settings(results);

        AddLeg2TransportAndCostSettings(results, isRu);
        AddLeadtimeLeg2Settings(results);

        return new DetailSettingsCollection<RateDetailDto>(results);
    }

    private void AddCodeSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.Code),
            Header = L["RateDetailDto.Code"],
            GroupHeader = L["RateDetailDto.Group1Parameters"],
            DisplaySelector = dto => dto.Code,
            VisibleSelector = _ => true
        });
    }

    private void AddIsDefRateSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.IsDefRate),
            Header = L["RateDetailDto.IsDefRate"],
            GroupHeader = L["RateDetailDto.Group1Parameters"],
            DisplaySelector = dto => dto.IsDefRate
                ? L["RateDetailDto.Yes"]
                : L["RateDetailDto.No"],
            VisibleSelector = _ => true
        });
    }

    private void AddCreationChangeDateSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.CreationDate),
                Header = L["RateDetailDto.CreationDate"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.CreationDate,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LastChangeDate),
                Header = L["RateDetailDto.LastChangeDate"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.LastChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddIsArchiveSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.IsArchive),
            Header = L["RateDetailDto.IsArchive"],
            GroupHeader = L["RateDetailDto.Group1Parameters"],
            DisplaySelector = dto => dto.IsArchive
                ? L["RateDetailDto.Archive"]
                : L["RateDetailDto.Active"],
            VisibleSelector = _ => true,
            HasMargin = true
        });
    }

    private void AddStartEndDateSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.StartDate),
                Header = L["RateDetailDto.StartDate"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.StartDate,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.EndDate),
                Header = L["RateDetailDto.EndDate"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.EndDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddAverageRateAndCostSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.AverageRateCode),
                Header = L["RateDetailDto.AverageRateCode"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.AverageRateCode,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.AverageRateLevel3TotalCostTon),
                Header = L["RateDetailDto.AverageRateLevel3TotalCostTon"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.AverageRateLevel3TotalCostTon,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTon),
                Header = L["RateDetailDto.TotalCostTon"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTon,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTransport),
                Header = L["RateDetailDto.TotalCostTransport"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTransport,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddCalcCurrencyAndTypeSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.CalcType),
                Header = L["RateDetailDto.CalcType"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.CalcType,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.CurrencyStandard),
                Header = L["RateDetailDto.Currency"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.CurrencyStandard,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.CurrencyRateMonth),
                Header = L["RateDetailDto.CurrencyRateMonth"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.CurrencyRateMonth,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.EffectiveLoadOfTransportType),
                Header = L["RateDetailDto.EffectiveLoadOfTransportType"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.EffectiveLoadOfTransportType,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TypeCode),
                Header = L["RateDetailDto.RateTypeCode"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TypeCode,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TypeName),
                Header = L["RateDetailDto.RateTypeName"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TypeName,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddMultiCurrencyCostSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTonRUB),
                Header = L["RateDetailDto.TotalCostTonRUB"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTonRUB,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTonEUR),
                Header = L["RateDetailDto.TotalCostTonEUR"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTonEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTonCNY),
                Header = L["RateDetailDto.TotalCostTonCNY"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTonCNY,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTonUSD),
                Header = L["RateDetailDto.TotalCostTonUSD"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTonUSD,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTransportRUB),
                Header = L["RateDetailDto.TotalCostTransportRUB"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTransportRUB,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTransportEUR),
                Header = L["RateDetailDto.TotalCostTransportEUR"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTransportEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTransportCNY),
                Header = L["RateDetailDto.TotalCostTransportCNY"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTransportCNY,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TotalCostTransportUSD),
                Header = L["RateDetailDto.TotalCostTransportUSD"],
                GroupHeader = L["RateDetailDto.Group1Parameters"],
                DisplaySelector = dto => dto.TotalCostTransportUSD,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddFeeComponentSettings(List<DetailSetting<RateDetailDto>> results)
    {
        AddFeePair(results, nameof(RateDetailDto.EmptyRFSize), nameof(RateDetailDto.EmptyRFCurrency),
            dto => dto.EmptyRFSize, dto => dto.EmptyRFCurrency);
        AddFeePair(results, nameof(RateDetailDto.EmptyCISSize), nameof(RateDetailDto.EmptyCISCurrency),
            dto => dto.EmptyCISSize, dto => dto.EmptyCISCurrency);
        AddFeePair(results, nameof(RateDetailDto.ProvisionTransportSize), nameof(RateDetailDto.ProvisionTransportCurrency),
            dto => dto.ProvisionTransportSize, dto => dto.ProvisionTransportCurrency);
        AddFeePair(results, nameof(RateDetailDto.FerryboatSize), nameof(RateDetailDto.FerryboatCurrency),
            dto => dto.FerryboatSize, dto => dto.FerryboatCurrency);
        AddFeePair(results, nameof(RateDetailDto.TEFromSize), nameof(RateDetailDto.TEFromCurrency),
            dto => dto.TEFromSize, dto => dto.TEFromCurrency);
        AddFeePair(results, nameof(RateDetailDto.RatePNPFromSize), nameof(RateDetailDto.PNPFromCurrency),
            dto => dto.RatePNPFromSize, dto => dto.PNPFromCurrency);
        AddFeePair(results, nameof(RateDetailDto.TEToSize), nameof(RateDetailDto.TEToCurrency),
            dto => dto.TEToSize, dto => dto.TEToCurrency);
        AddFeePair(results, nameof(RateDetailDto.PNPToSize), nameof(RateDetailDto.PNPToCurrency),
            dto => dto.PNPToSize, dto => dto.PNPToCurrency);
        AddFeePair(results, nameof(RateDetailDto.DrainLoadingSize), nameof(RateDetailDto.DrainLoadingCurrency),
            dto => dto.DrainLoadingSize, dto => dto.DrainLoadingCurrency);
        AddFeePair(results, nameof(RateDetailDto.TransshipmentSize), nameof(RateDetailDto.TransshipmentCurrency),
            dto => dto.TransshipmentSize, dto => dto.TransshipmentCurrency);
        AddFeePair(results, nameof(RateDetailDto.FreightSize), nameof(RateDetailDto.FreightCurrency),
            dto => dto.FreightSize, dto => dto.FreightCurrency);
        AddFeePair(results, nameof(RateDetailDto.AdditionalFeesCISSize), nameof(RateDetailDto.AdditionalFeesCISCurrency),
            dto => dto.AdditionalFeesCISSize, dto => dto.AdditionalFeesCISCurrency);
        AddFeePair(results, nameof(RateDetailDto.LoadedCISSize), nameof(RateDetailDto.LoadedCISCurrency),
            dto => dto.LoadedCISSize, dto => dto.LoadedCISCurrency);
        AddFeePair(results, nameof(RateDetailDto.LoadedRFSize), nameof(RateDetailDto.LoadedRFCurrency),
            dto => dto.LoadedRFSize, dto => dto.LoadedRFCurrency);
        AddFeePair(results, nameof(RateDetailDto.TEFromSize_fix), nameof(RateDetailDto.TEFromCurrency_fix),
            dto => dto.TEFromSize_fix, dto => dto.TEFromCurrency_fix);
        AddFeePair(results, nameof(RateDetailDto.TEToSize_fix), nameof(RateDetailDto.TEToCurrency_fix),
            dto => dto.TEToSize_fix, dto => dto.TEToCurrency_fix, hasMargin: true);
    }

    private void AddFeePair(
        List<DetailSetting<RateDetailDto>> results,
        string sizeName,
        string currencyName,
        Func<RateDetailDto, decimal> sizeSelector,
        Func<RateDetailDto, string?> currencySelector,
        bool hasMargin = false)
    {
        var group = L["RateDetailDto.Group1Parameters"];
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = sizeName,
            Header = L[$"RateDetailDto.{sizeName}"],
            GroupHeader = group,
            DisplaySelector = dto => sizeSelector(dto),
            VisibleSelector = _ => true,
            HasMargin = hasMargin
        });
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = currencyName,
            Header = L[$"RateDetailDto.{currencyName}"],
            GroupHeader = group,
            DisplaySelector = dto => currencySelector(dto)!,
            VisibleSelector = dto => currencySelector(dto) is not null
        });
    }

    private void AddNodeFromSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.NodeFromCode),
            Header = L["RateDetailDto.NodeFromCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.NodeFromCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.NodeFromNameRu),
                Header = L["RateDetailDto.NodeFromName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeFromNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.NodeFromNameEn),
                Header = L["RateDetailDto.NodeFromName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeFromNameEn,
                VisibleSelector = _ => true
            });
    }

    private void AddRegionFromSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.RegionFromCode),
            Header = L["RateDetailDto.RegionFromCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.RegionFromCode!,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.RegionFromNameRu),
                Header = L["RateDetailDto.RegionFromName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionFromNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.RegionFromNameEn),
                Header = L["RateDetailDto.RegionFromName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionFromNameEn!,
                VisibleSelector = _ => true
            });
    }

    private void AddProxyNodeSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProxyNodeCode),
            Header = L["RateDetailDto.ProxyNodeCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.ProxyNodeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null,
            HasMargin = true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProxyNodeNameRu),
                Header = L["RateDetailDto.ProxyNodeName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyNodeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProxyNodeNameEn),
                Header = L["RateDetailDto.ProxyNodeName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyNodeNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
    }

    private void AddProxyRegionSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProxyRegionCode),
            Header = L["RateDetailDto.ProxyRegionCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.ProxyRegionCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProxyRegionNameRu),
                Header = L["RateDetailDto.ProxyRegionName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyRegionNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProxyRegionNameEn),
                Header = L["RateDetailDto.ProxyRegionName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.ProxyRegionNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
    }

    private void AddNodeToSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.NodeToCode),
            Header = L["RateDetailDto.NodeToCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.NodeToCode,
            VisibleSelector = _ => true,
            HasMargin = true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.NodeToNameRu),
                Header = L["RateDetailDto.NodeToName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeToNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.NodeToNameEn),
                Header = L["RateDetailDto.NodeToName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.NodeToNameEn,
                VisibleSelector = _ => true
            });
    }

    private void AddRegionToSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.RegionToCode),
            Header = L["RateDetailDto.RegionToCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.RegionToCode!,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.RegionToNameRu),
                Header = L["RateDetailDto.RegionToName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionToNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.RegionToNameEn),
                Header = L["RateDetailDto.RegionToName"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.RegionToNameEn!,
                VisibleSelector = _ => true
            });
    }

    private void AddBasisSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.Basis),
            Header = L["RateDetailDto.Basis"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.Basis!,
            VisibleSelector = dto => dto.Basis is not null,
            HasMargin = true
        });

        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.BasisNodeCode),
            Header = L["RateDetailDto.BasisNodeCode"],
            GroupHeader = L["RateDetailDto.Group2FromTo"],
            DisplaySelector = dto => dto.BasisNodeCode!,
            VisibleSelector = dto => dto.BasisNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.BasisNodeNameRu),
                Header = L["RateDetailDto.BasisNodeNameRu"],
                GroupHeader = L["RateDetailDto.Group2FromTo"],
                DisplaySelector = dto => dto.BasisNodeNameRu!,
                VisibleSelector = dto => dto.BasisNodeCode is not null
            });
    }

    private void AddTransportKindSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.TransportKindCode),
            Header = L["RateDetailDto.TransportKindCode"],
            GroupHeader = L["RateDetailDto.Group3Transport"],
            DisplaySelector = dto => dto.TransportKindCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TransportKindNameRu),
                Header = L["RateDetailDto.TransportKindName"],
                GroupHeader = L["RateDetailDto.Group3Transport"],
                DisplaySelector = dto => dto.TransportKindNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TransportKindNameRuEn),
                Header = L["RateDetailDto.TransportKindName"],
                GroupHeader = L["RateDetailDto.Group3Transport"],
                DisplaySelector = dto => dto.TransportKindNameRuEn,
                VisibleSelector = _ => true
            });
    }

    private void AddTransportTypeSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.TransportTypeCode),
            Header = L["RateDetailDto.TransportTypeCode"],
            GroupHeader = L["RateDetailDto.Group3Transport"],
            DisplaySelector = dto => dto.TransportTypeCode,
            VisibleSelector = _ => true,
            HasMargin = true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TransportTypeNameRu),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group3Transport"],
                DisplaySelector = dto => dto.TransportTypeNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TransportTypeNameRuEn),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group3Transport"],
                DisplaySelector = dto => dto.TransportTypeNameRuEn,
                VisibleSelector = _ => true
            });
    }

    private void AddProductSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProductGroupCode),
            Header = L["RateDetailDto.ProductGroupCode"],
            GroupHeader = L["RateDetailDto.Group5ProductContractor"],
            DisplaySelector = dto => dto.ProductGroupCode!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });

        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProductGroupNameEnRu),
            Header = L["RateDetailDto.ProductGroupNameEnRu"],
            GroupHeader = L["RateDetailDto.Group5ProductContractor"],
            DisplaySelector = dto => dto.ProductGroupNameEnRu!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });

        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProductCode),
            Header = L["RateDetailDto.ProductCode"],
            GroupHeader = L["RateDetailDto.Group5ProductContractor"],
            DisplaySelector = dto => dto.ProductCode!,
            VisibleSelector = dto => dto.ProductCode is not null,
            HasMargin = true
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProductNameRu),
                Header = L["RateDetailDto.ProductName"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.ProductNameRu!,
                VisibleSelector = dto => dto.ProductCode is not null
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ProductNameEn),
                Header = L["RateDetailDto.ProductName"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.ProductNameEn!,
                VisibleSelector = dto => dto.ProductCode is not null
            });

        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.ProductDPOCOde),
            Header = L["RateDetailDto.ProductDPOCOde"],
            GroupHeader = L["RateDetailDto.Group5ProductContractor"],
            DisplaySelector = dto => dto.ProductDPOCOde!,
            VisibleSelector = dto => dto.ProductDPOCOde is not null
        });
    }

    private void AddContractorSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ContractorCode),
                Header = L["RateDetailDto.ContractorCode"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.ContractorCode!,
                VisibleSelector = dto => dto.ContractorCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ContractorNameSearch),
                Header = L["RateDetailDto.ContractorNameSearch"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.ContractorNameSearch!,
                VisibleSelector = dto => dto.ContractorCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.ContractorEGRUL),
                Header = L["RateDetailDto.ContractorEGRUL"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.ContractorEGRUL!,
                VisibleSelector = dto => dto.ContractorCode is not null
            }
        ]);
    }

    private void AddTenderAndCommentSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Nomination),
                Header = L["RateDetailDto.Nomination"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.Nomination!,
                VisibleSelector = dto => dto.Nomination is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TenderServicePack),
                Header = L["RateDetailDto.TenderServicePack"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.TenderServicePack!,
                VisibleSelector = dto => dto.TenderServicePack is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.TenderNumber),
                Header = L["RateDetailDto.TenderNumber"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.TenderNumber!,
                VisibleSelector = dto => dto.TenderNumber is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.AdditionalAgreementNumber),
                Header = L["RateDetailDto.AdditionalAgreementNumber"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.AdditionalAgreementNumber!,
                VisibleSelector = dto => dto.AdditionalAgreementNumber is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Comment),
                Header = L["RateDetailDto.Comment"],
                GroupHeader = L["RateDetailDto.Group5ProductContractor"],
                DisplaySelector = dto => dto.Comment!,
                VisibleSelector = dto => dto.Comment is not null
            }
        ]);
    }

    private void AddLegReferenceSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LegCode),
                Header = L["RateDetailDto.LegCode"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LegCode,
                VisibleSelector = _ => true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LegChangeDate),
                Header = L["RateDetailDto.LegChangeDate"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LegChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddLeadtimeMetaSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeCode),
                Header = L["RateDetailDto.LeadTimeCode"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeCode!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeStartDate),
                Header = L["RateDetailDto.LeadTimeStartDate"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeStartDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeEndDate),
                Header = L["RateDetailDto.LeadTimeEndDate"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeEndDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeChangeDate),
                Header = L["RateDetailDto.LeadTimeChangeDate"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeChangeDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            }
        ]);
    }

    private void AddLeadtimeSettings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeSearchTime),
                Header = L["RateDetailDto.SearchTime"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeSearchTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLoadTime),
                Header = L["RateDetailDto.LoadTime"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeDaysWaiting),
                Header = L["RateDetailDto.DaysWaiting"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDaysWaiting!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeTravelTime),
                Header = L["RateDetailDto.TravelTime"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTravelTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeUnLoadTime),
                Header = L["RateDetailDto.UnLoadTime"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeUnLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeTransportationTime),
                Header = L["RateDetailDto.TransportationTime"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTransportationTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeDistance),
                Header = L["RateDetailDto.Distance"],
                GroupHeader = L["RateDetailDto.Group4Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDistance!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddLeg1TransportAndCostSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.Leg1_TransportTypeCode),
            Header = L["RateDetailDto.TransportTypeCode"],
            GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
            DisplaySelector = dto => dto.Leg1_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TransportTypeNameRu),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TransportTypeNameRuEn),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_EffectiveLoad),
                Header = L["RateDetailDto.Leg1_EffectiveLoad"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTon),
                Header = L["RateDetailDto.Leg1_TotalCostTon"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTransport),
                Header = L["RateDetailDto.Leg1_TotalCostTransport"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_BaseCurrency),
                Header = L["RateDetailDto.Leg1_BaseCurrency"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTonRUB),
                Header = L["RateDetailDto.Leg1_TotalCostTonRUB"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTonUSD),
                Header = L["RateDetailDto.Leg1_TotalCostTonUSD"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTonEUR),
                Header = L["RateDetailDto.Leg1_TotalCostTonEUR"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTonCNY),
                Header = L["RateDetailDto.Leg1_TotalCostTonCNY"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTransportRUB),
                Header = L["RateDetailDto.Leg1_TotalCostTransportRUB"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTransportUSD),
                Header = L["RateDetailDto.Leg1_TotalCostTransportUSD"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTransportEUR),
                Header = L["RateDetailDto.Leg1_TotalCostTransportEUR"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg1_TotalCostTransportCNY),
                Header = L["RateDetailDto.Leg1_TotalCostTransportCNY"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.Leg1_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddLeadtimeLeg1Settings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_SearchTime),
                Header = L["RateDetailDto.Leg1_SearchTime"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_SearchTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_LoadTime),
                Header = L["RateDetailDto.Leg1_LoadTime"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_LoadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_DaysWaiting),
                Header = L["RateDetailDto.Leg1_DaysWaiting"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_TravelTime),
                Header = L["RateDetailDto.Leg1_TravelTime"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_TransportationTime),
                Header = L["RateDetailDto.Leg1_TransportationTime"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg1_Distance),
                Header = L["RateDetailDto.Leg1_Distance"],
                GroupHeader = L["RateDetailDto.Group41LeadtimesLeg1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddLeg2TransportAndCostSettings(List<DetailSetting<RateDetailDto>> results, bool isRu)
    {
        results.Add(new DetailSetting<RateDetailDto>
        {
            Name = nameof(RateDetailDto.Leg2_TransportTypeCode),
            Header = L["RateDetailDto.TransportTypeCode"],
            GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
            DisplaySelector = dto => dto.Leg2_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TransportTypeNameRu),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TransportTypeNameRuEn),
                Header = L["RateDetailDto.TransportTypeName"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_EffectiveLoad),
                Header = L["RateDetailDto.Leg2_EffectiveLoad"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTon),
                Header = L["RateDetailDto.Leg2_TotalCostTon"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTransport),
                Header = L["RateDetailDto.Leg2_TotalCostTransport"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_BaseCurrency),
                Header = L["RateDetailDto.Leg2_BaseCurrency"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTonRUB),
                Header = L["RateDetailDto.Leg2_TotalCostTonRUB"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTonUSD),
                Header = L["RateDetailDto.Leg2_TotalCostTonUSD"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTonEUR),
                Header = L["RateDetailDto.Leg2_TotalCostTonEUR"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTonCNY),
                Header = L["RateDetailDto.Leg2_TotalCostTonCNY"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTransportRUB),
                Header = L["RateDetailDto.Leg2_TotalCostTransportRUB"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTransportUSD),
                Header = L["RateDetailDto.Leg2_TotalCostTransportUSD"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTransportEUR),
                Header = L["RateDetailDto.Leg2_TotalCostTransportEUR"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.Leg2_TotalCostTransportCNY),
                Header = L["RateDetailDto.Leg2_TotalCostTransportCNY"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.Leg2_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddLeadtimeLeg2Settings(List<DetailSetting<RateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg2_TravelTime),
                Header = L["RateDetailDto.Leg2_TravelTime"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg2_DaysWaiting),
                Header = L["RateDetailDto.Leg2_DaysWaiting"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg2_UploadTime),
                Header = L["RateDetailDto.Leg2_UpLoadTime"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_UploadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg2_TransportationTime),
                Header = L["RateDetailDto.Leg2_TransportationTime"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<RateDetailDto>
            {
                Name = nameof(RateDetailDto.LeadTimeLeg2_Distance),
                Header = L["RateDetailDto.Leg2_Distance"],
                GroupHeader = L["RateDetailDto.Group42LeadtimesLeg2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }
}
