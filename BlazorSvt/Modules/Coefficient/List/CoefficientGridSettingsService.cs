using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Coefficient.List;

public class CoefficientGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Coefficient> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<CoefficientGridSettingsService> logger)
    : BaseGridSettingsService<CoefficientDto>(localStorage, logger)
{
    protected override string StorageKey => "CoefficientGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<CoefficientDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<CoefficientDto>(platform);

        b.Add(x => x.Code, L["CoefficientDto.Code"]);
        b.AddEnum(isRu, x => x.CoefficientTypeIdRu, x => x.CoefficientTypeIdEn, L["CoefficientDto.CoefficientTypeName"]);
        b.Add(x => x.Name, L["CoefficientDto.Name"]);
        b.AddDateOnly(x => x.StartDate, L["CoefficientDto.StartDate"]);
        b.AddDateOnly(x => x.FinishDate, L["CoefficientDto.FinishDate"]);
        b.AddYesNo(x => x.Active, L["CoefficientDto.Active"]);
        b.AddYesNo(x => x.OverallValue, L["CoefficientDto.OverallValue"]);
        b.AddEnum(isRu, x => x.RateTypeIdRu, x => x.RateTypeIdEn, L["CoefficientDto.RateTypeName"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["CoefficientDto.CreationDate"],
            L["CoefficientDto.LastChangeDate"],
            L["CoefficientDto.IsArchive"]);

        return b.Build();
    }
}
