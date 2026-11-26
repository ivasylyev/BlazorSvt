using BlazorBootstrap;
using Blazored.LocalStorage;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRoute.List;

public class TransportRouteGridSettingsService(
    ILocalStorageService localStorage,
    IStringLocalizer<Resources.TransportRoute> L,
    IStringLocalizer<PlatformResources> platform,
    ILogger<TransportRouteGridSettingsService> logger)
    : BaseGridSettingsService<TransportRouteDto>(localStorage, logger)
{
    protected override string StorageKey => "TransportRouteGridColumnSettings";

    public override IReadOnlyList<GridColumnSetting<TransportRouteDto>> GetDefaultSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new GridColumnSettingsBuilder<TransportRouteDto>(platform);

        b.Add(x => x.Code, L["TransportRouteDto.Code"]);
        b.Add(x => x.CodeSAP, L["TransportRouteDto.CodeSAP"]);
        b.Add(x => x.Route, L["TransportRouteDto.Route"]);
        b.Add(x => x.StartNodeCode, L["TransportRouteDto.StartNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.StartNodeNameRu, x => x.StartNodeNameEn, L["TransportRouteDto.StartNodeName"]);
        b.Add(x => x.RegionFromCode, L["TransportRouteDto.RegionFromCode"], visible: false);
        b.AddLocalized(isRu, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["TransportRouteDto.RegionFromName"]);
        b.Add(x => x.ProxyNodeCode, L["TransportRouteDto.ProxyNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportRouteDto.ProxyNodeName"]);
        b.Add(x => x.EndNodeCode, L["TransportRouteDto.EndNodeCode"], visible: false);
        b.AddLocalized(isRu, x => x.EndNodeNameRu, x => x.EndNodeNameEn, L["TransportRouteDto.EndNodeName"]);
        b.Add(x => x.RegionToCode, L["TransportRouteDto.RegionToCode"], visible: false);
        b.AddLocalized(isRu, x => x.RegionToNameRu, x => x.RegionToNameEn, L["TransportRouteDto.RegionToName"]);
        b.AddEnum(isRu, x => x.RouteTransportKindIdRu, x => x.RouteTransportKindIdEn, L["TransportRouteDto.RouteTransportKindName"], multiSelect: true);
        b.AddYesNo(x => x.IsActive, L["TransportRouteDto.IsActive"]);
        AddEnum(b, isRu, x => x.Leg1_TransportTypeIdRu, x => x.Leg1_TransportTypeIdEn, L["TransportRouteDto.Leg1_TransportTypeName"], multiSelect: true);
        AddEnum(b, isRu, x => x.Leg2_TransportTypeIdRu, x => x.Leg2_TransportTypeIdEn, L["TransportRouteDto.Leg2_TransportTypeName"], multiSelect: true);
        b.Add(x => x.ChainNodes, L["TransportRouteDto.ChainNodes"]);
        b.AddSystemColumns(
            x => x.CreationUserName,
            x => x.CreationDate,
            x => x.LastChangeUserName,
            x => x.LastChangeDate,
            x => x.IsArchive,
            L["TransportRouteDto.CreationDate"],
            L["TransportRouteDto.LastChangeDate"],
            L["TransportRouteDto.IsArchive"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        GridColumnSettingsBuilder<TransportRouteDto> builder,
        bool isRu,
        System.Linq.Expressions.Expression<Func<TransportRouteDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<TransportRouteDto, TEn?>> en,
        string header,
        bool multiSelect = false)
        where TRu : struct, Enum
        where TEn : struct, Enum
    {
        if (isRu)
        {
            var compiled = ru.Compile();
            builder.Add(ru, header, display: dto => Label(compiled(dto)), multiSelect: multiSelect);
        }
        else
        {
            var compiled = en.Compile();
            builder.Add(en, header, display: dto => Label(compiled(dto)), multiSelect: multiSelect);
        }
    }

    private static string Label<TEnum>(TEnum? value) where TEnum : struct, Enum
        => value is { } current
            ? typeof(TEnum).GetDisplayName(current.ToString()) ?? string.Empty
            : string.Empty;
}
