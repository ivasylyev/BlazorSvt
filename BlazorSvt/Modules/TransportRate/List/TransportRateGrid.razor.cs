using BlazorBootstrap;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Modules.TransportRate.List;

public partial class TransportRateGrid : BaseGridPage<TransportRateDto, TransportRateDetailDto>
{
    [Inject] 
    public IGridDataService<TransportRateDto, TransportRateDetailDto> TransportRateDataService { get; set; } = default!;

    protected override object DetailKeySelector(TransportRateDto request)
        => request.TransportRateId;

    protected override async Task<GridDataProviderResult<TransportRateDto>> GetDataAsync(GridDataProviderRequest<TransportRateDto> request, string lang) 
        => await TransportRateDataService.GetDataAsync(request, lang);

    protected override async Task<TransportRateDetailDto> GetDetailDataAsync(TransportRateDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await TransportRateDataService.GetDetailDataAsync(key);
    }
    private string GetCustomMessage(TransportRateDetailDto detail)
    {
        var message = L["TransportRateDetailDto.CustomMessage"].Value;
        message += detail.ProxyNodeCode is null
            ? L["TransportRateDetailDto.CustomMessageWithoutProxy"].Value 
            : L["TransportRateDetailDto.CustomMessageWithProxy"].Value; 
        return message;
    }
}

