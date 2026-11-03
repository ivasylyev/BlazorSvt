using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportTypeLevel3.List;

public class TransportTypeLevel3GridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.TransportTypeLevel3> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<TransportTypeLevel3GridSettingsService> logger)
    : BaseGridSettingsService<TransportTypeLevel3Dto>(localStorage, logger)
{
    protected override string StorageKey => "TransportTypeLevel3GridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<TransportTypeLevel3Dto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<TransportTypeLevel3Dto>(platform);

        b.Add(x => x.IsBasic_forCBD, L["TransportTypeLevel3Dto.IsBasic_forCBD"], display: dto => YesNo(dto.IsBasic_forCBD));
        b.Add(x => x.Code, L["TransportTypeLevel3Dto.Code"]);
        b.AddEnum(isRu, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportTypeLevel3Dto.TransportKindName"]);
        b.AddEnum(isRu, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["TransportTypeLevel3Dto.TransportTypeName"]);
        b.Add(x => x.Name, L["TransportTypeLevel3Dto.Name"]);
        b.Add(x => x.NameEnRu, L["TransportTypeLevel3Dto.NameEnRu"]);
        b.Add(x => x.TypeCode, L["TransportTypeLevel3Dto.TypeCode"]);
        b.Add(x => x.SAPCode, L["TransportTypeLevel3Dto.SAPCode"]);
        b.Add(x => x.OrionCode, L["TransportTypeLevel3Dto.OrionCode"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["TransportTypeLevel3Dto.CreationDate"],
            L["TransportTypeLevel3Dto.LastChangeDate"],
            L["TransportTypeLevel3Dto.IsArchive"]);

        return b.Build();

        string YesNo(bool? value) => value switch
        {
            true => platform["Common.Yes"],
            false => platform["Common.No"],
            _ => string.Empty
        };
    }
}
