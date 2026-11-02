using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.MTR.List;

public class MTRGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.MTR> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<MTRGridSettingsService> logger)
    : BaseGridSettingsService<MTRDto>(localStorage, logger)
{
    protected override string StorageKey => "MTRGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<MTRDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<MTRDto>(platform);

        b.Add(x => x.Code, L["MTRDto.Code"]);
        b.Add(x => x.NameFullRu, L["MTRDto.NameFullRu"]);
        b.Add(x => x.NameShortEn, L["MTRDto.NameShortEn"]);
        b.AddLocalized(isRu, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["MTRDto.ProductGroupName"]);
        b.Add(x => x.ProductGroupCode, L["MTRDto.ProductGroupCode"], visible: false);
        b.AddLocalized(isRu, x => x.EtsngNameRu, x => x.EtsngNameEn, L["MTRDto.EtsngName"]);
        b.Add(x => x.GngCode, L["MTRDto.GngCode"]);
        AddEnum(b, isRu, x => x.UnitIdRu, x => x.UnitIdEn, L["MTRDto.Unit"]);
        b.Add(x => x.DpoCode, L["MTRDto.DpoCode"]);
        b.Add(x => x.MtrType, L["MTRDto.MtrType"]);
        AddEnum(b, isRu, x => x.DangerClassIdRu, x => x.DangerClassIdEn, L["MTRDto.DangerClass"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["MTRDto.CreationDate"],
            L["MTRDto.LastChangeDate"],
            L["MTRDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        GridColumnSettingsBuilder<MTRDto> builder,
        bool isRu,
        System.Linq.Expressions.Expression<Func<MTRDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<MTRDto, TEn?>> en,
        string header)
        where TRu : struct, Enum
        where TEn : struct, Enum
    {
        if (isRu)
        {
            var compiled = ru.Compile();
            builder.Add(ru, header, display: dto => Label(compiled(dto)));
        }
        else
        {
            var compiled = en.Compile();
            builder.Add(en, header, display: dto => Label(compiled(dto)));
        }
    }

    private static string Label<TEnum>(TEnum? value) where TEnum : struct, Enum
        => value is { } current
            ? typeof(TEnum).GetDisplayName(current.ToString()) ?? string.Empty
            : string.Empty;
}
