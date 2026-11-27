using BlazorBootstrap;
using BlazorSvt.Platform.Domain.IdsEnum;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRoute.Detail;

public class TransportRouteDetailSettingsService(
    IStringLocalizer<Resources.TransportRoute> L,
    IStringLocalizer<PlatformResources> platform)
    : IDetailSettingsService<TransportRouteDetailDto>
{
    public DetailSettingsCollection<TransportRouteDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportRouteDetailDto>(platform);
        var group = L["TransportRouteDetailDto.Group.0.Default"];

        b.Add(group, x => x.Code, L["TransportRouteDetailDto.Code"]);
        b.Add(group, x => x.CodeSAP, L["TransportRouteDetailDto.CodeSAP"]);
        b.AddYesNo(group, x => x.IsActive, L["TransportRouteDetailDto.IsActive"]);
        b.Add(group, x => x.Route, L["TransportRouteDetailDto.Route"]);
        b.AddLocalized(isRu, group, x => x.StartNodeNameRu, x => x.StartNodeNameEn, L["TransportRouteDetailDto.StartNodeName"]);
        b.Add(group, x => x.StartNodeCode, L["TransportRouteDetailDto.StartNodeCode"]);
        b.AddLocalized(isRu, group, x => x.RegionFromNameRu, x => x.RegionFromNameEn, L["TransportRouteDetailDto.RegionFromName"]);
        b.Add(group, x => x.RegionFromCode, L["TransportRouteDetailDto.RegionFromCode"]);
        b.AddLocalized(isRu, group, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportRouteDetailDto.ProxyNodeName"]);
        b.Add(group, x => x.ProxyNodeCode, L["TransportRouteDetailDto.ProxyNodeCode"]);
        b.AddLocalized(isRu, group, x => x.EndNodeNameRu, x => x.EndNodeNameEn, L["TransportRouteDetailDto.EndNodeName"]);
        b.Add(group, x => x.EndNodeCode, L["TransportRouteDetailDto.EndNodeCode"]);
        b.AddLocalized(isRu, group, x => x.RegionToNameRu, x => x.RegionToNameEn, L["TransportRouteDetailDto.RegionToName"]);
        b.Add(group, x => x.RegionToCode, L["TransportRouteDetailDto.RegionToCode"]);
        b.AddEnum(isRu, group, x => x.RouteTransportKindIdRu, x => x.RouteTransportKindIdEn, L["TransportRouteDetailDto.RouteTransportKindName"]);
        AddEnum(b, isRu, group, x => x.Leg1_TransportTypeIdRu, x => x.Leg1_TransportTypeIdEn, L["TransportRouteDetailDto.Leg1_TransportTypeName"]);
        AddEnum(b, isRu, group, x => x.Leg2_TransportTypeIdRu, x => x.Leg2_TransportTypeIdEn, L["TransportRouteDetailDto.Leg2_TransportTypeName"]);
        b.Add(group, x => x.ShortNameRu, L["TransportRouteDetailDto.ShortNameRu"]);
        b.Add(group, x => x.ShortNameEN, L["TransportRouteDetailDto.ShortNameEN"]);
        b.Add(group, x => x.NameEN, L["TransportRouteDetailDto.NameEN"]);
        b.Add(group, x => x.ChainNodes, L["TransportRouteDetailDto.ChainNodes"]);
        b.Add(group, x => x.Leg_01Code, L["TransportRouteDetailDto.Leg_01Code"]);
        b.Add(group, x => x.Leg_02Code, L["TransportRouteDetailDto.Leg_02Code"]);
        b.Add(group, x => x.Leg_03Code, L["TransportRouteDetailDto.Leg_03Code"]);
        b.AddArchiveStatus(group, x => x.IsArchive, L["TransportRouteDetailDto.IsArchive"]);
        b.AddCreationUser(group, x => x.CreationUserName);
        b.Add(group, x => x.CreationDate, L["TransportRouteDetailDto.CreationDate"]);
        b.AddLastChangeUser(group, x => x.LastChangeUserName);
        b.Add(group, x => x.LastChangeDate, L["TransportRouteDetailDto.LastChangeDate"]);

        return b.Build();
    }

    private static void AddEnum<TRu, TEn>(
        DetailSettingsBuilder<TransportRouteDetailDto> builder,
        bool isRu,
        string group,
        System.Linq.Expressions.Expression<Func<TransportRouteDetailDto, TRu?>> ru,
        System.Linq.Expressions.Expression<Func<TransportRouteDetailDto, TEn?>> en,
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
