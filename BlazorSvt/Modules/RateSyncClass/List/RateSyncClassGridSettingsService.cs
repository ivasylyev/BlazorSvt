using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateSyncClass.List;

public class RateSyncClassGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.RateSyncClass> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RateSyncClassGridSettingsService> logger)
    : BaseGridSettingsService<RateSyncClassDto>(localStorage, logger)
{
    protected override string StorageKey => "RateSyncClassGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<RateSyncClassDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<RateSyncClassDto>(platform);

        b.Add(x => x.Code, L["RateSyncClassDto.Code"]);
        b.Add(x => x.Name, L["RateSyncClassDto.Name"]);
        b.AddSystemColumns(
            x => x.CreationUserName,
            x => x.CreationDate,
            x => x.LastChangeUserName,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["RateSyncClassDto.CreationDate"],
            L["RateSyncClassDto.LastChangeDate"],
            L["RateSyncClassDto.IsArchive"]);

        return b.Build();
    }
}
