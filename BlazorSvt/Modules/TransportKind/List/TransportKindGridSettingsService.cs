using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportKind.List;

public class TransportKindGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.TransportKind> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<TransportKindGridSettingsService> logger)
    : BaseGridSettingsService<TransportKindDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportKindGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<TransportKindDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<TransportKindDto>(platform);

        b.Add(x => x.Code, L["TransportKindDto.Code"]);
        b.Add(x => x.Name, L["TransportKindDto.Name"]);
        b.Add(x => x.NameEnRu, L["TransportKindDto.NameEnRu"]);
        AddEnum(b, isRu, x => x.StatusIdRu, x => x.StatusIdEn, L["TransportKindDto.Status"]);
        b.Add(x => x.Level, L["TransportKindDto.Level"]);
        b.Add(x => x.KindCode, L["TransportKindDto.KindCode"]);
        b.AddSystemColumns(
            x => x.CreationDate,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["TransportKindDto.CreationDate"],
            L["TransportKindDto.LastChangeDate"],
            L["TransportKindDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        GridColumnSettingsBuilder<TransportKindDto> builder,
        bool isRu,
        System.Linq.Expressions.Expression<Func<TransportKindDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<TransportKindDto, TEn?>> en,
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
