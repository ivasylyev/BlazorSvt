using BlazorBootstrap;
using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Contractor.List;

public class ContractorGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Contractor> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<ContractorGridSettingsService> logger)
    : BaseGridSettingsService<ContractorDto>(localStorage, logger)
{
    protected override string StorageKey => "ContractorGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<ContractorDto>> GetDefaultSettings(string lang)
    {
        var b = new GridColumnSettingsBuilder<ContractorDto>(platform);

        b.Add(x => x.Code, L["ContractorDto.Code"]);
        b.Add(x => x.NameSearch, L["ContractorDto.NameSearch"]);
        b.Add(x => x.Consignee, L["ContractorDto.Consignee"]);
        b.Add(x => x.IsTrader, L["ContractorDto.IsTrader"]);
        b.Add(x => x.IsProcessing, L["ContractorDto.IsProcessing"]);
        b.Add(x => x.IsSender, L["ContractorDto.IsSender"]);
        b.Add(x => x.IsDebtor, L["ContractorDto.IsDebtor"]);
        b.Add(x => x.IsCreditor, L["ContractorDto.IsCreditor"]);
        b.Add(x => x.INN, L["ContractorDto.INN"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["ContractorDto.CreationDate"],
            L["ContractorDto.LastChangeDate"],
            L["ContractorDto.IsArchive"]);

        return b.Build();
    }
}
