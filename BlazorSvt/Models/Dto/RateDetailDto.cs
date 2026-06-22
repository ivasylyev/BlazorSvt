
using BlazorSvt.Models.Config;

namespace BlazorSvt.Models.Dto;

[TableFunction("v2.fn_GetTransportRateDetail", "RateId")]
[FullReportExport("v2.ExportTransportRatesFull")]
public class RateDetailDto
{
    public long RateId { get; set; }
    public required long Code { get; set; }

    public bool IsArchive { get; set; }
    public bool IsDefRate { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastChangeDate { get; set; }

    public required string AverageRateCode { get; set; }
    public decimal AverageRateLevel3TotalCostTon { get; set; }
    public decimal TotalCostTon { get; set; }
    public decimal TotalCostTransport { get; set; }
    public required string CalcType { get; set; }
    public required string CurrencyStandard { get; set; }
    public DateOnly CurrencyRateMonth { get; set; }
    public decimal EffectiveLoadOfTransportType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public required string TypeCode { get; set; }
    public required string TypeName { get; set; }
    public decimal TotalCostTonRUB { get; set; }
    public decimal TotalCostTonEUR { get; set; }
    public decimal TotalCostTonCNY { get; set; }
    public decimal TotalCostTonUSD { get; set; }
    public decimal TotalCostTransportRUB { get; set; }
    public decimal TotalCostTransportEUR { get; set; }
    public decimal TotalCostTransportCNY { get; set; }
    public decimal TotalCostTransportUSD { get; set; }
    public decimal EmptyRFSize { get; set; }
    public string? EmptyRFCurrency { get; set; }
    public decimal EmptyCISSize { get; set; }
    public string? EmptyCISCurrency { get; set; }
    public decimal ProvisionTransportSize { get; set; }
    public string? ProvisionTransportCurrency { get; set; }
    public decimal FerryboatSize { get; set; }
    public string? FerryboatCurrency { get; set; }
    public decimal TEFromSize { get; set; }
    public string? TEFromCurrency { get; set; }
    public decimal RatePNPFromSize { get; set; }
    public string? PNPFromCurrency { get; set; }
    public decimal TEToSize { get; set; }
    public string? TEToCurrency { get; set; }
    public decimal PNPToSize { get; set; }
    public string? PNPToCurrency { get; set; }
    public decimal DrainLoadingSize { get; set; }
    public string? DrainLoadingCurrency { get; set; }
    public decimal TransshipmentSize { get; set; }
    public string? TransshipmentCurrency { get; set; }
    public decimal FreightSize { get; set; }
    public string? FreightCurrency { get; set; }
    public decimal AdditionalFeesCISSize { get; set; }
    public string? AdditionalFeesCISCurrency { get; set; }
    public decimal LoadedCISSize { get; set; }
    public string? LoadedCISCurrency { get; set; }
    public decimal LoadedRFSize { get; set; }
    public string? LoadedRFCurrency { get; set; }
    public decimal TEFromSize_fix { get; set; }
    public string? TEFromCurrency_fix { get; set; }
    public decimal TEToSize_fix { get; set; }
    public string? TEToCurrency_fix { get; set; }

    public required string NodeFromCode { get; set; }
    public required string NodeFromNameRu { get; set; }
    public required string NodeFromNameEn { get; set; }
    public string? RegionFromCode { get; set; }
    public string? RegionFromNameRu { get; set; }
    public string? RegionFromNameEn { get; set; }
    public string? ProxyNodeCode { get; set; }
    public string? ProxyNodeNameRu { get; set; }
    public string? ProxyNodeNameEn { get; set; }
    public string? ProxyRegionCode { get; set; }
    public string? ProxyRegionNameRu { get; set; }
    public string? ProxyRegionNameEn { get; set; }
    public required string NodeToCode { get; set; }
    public required string NodeToNameRu { get; set; }
    public required string NodeToNameEn { get; set; }
    public string? RegionToCode { get; set; }
    public string? RegionToNameRu { get; set; }
    public string? RegionToNameEn { get; set; }
    public string? Basis { get; set; }
    public string? BasisNodeCode { get; set; }
    public string? BasisNodeNameRu { get; set; }
    public required string TransportKindCode { get; set; }
    public required string TransportKindNameRu { get; set; }
    public required string TransportKindNameRuEn { get; set; }
    public required string TransportTypeCode { get; set; }
    public required string TransportTypeNameRu { get; set; }
    public required string TransportTypeNameRuEn { get; set; }
    public string? ProductGroupCode { get; set; }
    public string? ProductGroupNameEnRu { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductNameRu { get; set; }
    public string? ProductNameEn { get; set; }
    public string? ProductDPOCOde { get; set; }
    public string? ContractorCode { get; set; }
    public string? ContractorNameSearch { get; set; }
    public string? ContractorEGRUL { get; set; }
    public string? Nomination { get; set; }
    public string? TenderServicePack { get; set; }
    public string? TenderNumber { get; set; }
    public string? AdditionalAgreementNumber { get; set; }
    public string? Comment { get; set; }
    public required string LegCode { get; set; }
    public DateTime LegChangeDate { get; set; }
    public DateTime? LeadTimeChangeDate { get; set; }
    public DateOnly? LeadTimeStartDate { get; set; }
    public DateOnly? LeadTimeEndDate { get; set; }
    public string? LeadTimeCode { get; set; }
    public decimal? LeadTimeSearchTime { get; set; }
    public decimal? LeadTimeLoadTime { get; set; }
    public decimal? LeadTimeTravelTime { get; set; }
    public decimal? LeadTimeDaysWaiting { get; set; }
    public decimal? LeadTimeUnLoadTime { get; set; }
    public decimal? LeadTimeTransportationTime { get; set; }
    public int? LeadTimeDistance { get; set; }
    public string? Leg1_TransportTypeCode { get; set; }
    public string? Leg1_TransportTypeNameRu { get; set; }
    public string? Leg1_TransportTypeNameRuEn { get; set; }
    public decimal Leg1_EffectiveLoad { get; set; }
    public decimal Leg1_TotalCostTon { get; set; }
    public decimal Leg1_TotalCostTransport { get; set; }
    public string? Leg1_BaseCurrency { get; set; }
    public decimal Leg1_TotalCostTonRUB { get; set; }
    public decimal Leg1_TotalCostTonUSD { get; set; }
    public decimal Leg1_TotalCostTonEUR { get; set; }
    public decimal Leg1_TotalCostTonCNY { get; set; }
    public decimal Leg1_TotalCostTransportRUB { get; set; }
    public decimal Leg1_TotalCostTransportUSD { get; set; }
    public decimal Leg1_TotalCostTransportEUR { get; set; }
    public decimal Leg1_TotalCostTransportCNY { get; set; }
    public decimal? LeadTimeLeg1_SearchTime { get; set; }
    public decimal? LeadTimeLeg1_LoadTime { get; set; }
    public decimal? LeadTimeLeg1_TravelTime { get; set; }
    public decimal? LeadTimeLeg1_DaysWaiting { get; set; }
    public decimal? LeadTimeLeg1_TransportationTime { get; set; }
    public int? LeadTimeLeg1_Distance { get; set; }
    public string? Leg2_TransportTypeCode { get; set; }
    public string? Leg2_TransportTypeNameRu { get; set; }
    public string? Leg2_TransportTypeNameRuEn { get; set; }
    public decimal Leg2_EffectiveLoad { get; set; }
    public decimal Leg2_TotalCostTon { get; set; }
    public decimal Leg2_TotalCostTransport { get; set; }
    public string? Leg2_BaseCurrency { get; set; }
    public decimal Leg2_TotalCostTonRUB { get; set; }
    public decimal Leg2_TotalCostTonUSD { get; set; }
    public decimal Leg2_TotalCostTonEUR { get; set; }
    public decimal Leg2_TotalCostTonCNY { get; set; }
    public decimal Leg2_TotalCostTransportRUB { get; set; }
    public decimal Leg2_TotalCostTransportUSD { get; set; }
    public decimal Leg2_TotalCostTransportEUR { get; set; }
    public decimal Leg2_TotalCostTransportCNY { get; set; }
    public decimal? LeadTimeLeg2_TravelTime { get; set; }
    public decimal? LeadTimeLeg2_DaysWaiting { get; set; }
    public decimal? LeadTimeLeg2_UploadTime { get; set; }
    public decimal? LeadTimeLeg2_TransportationTime { get; set; }
    public int? LeadTimeLeg2_Distance { get; set; }
}
