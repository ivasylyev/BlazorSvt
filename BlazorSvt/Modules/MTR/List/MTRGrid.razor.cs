using BlazorSvt.Modules.MTR.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.MTR.List;

public partial class MTRGrid : BaseGridPage<MTRDto, MTRDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.MTR> EL { get; set; } = default!;

    protected override object DetailKeySelector(MTRDto request)
        => request.MTRId;
}
