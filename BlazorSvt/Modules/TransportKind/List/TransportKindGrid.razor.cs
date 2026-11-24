using BlazorSvt.Modules.TransportKind.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportKind.List;

public partial class TransportKindGrid : BaseGridPage<TransportKindDto, TransportKindDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportKind> EL { get; set; } = default!;

    protected override object DetailKeySelector(TransportKindDto request)
        => request.TransportKindId;
}
