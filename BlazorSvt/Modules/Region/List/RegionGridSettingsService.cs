using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Region.List;

public class RegionGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.Region> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<RegionGridSettingsService> logger)
    : BaseGridSettingsService<RegionDto>(localStorage, logger)
{
    protected override string StorageKey => "RegionGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<RegionDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<RegionDto>(platform);

        b.Add(x => x.Code, L["RegionDto.Code"]);
        b.Add(x => x.RegionCode, L["RegionDto.RegionCode"]);
        b.Add(x => x.Code_NSI, L["RegionDto.Code_NSI"]);
        b.Add(x => x.RegionCodePlanning, L["RegionDto.RegionCodePlanning"]);
        b.Add(x => x.RegionCodeRF, L["RegionDto.RegionCodeRF"]);
        b.AddLocalized(isRu, x => x.NameRu, x => x.NameEn, L["RegionDto.Name"]);
        b.AddLocalized(isRu, x => x.CountryNameRu, x => x.CountryNameEn, L["RegionDto.CountryName"]);
        b.Add(x => x.Market, L["RegionDto.Market"]);
        AddEnum(b, isRu, x => x.FederalDistrictsIdRu, x => x.FederalDistrictsIdEn, L["RegionDto.FederalDistricts"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["RegionDto.CreationDate"],
            L["RegionDto.LastChangeDate"],
            L["RegionDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        GridColumnSettingsBuilder<RegionDto> builder,
        bool isRu,
        System.Linq.Expressions.Expression<Func<RegionDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<RegionDto, TEn?>> en,
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
