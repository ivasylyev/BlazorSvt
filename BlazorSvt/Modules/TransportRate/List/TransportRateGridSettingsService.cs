using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.List;

// ReSharper disable once InconsistentNaming
public class TransportRateGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.TransportRate> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<TransportRateGridSettingsService> logger)
    : BaseGridSettingsService<TransportRateDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportRateGridColumnSettings";

    protected override List<GridColumnSetting<TransportRateDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<TransportRateDto>(platform);

        b.Add(x => x.Code, L["TransportRateDto.Code"]);
        b.AddYesNo(x => x.IsDefRate, L["TransportRateDto.IsDefRate"]);
        b.AddEnum(isRu, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["TransportRateDto.RateTypeName"]);
        b.Add(x => x.TransportKindCode, L["TransportRateDto.TransportKindCode"], visible: false, filterable: false);
        b.AddEnum(isRu, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportRateDto.TransportKindName"]);
        b.Add(x => x.TransportTypeCode, L["TransportRateDto.TransportTypeCode"], visible: false, filterable: false);
        b.AddEnum(isRu, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["TransportRateDto.TransportTypeName"]);
        b.Add(x => x.NodeFromCode, L["TransportRateDto.NodeFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["TransportRateDto.NodeFromName"]);
        b.Add(x => x.ProxyNodeCode, L["TransportRateDto.ProxyNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportRateDto.ProxyNodeName"]);
        b.Add(x => x.NodeToCode, L["TransportRateDto.NodeToCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeToNameRu, x => x.NodeToNameEn, L["TransportRateDto.NodeToName"]);
        b.Add(x => x.ProductGroupCode, L["TransportRateDto.ProductGroupCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["TransportRateDto.ProductGroupName"]);
        b.Add(x => x.ProductCode, L["TransportRateDto.ProductCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductNameRu, x => x.ProductNameEn, L["TransportRateDto.ProductName"]);
        b.AddDateOnly(x => x.StartDate, L["TransportRateDto.StartDate"]);
        b.AddDateOnly(x => x.EndDate, L["TransportRateDto.EndDate"]);
        b.Add(x => x.TotalCostTon, L["TransportRateDto.TotalCostTon"], filterable: false);
        b.Add(x => x.TotalCostTransport, L["TransportRateDto.TotalCostTransport"], filterable: false);
        b.AddEnum(x => x.CurrencyId, L["TransportRateDto.Currency"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["TransportRateDto.CreationDate"],
            L["TransportRateDto.LastChangeDate"],
            L["TransportRateDto.IsArchive"]);

        return b.Build();
    }
}
