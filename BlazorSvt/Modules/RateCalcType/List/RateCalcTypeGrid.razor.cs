using BlazorSvt.Modules.RateCalcType.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateCalcType.List;

public partial class RateCalcTypeGrid : BaseGridPage<RateCalcTypeDto, RateCalcTypeDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.RateCalcType> EL { get; set; } = default!;

    protected override object DetailKeySelector(RateCalcTypeDto request)
        => request.RateCalcTypeId;
}
