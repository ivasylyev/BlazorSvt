using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Currency.List;

public class CurrencyGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Currency> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<CurrencyGridSettingsService> logger)
    : BaseGridSettingsService<CurrencyDto>(localStorage, logger)
{
    protected override string StorageKey => "CurrencyGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<CurrencyDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<CurrencyDto>(platform);

        b.Add(x => x.Code, L["CurrencyDto.Code"]);
        b.Add(x => x.Name, L["CurrencyDto.Name"]);
        b.Add(x => x.Symbolic, L["CurrencyDto.Symbolic"]);
        b.Add(x => x.Digital, L["CurrencyDto.Digital"]);
        b.Add(x => x.Code_NSI, L["CurrencyDto.Code_NSI"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["CurrencyDto.CreationDate"],
            L["CurrencyDto.LastChangeDate"],
            L["CurrencyDto.IsArchive"]);

        return b.Build();
    }
}
