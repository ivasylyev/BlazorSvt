using BlazorSvt.Modules.TransportRate.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportRate.List;

public partial class TransportRateGrid : BaseGridPage<TransportRateDto, TransportRateDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportRate> EL { get; set; } = default!;

    protected override object DetailKeySelector(TransportRateDto request)
        => request.TransportRateId;

    private string GetCustomMessage(TransportRateDetailDto detail)
    {
        var message = EL["TransportRateDetailDto.CustomMessage"].Value;
        message += detail.ProxyNodeCode is null
            ? EL["TransportRateDetailDto.CustomMessageWithoutProxy"].Value
            : EL["TransportRateDetailDto.CustomMessageWithProxy"].Value;
        return message;
    }
}
