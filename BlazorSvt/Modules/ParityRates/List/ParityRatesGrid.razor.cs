using BlazorSvt.Modules.ParityRates.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ParityRates.List;

public partial class ParityRatesGrid : BaseGridPage<ParityRatesDto, ParityRatesDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.ParityRates> EL { get; set; } = default!;

    protected override object DetailKeySelector(ParityRatesDto request)
        => request.ParityRatesId;
}
