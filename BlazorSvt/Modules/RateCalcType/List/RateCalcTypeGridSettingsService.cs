using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateCalcType.List;

public class RateCalcTypeGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.RateCalcType> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RateCalcTypeGridSettingsService> logger)
    : BaseGridSettingsService<RateCalcTypeDto>(localStorage, logger)
{
    protected override string StorageKey => "RateCalcTypeGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<RateCalcTypeDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<RateCalcTypeDto>(platform);

        b.Add(x => x.Code, L["RateCalcTypeDto.Code"]);
        b.Add(x => x.Name, L["RateCalcTypeDto.Name"]);
        b.Add(x => x.SearchCode, L["RateCalcTypeDto.SearchCode"]);
        b.AddSystemColumns(
            x => x.CreationUserName,
            x => x.CreationDate,
            x => x.LastChangeUserName,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["RateCalcTypeDto.CreationDate"],
            L["RateCalcTypeDto.LastChangeDate"],
            L["RateCalcTypeDto.IsArchive"]);

        return b.Build();
    }
}
