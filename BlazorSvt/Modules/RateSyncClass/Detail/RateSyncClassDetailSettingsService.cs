using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateSyncClass.Detail;

public class RateSyncClassDetailSettingsService(
    IStringLocalizer<Resources.RateSyncClass> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RateSyncClassDetailDto>
{
    public DetailSettingsCollection<RateSyncClassDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<RateSyncClassDetailDto>(platform);
        var group = L["RateSyncClassDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RateSyncClassDetailDto.Code"]);
        b.Add(group, x => x.Name, L["RateSyncClassDetailDto.Name"]);
        b.AddCreationUser(group, x => x.CreationUserName);
        b.Add(group, x => x.CreationDate, L["RateSyncClassDetailDto.CreationDate"]);
        b.AddLastChangeUser(group, x => x.LastChangeUserName);
        b.Add(group, x => x.LastChangeDate, L["RateSyncClassDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RateSyncClassDetailDto.IsArchive"]);

        return b.Build();
    }
}
