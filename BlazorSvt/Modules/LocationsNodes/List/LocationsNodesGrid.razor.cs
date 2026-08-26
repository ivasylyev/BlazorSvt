using BlazorSvt.Modules.LocationsNodes.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.List;

public partial class LocationsNodesGrid : BaseGridPage<LocationsNodesDto, LocationsNodesDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.LocationsNodes> EL { get; set; } = default!;

    protected override object DetailKeySelector(LocationsNodesDto request)
        => request.LocationsNodesId;
}
