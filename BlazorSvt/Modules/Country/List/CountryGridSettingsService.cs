using BlazorBootstrap;
using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Country.List;

public class CountryGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Country> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<CountryGridSettingsService> logger)
    : BaseGridSettingsService<CountryDto>(localStorage, logger)
{
    protected override string StorageKey => "CountryGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<CountryDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<CountryDto>(platform);

        b.Add(x => x.Code, L["CountryDto.Code"]);
        b.AddLocalized(isRu, x => x.NameRu, x => x.NameEn, L["CountryDto.Name"]);
        b.Add(x => x.ISOCode, L["CountryDto.ISOCode"]);
        b.Add(x => x.PlanCode, L["CountryDto.PlanCode"]);
        b.Add(x => x.Market, L["CountryDto.Market"]);
        b.Add(x => x.ISOCode3, L["CountryDto.ISOCode3"]);
        b.Add(x => x.CreationDate, L["CountryDto.CreationDate"], visible: false);
        b.Add(x => x.LastChangeDate, L["CountryDto.LastChangeDate"], visible: false);
        b.Add(
            x => x.IsArchive,
            L["CountryDto.IsArchive"],
            visible: false,
            display: dto => dto.IsArchive ? platform["Common.Archive"] : platform["Common.Active"]);

        var settings = b.Build();
        settings.Single(c => c.Name == nameof(CountryDto.IsArchive)).FilterValue = "False";
        return settings;
    }
}
