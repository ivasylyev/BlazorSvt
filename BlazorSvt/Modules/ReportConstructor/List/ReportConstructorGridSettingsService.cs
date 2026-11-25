using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ReportConstructor.List;

public class ReportConstructorGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.ReportConstructor> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<ReportConstructorGridSettingsService> logger)
    : BaseGridSettingsService<ReportConstructorDto>(localStorage, logger)
{
    protected override string StorageKey => "ReportConstructorGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<ReportConstructorDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<ReportConstructorDto>(platform);

        b.Add(x => x.Code, L["ReportConstructorDto.Code"]);
        b.AddEnum(isRu, x => x.ReportTypeIdRu, x => x.ReportTypeIdEn, L["ReportConstructorDto.ReportTypeName"]);
        b.AddDateOnly(x => x.DateFrom, L["ReportConstructorDto.DateFrom"]);
        b.AddDateOnly(x => x.DateTo, L["ReportConstructorDto.DateTo"]);
        b.Add(x => x.NodeFromCode, L["ReportConstructorDto.NodeFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["ReportConstructorDto.NodeFromName"]);
        b.Add(x => x.NodeToCode, L["ReportConstructorDto.NodeToCode"], visible: false);
        b.AddLocalized(isRu, x => x.NodeToNameRu, x => x.NodeToNameEn, L["ReportConstructorDto.NodeToName"]);
        b.Add(x => x.TransportKindCode, L["ReportConstructorDto.TransportKindCode"], visible: false);
        AddNullableEnum(b, isRu, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["ReportConstructorDto.TransportKindName"]);
        b.Add(x => x.TransportTypeCode, L["ReportConstructorDto.TransportTypeCode"], visible: false);
        AddNullableEnum(b, isRu, x => x.TransportTypeIdRu, x => x.TransportTypeIdEn, L["ReportConstructorDto.TransportTypeName"]);
        b.Add(x => x.ProductGroupCode, L["ReportConstructorDto.ProductGroupCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductGroupNameRu, x => x.ProductGroupNameEn, L["ReportConstructorDto.ProductGroupName"]);
        b.Add(x => x.ProductCode, L["ReportConstructorDto.ProductCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProductNameRu, x => x.ProductNameEn, L["ReportConstructorDto.ProductName"]);
        b.AddSystemColumns(
            x => x.CreationUserName,
            x => x.CreationDate,
            x => x.LastChangeUserName,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["ReportConstructorDto.CreationDate"],
            L["ReportConstructorDto.LastChangeDate"],
            L["ReportConstructorDto.IsArchive"]);

        return b.Build();
    }

    private static void AddNullableEnum<TRu, TEn>(
        GridColumnSettingsBuilder<ReportConstructorDto> builder,
        bool isRu,
        System.Linq.Expressions.Expression<Func<ReportConstructorDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<ReportConstructorDto, TEn?>> en,
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
