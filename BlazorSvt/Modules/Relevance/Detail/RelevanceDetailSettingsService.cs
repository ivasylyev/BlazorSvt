using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Relevance.Detail;

public class RelevanceDetailSettingsService(
    IStringLocalizer<Resources.Relevance> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RelevanceDetailDto>
{
    public DetailSettingsCollection<RelevanceDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<RelevanceDetailDto>(platform);
        var group = L["RelevanceDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RelevanceDetailDto.Code"]);
        b.Add(group, x => x.Name, L["RelevanceDetailDto.Name"]);
        b.Add(group, x => x.CreationDate, L["RelevanceDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["RelevanceDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RelevanceDetailDto.IsArchive"]);

        return b.Build();
    }
}
