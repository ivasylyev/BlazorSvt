using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Country.Detail;

public class CountryDetailSettingsService(
    IStringLocalizer<Resources.Country> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<CountryDetailDto>
{
    public DetailSettingsCollection<CountryDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<CountryDetailDto>(platform);
        var group = L["CountryDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["CountryDetailDto.Code"]);
        b.Add(group, x => x.NameRu, L["CountryDetailDto.NameRu"]);
        b.Add(group, x => x.ISOCode, L["CountryDetailDto.ISOCode"]);
        b.Add(group, x => x.NameEn, L["CountryDetailDto.NameEn"]);
        b.Add(group, x => x.PlanCode, L["CountryDetailDto.PlanCode"]);
        b.Add(group, x => x.Market, L["CountryDetailDto.Market"]);
        b.Add(group, x => x.ISOCode3, L["CountryDetailDto.ISOCode3"]);
        AddEnum(b, isRu, group, x => x.StatusIdRu, x => x.StatusIdEn, L["CountryDetailDto.Status"]);
        b.Add(group, x => x.CreationDate, L["CountryDetailDto.CreationDate"]);
        b.Add(group, x => x.LastChangeDate, L["CountryDetailDto.LastChangeDate"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["CountryDetailDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        DetailSettingsBuilder<CountryDetailDto> builder,
        bool isRu,
        string group,
        System.Linq.Expressions.Expression<Func<CountryDetailDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<CountryDetailDto, TEn?>> en,
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
