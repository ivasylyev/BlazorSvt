using BlazorSvt.Modules.Region.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Region.List;

public partial class RegionGrid : BaseGridPage<RegionDto, RegionDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Region> EL { get; set; } = default!;

    protected override object DetailKeySelector(RegionDto request)
        => request.RegionId;
}
