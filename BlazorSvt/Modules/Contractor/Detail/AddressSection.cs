using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Contractor.Detail;

/// <summary>«Адреса контрагентов» — дочерний примитив Address (PEI 2027).</summary>
public class AddressSection(
    IDetailChildDataService dataService,
    IStringLocalizer<Resources.Contractor> L,
    IStringLocalizer<PlatformResources> platform)
    : DetailChildSection<ContractorDetailDto, AddressDto>(dataService)
{
    public override string Title => L["AddressDto.Title"];

    protected override object ParentKey(ContractorDetailDto parent) => parent.ContractorId;

    protected override bool ParentIsArchive(ContractorDetailDto parent) => parent.IsArchive;

    protected override DetailSettingsCollection<AddressDto> BuildColumns(string lang, bool parentIsArchive)
    {
        var isRu = lang == "ru";
        var b = new DetailSettingsBuilder<AddressDto>(platform);
        var group = Title;

        b.Add(group, x => x.Code, L["AddressDto.Code"]);
        b.Add(group, x => x.AddressType, L["AddressDto.AddressType"]);
        b.Add(group, x => x.City, L["AddressDto.City"]);
        b.AddEnum(isRu, group, x => x.StatusIdRu, x => x.StatusIdEn, L["AddressDto.Status"]);
        b.Add(group, x => x.AddressKindName, L["AddressDto.AddressKind"]);
        b.Add(group, x => x.Pobox, L["AddressDto.Pobox"]);
        b.Add(group, x => x.RegionCode, L["AddressDto.RegionCode"]);
        b.Add(group, x => x.District_ru, L["AddressDto.District_ru"]);
        b.Add(group, x => x.Street, L["AddressDto.Street"]);
        b.Add(group, x => x.House, L["AddressDto.House"]);
        b.Add(group, x => x.OfficeApart, L["AddressDto.OfficeApart"]);
        b.Add(group, x => x.FullAddress_ru, L["AddressDto.FullAddress_ru"]);
        b.Add(group, x => x.CoordinateW, L["AddressDto.CoordinateW"]);
        b.Add(group, x => x.CoordinateL, L["AddressDto.CoordinateL"]);
        b.Add(group, x => x.FIASCodeRegion, L["AddressDto.FIASCodeRegion"]);
        b.Add(group, x => x.FIASCodeDistrict, L["AddressDto.FIASCodeDistrict"]);
        b.Add(group, x => x.FIASCodeSettlement, L["AddressDto.FIASCodeSettlement"]);
        b.AddLocalized(isRu, group, x => x.RegionNameRu, x => x.RegionNameEn, L["AddressDto.Region"]);
        b.AddLocalized(isRu, group, x => x.CountryNameRu, x => x.CountryNameEn, L["AddressDto.Country"]);
        if (parentIsArchive)
            b.AddArchiveStatus(group, x => x.IsArchive, L["AddressDto.IsArchive"]);

        return b.Build();
    }
}
