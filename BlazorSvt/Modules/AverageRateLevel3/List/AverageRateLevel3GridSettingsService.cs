using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.List;

// ReSharper disable once InconsistentNaming
public class AverageRateLevel3GridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.AverageRateLevel3> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<AverageRateLevel3GridSettingsService> logger)
    : BaseGridSettingsService<AverageRateLevel3Dto>(localStorage, logger)
{
    protected override string StorageKey => "AverageRateLevel3GridColumnSettings";

    protected override List<GridColumnSetting<AverageRateLevel3Dto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<AverageRateLevel3Dto>(platform);

        b.Add(x => x.Code, L["AverageRateLevel3Dto.Code"]);
        b.AddYesNo(x => x.IsDefRate, L["AverageRateLevel3Dto.IsDefRate"]);
        b.AddEnum(isRu, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["AverageRateLevel3Dto.RateTypeName"]);
        b.Add(x => x.TransportKindCode, L["AverageRateLevel3Dto.TransportKindCode"], visible: false);
        b.AddEnum(isRu, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["AverageRateLevel3Dto.TransportKindName"]);
        b.Add(x => x.TransportTypeCode, L["AverageRateLevel3Dto.TransportTypeCode"], visible: false);
        b.AddEnum(isRu, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["AverageRateLevel3Dto.TransportTypeName"]);
        b.Add(x => x.NodeFromCode, L["AverageRateLevel3Dto.NodeFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["AverageRateLevel3Dto.NodeFromName"]);
        b.Add(x => x.ProxyNodeCode, L["AverageRateLevel3Dto.ProxyNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["AverageRateLevel3Dto.ProxyNodeName"]);
        b.Add(x => x.NodeToCode, L["AverageRateLevel3Dto.NodeToCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeToNameRu, x => x.NodeToNameEn, L["AverageRateLevel3Dto.NodeToName"]);
        b.Add(x => x.ProductGroupCode, L["AverageRateLevel3Dto.ProductGroupCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["AverageRateLevel3Dto.ProductGroupName"]);
        b.Add(x => x.ProductCode, L["AverageRateLevel3Dto.ProductCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductNameRu, x => x.ProductNameEn, L["AverageRateLevel3Dto.ProductName"]);
        b.AddDateOnly(x => x.StartDate, L["AverageRateLevel3Dto.StartDate"]);
        b.AddDateOnly(x => x.EndDate, L["AverageRateLevel3Dto.EndDate"]);
        b.Add(x => x.RateLevel3, L["AverageRateLevel3Dto.RateLevel3"]);
        b.Add(x => x.EffectiveLoadOfTransportType, L["AverageRateLevel3Dto.EffectiveLoadOfTransportType"]);
        b.AddEnum(x => x.CurrencyId, L["AverageRateLevel3Dto.Currency"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["AverageRateLevel3Dto.CreationDate"],
            L["AverageRateLevel3Dto.LastChangeDate"],
            L["AverageRateLevel3Dto.IsArchive"]);

        return b.Build();
    }
}
