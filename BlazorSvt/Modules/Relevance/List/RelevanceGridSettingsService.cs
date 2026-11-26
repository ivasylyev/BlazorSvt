using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Relevance.List;

public class RelevanceGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Relevance> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RelevanceGridSettingsService> logger)
    : BaseGridSettingsService<RelevanceDto>(localStorage, logger)
{
    protected override string StorageKey => "RelevanceGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<RelevanceDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<RelevanceDto>(platform);

        b.Add(x => x.Code, L["RelevanceDto.Code"]);
        b.Add(x => x.Name, L["RelevanceDto.Name"]);
        b.AddSystemColumns(
            x => x.CreationUserName,
            x => x.CreationDate,
            x => x.LastChangeUserName,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["RelevanceDto.CreationDate"],
            L["RelevanceDto.LastChangeDate"],
            L["RelevanceDto.IsArchive"]);

        return b.Build();
    }
}
