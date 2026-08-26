using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.Detail;

// ReSharper disable once InconsistentNaming
public class AverageRateLevel3DetailSettingsService(
    IStringLocalizer<Resources.AverageRateLevel3> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<AverageRateLevel3DetailDto>
{
    public DetailSettingsCollection<AverageRateLevel3DetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<AverageRateLevel3DetailDto>(platform);

        var g1 = L["AverageRateLevel3DetailDto.Group.1.RateParameters"];
        var g2 = L["AverageRateLevel3DetailDto.Group.2.RateDates"];
        var g3 = L["AverageRateLevel3DetailDto.Group.3.OriginDestination"];
        var g4 = L["AverageRateLevel3DetailDto.Group.4.Transport"];
        var g6 = L["AverageRateLevel3DetailDto.Group.6.Cargo"];
        var g7 = L["AverageRateLevel3DetailDto.Group.7.RateAmountExcludingVAT"];
        var g8 = L["AverageRateLevel3DetailDto.Group.8.RateComponentsExcludingVATPerTon"].Value;
        var g10 = L["AverageRateLevel3DetailDto.Group.10.Comments"];
        var g11 = L["AverageRateLevel3DetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"];
        var g12 = L["AverageRateLevel3DetailDto.Group.12.Leadtimes"];
        var g13 = L["AverageRateLevel3DetailDto.Group.13.Leg1"];
        var g14 = L["AverageRateLevel3DetailDto.Group.14.LeadtimesRate1"];
        var g15 = L["AverageRateLevel3DetailDto.Group.15.Leg2"];
        var g16 = L["AverageRateLevel3DetailDto.Group.16.LeadtimesRate2"];

        Func<AverageRateLevel3DetailDto, bool> hasProxy = dto => dto.ProxyNodeCode is not null;
        Func<AverageRateLevel3DetailDto, bool> hasLeadTime = dto => dto.LeadTimeCode is not null;

        b.Add(g1, x => x.Code, L["AverageRateLevel3DetailDto.Code"]);
        b.Add(g1, x => x.TransportRateCodes, L["AverageRateLevel3DetailDto.TransportRateCodes"],
            visible: dto => dto.TransportRateCodes is not null, display: dto => dto.TransportRateCodes!);
        b.Add(g1, x => x.TypeName, L["AverageRateLevel3DetailDto.RateTypeName"]);
        b.Add(g1, x => x.TypeCode, L["AverageRateLevel3DetailDto.RateTypeCode"]);
        b.AddYesNo(g1, x => x.IsDefRate, L["AverageRateLevel3DetailDto.IsDefRate"]);
        b.Add(g1, x => x.CreationDate, L["AverageRateLevel3DetailDto.CreationDate"], hasMargin: true);
        b.Add(g1, x => x.LastChangeDate, L["AverageRateLevel3DetailDto.LastChangeDate"]);
        b.AddArchiveStatus(g1, x => x.IsArchive, L["AverageRateLevel3DetailDto.IsArchive"]);

        b.Add(g2, x => x.StartDate, L["AverageRateLevel3DetailDto.StartDate"]);
        b.Add(g2, x => x.EndDate, L["AverageRateLevel3DetailDto.EndDate"]);

        b.AddLocalized(isRu, g3, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["AverageRateLevel3DetailDto.NodeFromName"]);
        b.Add(g3, x => x.NodeFromCode, L["AverageRateLevel3DetailDto.NodeFromCode"]);
        b.AddLocalized(isRu, g3, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["AverageRateLevel3DetailDto.RegionFromName"]);
        b.Add(g3, x => x.RegionFromCode, L["AverageRateLevel3DetailDto.RegionFromCode"]);
        b.AddLocalized(isRu, g3, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["AverageRateLevel3DetailDto.ProxyNodeName"],
            visible: hasProxy, hasMargin: true);
        b.Add(g3, x => x.ProxyNodeCode, L["AverageRateLevel3DetailDto.ProxyNodeCode"], visible: hasProxy);
        b.AddLocalized(isRu, g3, x => x.ProxyRegionNameRu, x => x.ProxyRegionNameEn, L["AverageRateLevel3DetailDto.ProxyRegionName"],
            visible: hasProxy);
        b.Add(g3, x => x.ProxyRegionCode, L["AverageRateLevel3DetailDto.ProxyRegionCode"], visible: hasProxy);
        b.AddLocalized(isRu, g3, x => x.NodeToNameRu, x => x.NodeToNameEn, L["AverageRateLevel3DetailDto.NodeToName"], hasMargin: true);
        b.Add(g3, x => x.NodeToCode, L["AverageRateLevel3DetailDto.NodeToCode"]);
        b.AddLocalized(isRu, g3, x => x.RegionToNameRu, x => x.RegionToNameEn, L["AverageRateLevel3DetailDto.RegionToName"]);
        b.Add(g3, x => x.RegionToCode, L["AverageRateLevel3DetailDto.RegionToCode"]);
        b.Add(g3, x => x.Basis, L["AverageRateLevel3DetailDto.Basis"],
            visible: dto => dto.Basis is not null, display: dto => dto.Basis!);
        if (isRu)
            b.Add(g3, x => x.BasisNodeNameRu, L["AverageRateLevel3DetailDto.BasisNodeNameRu"],
                visible: dto => dto.BasisNodeCode is not null, display: dto => dto.BasisNodeNameRu!);
        b.Add(g3, x => x.BasisNodeCode, L["AverageRateLevel3DetailDto.BasisNodeCode"],
            visible: dto => dto.BasisNodeCode is not null, display: dto => dto.BasisNodeCode!);

        b.AddLocalized(isRu, g4, x => x.TransportKindNameRu, x => x.TransportKindNameRuEn, L["AverageRateLevel3DetailDto.TransportKindName"]);
        b.Add(g4, x => x.TransportKindCode, L["AverageRateLevel3DetailDto.TransportKindCode"]);
        b.AddLocalized(isRu, g4, x => x.TransportTypeNameRu, x => x.TransportTypeNameRuEn, L["AverageRateLevel3DetailDto.TransportTypeName"]);
        b.Add(g4, x => x.TransportTypeCode, L["AverageRateLevel3DetailDto.TransportTypeCode"]);

        b.Add(g6, x => x.ProductGroupNameEnRu, L["AverageRateLevel3DetailDto.ProductGroupNameEnRu"],
            visible: dto => dto.ProductGroupCode is not null, display: dto => dto.ProductGroupNameEnRu!);
        b.AddLocalized(isRu, g6, x => x.ProductNameRu, x => x.ProductNameEn, L["AverageRateLevel3DetailDto.ProductName"],
            visible: dto => dto.ProductCode is not null, hasMargin: true);
        b.Add(g6, x => x.EffectiveLoadOfTransportType, L["AverageRateLevel3DetailDto.EffectiveLoadOfTransportType"]);
        b.Add(g6, x => x.ProductGroupCode, L["AverageRateLevel3DetailDto.ProductGroupCode"],
            visible: dto => dto.ProductGroupCode is not null, display: dto => dto.ProductGroupCode!);
        b.Add(g6, x => x.ProductCode, L["AverageRateLevel3DetailDto.ProductCode"],
            visible: dto => dto.ProductCode is not null, hasMargin: true, display: dto => dto.ProductCode!);
        b.Add(g6, x => x.ProductDPOCOde, L["AverageRateLevel3DetailDto.ProductDPOCOde"],
            visible: dto => dto.ProductDPOCOde is not null, display: dto => dto.ProductDPOCOde!);

        b.Add(g7, x => x.RateLevel3, L["AverageRateLevel3DetailDto.RateLevel3"]);
        b.Add(g7, x => x.CalcDate, L["AverageRateLevel3DetailDto.CalcDate"]);
        b.Add(g7, x => x.MinDailyTransportation, L["AverageRateLevel3DetailDto.MinDailyTransportation"]);
        b.Add(g7, x => x.MaxDailyTransportation, L["AverageRateLevel3DetailDto.MaxDailyTransportation"]);
        b.Add(g7, x => x.CurrencyStandard, L["AverageRateLevel3DetailDto.CurrencyStandard"]);
        b.Add(g7, x => x.CurrencyRateMonth, L["AverageRateLevel3DetailDto.CurrencyRateMonth"]);
        b.Add(g7, x => x.TotalCostTonRUB, L["AverageRateLevel3DetailDto.TotalCostTonRUB"]);

        AddFeePair(b, g8, x => x.LoadedRFSize, x => x.LoadedRFCurrency);
        AddFeePair(b, g8, x => x.LoadedCISSize, x => x.LoadedCISCurrency);
        AddFeePair(b, g8, x => x.EmptyRFSize, x => x.EmptyRFCurrency);
        AddFeePair(b, g8, x => x.EmptyCISSize, x => x.EmptyCISCurrency);
        AddFeePair(b, g8, x => x.ProvisionTransportSize, x => x.ProvisionTransportCurrency);
        AddFeePair(b, g8, x => x.TEFromSize, x => x.TEFromCurrency);
        AddFeePair(b, g8, x => x.PNPFromSize, x => x.PNPFromCurrency);
        AddFeePair(b, g8, x => x.TEFromSize_fix, x => x.TEFromCurrency_fix);
        AddFeePair(b, g8, x => x.TEToSize, x => x.TEToCurrency);
        AddFeePair(b, g8, x => x.PNPToSize, x => x.PNPToCurrency);
        AddFeePair(b, g8, x => x.TEToSize_fix, x => x.TEToCurrency_fix);
        AddFeePair(b, g8, x => x.DrainLoadingSize, x => x.DrainLoadingCurrency);
        AddFeePair(b, g8, x => x.TransshipmentSize, x => x.TransshipmentCurrency);
        AddFeePair(b, g8, x => x.FreightSize, x => x.FreightCurrency);
        AddFeePair(b, g8, x => x.AdditionalFeesCISSize, x => x.AdditionalFeesCISCurrency);
        AddFeePair(b, g8, x => x.FerryboatSize, x => x.FerryboatCurrency);

        b.Add(g10, x => x.Comment, L["AverageRateLevel3DetailDto.Comment"],
            visible: dto => dto.Comment is not null, display: dto => dto.Comment!);

        b.Add(g11, x => x.TotalCostTonUSD, L["AverageRateLevel3DetailDto.TotalCostTonUSD"]);
        b.Add(g11, x => x.TotalCostTonEUR, L["AverageRateLevel3DetailDto.TotalCostTonEUR"]);
        b.Add(g11, x => x.TotalCostTonCNY, L["AverageRateLevel3DetailDto.TotalCostTonCNY"]);
        b.Add(g11, x => x.TotalCostTransportRUB, L["AverageRateLevel3DetailDto.TotalCostTransportRUB"], hasMargin: true);
        b.Add(g11, x => x.TotalCostTransportUSD, L["AverageRateLevel3DetailDto.TotalCostTransportUSD"]);
        b.Add(g11, x => x.TotalCostTransportEUR, L["AverageRateLevel3DetailDto.TotalCostTransportEUR"]);
        b.Add(g11, x => x.TotalCostTransportCNY, L["AverageRateLevel3DetailDto.TotalCostTransportCNY"]);

        b.Add(g12, x => x.LegCode, L["AverageRateLevel3DetailDto.LegCode"]);
        b.Add(g12, x => x.LegChangeDate, L["AverageRateLevel3DetailDto.LegChangeDate"]);
        b.Add(g12, x => x.LeadTimeCode, L["AverageRateLevel3DetailDto.LeadTimeCode"],
            visible: hasLeadTime, hasMargin: true, display: dto => dto.LeadTimeCode!);
        b.Add(g12, x => x.LeadTimeStartDate, L["AverageRateLevel3DetailDto.LeadTimeStartDate"],
            visible: hasLeadTime, display: dto => dto.LeadTimeStartDate!);
        b.Add(g12, x => x.LeadTimeEndDate, L["AverageRateLevel3DetailDto.LeadTimeEndDate"],
            visible: hasLeadTime, display: dto => dto.LeadTimeEndDate!);
        b.Add(g12, x => x.LeadTimeChangeDate, L["AverageRateLevel3DetailDto.LeadTimeChangeDate"],
            visible: hasLeadTime, display: dto => dto.LeadTimeChangeDate!);
        b.Add(g12, x => x.LeadTimeSearchTime, L["AverageRateLevel3DetailDto.SearchTime"],
            visible: hasLeadTime, hasMargin: true, display: dto => dto.LeadTimeSearchTime!);
        b.Add(g12, x => x.LeadTimeLoadTime, L["AverageRateLevel3DetailDto.LoadTime"],
            visible: hasLeadTime, display: dto => dto.LeadTimeLoadTime!);
        b.Add(g12, x => x.LeadTimeDaysWaiting, L["AverageRateLevel3DetailDto.DaysWaiting"],
            visible: hasLeadTime, display: dto => dto.LeadTimeDaysWaiting!);
        b.Add(g12, x => x.LeadTimeTravelTime, L["AverageRateLevel3DetailDto.TravelTime"],
            visible: hasLeadTime, display: dto => dto.LeadTimeTravelTime!);
        b.Add(g12, x => x.LeadTimeUnLoadTime, L["AverageRateLevel3DetailDto.UnLoadTime"],
            visible: hasLeadTime, display: dto => dto.LeadTimeUnLoadTime!);
        b.Add(g12, x => x.LeadTimeTransportationTime, L["AverageRateLevel3DetailDto.TransportationTime"],
            visible: hasLeadTime, display: dto => dto.LeadTimeTransportationTime!);
        b.Add(g12, x => x.LeadTimeDistance, L["AverageRateLevel3DetailDto.Distance"],
            visible: hasLeadTime, hasMargin: true, display: dto => dto.LeadTimeDistance!);

        b.Add(g13, x => x.Leg1_TransportTypeCode, L["AverageRateLevel3DetailDto.TransportTypeCode"],
            visible: hasProxy, display: dto => dto.Leg1_TransportTypeCode!);
        b.AddLocalized(isRu, g13, x => x.Leg1_TransportTypeNameRu, x => x.Leg1_TransportTypeNameRuEn,
            L["AverageRateLevel3DetailDto.TransportTypeName"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_EffectiveLoad, L["AverageRateLevel3DetailDto.Leg1_EffectiveLoad"],
            visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTon, L["AverageRateLevel3DetailDto.Leg1_TotalCostTon"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransport, L["AverageRateLevel3DetailDto.Leg1_TotalCostTransport"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_BaseCurrency, L["AverageRateLevel3DetailDto.Leg1_BaseCurrency"],
            visible: hasProxy, display: dto => dto.Leg1_BaseCurrency!);
        b.Add(g13, x => x.Leg1_TotalCostTonRUB, L["AverageRateLevel3DetailDto.Leg1_TotalCostTonRUB"],
            visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTonUSD, L["AverageRateLevel3DetailDto.Leg1_TotalCostTonUSD"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTonEUR, L["AverageRateLevel3DetailDto.Leg1_TotalCostTonEUR"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTonCNY, L["AverageRateLevel3DetailDto.Leg1_TotalCostTonCNY"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportRUB, L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportRUB"],
            visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTransportUSD, L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportUSD"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportEUR, L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportEUR"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportCNY, L["AverageRateLevel3DetailDto.Leg1_TotalCostTransportCNY"], visible: hasProxy);

        b.Add(g14, x => x.LeadTimeLeg1_SearchTime, L["AverageRateLevel3DetailDto.Leg1_SearchTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg1_SearchTime!);
        b.Add(g14, x => x.LeadTimeLeg1_LoadTime, L["AverageRateLevel3DetailDto.Leg1_LoadTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg1_LoadTime!);
        b.Add(g14, x => x.LeadTimeLeg1_DaysWaiting, L["AverageRateLevel3DetailDto.Leg1_DaysWaiting"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg1_DaysWaiting!);
        b.Add(g14, x => x.LeadTimeLeg1_TravelTime, L["AverageRateLevel3DetailDto.Leg1_TravelTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg1_TravelTime!);
        b.Add(g14, x => x.LeadTimeLeg1_TransportationTime, L["AverageRateLevel3DetailDto.Leg1_TransportationTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg1_TransportationTime!);
        b.Add(g14, x => x.LeadTimeLeg1_Distance, L["AverageRateLevel3DetailDto.Leg1_Distance"],
            visible: hasProxy, hasMargin: true, display: dto => dto.LeadTimeLeg1_Distance!);

        b.Add(g15, x => x.Leg2_TransportTypeCode, L["AverageRateLevel3DetailDto.TransportTypeCode"],
            visible: hasProxy, display: dto => dto.Leg2_TransportTypeCode!);
        b.AddLocalized(isRu, g15, x => x.Leg2_TransportTypeNameRu, x => x.Leg2_TransportTypeNameRuEn,
            L["AverageRateLevel3DetailDto.TransportTypeName"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_EffectiveLoad, L["AverageRateLevel3DetailDto.Leg2_EffectiveLoad"],
            visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTon, L["AverageRateLevel3DetailDto.Leg2_TotalCostTon"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransport, L["AverageRateLevel3DetailDto.Leg2_TotalCostTransport"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_BaseCurrency, L["AverageRateLevel3DetailDto.Leg2_BaseCurrency"],
            visible: hasProxy, display: dto => dto.Leg2_BaseCurrency!);
        b.Add(g15, x => x.Leg2_TotalCostTonRUB, L["AverageRateLevel3DetailDto.Leg2_TotalCostTonRUB"],
            visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTonUSD, L["AverageRateLevel3DetailDto.Leg2_TotalCostTonUSD"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTonEUR, L["AverageRateLevel3DetailDto.Leg2_TotalCostTonEUR"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTonCNY, L["AverageRateLevel3DetailDto.Leg2_TotalCostTonCNY"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportRUB, L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportRUB"],
            visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTransportUSD, L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportUSD"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportEUR, L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportEUR"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportCNY, L["AverageRateLevel3DetailDto.Leg2_TotalCostTransportCNY"], visible: hasProxy);

        b.Add(g16, x => x.LeadTimeLeg2_TravelTime, L["AverageRateLevel3DetailDto.Leg2_TravelTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg2_TravelTime!);
        b.Add(g16, x => x.LeadTimeLeg2_DaysWaiting, L["AverageRateLevel3DetailDto.Leg2_DaysWaiting"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg2_DaysWaiting!);
        b.Add(g16, x => x.LeadTimeLeg2_UploadTime, L["AverageRateLevel3DetailDto.Leg2_UpLoadTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg2_UploadTime!);
        b.Add(g16, x => x.LeadTimeLeg2_TransportationTime, L["AverageRateLevel3DetailDto.Leg2_TransportationTime"],
            visible: hasProxy, display: dto => dto.LeadTimeLeg2_TransportationTime!);
        b.Add(g16, x => x.LeadTimeLeg2_Distance, L["AverageRateLevel3DetailDto.Leg2_Distance"],
            visible: hasProxy, hasMargin: true, display: dto => dto.LeadTimeLeg2_Distance!);

        return b.Build();
    }

    private void AddFeePair(
        DetailSettingsBuilder<AverageRateLevel3DetailDto> b,
        string group,
        System.Linq.Expressions.Expression<Func<AverageRateLevel3DetailDto, decimal>> sizeProperty,
        System.Linq.Expressions.Expression<Func<AverageRateLevel3DetailDto, string?>> currencyProperty)
    {
        var sizeName = sizeProperty.Body switch
        {
            System.Linq.Expressions.MemberExpression member => member.Member.Name,
            System.Linq.Expressions.UnaryExpression { Operand: System.Linq.Expressions.MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException($"Expression must be a property access: {sizeProperty}", nameof(sizeProperty))
        };
        var currencyName = currencyProperty.Body switch
        {
            System.Linq.Expressions.MemberExpression member => member.Member.Name,
            System.Linq.Expressions.UnaryExpression { Operand: System.Linq.Expressions.MemberExpression member } => member.Member.Name,
            _ => throw new ArgumentException($"Expression must be a property access: {currencyProperty}", nameof(currencyProperty))
        };

        var sizeCompiled = sizeProperty.Compile();
        b.Add(group, sizeProperty, L[$"AverageRateLevel3DetailDto.{sizeName}"],
            display: dto => sizeCompiled(dto) != 0 ? sizeCompiled(dto) : string.Empty);
        b.Add(group, currencyProperty, L[$"AverageRateLevel3DetailDto.{currencyName}"],
            display: dto => currencyProperty.Compile()(dto)!);
    }
}
