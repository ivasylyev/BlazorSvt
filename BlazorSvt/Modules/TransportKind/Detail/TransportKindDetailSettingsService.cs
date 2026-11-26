using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportKind.Detail;

public class TransportKindDetailSettingsService(
    IStringLocalizer<Resources.TransportKind> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<TransportKindDetailDto>
{
    public DetailSettingsCollection<TransportKindDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportKindDetailDto>(platform);
        var group = L["TransportKindDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["TransportKindDetailDto.Code"]);
        b.Add(group, x => x.Name, L["TransportKindDetailDto.Name"]);
        b.Add(group, x => x.NameEnRu, L["TransportKindDetailDto.NameEnRu"]);
        b.AddEnum(isRu, group, x => x.StatusIdRu, x => x.StatusIdEn, L["TransportKindDetailDto.Status"]);
        b.Add(group, x => x.Level, L["TransportKindDetailDto.Level"]);
        b.Add(group, x => x.KindCode, L["TransportKindDetailDto.KindCode"]);
        b.AddCreationUser(group, x => x.CreationUserName);
        b.Add(group, x => x.CreationDate, L["TransportKindDetailDto.CreationDate"]);
        b.AddLastChangeUser(group, x => x.LastChangeUserName);
        b.Add(group, x => x.LastChangeDate, L["TransportKindDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["TransportKindDetailDto.IsArchive"]);

        return b.Build();
    }
}
