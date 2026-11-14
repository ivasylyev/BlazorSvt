using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Contractor.Detail;

public class ContractorDetailSettingsService(
    IStringLocalizer<Resources.Contractor> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<ContractorDetailDto>
{
    public DetailSettingsCollection<ContractorDetailDto> GetGridDetailSettings(string lang)
    {
        var b = new DetailSettingsBuilder<ContractorDetailDto>(platform);
        var group = L["ContractorDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["ContractorDetailDto.Code"]);
        b.Add(group, x => x.NameSearch, L["ContractorDetailDto.NameSearch"]);
        b.Add(group, x => x.IsTrader, L["ContractorDetailDto.IsTrader"]);
        b.Add(group, x => x.IsProcessing, L["ContractorDetailDto.IsProcessing"]);
        b.Add(group, x => x.IsSender, L["ContractorDetailDto.IsSender"]);
        b.Add(group, x => x.IsDebtor, L["ContractorDetailDto.IsDebtor"]);
        b.Add(group, x => x.IsCreditor, L["ContractorDetailDto.IsCreditor"]);
        b.Add(group, x => x.ShortNameEGRUL, L["ContractorDetailDto.ShortNameEGRUL"]);
        b.Add(group, x => x.FullNameEGRUL, L["ContractorDetailDto.FullNameEGRUL"]);
        b.Add(group, x => x.INN, L["ContractorDetailDto.INN"]);
        b.Add(group, x => x.Customs, L["ContractorDetailDto.Customs"]);
        b.Add(group, x => x.CustomCode, L["ContractorDetailDto.CustomCode"]);
        b.Add(group, x => x.OrgAffiliation, L["ContractorDetailDto.OrgAffiliation"]);
        b.Add(group, x => x.Consignee, L["ContractorDetailDto.Consignee"]);
        b.Add(group, x => x.CreationDate, L["ContractorDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["ContractorDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["ContractorDetailDto.IsArchive"]);

        return b.Build();
    }
}
