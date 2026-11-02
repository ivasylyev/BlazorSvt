using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ProductGroup.Detail;

public class ProductGroupDetailSettingsService(
    IStringLocalizer<Resources.ProductGroup> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<ProductGroupDetailDto>
{
    public DetailSettingsCollection<ProductGroupDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<ProductGroupDetailDto>(platform);
        var group = L["ProductGroupDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["ProductGroupDetailDto.Code"]);
        b.Add(group, x => x.Name, L["ProductGroupDetailDto.Name"]);
        b.Add(group, x => x.ShortName, L["ProductGroupDetailDto.ShortName"]);
        b.Add(group, x => x.GroupCode, L["ProductGroupDetailDto.GroupCode"]);
        b.Add(group, x => x.ProductGroupCBD, L["ProductGroupDetailDto.ProductGroupCBD"]);
        b.Add(group, x => x.NameEnCode, L["ProductGroupDetailDto.NameEnCode"]);
        b.Add(group, x => x.NameEnRu, L["ProductGroupDetailDto.NameEnRu"]);
        b.Add(group, x => x.Code_NSI, L["ProductGroupDetailDto.Code_NSI"]);
        b.AddYesNo(group, x => x.SVTUseProductGroupWhenNoProductRateExists, L["ProductGroupDetailDto.SVTUseProductGroupWhenNoProductRateExists"]);
        b.Add(group, x => x.NameEn, L["ProductGroupDetailDto.NameEn"]);
        b.AddEnum(isRu, group, x => x.RateSyncClassIdRu, x => x.RateSyncClassIdEn, L["ProductGroupDetailDto.RateSyncClass"]);
        b.Add(group, x => x.CreationDate, L["ProductGroupDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["ProductGroupDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["ProductGroupDetailDto.IsArchive"]);

        return b.Build();
    }
}
