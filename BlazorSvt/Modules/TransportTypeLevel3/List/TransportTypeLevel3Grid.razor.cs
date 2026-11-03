using BlazorSvt.Modules.TransportTypeLevel3.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportTypeLevel3.List;

public partial class TransportTypeLevel3Grid : BaseGridPage<TransportTypeLevel3Dto, TransportTypeLevel3DetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportTypeLevel3> EL { get; set; } = default!;

    protected override object DetailKeySelector(TransportTypeLevel3Dto request)
        => request.TransportTypeLevel3Id;
}
