using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ParityRates.List;

// ReSharper disable once InconsistentNaming
public class ParityRatesGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.ParityRates> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<ParityRatesGridSettingsService> logger)
    : BaseGridSettingsService<ParityRatesDto>(localStorage, logger)
{
    protected override string StorageKey => "ParityRatesGridColumnSettings";

    protected override List<GridColumnSetting<ParityRatesDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<ParityRatesDto>(platform);

        // Порядок = короткий список MDM (после фильтрации неиспользуемых)
        b.Add(x => x.Code, L["ParityRatesDto.Code"]);
        b.AddDateOnly(x => x.StartDate, L["ParityRatesDto.StartDate"]);
        b.AddDateOnly(x => x.EndDate, L["ParityRatesDto.EndDate"]);
        b.AddEnum(isRu, x => x.RelevanceIdRu, x => x.RelevanceIdEn, L["ParityRatesDto.RelevanceName"]);
        b.Add(x => x.NodeFromCode, L["ParityRatesDto.NodeFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["ParityRatesDto.NodeFromName"]);
        b.Add(x => x.ProxyNode1Code, L["ParityRatesDto.ProxyNode1Code"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNode1NameRu, x => x.ProxyNode1NameEn, L["ParityRatesDto.ProxyNode1Name"]);
        b.Add(x => x.ProxyNode2Code, L["ParityRatesDto.ProxyNode2Code"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNode2NameRu, x => x.ProxyNode2NameEn, L["ParityRatesDto.ProxyNode2Name"]);
        b.Add(x => x.NodeToCode, L["ParityRatesDto.NodeToCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeToNameRu, x => x.NodeToNameEn, L["ParityRatesDto.NodeToName"]);
        b.Add(x => x.TransportTypeCode, L["ParityRatesDto.TransportTypeCode"], visible: false);
        b.AddEnum(isRu, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["ParityRatesDto.TransportTypeName"]);
        b.Add(x => x.ProductGroupCode, L["ParityRatesDto.ProductGroupCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["ParityRatesDto.ProductGroupName"]);
        b.Add(x => x.ProductCode, L["ParityRatesDto.ProductCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductNameRu, x => x.ProductNameEn, L["ParityRatesDto.ProductName"]);
        b.Add(x => x.Level_Danger_Product, L["ParityRatesDto.Level_Danger_Product"]);
        b.Add(x => x.TotalCostTransport, L["ParityRatesDto.TotalCostTransport"]);
        b.Add(x => x.LoadOfTransport, L["ParityRatesDto.LoadOfTransport"]);
        b.Add(x => x.TotalCostTon, L["ParityRatesDto.TotalCostTon"]);
        b.AddEnum(x => x.CurrencyId, L["ParityRatesDto.Currency"]);
        b.Add(x => x.Comment, L["ParityRatesDto.Comment"]);
        b.Add(x => x.DataSource, L["ParityRatesDto.DataSource"]);
        b.Add(x => x.FactRate, L["ParityRatesDto.FactRate"]);
        b.Add(x => x.BusinessPlanningRate, L["ParityRatesDto.BusinessPlanningRate"]);
        b.Add(x => x.DepartmentResponsibilityArea, L["ParityRatesDto.DepartmentResponsibilityArea"]);
        b.Add(x => x.EmployeeResponsibilityArea, L["ParityRatesDto.EmployeeResponsibilityArea"]);
        b.Add(x => x.Methodology, L["ParityRatesDto.Methodology"]);
        b.Add(x => x.PriorityText, L["ParityRatesDto.PriorityText"]);
        b.Add(x => x.MarketingDataStructure, L["ParityRatesDto.MarketingDataStructure"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["ParityRatesDto.CreationDate"],
            L["ParityRatesDto.LastChangeDate"],
            L["ParityRatesDto.IsArchive"]);

        return b.Build();
    }
}
