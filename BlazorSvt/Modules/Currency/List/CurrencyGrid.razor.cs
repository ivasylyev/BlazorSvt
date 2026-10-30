using BlazorSvt.Modules.Currency.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Currency.List;

public partial class CurrencyGrid : BaseGridPage<CurrencyDto, CurrencyDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Currency> EL { get; set; } = default!;

    protected override object DetailKeySelector(CurrencyDto request)
        => request.CurrencyId;
}
