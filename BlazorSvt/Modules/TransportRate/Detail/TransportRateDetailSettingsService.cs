using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.Detail;

// ReSharper disable once InconsistentNaming
public class TransportRateDetailSettingsService(
    IStringLocalizer<Resources.TransportRate> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<TransportRateDetailDto>
{
    public DetailSettingsCollection<TransportRateDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportRateDetailDto>(platform);

        var g1 = L["TransportRateDetailDto.Group.1.RateParameters"];
        var g2 = L["TransportRateDetailDto.Group.2.RateDates"];
        var g3 = L["TransportRateDetailDto.Group.3.OriginDestination"];
        var g4 = L["TransportRateDetailDto.Group.4.Transport"];
        var g5 = L["TransportRateDetailDto.Group.5.MultimodalTransportation"];
        var g6 = L["TransportRateDetailDto.Group.6.Cargo"];
        var g7 = L["TransportRateDetailDto.Group.7.RateAmountExcludingVAT"];
        var g8 = L["TransportRateDetailDto.Group.8.RateComponentsExcludingVATPerTon"];
        var g10 = L["TransportRateDetailDto.Group.10.Comments"];
        var g11 = L["TransportRateDetailDto.Group.11.ReferenceCalculatedCostsInCurrencies"];
        var g12 = L["TransportRateDetailDto.Group.12.Leadtimes"];
        var g13 = L["TransportRateDetailDto.Group.13.Leg1"];
        var g14 = L["TransportRateDetailDto.Group.14.LeadtimesRate1"];
        var g15 = L["TransportRateDetailDto.Group.15.Leg2"];
        var g16 = L["TransportRateDetailDto.Group.16.LeadtimesRate2"];

        Func<TransportRateDetailDto, bool> hasProxy = dto => dto.ProxyNodeCode is not null;
        Func<TransportRateDetailDto, bool> hasLeadTime = dto => dto.LeadTimeCode is not null;
        Func<TransportRateDetailDto, bool> notTc = dto => dto.CalcType != "ТС";

        b.Add(g1, x => x.Code, L["TransportRateDetailDto.Code"]);
        b.Add(g1, x => x.AverageRateCode, L["TransportRateDetailDto.AverageRateCode"]);
        b.Add(g1, x => x.TypeName, L["TransportRateDetailDto.RateTypeName"]);
        b.Add(g1, x => x.TypeCode, L["TransportRateDetailDto.RateTypeCode"]);
        b.Add(g1, x => x.Nomination, L["TransportRateDetailDto.Nomination"], visible: dto => dto.Nomination is not null, hasMargin: true);
        b.Add(g1, x => x.ContractorNameSearch, L["TransportRateDetailDto.ContractorNameSearch"], visible: dto => dto.ContractorCode is not null);
        b.Add(g1, x => x.TenderNumber, L["TransportRateDetailDto.TenderNumber"], visible: dto => dto.TenderNumber is not null);
        b.Add(g1, x => x.AdditionalAgreementNumber, L["TransportRateDetailDto.AdditionalAgreementNumber"], visible: dto => dto.AdditionalAgreementNumber is not null);
        b.AddYesNo(g1, x => x.IsDefRate, L["TransportRateDetailDto.IsDefRate"]);
        b.Add(g1, x => x.CreationDate, L["TransportRateDetailDto.CreationDate"], hasMargin: true);
        b.Add(g1, x => x.LastChangeDate, L["TransportRateDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(g1, x => x.IsArchive, L["TransportRateDetailDto.IsArchive"]);

        b.Add(g2, x => x.StartDate, L["TransportRateDetailDto.StartDate"]);
        b.Add(g2, x => x.EndDate, L["TransportRateDetailDto.EndDate"]);

        b.AddLocalized(isRu, g3, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["TransportRateDetailDto.NodeFromName"]);
        b.Add(g3, x => x.NodeFromCode, L["TransportRateDetailDto.NodeFromCode"]);
        b.AddLocalized(isRu, g3, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["TransportRateDetailDto.RegionFromName"]);
        b.Add(g3, x => x.RegionFromCode, L["TransportRateDetailDto.RegionFromCode"]);
        b.AddLocalized(isRu, g3, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportRateDetailDto.ProxyNodeName"], visible: hasProxy, hasMargin: true);
        b.Add(g3, x => x.ProxyNodeCode, L["TransportRateDetailDto.ProxyNodeCode"], visible: hasProxy);
        b.AddLocalized(isRu, g3, x => x.ProxyRegionNameRu, x => x.ProxyRegionNameEn, L["TransportRateDetailDto.ProxyRegionName"], visible: hasProxy);
        b.Add(g3, x => x.ProxyRegionCode, L["TransportRateDetailDto.ProxyRegionCode"], visible: hasProxy);
        b.AddLocalized(isRu, g3, x => x.NodeToNameRu, x => x.NodeToNameEn, L["TransportRateDetailDto.NodeToName"], hasMargin: true);
        b.Add(g3, x => x.NodeToCode, L["TransportRateDetailDto.NodeToCode"]);
        b.AddLocalized(isRu, g3, x => x.RegionToNameRu, x => x.RegionToNameEn, L["TransportRateDetailDto.RegionToName"]);
        b.Add(g3, x => x.RegionToCode, L["TransportRateDetailDto.RegionToCode"]);
        b.Add(g3, x => x.Basis, L["TransportRateDetailDto.Basis"], visible: dto => dto.Basis is not null);
        if (isRu)
            b.Add(g3, x => x.BasisNodeNameRu, L["TransportRateDetailDto.BasisNodeNameRu"], visible: dto => dto.BasisNodeCode is not null);
        b.Add(g3, x => x.BasisNodeCode, L["TransportRateDetailDto.BasisNodeCode"], visible: dto => dto.BasisNodeCode is not null);

        b.AddLocalized(isRu, g4, x => x.TransportKindNameRu, x => x.TransportKindNameRuEn, L["TransportRateDetailDto.TransportKindName"]);
        b.Add(g4, x => x.TransportKindCode, L["TransportRateDetailDto.TransportKindCode"]);
        b.AddLocalized(isRu, g4, x => x.TransportTypeNameRu, x => x.TransportTypeNameRuEn, L["TransportRateDetailDto.TransportTypeName"]);
        b.Add(g4, x => x.TransportTypeCode, L["TransportRateDetailDto.TransportTypeCode"]);

        b.Add(g5, x => x.TenderServicePack, L["TransportRateDetailDto.TenderServicePack"], visible: dto => dto.TenderServicePack is not null);

        b.Add(g6, x => x.ProductGroupNameEnRu, L["TransportRateDetailDto.ProductGroupNameEnRu"], visible: dto => dto.ProductGroupCode is not null);
        b.AddLocalized(isRu, g6, x => x.ProductNameRu, x => x.ProductNameEn, L["TransportRateDetailDto.ProductName"], visible: dto => dto.ProductCode is not null, hasMargin: true);
        b.Add(g6, x => x.EffectiveLoadOfTransportType, L["TransportRateDetailDto.EffectiveLoadOfTransportType"]);
        b.Add(g6, x => x.ProductGroupCode, L["TransportRateDetailDto.ProductGroupCode"], visible: dto => dto.ProductGroupCode is not null);
        b.Add(g6, x => x.ProductCode, L["TransportRateDetailDto.ProductCode"], visible: dto => dto.ProductCode is not null, hasMargin: true);
        b.Add(g6, x => x.ProductDPOCOde, L["TransportRateDetailDto.ProductDPOCOde"], visible: dto => dto.ProductDPOCOde is not null);
        b.Add(g6, x => x.ContractorCode, L["TransportRateDetailDto.ContractorCode"], visible: dto => dto.ContractorCode is not null, hasMargin: true);
        b.Add(g6, x => x.ContractorEGRUL, L["TransportRateDetailDto.ContractorEGRUL"], visible: dto => dto.ContractorCode is not null);

        b.Add(g7, x => x.CalcType, L["TransportRateDetailDto.CalcType"]);
        b.Add(g7, x => x.TotalCostTransport, L["TransportRateDetailDto.TotalCostTransport"]);
        b.Add(g7, x => x.TotalCostTon, L["TransportRateDetailDto.TotalCostTon"]);
        b.Add(g7, x => x.CurrencyStandard, L["TransportRateDetailDto.Currency"]);
        b.Add(g7, x => x.CurrencyRateMonth, L["TransportRateDetailDto.CurrencyRateMonth"]);
        b.Add(g7, x => x.TotalCostTonRUB, L["TransportRateDetailDto.TotalCostTonRUB"]);

        b.Add(g8, x => x.LoadedRFSize, L["TransportRateDetailDto.LoadedRFSize"], display: dto => dto.LoadedRFSize != 0 ? dto.LoadedRFSize : string.Empty);
        b.Add(g8, x => x.LoadedRFCurrency, L["TransportRateDetailDto.LoadedRFCurrency"]);
        b.Add(g8, x => x.LoadedCISSize, L["TransportRateDetailDto.LoadedCISSize"], visible: notTc, display: dto => dto.LoadedCISSize != 0 ? dto.LoadedCISSize : string.Empty);
        b.Add(g8, x => x.LoadedCISCurrency, L["TransportRateDetailDto.LoadedCISCurrency"], visible: notTc);
        b.Add(g8, x => x.EmptyRFSize, L["TransportRateDetailDto.EmptyRFSize"], visible: notTc, display: dto => dto.EmptyRFSize != 0 ? dto.EmptyRFSize : string.Empty);
        b.Add(g8, x => x.EmptyRFCurrency, L["TransportRateDetailDto.EmptyRFCurrency"], visible: notTc);
        b.Add(g8, x => x.EmptyCISSize, L["TransportRateDetailDto.EmptyCISSize"], visible: notTc, display: dto => dto.EmptyCISSize != 0 ? dto.EmptyCISSize : string.Empty);
        b.Add(g8, x => x.EmptyCISCurrency, L["TransportRateDetailDto.EmptyCISCurrency"], visible: notTc);
        b.Add(g8, x => x.ProvisionTransportSize, L["TransportRateDetailDto.ProvisionTransportSize"], visible: notTc, display: dto => dto.ProvisionTransportSize != 0 ? dto.ProvisionTransportSize : string.Empty);
        b.Add(g8, x => x.ProvisionTransportCurrency, L["TransportRateDetailDto.ProvisionTransportCurrency"], visible: notTc);
        b.Add(g8, x => x.TEFromSize, L["TransportRateDetailDto.TEFromSize"], visible: notTc, display: dto => dto.TEFromSize != 0 ? dto.TEFromSize : string.Empty);
        b.Add(g8, x => x.TEFromCurrency, L["TransportRateDetailDto.TEFromCurrency"], visible: notTc);
        b.Add(g8, x => x.RatePNPFromSize, L["TransportRateDetailDto.RatePNPFromSize"], visible: notTc, display: dto => dto.RatePNPFromSize != 0 ? dto.RatePNPFromSize : string.Empty);
        b.Add(g8, x => x.PNPFromCurrency, L["TransportRateDetailDto.PNPFromCurrency"], visible: notTc);
        b.Add(g8, x => x.TEFromSize_fix, L["TransportRateDetailDto.TEFromSize_fix"], visible: notTc, display: dto => dto.TEFromSize_fix != 0 ? dto.TEFromSize_fix : string.Empty);
        b.Add(g8, x => x.TEFromCurrency_fix, L["TransportRateDetailDto.TEFromCurrency_fix"], visible: notTc);
        b.Add(g8, x => x.TEToSize, L["TransportRateDetailDto.TEToSize"], visible: notTc, display: dto => dto.TEToSize != 0 ? dto.TEToSize : string.Empty);
        b.Add(g8, x => x.TEToCurrency, L["TransportRateDetailDto.TEToCurrency"], visible: notTc);
        b.Add(g8, x => x.PNPToSize, L["TransportRateDetailDto.PNPToSize"], visible: notTc, display: dto => dto.PNPToSize != 0 ? dto.PNPToSize : string.Empty);
        b.Add(g8, x => x.PNPToCurrency, L["TransportRateDetailDto.PNPToCurrency"], visible: notTc);
        b.Add(g8, x => x.TEToSize_fix, L["TransportRateDetailDto.TEToSize_fix"], visible: notTc, display: dto => dto.TEToSize_fix != 0 ? dto.TEToSize_fix : string.Empty);
        b.Add(g8, x => x.TEToCurrency_fix, L["TransportRateDetailDto.TEToCurrency_fix"], visible: notTc);
        b.Add(g8, x => x.DrainLoadingSize, L["TransportRateDetailDto.DrainLoadingSize"], visible: notTc, display: dto => dto.DrainLoadingSize != 0 ? dto.DrainLoadingSize : string.Empty);
        b.Add(g8, x => x.DrainLoadingCurrency, L["TransportRateDetailDto.DrainLoadingCurrency"], visible: notTc);
        b.Add(g8, x => x.TransshipmentSize, L["TransportRateDetailDto.TransshipmentSize"], visible: notTc, display: dto => dto.TransshipmentSize != 0 ? dto.TransshipmentSize : string.Empty);
        b.Add(g8, x => x.TransshipmentCurrency, L["TransportRateDetailDto.TransshipmentCurrency"], visible: notTc);
        b.Add(g8, x => x.FreightSize, L["TransportRateDetailDto.FreightSize"], visible: notTc, display: dto => dto.FreightSize != 0 ? dto.FreightSize : string.Empty);
        b.Add(g8, x => x.FreightCurrency, L["TransportRateDetailDto.FreightCurrency"], visible: notTc);
        b.Add(g8, x => x.AdditionalFeesCISSize, L["TransportRateDetailDto.AdditionalFeesCISSize"], visible: notTc, display: dto => dto.AdditionalFeesCISSize != 0 ? dto.AdditionalFeesCISSize : string.Empty);
        b.Add(g8, x => x.AdditionalFeesCISCurrency, L["TransportRateDetailDto.AdditionalFeesCISCurrency"], visible: notTc);
        b.Add(g8, x => x.FerryboatSize, L["TransportRateDetailDto.FerryboatSize"], visible: notTc, display: dto => dto.FerryboatSize != 0 ? dto.FerryboatSize : string.Empty);
        b.Add(g8, x => x.FerryboatCurrency, L["TransportRateDetailDto.FerryboatCurrency"], visible: notTc);

        b.Add(g10, x => x.Comment, L["TransportRateDetailDto.Comment"], visible: dto => dto.Comment is not null);

        b.Add(g11, x => x.TotalCostTonUSD, L["TransportRateDetailDto.TotalCostTonUSD"]);
        b.Add(g11, x => x.TotalCostTonEUR, L["TransportRateDetailDto.TotalCostTonEUR"]);
        b.Add(g11, x => x.TotalCostTonCNY, L["TransportRateDetailDto.TotalCostTonCNY"]);
        b.Add(g11, x => x.TotalCostTransportRUB, L["TransportRateDetailDto.TotalCostTransportRUB"], hasMargin: true);
        b.Add(g11, x => x.TotalCostTransportUSD, L["TransportRateDetailDto.TotalCostTransportUSD"]);
        b.Add(g11, x => x.TotalCostTransportEUR, L["TransportRateDetailDto.TotalCostTransportEUR"]);
        b.Add(g11, x => x.TotalCostTransportCNY, L["TransportRateDetailDto.TotalCostTransportCNY"]);

        b.Add(g12, x => x.LegCode, L["TransportRateDetailDto.LegCode"]);
        b.Add(g12, x => x.LegChangeDate, L["TransportRateDetailDto.LegChangeDate"]);
        b.Add(g12, x => x.LeadTimeCode, L["TransportRateDetailDto.LeadTimeCode"], visible: hasLeadTime, hasMargin: true);
        b.Add(g12, x => x.LeadTimeStartDate, L["TransportRateDetailDto.LeadTimeStartDate"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeEndDate, L["TransportRateDetailDto.LeadTimeEndDate"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeChangeDate, L["TransportRateDetailDto.LeadTimeChangeDate"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeSearchTime, L["TransportRateDetailDto.SearchTime"], visible: hasLeadTime, hasMargin: true);
        b.Add(g12, x => x.LeadTimeLoadTime, L["TransportRateDetailDto.LoadTime"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeDaysWaiting, L["TransportRateDetailDto.DaysWaiting"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeTravelTime, L["TransportRateDetailDto.TravelTime"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeUnLoadTime, L["TransportRateDetailDto.UnLoadTime"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeTransportationTime, L["TransportRateDetailDto.TransportationTime"], visible: hasLeadTime);
        b.Add(g12, x => x.LeadTimeDistance, L["TransportRateDetailDto.Distance"], visible: hasLeadTime, hasMargin: true);

        b.Add(g13, x => x.Leg1_TransportTypeCode, L["TransportRateDetailDto.TransportTypeCode"], visible: hasProxy);
        b.AddLocalized(isRu, g13, x => x.Leg1_TransportTypeNameRu, x => x.Leg1_TransportTypeNameRuEn, L["TransportRateDetailDto.TransportTypeName"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_EffectiveLoad, L["TransportRateDetailDto.Leg1_EffectiveLoad"], visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTon, L["TransportRateDetailDto.Leg1_TotalCostTon"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransport, L["TransportRateDetailDto.Leg1_TotalCostTransport"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_BaseCurrency, L["TransportRateDetailDto.Leg1_BaseCurrency"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTonRUB, L["TransportRateDetailDto.Leg1_TotalCostTonRUB"], visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTonUSD, L["TransportRateDetailDto.Leg1_TotalCostTonUSD"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTonEUR, L["TransportRateDetailDto.Leg1_TotalCostTonEUR"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTonCNY, L["TransportRateDetailDto.Leg1_TotalCostTonCNY"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportRUB, L["TransportRateDetailDto.Leg1_TotalCostTransportRUB"], visible: hasProxy, hasMargin: true);
        b.Add(g13, x => x.Leg1_TotalCostTransportUSD, L["TransportRateDetailDto.Leg1_TotalCostTransportUSD"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportEUR, L["TransportRateDetailDto.Leg1_TotalCostTransportEUR"], visible: hasProxy);
        b.Add(g13, x => x.Leg1_TotalCostTransportCNY, L["TransportRateDetailDto.Leg1_TotalCostTransportCNY"], visible: hasProxy);

        b.Add(g14, x => x.LeadTimeLeg1_SearchTime, L["TransportRateDetailDto.Leg1_SearchTime"], visible: hasProxy);
        b.Add(g14, x => x.LeadTimeLeg1_LoadTime, L["TransportRateDetailDto.Leg1_LoadTime"], visible: hasProxy);
        b.Add(g14, x => x.LeadTimeLeg1_DaysWaiting, L["TransportRateDetailDto.Leg1_DaysWaiting"], visible: hasProxy);
        b.Add(g14, x => x.LeadTimeLeg1_TravelTime, L["TransportRateDetailDto.Leg1_TravelTime"], visible: hasProxy);
        b.Add(g14, x => x.LeadTimeLeg1_TransportationTime, L["TransportRateDetailDto.Leg1_TransportationTime"], visible: hasProxy);
        b.Add(g14, x => x.LeadTimeLeg1_Distance, L["TransportRateDetailDto.Leg1_Distance"], visible: hasProxy, hasMargin: true);

        b.Add(g15, x => x.Leg2_TransportTypeCode, L["TransportRateDetailDto.TransportTypeCode"], visible: hasProxy);
        b.AddLocalized(isRu, g15, x => x.Leg2_TransportTypeNameRu, x => x.Leg2_TransportTypeNameRuEn, L["TransportRateDetailDto.TransportTypeName"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_EffectiveLoad, L["TransportRateDetailDto.Leg2_EffectiveLoad"], visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTon, L["TransportRateDetailDto.Leg2_TotalCostTon"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransport, L["TransportRateDetailDto.Leg2_TotalCostTransport"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_BaseCurrency, L["TransportRateDetailDto.Leg2_BaseCurrency"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTonRUB, L["TransportRateDetailDto.Leg2_TotalCostTonRUB"], visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTonUSD, L["TransportRateDetailDto.Leg2_TotalCostTonUSD"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTonEUR, L["TransportRateDetailDto.Leg2_TotalCostTonEUR"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTonCNY, L["TransportRateDetailDto.Leg2_TotalCostTonCNY"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportRUB, L["TransportRateDetailDto.Leg2_TotalCostTransportRUB"], visible: hasProxy, hasMargin: true);
        b.Add(g15, x => x.Leg2_TotalCostTransportUSD, L["TransportRateDetailDto.Leg2_TotalCostTransportUSD"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportEUR, L["TransportRateDetailDto.Leg2_TotalCostTransportEUR"], visible: hasProxy);
        b.Add(g15, x => x.Leg2_TotalCostTransportCNY, L["TransportRateDetailDto.Leg2_TotalCostTransportCNY"], visible: hasProxy);

        b.Add(g16, x => x.LeadTimeLeg2_TravelTime, L["TransportRateDetailDto.Leg2_TravelTime"], visible: hasProxy);
        b.Add(g16, x => x.LeadTimeLeg2_DaysWaiting, L["TransportRateDetailDto.Leg2_DaysWaiting"], visible: hasProxy);
        b.Add(g16, x => x.LeadTimeLeg2_UploadTime, L["TransportRateDetailDto.Leg2_UpLoadTime"], visible: hasProxy);
        b.Add(g16, x => x.LeadTimeLeg2_TransportationTime, L["TransportRateDetailDto.Leg2_TransportationTime"], visible: hasProxy);
        b.Add(g16, x => x.LeadTimeLeg2_Distance, L["TransportRateDetailDto.Leg2_Distance"], visible: hasProxy, hasMargin: true);

        return b.Build();
    }
}
