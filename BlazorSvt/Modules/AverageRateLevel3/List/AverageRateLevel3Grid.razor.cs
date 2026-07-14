using BlazorSvt.Modules.AverageRateLevel3.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.List;

public partial class AverageRateLevel3Grid : BaseGridPage<AverageRateLevel3Dto, AverageRateLevel3DetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.AverageRateLevel3> EL { get; set; } = default!;

    protected override object DetailKeySelector(AverageRateLevel3Dto request)
        => request.AverageRateLevel3Id;
}
