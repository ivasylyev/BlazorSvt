using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.Detail;

/// <summary>«Лидтаймы» — дочерний примитив Leadtime (PEI 2126).</summary>
public class LeadtimeSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.TransportLeg> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<TransportLegDetailDto, LeadtimeDto>(dataService)
{
    public override string Title => L["LeadtimeDto.Title"];

    protected override object ParentKey(TransportLegDetailDto parent) => parent.TransportLegId;

    protected override bool ParentIsArchive(TransportLegDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<LeadtimeDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<LeadtimeDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["LeadtimeDto.Code"]);
        b.Add(group, x => x.StartDate, L["LeadtimeDto.StartDate"]);
        b.Add(group, x => x.EndDate, L["LeadtimeDto.EndDate"]);
        b.AddLocalized(isRu, group, x => x.ShipmentTypeNameRu, x => x.ShipmentTypeNameEn, L["LeadtimeDto.ShipmentTypeName"]);
        b.AddYesNo(group, x => x.LeadTimeIsActive, L["LeadtimeDto.LeadTimeIsActive"]);
        b.Add(group, x => x.CommercialLT, L["LeadtimeDto.CommercialLT"]);
        b.Add(group, x => x.SearchTime, L["LeadtimeDto.SearchTime"]);
        b.Add(group, x => x.LoadTime, L["LeadtimeDto.LoadTime"]);
        b.Add(group, x => x.TravelTime, L["LeadtimeDto.TravelTime"]);
        b.Add(group, x => x.DaysWaiting, L["LeadtimeDto.DaysWaiting"]);
        b.Add(group, x => x.UnLoadTime, L["LeadtimeDto.UnLoadTime"]);
        b.Add(group, x => x.TransportationTime, L["LeadtimeDto.TransportationTime"]);
        b.Add(group, x => x.Distance, L["LeadtimeDto.Distance"]);
        b.Add(group, x => x.Leg1_SearchTime, L["LeadtimeDto.Leg1_SearchTime"]);
        b.Add(group, x => x.Leg1_LoadTime, L["LeadtimeDto.Leg1_LoadTime"]);
        b.Add(group, x => x.Leg1_TravelTime, L["LeadtimeDto.Leg1_TravelTime"]);
        b.Add(group, x => x.Leg1_DaysWaiting, L["LeadtimeDto.Leg1_DaysWaiting"]);
        b.Add(group, x => x.Leg1_TransportationTime, L["LeadtimeDto.Leg1_TransportationTime"]);
        b.Add(group, x => x.Leg1_Distance, L["LeadtimeDto.Leg1_Distance"]);
        b.Add(group, x => x.Leg2_TravelTime, L["LeadtimeDto.Leg2_TravelTime"]);
        b.Add(group, x => x.Leg2_DaysWaiting, L["LeadtimeDto.Leg2_DaysWaiting"]);
        b.Add(group, x => x.Leg2_UploadTime, L["LeadtimeDto.Leg2_UploadTime"]);
        b.Add(group, x => x.Leg2_TransportationTime, L["LeadtimeDto.Leg2_TransportationTime"]);
        b.Add(group, x => x.Leg2_Distance, L["LeadtimeDto.Leg2_Distance"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["LeadtimeDto.IsArchive"]);

        return b.Build();
    }
}
