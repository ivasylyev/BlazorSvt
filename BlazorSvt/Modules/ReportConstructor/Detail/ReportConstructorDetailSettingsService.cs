using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ReportConstructor.Detail;

public class ReportConstructorDetailSettingsService(
    IStringLocalizer<Resources.ReportConstructor> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<ReportConstructorDetailDto>
{
    public DetailSettingsCollection<ReportConstructorDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<ReportConstructorDetailDto>(platform);
        var group = L["ReportConstructorDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["ReportConstructorDetailDto.Code"]);
        b.AddEnum(isRu, group, x => x.ReportTypeIdRu, x => x.ReportTypeIdEn, L["ReportConstructorDetailDto.ReportTypeName"]);
        b.Add(group, x => x.DateFrom, L["ReportConstructorDetailDto.DateFrom"]);
        b.Add(group, x => x.DateTo, L["ReportConstructorDetailDto.DateTo"]);
        b.Add(group, x => x.NodeFromCode, L["ReportConstructorDetailDto.NodeFromCode"]);
        b.AddLocalized(isRu, group, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["ReportConstructorDetailDto.NodeFromName"]);
        b.Add(group, x => x.NodeToCode, L["ReportConstructorDetailDto.NodeToCode"]);
        b.AddLocalized(isRu, group, x => x.NodeToNameRu, x => x.NodeToNameEn, L["ReportConstructorDetailDto.NodeToName"]);
        AddNullableEnum(b, isRu, group, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["ReportConstructorDetailDto.TransportKindName"]);
        b.Add(group, x => x.TransportKindCode, L["ReportConstructorDetailDto.TransportKindCode"]);
        AddNullableEnum(b, isRu, group, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["ReportConstructorDetailDto.TransportTypeName"]);
        b.Add(group, x => x.TransportTypeCode, L["ReportConstructorDetailDto.TransportTypeCode"]);
        b.Add(group, x => x.ProductGroupCode, L["ReportConstructorDetailDto.ProductGroupCode"]);
        b.AddLocalized(isRu, group, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["ReportConstructorDetailDto.ProductGroupName"]);
        b.Add(group, x => x.ProductCode, L["ReportConstructorDetailDto.ProductCode"]);
        b.AddLocalized(isRu, group, x => x.ProductNameRu, x => x.ProductNameEn, L["ReportConstructorDetailDto.ProductName"]);

        return b.Build();
    }

    private static void AddNullableEnum<TRu, TEn>(
        DetailSettingsBuilder<ReportConstructorDetailDto> builder,
        bool isRu,
        string group,
        System.Linq.Expressions.Expression<Func<ReportConstructorDetailDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<ReportConstructorDetailDto, TEn?>> en,
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
