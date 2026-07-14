using BlazorSvt.Modules.TransportLeg.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.List;

public partial class TransportLegGrid : BaseGridPage<TransportLegDto, TransportLegDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportLeg> EL { get; set; } = default!;

    protected override object DetailKeySelector(TransportLegDto request)
        => request.TransportLegId;

    private string GetCustomMessage(TransportLegDetailDto detail)
    {
        var message = EL["TransportLegDetailDto.CustomMessage"].Value;
        message += detail.ProxyNodeCode is null
            ? EL["TransportLegDetailDto.CustomMessageWithoutProxy"].Value
            : EL["TransportLegDetailDto.CustomMessageWithProxy"].Value;
        return message;
    }
}
