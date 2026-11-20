using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRoute.Detail;

/// <summary>«Плечо маршрута» — дочерний примитив TransportRouteLeg (PEI 2035).</summary>
public class TransportRouteLegSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportRoute> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportRouteDetailDto, TransportRouteLegDto>(dataService)
{
    public override string Title => L["TransportRouteLegDto.Title"];

    protected override object ParentKey(TransportRouteDetailDto parent) => parent.TransportRouteId;

    protected override bool ParentIsArchive(TransportRouteDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<TransportRouteLegDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<TransportRouteLegDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["TransportRouteLegDto.Code"]);
        b.Add(group, x => x.Step, L["TransportRouteLegDto.Step"]);
        b.Add(group, x => x.LegCode, L["TransportRouteLegDto.LegCode"]);
        b.AddLocalized(isRu, group, x => x.NodeFromNameRu, x => x.NodeFromNameEn, L["TransportRouteLegDto.NodeFromName"]);
        b.AddLocalized(isRu, group, x => x.ProxyNodeNameRu, x => x.ProxyNodeNameEn, L["TransportRouteLegDto.ProxyNodeName"]);
        b.AddLocalized(isRu, group, x => x.NodeToNameRu, x => x.NodeToNameEn, L["TransportRouteLegDto.NodeToName"]);
        b.AddEnum(isRu, group, x => x.TransportKindIdRu, x => x.TransportKindIdEn, L["TransportRouteLegDto.TransportKindName"]);
        b.AddEnum(isRu, group, x => x.Leg1_TransportTypeIdRu, x => x.Leg1_TransportTypeIdEn, L["TransportRouteLegDto.Leg1_TransportTypeName"]);
        b.AddEnum(isRu, group, x => x.Leg2_TransportTypeIdRu, x => x.Leg2_TransportTypeIdEn, L["TransportRouteLegDto.Leg2_TransportTypeName"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["TransportRouteLegDto.IsArchive"]);

        return b.Build();
    }
}
