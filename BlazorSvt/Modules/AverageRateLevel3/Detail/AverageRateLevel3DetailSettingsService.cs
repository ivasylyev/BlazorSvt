using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.Detail;

// ReSharper disable once InconsistentNaming
public class AverageRateLevel3DetailSettingsService(IStringLocalizer<Resources.AverageRateLevel3> L, ILogger<AverageRateLevel3DetailSettingsService> logger) : IDetailSettingsService<AverageRateLevel3DetailDto>
{
    public DetailSettingsCollection<AverageRateLevel3DetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<DetailSetting<AverageRateLevel3DetailDto>>();

        AddGroup1CodeSettings(results);
        AddGroup1TransportRateCodesSettings(results);
        AddGroup1TypeSettings(results);
        AddGroup1DeflatorSettings(results);
        AddGroup1CreationChangeDateSettings(results);
        AddGroup1IsArchiveSettings(results);


        AddGroup2Settings(results);

        AddGroup3Settings(results, isRu);

        AddGroup4Settings(results, isRu);

        AddGroup6Settings(results, isRu);

        AddGroup7Settings(results);

        AddGroup8Settings(results);

        AddGroup10Settings(results);

        AddGroup11Settings(results);

        AddGroup12LegReferenceSettings(results);
        AddGroup12LeadtimeMetaSettings(results);
        AddGroup12LeadtimeSettings(results);

        AddGroup13Settings(results, isRu);
        AddGroup14Settings(results);

        AddGroup15Settings(results, isRu);
        AddGroup16Settings(results);

        return new DetailSettingsCollection<AverageRateLevel3DetailDto>(results);
    }

    private void AddGroup1CodeSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.Code),
            Header = L["AverageRateLevel3DetailDto.Code"],
            GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.Code,
            VisibleSelector = _ => true
        });
    }
    private void AddGroup1TransportRateCodesSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.TransportRateCodes),
            Header = L["AverageRateLevel3DetailDto.TransportRateCodes"],
            GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.TransportRateCodes!,
            VisibleSelector = dto => dto.TransportRateCodes is not null
        });
    }

    private void AddGroup1TypeSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TypeName),
                Header = L["AverageRateLevel3DetailDto.RateTypeName"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.TypeName,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TypeCode),
                Header = L["AverageRateLevel3DetailDto.RateTypeCode"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.TypeCode,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup1DeflatorSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.IsDefRate),
            Header = L["AverageRateLevel3DetailDto.IsDefRate"],
            GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.IsDefRate
                ? L["AverageRateLevel3DetailDto.Yes"]
                : L["AverageRateLevel3DetailDto.No"],
            VisibleSelector = _ => true
        });
    }

    private void AddGroup1CreationChangeDateSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.CreationDate),
                Header = L["AverageRateLevel3DetailDto.CreationDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.CreationDate,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LastChangeDate),
                Header = L["AverageRateLevel3DetailDto.LastChangeDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.LastChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup1IsArchiveSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.IsArchive),
            Header = L["AverageRateLevel3DetailDto.IsArchive"],
            GroupHeader = L["AverageRateLevel3DetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.IsArchive
                ? L["AverageRateLevel3DetailDto.Archive"]
                : L["AverageRateLevel3DetailDto.Active"],
            VisibleSelector = _ => true
        });
    }

   

    private void AddGroup2Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.StartDate),
                Header = L["AverageRateLevel3DetailDto.StartDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.2.RateDates"],
                DisplaySelector = dto => dto.StartDate,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.EndDate),
                Header = L["AverageRateLevel3DetailDto.EndDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.2.RateDates"],
                DisplaySelector = dto => dto.EndDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup3Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results, bool isRu)
    {
        var group = L["AverageRateLevel3DetailDto.Group.3.OriginDestination"];

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.NodeFromNameRu),
                Header = L["AverageRateLevel3DetailDto.NodeFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeFromNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.NodeFromNameEn),
                Header = L["AverageRateLevel3DetailDto.NodeFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeFromNameEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.NodeFromCode),
            Header = L["AverageRateLevel3DetailDto.NodeFromCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.NodeFromCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.RegionFromNameRu),
                Header = L["AverageRateLevel3DetailDto.RegionFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionFromNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.RegionFromNameEn),
                Header = L["AverageRateLevel3DetailDto.RegionFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionFromNameEn!,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.RegionFromCode),
            Header = L["AverageRateLevel3DetailDto.RegionFromCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.RegionFromCode!,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProxyNodeNameRu),
                Header = L["AverageRateLevel3DetailDto.ProxyNodeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyNodeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProxyNodeNameEn),
                Header = L["AverageRateLevel3DetailDto.ProxyNodeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyNodeNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProxyNodeCode),
            Header = L["AverageRateLevel3DetailDto.ProxyNodeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProxyNodeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProxyRegionNameRu),
                Header = L["AverageRateLevel3DetailDto.ProxyRegionName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyRegionNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProxyRegionNameEn),
                Header = L["AverageRateLevel3DetailDto.ProxyRegionName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyRegionNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProxyRegionCode),
            Header = L["AverageRateLevel3DetailDto.ProxyRegionCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProxyRegionCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.NodeToNameRu),
                Header = L["AverageRateLevel3DetailDto.NodeToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeToNameRu,
                VisibleSelector = _ => true,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.NodeToNameEn),
                Header = L["AverageRateLevel3DetailDto.NodeToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeToNameEn,
                VisibleSelector = _ => true,
                HasMargin = true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.NodeToCode),
            Header = L["AverageRateLevel3DetailDto.NodeToCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.NodeToCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.RegionToNameRu),
                Header = L["AverageRateLevel3DetailDto.RegionToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionToNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.RegionToNameEn),
                Header = L["AverageRateLevel3DetailDto.RegionToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionToNameEn!,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.RegionToCode),
            Header = L["AverageRateLevel3DetailDto.RegionToCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.RegionToCode!,
            VisibleSelector = _ => true
        });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.Basis),
            Header = L["AverageRateLevel3DetailDto.Basis"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Basis!,
            VisibleSelector = dto => dto.Basis is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.BasisNodeNameRu),
                Header = L["AverageRateLevel3DetailDto.BasisNodeNameRu"],
                GroupHeader = group,
                DisplaySelector = dto => dto.BasisNodeNameRu!,
                VisibleSelector = dto => dto.BasisNodeCode is not null
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.BasisNodeCode),
            Header = L["AverageRateLevel3DetailDto.BasisNodeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.BasisNodeCode!,
            VisibleSelector = dto => dto.BasisNodeCode is not null
        });
    }

    private void AddGroup4Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results, bool isRu)
    {
        var group = L["AverageRateLevel3DetailDto.Group.4.Transport"];

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TransportKindNameRu),
                Header = L["AverageRateLevel3DetailDto.TransportKindName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportKindNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TransportKindNameRuEn),
                Header = L["AverageRateLevel3DetailDto.TransportKindName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportKindNameRuEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.TransportKindCode),
            Header = L["AverageRateLevel3DetailDto.TransportKindCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.TransportKindCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TransportTypeNameRu),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportTypeNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TransportTypeNameRuEn),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportTypeNameRuEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.TransportTypeCode),
            Header = L["AverageRateLevel3DetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.TransportTypeCode,
            VisibleSelector = _ => true
        });
    }

    private void AddGroup6Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results, bool isRu)
    {
        var group = L["AverageRateLevel3DetailDto.Group.6.Cargo"];

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProductGroupNameEnRu),
            Header = L["AverageRateLevel3DetailDto.ProductGroupNameEnRu"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductGroupNameEnRu!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProductNameRu),
                Header = L["AverageRateLevel3DetailDto.ProductName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProductNameRu!,
                VisibleSelector = dto => dto.ProductCode is not null,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.ProductNameEn),
                Header = L["AverageRateLevel3DetailDto.ProductName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProductNameEn!,
                VisibleSelector = dto => dto.ProductCode is not null,
                HasMargin = true
            });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.EffectiveLoadOfTransportType),
            Header = L["AverageRateLevel3DetailDto.EffectiveLoadOfTransportType"],
            GroupHeader = group,
            DisplaySelector = dto => dto.EffectiveLoadOfTransportType,
            VisibleSelector = _ => true
        });

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProductGroupCode),
            Header = L["AverageRateLevel3DetailDto.ProductGroupCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductGroupCode!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProductCode),
            Header = L["AverageRateLevel3DetailDto.ProductCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductCode!,
            VisibleSelector = dto => dto.ProductCode is not null,
            HasMargin = true
        });
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.ProductDPOCOde),
            Header = L["AverageRateLevel3DetailDto.ProductDPOCOde"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductDPOCOde!,
            VisibleSelector = dto => dto.ProductDPOCOde is not null
        });
    }

    private void AddGroup7Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.RateLevel3),
                Header = L["AverageRateLevel3DetailDto.RateLevel3"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.RateLevel3,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.CalcDate),
                Header = L["AverageRateLevel3DetailDto.CalcDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CalcDate,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.MinDailyTransportation),
                Header = L["AverageRateLevel3DetailDto.MinDailyTransportation"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.MinDailyTransportation,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.MaxDailyTransportation),
                Header = L["AverageRateLevel3DetailDto.MaxDailyTransportation"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.MaxDailyTransportation,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.CurrencyStandard),
                Header = L["AverageRateLevel3DetailDto.CurrencyStandard"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CurrencyStandard,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.CurrencyRateMonth),
                Header = L["AverageRateLevel3DetailDto.CurrencyRateMonth"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CurrencyRateMonth,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTonRUB),
                Header = L["AverageRateLevel3DetailDto.TotalCostTonRUB"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.TotalCostTonRUB,
                VisibleSelector = _ => true
            }
        ]);
    }

   

    private void AddGroup8Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        var group = L["AverageRateLevel3DetailDto.Group.8.RateComponentsExcludingVATPerTon"].Value;

        AddFeePair(results, nameof(AverageRateLevel3DetailDto.LoadedRFSize), nameof(AverageRateLevel3DetailDto.LoadedRFCurrency),
            dto => dto.LoadedRFSize, dto => dto.LoadedRFCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.LoadedCISSize), nameof(AverageRateLevel3DetailDto.LoadedCISCurrency),
            dto => dto.LoadedCISSize, dto => dto.LoadedCISCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.EmptyRFSize), nameof(AverageRateLevel3DetailDto.EmptyRFCurrency),
            dto => dto.EmptyRFSize, dto => dto.EmptyRFCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.EmptyCISSize), nameof(AverageRateLevel3DetailDto.EmptyCISCurrency),
            dto => dto.EmptyCISSize, dto => dto.EmptyCISCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.ProvisionTransportSize), nameof(AverageRateLevel3DetailDto.ProvisionTransportCurrency),
            dto => dto.ProvisionTransportSize, dto => dto.ProvisionTransportCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.TEFromSize), nameof(AverageRateLevel3DetailDto.TEFromCurrency),
            dto => dto.TEFromSize, dto => dto.TEFromCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.PNPFromSize), nameof(AverageRateLevel3DetailDto.PNPFromCurrency),
            dto => dto.PNPFromSize, dto => dto.PNPFromCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.TEFromSize_fix), nameof(AverageRateLevel3DetailDto.TEFromCurrency_fix),
            dto => dto.TEFromSize_fix, dto => dto.TEFromCurrency_fix, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.TEToSize), nameof(AverageRateLevel3DetailDto.TEToCurrency),
            dto => dto.TEToSize, dto => dto.TEToCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.PNPToSize), nameof(AverageRateLevel3DetailDto.PNPToCurrency),
            dto => dto.PNPToSize, dto => dto.PNPToCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.TEToSize_fix), nameof(AverageRateLevel3DetailDto.TEToCurrency_fix),
            dto => dto.TEToSize_fix, dto => dto.TEToCurrency_fix, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.DrainLoadingSize), nameof(AverageRateLevel3DetailDto.DrainLoadingCurrency),
            dto => dto.DrainLoadingSize, dto => dto.DrainLoadingCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.TransshipmentSize), nameof(AverageRateLevel3DetailDto.TransshipmentCurrency),
            dto => dto.TransshipmentSize, dto => dto.TransshipmentCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.FreightSize), nameof(AverageRateLevel3DetailDto.FreightCurrency),
            dto => dto.FreightSize, dto => dto.FreightCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.AdditionalFeesCISSize), nameof(AverageRateLevel3DetailDto.AdditionalFeesCISCurrency),
            dto => dto.AdditionalFeesCISSize, dto => dto.AdditionalFeesCISCurrency, _ => true, group);
        AddFeePair(results, nameof(AverageRateLevel3DetailDto.FerryboatSize), nameof(AverageRateLevel3DetailDto.FerryboatCurrency),
            dto => dto.FerryboatSize, dto => dto.FerryboatCurrency, _ => true, group);
    }
    private void AddFeePair(
        List<DetailSetting<AverageRateLevel3DetailDto>> results,
        string sizeName,
        string currencyName,
        Func<AverageRateLevel3DetailDto, decimal> sizeSelector,
        Func<AverageRateLevel3DetailDto, string?> currencySelector,
        Func<AverageRateLevel3DetailDto, bool> visibleSelector,
        string group)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = sizeName,
            Header = L[$"AverageRateLevel3DetailDto.{sizeName}"],
            GroupHeader = group,
            DisplaySelector = dto => sizeSelector(dto) != 0 ? sizeSelector(dto):string.Empty,
            VisibleSelector = visibleSelector
        });
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = currencyName,
            Header = L[$"AverageRateLevel3DetailDto.{currencyName}"],
            GroupHeader = group,
            DisplaySelector = dto => currencySelector(dto)!,
            VisibleSelector = visibleSelector
        });
    }

    private void AddGroup10Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.Comment),
            Header = L["AverageRateLevel3DetailDto.Comment"],
            GroupHeader = L["AverageRateLevel3DetailDto.Group.10.Comments"],
            DisplaySelector = dto => dto.Comment!,
            VisibleSelector = dto => dto.Comment is not null
        });
    }

    private void AddGroup11Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTonUSD),
                Header = L["AverageRateLevel3DetailDto.TotalCostTonUSD"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonUSD,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTonEUR),
                Header = L["AverageRateLevel3DetailDto.TotalCostTonEUR"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTonCNY),
                Header = L["AverageRateLevel3DetailDto.TotalCostTonCNY"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonCNY,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTransportRUB),
                Header = L["AverageRateLevel3DetailDto.TotalCostTransportRUB"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportRUB,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTransportUSD),
                Header = L["AverageRateLevel3DetailDto.TotalCostTransportUSD"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportUSD,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTransportEUR),
                Header = L["AverageRateLevel3DetailDto.TotalCostTransportEUR"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.TotalCostTransportCNY),
                Header = L["AverageRateLevel3DetailDto.TotalCostTransportCNY"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportCNY,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup12LegReferenceSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LegCode),
                Header = L["AverageRateLevel3DetailDto.LegCode"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LegCode,
                VisibleSelector = _ => true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LegChangeDate),
                Header = L["AverageRateLevel3DetailDto.LegChangeDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LegChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup12LeadtimeMetaSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeCode),
                Header = L["AverageRateLevel3DetailDto.LeadTimeCode"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeCode!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeStartDate),
                Header = L["AverageRateLevel3DetailDto.LeadTimeStartDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeStartDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeEndDate),
                Header = L["AverageRateLevel3DetailDto.LeadTimeEndDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeEndDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeChangeDate),
                Header = L["AverageRateLevel3DetailDto.LeadTimeChangeDate"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeChangeDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            }
        ]);
    }

    private void AddGroup12LeadtimeSettings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeSearchTime),
                Header = L["AverageRateLevel3DetailDto.SearchTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeSearchTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLoadTime),
                Header = L["AverageRateLevel3DetailDto.LoadTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeDaysWaiting),
                Header = L["AverageRateLevel3DetailDto.DaysWaiting"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDaysWaiting!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeTravelTime),
                Header = L["AverageRateLevel3DetailDto.TravelTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTravelTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeUnLoadTime),
                Header = L["AverageRateLevel3DetailDto.UnLoadTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeUnLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeTransportationTime),
                Header = L["AverageRateLevel3DetailDto.TransportationTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTransportationTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeDistance),
                Header = L["AverageRateLevel3DetailDto.Distance"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDistance!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddGroup13Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results, bool isRu)
    {
        var group = L["AverageRateLevel3DetailDto.Group.13.Leg1"];

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.Leg1_TransportTypeCode),
            Header = L["AverageRateLevel3DetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Leg1_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TransportTypeNameRu),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TransportTypeNameRuEn),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_EffectiveLoad),
                Header = L["AverageRateLevel3DetailDto.Leg1_EffectiveLoad"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTon),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTon"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTransport),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTransport"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_BaseCurrency),
                Header = L["AverageRateLevel3DetailDto.Leg1_BaseCurrency"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTonRUB),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTonRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTonUSD),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTonUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTonEUR),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTonEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTonCNY),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTonCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTransportRUB),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTransportUSD),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTransportEUR),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg1_TotalCostTransportCNY),
                Header = L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddGroup14Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_SearchTime),
                Header = L["AverageRateLevel3DetailDto.Leg1_SearchTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_SearchTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_LoadTime),
                Header = L["AverageRateLevel3DetailDto.Leg1_LoadTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_LoadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_DaysWaiting),
                Header = L["AverageRateLevel3DetailDto.Leg1_DaysWaiting"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_TravelTime),
                Header = L["AverageRateLevel3DetailDto.Leg1_TravelTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_TransportationTime),
                Header = L["AverageRateLevel3DetailDto.Leg1_TransportationTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg1_Distance),
                Header = L["AverageRateLevel3DetailDto.Leg1_Distance"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddGroup15Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results, bool isRu)
    {
        var group = L["AverageRateLevel3DetailDto.Group.15.Leg2"];

        results.Add(new DetailSetting<AverageRateLevel3DetailDto>
        {
            Name = nameof(AverageRateLevel3DetailDto.Leg2_TransportTypeCode),
            Header = L["AverageRateLevel3DetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Leg2_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TransportTypeNameRu),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TransportTypeNameRuEn),
                Header = L["AverageRateLevel3DetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_EffectiveLoad),
                Header = L["AverageRateLevel3DetailDto.Leg2_EffectiveLoad"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTon),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTon"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTransport),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTransport"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_BaseCurrency),
                Header = L["AverageRateLevel3DetailDto.Leg2_BaseCurrency"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTonRUB),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTonRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTonUSD),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTonUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTonEUR),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTonEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTonCNY),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTonCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTransportRUB),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTransportUSD),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTransportEUR),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.Leg2_TotalCostTransportCNY),
                Header = L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddGroup16Settings(List<DetailSetting<AverageRateLevel3DetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg2_TravelTime),
                Header = L["AverageRateLevel3DetailDto.Leg2_TravelTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg2_DaysWaiting),
                Header = L["AverageRateLevel3DetailDto.Leg2_DaysWaiting"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg2_UploadTime),
                Header = L["AverageRateLevel3DetailDto.Leg2_UpLoadTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_UploadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg2_TransportationTime),
                Header = L["AverageRateLevel3DetailDto.Leg2_TransportationTime"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<AverageRateLevel3DetailDto>
            {
                Name = nameof(AverageRateLevel3DetailDto.LeadTimeLeg2_Distance),
                Header = L["AverageRateLevel3DetailDto.Leg2_Distance"],
                GroupHeader = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }
}


