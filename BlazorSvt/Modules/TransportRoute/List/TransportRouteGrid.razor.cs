using BlazorSvt.Modules.TransportRoute.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRoute.List;

public partial class TransportRouteGrid : BaseGridPage<TransportRouteDto, TransportRouteDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportRoute> EL { get; set; } = default!;

    protected override object DetailKeySelector(TransportRouteDto request)
        => request.TransportRouteId;
}
