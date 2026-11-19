using BlazorSvt.Modules.Coefficient.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Coefficient.List;

public partial class CoefficientGrid : BaseGridPage<CoefficientDto, CoefficientDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Coefficient> EL { get; set; } = default!;

    protected override object DetailKeySelector(CoefficientDto request)
        => request.CoefficientId;
}
