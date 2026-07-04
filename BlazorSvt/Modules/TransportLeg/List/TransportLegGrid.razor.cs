using BlazorBootstrap;
using BlazorSvt.Modules.TransportLeg.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.TransportLeg.List;

public partial class TransportLegGrid : BaseGridPage<TransportLegDto, TransportLegDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.TransportLeg> EL { get; set; } = default!;

    [Inject] 
    public IGridDataService<TransportLegDto, TransportLegDetailDto> TransportLegDataService { get; set; } = default!;

    protected override object DetailKeySelector(TransportLegDto request)
    {
        return request.TransportLegId;
    }

    protected override async Task<GridDataProviderResult<TransportLegDto>> GetDataAsync(GridDataProviderRequest<TransportLegDto> request, string lang)
    {
        return await TransportLegDataService.GetDataAsync(request, lang);
    }

    protected override async Task<TransportLegDetailDto> GetDetailDataAsync(TransportLegDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await TransportLegDataService.GetDetailDataAsync(key);
    }

    private string GetCustomMessage(TransportLegDetailDto detail)
    {
        var message = EL["TransportLegDetailDto.CustomMessage"].Value;
        message += detail.ProxyNodeCode is null
            ? EL["TransportLegDetailDto.CustomMessageWithoutProxy"].Value
            : EL["TransportLegDetailDto.CustomMessageWithProxy"].Value;
        return message;
    }
}