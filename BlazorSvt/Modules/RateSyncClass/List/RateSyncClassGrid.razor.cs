using BlazorSvt.Modules.RateSyncClass.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateSyncClass.List;

public partial class RateSyncClassGrid : BaseGridPage<RateSyncClassDto, RateSyncClassDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.RateSyncClass> EL { get; set; } = default!;

    protected override object DetailKeySelector(RateSyncClassDto request)
        => request.RateSyncClassId;
}
