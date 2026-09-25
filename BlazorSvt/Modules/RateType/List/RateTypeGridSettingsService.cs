using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateType.List;

public class RateTypeGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.RateType> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RateTypeGridSettingsService> logger)
    : BaseGridSettingsService<RateTypeDto>(localStorage, logger)
{
    protected override string StorageKey => "RateTypeGridColumnSettings";

    protected override List<GridColumnSetting<RateTypeDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<RateTypeDto>(platform);

        b.Add(x => x.Code, L["RateTypeDto.Code"]);
        b.Add(x => x.Name, L["RateTypeDto.Name"]);
        b.Add(x => x.IsUseNomination, L["RateTypeDto.IsUseNomination"], display: dto => YesNo(dto.IsUseNomination));
        b.Add(x => x.IsDeflated, L["RateTypeDto.IsDeflated"], display: dto => YesNo(dto.IsDeflated));
        b.Add(x => x.Comment, L["RateTypeDto.Comment"]);
        b.Add(x => x.IsIntoSyncClass, L["RateTypeDto.IsIntoSyncClass"], display: dto => YesNo(dto.IsIntoSyncClass));
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["RateTypeDto.CreationDate"],
            L["RateTypeDto.LastChangeDate"],
            L["RateTypeDto.IsArchive"]);

        return b.Build();

        string YesNo(bool? value) => value switch
        {
            true => platform["Common.Yes"],
            false => platform["Common.No"],
            _ => string.Empty
        };
    }
}
