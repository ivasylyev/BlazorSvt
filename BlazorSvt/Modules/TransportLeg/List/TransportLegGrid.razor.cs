using BlazorBootstrap;
using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Modules.TransportLeg.List;

public partial class TransportLegGrid : BaseGridPage<TransportLegDto, TransportLegDetailDto>
{
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
        var message = L["TransportLegDetailDto.CustomMessage"].Value;//"Кастомное сообщение от транспортных плеч: ";
        message += detail.ProxyNodeCode is null
            ? L["TransportLegDetailDto.CustomMessageWithoutProxy"].Value //"Плечо без промежуточного узла"
            : L["TransportLegDetailDto.CustomMessageWithProxy"].Value; //"Плечо с промежуточным узлом";
        return message;
    }
}