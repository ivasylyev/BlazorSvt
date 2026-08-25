using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ParityRates.Detail;

// ReSharper disable once InconsistentNaming
public class ParityRatesDetailSettingsService(
    IStringLocalizer<Resources.ParityRates> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<ParityRatesDetailDto>
{
    public DetailSettingsCollection<ParityRatesDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<ParityRatesDetailDto>(platform);

        var g0 = L["ParityRatesDetailDto.Group.0.Default"];

        Func<ParityRatesDetailDto, bool> hasProxy1 = dto => dto.ProxyNode1Code is not null;
        Func<ParityRatesDetailDto, bool> hasProxy2 = dto => dto.ProxyNode2Code is not null;
        Func<ParityRatesDetailDto, bool> hasProduct = dto => dto.ProductCode is not null;

        b.Add(g0, x => x.Code, L["ParityRatesDetailDto.Code"]);
        b.Add(g0, x => x.RelevanceName, L["ParityRatesDetailDto.RelevanceName"]);
        b.Add(g0, x => x.RelevanceCode, L["ParityRatesDetailDto.RelevanceCode"]);
        b.Add(g0, x => x.CreationDate, L["ParityRatesDetailDto.CreationDate"], hasMargin: true);
        b.Add(g0, x => x.LastChangeDate, L["ParityRatesDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(g0, x => x.IsArchive, L["ParityRatesDetailDto.IsArchive"]);

        b.Add(g0, x => x.StartDate, L["ParityRatesDetailDto.StartDate"], hasMargin: true);
        b.Add(g0, x => x.EndDate, L["ParityRatesDetailDto.EndDate"]);

        b.AddLocalized(isRu, g0, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["ParityRatesDetailDto.NodeFromName"], hasMargin: true);
        b.Add(g0, x => x.NodeFromCode, L["ParityRatesDetailDto.NodeFromCode"]);
        b.AddLocalized(isRu, g0, x => x.ProxyNode1NameRu, x => x.ProxyNode1NameEn, L["ParityRatesDetailDto.ProxyNode1Name"], visible: hasProxy1, hasMargin: true);
        b.Add(g0, x => x.ProxyNode1Code, L["ParityRatesDetailDto.ProxyNode1Code"], visible: hasProxy1);
        b.AddLocalized(isRu, g0, x => x.ProxyNode2NameRu, x => x.ProxyNode2NameEn, L["ParityRatesDetailDto.ProxyNode2Name"], visible: hasProxy2, hasMargin: true);
        b.Add(g0, x => x.ProxyNode2Code, L["ParityRatesDetailDto.ProxyNode2Code"], visible: hasProxy2);
        b.AddLocalized(isRu, g0, x => x.NodeToNameRu, x => x.NodeToNameEn, L["ParityRatesDetailDto.NodeToName"], hasMargin: true);
        b.Add(g0, x => x.NodeToCode, L["ParityRatesDetailDto.NodeToCode"]);

        b.AddLocalized(isRu, g0, x => x.TransportTypeNameRu, x => x.TransportTypeNameEn, L["ParityRatesDetailDto.TransportTypeName"], hasMargin: true);
        b.Add(g0, x => x.TransportTypeCode, L["ParityRatesDetailDto.TransportTypeCode"]);

        b.AddLocalized(isRu, g0, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["ParityRatesDetailDto.ProductGroupName"], hasMargin: true);
        b.Add(g0, x => x.ProductGroupCode, L["ParityRatesDetailDto.ProductGroupCode"]);
        b.AddLocalized(isRu, g0, x => x.ProductNameRu, x => x.ProductNameEn, L["ParityRatesDetailDto.ProductName"], visible: hasProduct, hasMargin: true);
        b.Add(g0, x => x.ProductCode, L["ParityRatesDetailDto.ProductCode"], visible: hasProduct);

        b.Add(g0, x => x.Level_Danger_Product, L["ParityRatesDetailDto.Level_Danger_Product"], hasMargin: true);
        b.AddYesNo(g0, x => x.Dangerous_Cargo, L["ParityRatesDetailDto.Dangerous_Cargo"]);

        b.Add(g0, x => x.TotalCostTransport, L["ParityRatesDetailDto.TotalCostTransport"], hasMargin: true);
        b.Add(g0, x => x.LoadOfTransport, L["ParityRatesDetailDto.LoadOfTransport"]);
        b.Add(g0, x => x.TotalCostTon, L["ParityRatesDetailDto.TotalCostTon"]);
        b.Add(g0, x => x.CurrencyStandard, L["ParityRatesDetailDto.Currency"]);

        b.Add(g0, x => x.Comment, L["ParityRatesDetailDto.Comment"], hasMargin: true);
        b.Add(g0, x => x.DataSource, L["ParityRatesDetailDto.DataSource"]);
        b.Add(g0, x => x.FactRate, L["ParityRatesDetailDto.FactRate"]);
        b.Add(g0, x => x.BusinessPlanningRate, L["ParityRatesDetailDto.BusinessPlanningRate"]);
        b.Add(g0, x => x.DepartmentResponsibilityArea, L["ParityRatesDetailDto.DepartmentResponsibilityArea"]);
        b.Add(g0, x => x.EmployeeResponsibilityArea, L["ParityRatesDetailDto.EmployeeResponsibilityArea"]);
        b.Add(g0, x => x.Methodology, L["ParityRatesDetailDto.Methodology"]);
        b.Add(g0, x => x.PriorityText, L["ParityRatesDetailDto.PriorityText"]);
        b.Add(g0, x => x.MarketingDataStructure, L["ParityRatesDetailDto.MarketingDataStructure"]);

        return b.Build();
    }
}
