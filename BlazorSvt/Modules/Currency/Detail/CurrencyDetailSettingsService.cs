using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Currency.Detail;

public class CurrencyDetailSettingsService(
    IStringLocalizer<Resources.Currency> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<CurrencyDetailDto>
{
    public DetailSettingsCollection<CurrencyDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<CurrencyDetailDto>(platform);
        var group = L["CurrencyDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["CurrencyDetailDto.Code"]);
        b.Add(group, x => x.Name, L["CurrencyDetailDto.Name"]);
        b.Add(group, x => x.Symbolic, L["CurrencyDetailDto.Symbolic"]);
        b.Add(group, x => x.Digital, L["CurrencyDetailDto.Digital"]);
        b.Add(group, x => x.Code_NSI, L["CurrencyDetailDto.Code_NSI"]);
        b.Add(group, x => x.CreationDate, L["CurrencyDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["CurrencyDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["CurrencyDetailDto.IsArchive"]);

        return b.Build();
    }
}
