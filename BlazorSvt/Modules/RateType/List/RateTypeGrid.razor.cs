using BlazorSvt.Modules.RateType.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateType.List;

public partial class RateTypeGrid : BaseGridPage<RateTypeDto, RateTypeDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.RateType> EL { get; set; } = default!;

    protected override object DetailKeySelector(RateTypeDto request)
        => request.RateTypeId;
}
