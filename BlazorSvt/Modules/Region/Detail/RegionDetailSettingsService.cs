using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Region.Detail;

public class RegionDetailSettingsService(
    IStringLocalizer<Resources.Region> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<RegionDetailDto>
{
    public DetailSettingsCollection<RegionDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<RegionDetailDto>(platform);
        var group = L["RegionDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["RegionDetailDto.Code"]);
        b.Add(group, x => x.CreationDate, L["RegionDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["RegionDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["RegionDetailDto.IsArchive"]);
        b.Add(group, x => x.NameRu, L["RegionDetailDto.NameRu"]);
        b.Add(group, x => x.RegionCode, L["RegionDetailDto.RegionCode"]);
        b.AddLocalized(isRu, group, x => x.CountryNameRu, x => x.CountryNameEn, L["RegionDetailDto.CountryName"]);
        b.AddEnum(isRu, group, x => x.StatusIdRu, x => x.StatusIdEn, L["RegionDetailDto.Status"]);
        b.Add(group, x => x.NameEn, L["RegionDetailDto.NameEn"]);
        b.Add(group, x => x.RegionCodeRF, L["RegionDetailDto.RegionCodeRF"]);
        b.Add(group, x => x.RegionCodePlanning, L["RegionDetailDto.RegionCodePlanning"]);
        b.Add(group, x => x.Market, L["RegionDetailDto.Market"]);
        b.Add(group, x => x.Code_NSI, L["RegionDetailDto.Code_NSI"]);
        AddEnum(b, isRu, group, x => x.FederalDistrictsIdRu, x => x.FederalDistrictsIdEn, L["RegionDetailDto.FederalDistricts"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        DetailSettingsBuilder<RegionDetailDto> builder,
        bool isRu,
        string group,
        System.Linq.Expressions.Expression<Func<RegionDetailDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<RegionDetailDto, TEn?>> en,
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
