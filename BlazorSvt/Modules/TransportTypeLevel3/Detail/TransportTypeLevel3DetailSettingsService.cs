using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportTypeLevel3.Detail;

public class TransportTypeLevel3DetailSettingsService(
    IStringLocalizer<Resources.TransportTypeLevel3> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<TransportTypeLevel3DetailDto>
{
    public DetailSettingsCollection<TransportTypeLevel3DetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportTypeLevel3DetailDto>(platform);
        var group = L["TransportTypeLevel3DetailDto.Group.0.Default"];

        b.AddYesNo(group, x => x.IsBasic_forCBD, L["TransportTypeLevel3DetailDto.IsBasic_forCBD"]);
        b.Add(group, x => x.Code, L["TransportTypeLevel3DetailDto.Code"]);
        b.AddEnum(isRu, group, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportTypeLevel3DetailDto.TransportKindName"]);
        b.AddEnum(isRu, group, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["TransportTypeLevel3DetailDto.TransportTypeName"]);
        b.Add(group, x => x.Name, L["TransportTypeLevel3DetailDto.Name"]);
        b.Add(group, x => x.NameEnRu, L["TransportTypeLevel3DetailDto.NameEnRu"]);
        b.Add(group, x => x.TypeCode, L["TransportTypeLevel3DetailDto.TypeCode"]);
        b.Add(group, x => x.SAPCode, L["TransportTypeLevel3DetailDto.SAPCode"]);
        b.Add(group, x => x.OrionCode, L["TransportTypeLevel3DetailDto.OrionCode"]);
        if (isRu)
            b.Add(group, x => x.StatusIdRu, L["TransportTypeLevel3DetailDto.Status"], display: dto => Label(dto.StatusIdRu));
        else
            b.Add(group, x => x.StatusIdEn, L["TransportTypeLevel3DetailDto.Status"], display: dto => Label(dto.StatusIdEn));
        b.Add(group, x => x.CreationDate, L["TransportTypeLevel3DetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["TransportTypeLevel3DetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["TransportTypeLevel3DetailDto.IsArchive"]);

        return b.Build();
    }

    private static string Label<TEnum>(TEnum? value) where TEnum : struct, Enum
        => value is { } current
            ? typeof(TEnum).GetDisplayName(current.ToString()) ?? string.Empty
            : string.Empty;
}
