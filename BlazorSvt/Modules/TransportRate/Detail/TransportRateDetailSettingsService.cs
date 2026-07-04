using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.Detail;

// ReSharper disable once InconsistentNaming
public class TransportRateDetailSettingsService(IStringLocalizer<Resources.TransportRate> L, ILogger<TransportRateDetailSettingsService> logger) : IDetailSettingsService<TransportRateDetailDto>
{
    public DetailSettingsCollection<TransportRateDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var results = new List<DetailSetting<TransportRateDetailDto>>();

        AddGroup1CodeSettings(results);
        AddGroup1AverageRateCodeSettings(results);
        AddGroup1TypeSettings(results);
        AddGroup1TenderAndContractorSettings(results);
        AddGroup1DeflatorSettings(results);
        AddGroup1CreationChangeDateSettings(results);
        AddGroup1IsArchiveSettings(results);


        AddGroup2Settings(results);

        AddGroup3Settings(results, isRu);

        AddGroup4Settings(results, isRu);

        AddGroup5Settings(results);

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

        return new DetailSettingsCollection<TransportRateDetailDto>(results);
    }

    private void AddGroup1CodeSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.Code),
            Header = L["TransportRateDetailDto.Code"],
            GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.Code,
            VisibleSelector = _ => true
        });
    }
    private void AddGroup1AverageRateCodeSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.AverageRateCode),
            Header = L["TransportRateDetailDto.AverageRateCode"],
            GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.AverageRateCode,
            VisibleSelector = _ => true
        });
    }

    private void AddGroup1TypeSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TypeName),
                Header = L["TransportRateDetailDto.RateTypeName"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.TypeName,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TypeCode),
                Header = L["TransportRateDetailDto.RateTypeCode"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.TypeCode,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup1TenderAndContractorSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Nomination),
                Header = L["TransportRateDetailDto.Nomination"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.Nomination!,
                VisibleSelector = dto => dto.Nomination is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ContractorNameSearch),
                Header = L["TransportRateDetailDto.ContractorNameSearch"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.ContractorNameSearch!,
                VisibleSelector = dto => dto.ContractorCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TenderNumber),
                Header = L["TransportRateDetailDto.TenderNumber"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.TenderNumber!,
                VisibleSelector = dto => dto.TenderNumber is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.AdditionalAgreementNumber),
                Header = L["TransportRateDetailDto.AdditionalAgreementNumber"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.AdditionalAgreementNumber!,
                VisibleSelector = dto => dto.AdditionalAgreementNumber is not null
            }
        ]);
    }

    private void AddGroup1DeflatorSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.IsDefRate),
            Header = L["TransportRateDetailDto.IsDefRate"],
            GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.IsDefRate
                ? L["TransportRateDetailDto.Yes"]
                : L["TransportRateDetailDto.No"],
            VisibleSelector = _ => true
        });
    }

    private void AddGroup1CreationChangeDateSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.CreationDate),
                Header = L["TransportRateDetailDto.CreationDate"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.CreationDate,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LastChangeDate),
                Header = L["TransportRateDetailDto.LastChangeDate"],
                GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
                DisplaySelector = dto => dto.LastChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup1IsArchiveSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.IsArchive),
            Header = L["TransportRateDetailDto.IsArchive"],
            GroupHeader = L["TransportRateDetailDto.Group.1.RateParameters"],
            DisplaySelector = dto => dto.IsArchive
                ? L["TransportRateDetailDto.Archive"]
                : L["TransportRateDetailDto.Active"],
            VisibleSelector = _ => true
        });
    }

   

    private void AddGroup2Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.StartDate),
                Header = L["TransportRateDetailDto.StartDate"],
                GroupHeader = L["TransportRateDetailDto.Group.2.RateDates"],
                DisplaySelector = dto => dto.StartDate,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.EndDate),
                Header = L["TransportRateDetailDto.EndDate"],
                GroupHeader = L["TransportRateDetailDto.Group.2.RateDates"],
                DisplaySelector = dto => dto.EndDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup3Settings(List<DetailSetting<TransportRateDetailDto>> results, bool isRu)
    {
        var group = L["TransportRateDetailDto.Group.3.OriginDestination"];

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.NodeFromNameRu),
                Header = L["TransportRateDetailDto.NodeFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeFromNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.NodeFromNameEn),
                Header = L["TransportRateDetailDto.NodeFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeFromNameEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.NodeFromCode),
            Header = L["TransportRateDetailDto.NodeFromCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.NodeFromCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.RegionFromNameRu),
                Header = L["TransportRateDetailDto.RegionFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionFromNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.RegionFromNameEn),
                Header = L["TransportRateDetailDto.RegionFromName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionFromNameEn!,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.RegionFromCode),
            Header = L["TransportRateDetailDto.RegionFromCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.RegionFromCode!,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProxyNodeNameRu),
                Header = L["TransportRateDetailDto.ProxyNodeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyNodeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProxyNodeNameEn),
                Header = L["TransportRateDetailDto.ProxyNodeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyNodeNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProxyNodeCode),
            Header = L["TransportRateDetailDto.ProxyNodeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProxyNodeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProxyRegionNameRu),
                Header = L["TransportRateDetailDto.ProxyRegionName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyRegionNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProxyRegionNameEn),
                Header = L["TransportRateDetailDto.ProxyRegionName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProxyRegionNameEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProxyRegionCode),
            Header = L["TransportRateDetailDto.ProxyRegionCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProxyRegionCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.NodeToNameRu),
                Header = L["TransportRateDetailDto.NodeToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeToNameRu,
                VisibleSelector = _ => true,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.NodeToNameEn),
                Header = L["TransportRateDetailDto.NodeToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.NodeToNameEn,
                VisibleSelector = _ => true,
                HasMargin = true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.NodeToCode),
            Header = L["TransportRateDetailDto.NodeToCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.NodeToCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.RegionToNameRu),
                Header = L["TransportRateDetailDto.RegionToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionToNameRu!,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.RegionToNameEn),
                Header = L["TransportRateDetailDto.RegionToName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.RegionToNameEn!,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.RegionToCode),
            Header = L["TransportRateDetailDto.RegionToCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.RegionToCode!,
            VisibleSelector = _ => true
        });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.Basis),
            Header = L["TransportRateDetailDto.Basis"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Basis!,
            VisibleSelector = dto => dto.Basis is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.BasisNodeNameRu),
                Header = L["TransportRateDetailDto.BasisNodeNameRu"],
                GroupHeader = group,
                DisplaySelector = dto => dto.BasisNodeNameRu!,
                VisibleSelector = dto => dto.BasisNodeCode is not null
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.BasisNodeCode),
            Header = L["TransportRateDetailDto.BasisNodeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.BasisNodeCode!,
            VisibleSelector = dto => dto.BasisNodeCode is not null
        });
    }

    private void AddGroup4Settings(List<DetailSetting<TransportRateDetailDto>> results, bool isRu)
    {
        var group = L["TransportRateDetailDto.Group.4.Transport"];

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TransportKindNameRu),
                Header = L["TransportRateDetailDto.TransportKindName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportKindNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TransportKindNameRuEn),
                Header = L["TransportRateDetailDto.TransportKindName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportKindNameRuEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.TransportKindCode),
            Header = L["TransportRateDetailDto.TransportKindCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.TransportKindCode,
            VisibleSelector = _ => true
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TransportTypeNameRu),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportTypeNameRu,
                VisibleSelector = _ => true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TransportTypeNameRuEn),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.TransportTypeNameRuEn,
                VisibleSelector = _ => true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.TransportTypeCode),
            Header = L["TransportRateDetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.TransportTypeCode,
            VisibleSelector = _ => true
        });
    }

    private void AddGroup5Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.TenderServicePack),
            Header = L["TransportRateDetailDto.TenderServicePack"],
            GroupHeader = L["TransportRateDetailDto.Group.5.MultimodalTransportation"],
            DisplaySelector = dto => dto.TenderServicePack!,
            VisibleSelector = dto => dto.TenderServicePack is not null
        });
    }

    private void AddGroup6Settings(List<DetailSetting<TransportRateDetailDto>> results, bool isRu)
    {
        var group = L["TransportRateDetailDto.Group.6.Cargo"];

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProductGroupNameEnRu),
            Header = L["TransportRateDetailDto.ProductGroupNameEnRu"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductGroupNameEnRu!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProductNameRu),
                Header = L["TransportRateDetailDto.ProductName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProductNameRu!,
                VisibleSelector = dto => dto.ProductCode is not null,
                HasMargin = true
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.ProductNameEn),
                Header = L["TransportRateDetailDto.ProductName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.ProductNameEn!,
                VisibleSelector = dto => dto.ProductCode is not null,
                HasMargin = true
            });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.EffectiveLoadOfTransportType),
            Header = L["TransportRateDetailDto.EffectiveLoadOfTransportType"],
            GroupHeader = group,
            DisplaySelector = dto => dto.EffectiveLoadOfTransportType,
            VisibleSelector = _ => true
        });

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProductGroupCode),
            Header = L["TransportRateDetailDto.ProductGroupCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductGroupCode!,
            VisibleSelector = dto => dto.ProductGroupCode is not null
        });
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProductCode),
            Header = L["TransportRateDetailDto.ProductCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductCode!,
            VisibleSelector = dto => dto.ProductCode is not null,
            HasMargin = true
        });
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ProductDPOCOde),
            Header = L["TransportRateDetailDto.ProductDPOCOde"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ProductDPOCOde!,
            VisibleSelector = dto => dto.ProductDPOCOde is not null
        });
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ContractorCode),
            Header = L["TransportRateDetailDto.ContractorCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ContractorCode!,
            VisibleSelector = dto => dto.ContractorCode is not null,
            HasMargin = true
        });
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.ContractorEGRUL),
            Header = L["TransportRateDetailDto.ContractorEGRUL"],
            GroupHeader = group,
            DisplaySelector = dto => dto.ContractorEGRUL!,
            VisibleSelector = dto => dto.ContractorCode is not null
        });
    }

    private void AddGroup7Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.CalcType),
                Header = L["TransportRateDetailDto.CalcType"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CalcType,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTransport),
                Header = L["TransportRateDetailDto.TotalCostTransport"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.TotalCostTransport,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTon),
                Header = L["TransportRateDetailDto.TotalCostTon"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.TotalCostTon,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.CurrencyStandard),
                Header = L["TransportRateDetailDto.Currency"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CurrencyStandard,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.CurrencyRateMonth),
                Header = L["TransportRateDetailDto.CurrencyRateMonth"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.CurrencyRateMonth,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTonRUB),
                Header = L["TransportRateDetailDto.TotalCostTonRUB"],
                GroupHeader = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"],
                DisplaySelector = dto => dto.TotalCostTonRUB,
                VisibleSelector = _ => true
            }
        ]);
    }

   

    private void AddGroup8Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        Func<TransportRateDetailDto, bool> visibleSelector = dto => dto.CalcType != "ТС";
        var group = L["TransportRateDetailDto.Group.8.RateComponentsExcludingVATPerTon"].Value;

        AddFeePair(results, nameof(TransportRateDetailDto.LoadedRFSize), nameof(TransportRateDetailDto.LoadedRFCurrency),
            dto => dto.LoadedRFSize, dto => dto.LoadedRFCurrency, _=>true, group);
        AddFeePair(results, nameof(TransportRateDetailDto.LoadedCISSize), nameof(TransportRateDetailDto.LoadedCISCurrency),
            dto => dto.LoadedCISSize, dto => dto.LoadedCISCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.EmptyRFSize), nameof(TransportRateDetailDto.EmptyRFCurrency),
            dto => dto.EmptyRFSize, dto => dto.EmptyRFCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.EmptyCISSize), nameof(TransportRateDetailDto.EmptyCISCurrency),
            dto => dto.EmptyCISSize, dto => dto.EmptyCISCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.ProvisionTransportSize), nameof(TransportRateDetailDto.ProvisionTransportCurrency),
            dto => dto.ProvisionTransportSize, dto => dto.ProvisionTransportCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.TEFromSize), nameof(TransportRateDetailDto.TEFromCurrency),
            dto => dto.TEFromSize, dto => dto.TEFromCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.RatePNPFromSize), nameof(TransportRateDetailDto.PNPFromCurrency),
            dto => dto.RatePNPFromSize, dto => dto.PNPFromCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.TEFromSize_fix), nameof(TransportRateDetailDto.TEFromCurrency_fix),
            dto => dto.TEFromSize_fix, dto => dto.TEFromCurrency_fix, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.TEToSize), nameof(TransportRateDetailDto.TEToCurrency),
            dto => dto.TEToSize, dto => dto.TEToCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.PNPToSize), nameof(TransportRateDetailDto.PNPToCurrency),
            dto => dto.PNPToSize, dto => dto.PNPToCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.TEToSize_fix), nameof(TransportRateDetailDto.TEToCurrency_fix),
            dto => dto.TEToSize_fix, dto => dto.TEToCurrency_fix, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.DrainLoadingSize), nameof(TransportRateDetailDto.DrainLoadingCurrency),
            dto => dto.DrainLoadingSize, dto => dto.DrainLoadingCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.TransshipmentSize), nameof(TransportRateDetailDto.TransshipmentCurrency),
            dto => dto.TransshipmentSize, dto => dto.TransshipmentCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.FreightSize), nameof(TransportRateDetailDto.FreightCurrency),
            dto => dto.FreightSize, dto => dto.FreightCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.AdditionalFeesCISSize), nameof(TransportRateDetailDto.AdditionalFeesCISCurrency),
            dto => dto.AdditionalFeesCISSize, dto => dto.AdditionalFeesCISCurrency, visibleSelector, group);
        AddFeePair(results, nameof(TransportRateDetailDto.FerryboatSize), nameof(TransportRateDetailDto.FerryboatCurrency),
            dto => dto.FerryboatSize, dto => dto.FerryboatCurrency, visibleSelector, group);
    }
    private void AddFeePair(
        List<DetailSetting<TransportRateDetailDto>> results,
        string sizeName,
        string currencyName,
        Func<TransportRateDetailDto, decimal> sizeSelector,
        Func<TransportRateDetailDto, string?> currencySelector,
        Func<TransportRateDetailDto, bool> visibleSelector,
        string group)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = sizeName,
            Header = L[$"TransportRateDetailDto.{sizeName}"],
            GroupHeader = group,
            DisplaySelector = dto => sizeSelector(dto) != 0 ? sizeSelector(dto):string.Empty,
            VisibleSelector = visibleSelector
        });
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = currencyName,
            Header = L[$"TransportRateDetailDto.{currencyName}"],
            GroupHeader = group,
            DisplaySelector = dto => currencySelector(dto)!,
            VisibleSelector = visibleSelector
        });
    }

    private void AddGroup10Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.Comment),
            Header = L["TransportRateDetailDto.Comment"],
            GroupHeader = L["TransportRateDetailDto.Group.10.Comments"],
            DisplaySelector = dto => dto.Comment!,
            VisibleSelector = dto => dto.Comment is not null
        });
    }

    private void AddGroup11Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTonUSD),
                Header = L["TransportRateDetailDto.TotalCostTonUSD"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonUSD,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTonEUR),
                Header = L["TransportRateDetailDto.TotalCostTonEUR"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTonCNY),
                Header = L["TransportRateDetailDto.TotalCostTonCNY"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTonCNY,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTransportRUB),
                Header = L["TransportRateDetailDto.TotalCostTransportRUB"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportRUB,
                VisibleSelector = _ => true,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTransportUSD),
                Header = L["TransportRateDetailDto.TotalCostTransportUSD"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportUSD,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTransportEUR),
                Header = L["TransportRateDetailDto.TotalCostTransportEUR"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportEUR,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.TotalCostTransportCNY),
                Header = L["TransportRateDetailDto.TotalCostTransportCNY"],
                GroupHeader = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"],
                DisplaySelector = dto => dto.TotalCostTransportCNY,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup12LegReferenceSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LegCode),
                Header = L["TransportRateDetailDto.LegCode"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LegCode,
                VisibleSelector = _ => true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LegChangeDate),
                Header = L["TransportRateDetailDto.LegChangeDate"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LegChangeDate,
                VisibleSelector = _ => true
            }
        ]);
    }

    private void AddGroup12LeadtimeMetaSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeCode),
                Header = L["TransportRateDetailDto.LeadTimeCode"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeCode!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeStartDate),
                Header = L["TransportRateDetailDto.LeadTimeStartDate"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeStartDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeEndDate),
                Header = L["TransportRateDetailDto.LeadTimeEndDate"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeEndDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeChangeDate),
                Header = L["TransportRateDetailDto.LeadTimeChangeDate"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeChangeDate!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            }
        ]);
    }

    private void AddGroup12LeadtimeSettings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeSearchTime),
                Header = L["TransportRateDetailDto.SearchTime"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeSearchTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLoadTime),
                Header = L["TransportRateDetailDto.LoadTime"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeDaysWaiting),
                Header = L["TransportRateDetailDto.DaysWaiting"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDaysWaiting!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeTravelTime),
                Header = L["TransportRateDetailDto.TravelTime"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTravelTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeUnLoadTime),
                Header = L["TransportRateDetailDto.UnLoadTime"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeUnLoadTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeTransportationTime),
                Header = L["TransportRateDetailDto.TransportationTime"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeTransportationTime!,
                VisibleSelector = dto => dto.LeadTimeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeDistance),
                Header = L["TransportRateDetailDto.Distance"],
                GroupHeader = L["TransportRateDetailDto.Group.12.Leadtimes"],
                DisplaySelector = dto => dto.LeadTimeDistance!,
                VisibleSelector = dto => dto.LeadTimeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddGroup13Settings(List<DetailSetting<TransportRateDetailDto>> results, bool isRu)
    {
        var group = L["TransportRateDetailDto.Group.13.Leg1"];

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.Leg1_TransportTypeCode),
            Header = L["TransportRateDetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Leg1_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TransportTypeNameRu),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TransportTypeNameRuEn),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_EffectiveLoad),
                Header = L["TransportRateDetailDto.Leg1_EffectiveLoad"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTon),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTon"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTransport),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTransport"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_BaseCurrency),
                Header = L["TransportRateDetailDto.Leg1_BaseCurrency"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTonRUB),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTonRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTonUSD),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTonUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTonEUR),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTonEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTonCNY),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTonCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTransportRUB),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTransportRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTransportUSD),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTransportUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTransportEUR),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTransportEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg1_TotalCostTransportCNY),
                Header = L["TransportRateDetailDto.Leg1_TotalCostTransportCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg1_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddGroup14Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_SearchTime),
                Header = L["TransportRateDetailDto.Leg1_SearchTime"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_SearchTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_LoadTime),
                Header = L["TransportRateDetailDto.Leg1_LoadTime"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_LoadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_DaysWaiting),
                Header = L["TransportRateDetailDto.Leg1_DaysWaiting"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_TravelTime),
                Header = L["TransportRateDetailDto.Leg1_TravelTime"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_TransportationTime),
                Header = L["TransportRateDetailDto.Leg1_TransportationTime"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg1_Distance),
                Header = L["TransportRateDetailDto.Leg1_Distance"],
                GroupHeader = L["TransportRateDetailDto.Group.14.LeadtimesRate1"],
                DisplaySelector = dto => dto.LeadTimeLeg1_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }

    private void AddGroup15Settings(List<DetailSetting<TransportRateDetailDto>> results, bool isRu)
    {
        var group = L["TransportRateDetailDto.Group.15.Leg2"];

        results.Add(new DetailSetting<TransportRateDetailDto>
        {
            Name = nameof(TransportRateDetailDto.Leg2_TransportTypeCode),
            Header = L["TransportRateDetailDto.TransportTypeCode"],
            GroupHeader = group,
            DisplaySelector = dto => dto.Leg2_TransportTypeCode!,
            VisibleSelector = dto => dto.ProxyNodeCode is not null
        });

        if (isRu)
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TransportTypeNameRu),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRu!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });
        else
            results.Add(new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TransportTypeNameRuEn),
                Header = L["TransportRateDetailDto.TransportTypeName"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TransportTypeNameRuEn!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            });

        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_EffectiveLoad),
                Header = L["TransportRateDetailDto.Leg2_EffectiveLoad"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_EffectiveLoad,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTon),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTon"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTon,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTransport),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTransport"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransport,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_BaseCurrency),
                Header = L["TransportRateDetailDto.Leg2_BaseCurrency"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_BaseCurrency!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTonRUB),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTonRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTonUSD),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTonUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTonEUR),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTonEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTonCNY),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTonCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTonCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTransportRUB),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTransportRUB"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportRUB,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTransportUSD),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTransportUSD"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportUSD,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTransportEUR),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTransportEUR"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportEUR,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.Leg2_TotalCostTransportCNY),
                Header = L["TransportRateDetailDto.Leg2_TotalCostTransportCNY"],
                GroupHeader = group,
                DisplaySelector = dto => dto.Leg2_TotalCostTransportCNY,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            }
        ]);
    }

    private void AddGroup16Settings(List<DetailSetting<TransportRateDetailDto>> results)
    {
        results.AddRange(
        [
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg2_TravelTime),
                Header = L["TransportRateDetailDto.Leg2_TravelTime"],
                GroupHeader = L["TransportRateDetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TravelTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg2_DaysWaiting),
                Header = L["TransportRateDetailDto.Leg2_DaysWaiting"],
                GroupHeader = L["TransportRateDetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_DaysWaiting!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg2_UploadTime),
                Header = L["TransportRateDetailDto.Leg2_UpLoadTime"],
                GroupHeader = L["TransportRateDetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_UploadTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg2_TransportationTime),
                Header = L["TransportRateDetailDto.Leg2_TransportationTime"],
                GroupHeader = L["TransportRateDetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_TransportationTime!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null
            },
            new DetailSetting<TransportRateDetailDto>
            {
                Name = nameof(TransportRateDetailDto.LeadTimeLeg2_Distance),
                Header = L["TransportRateDetailDto.Leg2_Distance"],
                GroupHeader = L["TransportRateDetailDto.Group.16.LeadtimesRate2"],
                DisplaySelector = dto => dto.LeadTimeLeg2_Distance!,
                VisibleSelector = dto => dto.ProxyNodeCode is not null,
                HasMargin = true
            }
        ]);
    }
}


