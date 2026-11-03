using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.MTR.Detail;

public class MTRDetailSettingsService(
    IStringLocalizer<Resources.MTR> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<MTRDetailDto>
{
    public DetailSettingsCollection<MTRDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<MTRDetailDto>(platform);
        var group = L["MTRDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["MTRDetailDto.Code"]);
        b.Add(group, x => x.NameFullRu, L["MTRDetailDto.NameFullRu"]);
        b.Add(group, x => x.NameShortRu, L["MTRDetailDto.NameShortRu"]);
        b.Add(group, x => x.NameFullEn, L["MTRDetailDto.NameFullEn"]);
        b.Add(group, x => x.NameShortEn, L["MTRDetailDto.NameShortEn"]);
        b.AddLocalized(isRu, group, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["MTRDetailDto.ProductGroupName"]);
        b.Add(group, x => x.ProductGroupCode, L["MTRDetailDto.ProductGroupCode"]);
        b.AddLocalized(isRu, group, x => x.EtsngNameRu, x => x.EtsngNameEn, L["MTRDetailDto.EtsngName"]);
        b.Add(group, x => x.GngCode, L["MTRDetailDto.GngCode"]);
        AddEnum(b, isRu, group, x => x.UnitIdRu, x => x.UnitIdEn, L["MTRDetailDto.Unit"]);
        b.Add(group, x => x.DpoCode, L["MTRDetailDto.DpoCode"]);
        b.Add(group, x => x.MtrType, L["MTRDetailDto.MtrType"]);
        AddEnum(b, isRu, group, x => x.DangerClassIdRu, x => x.DangerClassIdEn, L["MTRDetailDto.DangerClass"]);
        AddEnum(b, isRu, group, x => x.StatusIdRu, x => x.StatusIdEn, L["MTRDetailDto.Status"]);
        b.Add(group, x => x.ProductCode, L["MTRDetailDto.ProductCode"]);
        b.Add(group, x => x.CreationDate, L["MTRDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["MTRDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["MTRDetailDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        DetailSettingsBuilder<MTRDetailDto> builder,
        bool isRu,
        string group,
        System.Linq.Expressions.Expression<Func<MTRDetailDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<MTRDetailDto, TEn?>> en,
        string header)
        where TRu : struct, Enum
        where TEn : struct, Enum
    {
        if (isRu)
        {
            var compiled = ru.Compile();
            builder.Add(group, ru, header, display: dto => Label(compiled(dto)));
        }
        else
        {
            var compiled = en.Compile();
            builder.Add(group, en, header, display: dto => Label(compiled(dto)));
        }
    }

    private static string Label<TEnum>(TEnum? value) where TEnum : struct, Enum
        => value is { } current
            ? typeof(TEnum).GetDisplayName(current.ToString()) ?? string.Empty
            : string.Empty;
}
