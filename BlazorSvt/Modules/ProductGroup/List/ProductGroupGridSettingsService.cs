using Blazored.LocalStorage;
using BlazorSvt.Modules.ProductGroup.List.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ProductGroup.List;

public class ProductGroupGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.ProductGroup> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<ProductGroupGridSettingsService> logger)
    : BaseGridSettingsService<ProductGroupDto>(localStorage, logger)
{
    protected override string StorageKey => "ProductGroupGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<ProductGroupDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<ProductGroupDto>(platform);

        b.Add(x => x.Code, L["ProductGroupDto.Code"]);
        b.Add(x => x.Name, L["ProductGroupDto.Name"]);
        b.Add(x => x.ShortName, L["ProductGroupDto.ShortName"]);
        b.Add(x => x.GroupCode, L["ProductGroupDto.GroupCode"]);
        b.Add(x => x.ProductGroupCBD, L["ProductGroupDto.ProductGroupCBD"]);
        b.Add(x => x.NameEnCode, L["ProductGroupDto.NameEnCode"]);
        b.Add(x => x.NameEnRu, L["ProductGroupDto.NameEnRu"]);
        b.Add(x => x.Code_NSI, L["ProductGroupDto.Code_NSI"]);
        b.AddYesNo(x => x.SVTUseProductGroupWhenNoProductRateExists, L["ProductGroupDto.SVTUseProductGroupWhenNoProductRateExists"]);
        b.Add(x => x.NameEn, L["ProductGroupDto.NameEn"]);
        b.AddEnum(isRu, x => x.RateSyncClassIdRu, x => x.RateSyncClassIdEn, L["ProductGroupDto.RateSyncClass"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["ProductGroupDto.CreationDate"],
            L["ProductGroupDto.LastChangeDate"],
            L["ProductGroupDto.IsArchive"]);

        return b.Build();
    }
}
