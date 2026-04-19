using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class RatesGrid : BaseGridPage<RateDto, RateDetailsDto>
{
    [Inject] 
    public IGridDataService<RateDto, RateDetailsDto> RatesDataService { get; set; } = default!;

    protected override object DetailKeySelector(RateDto request)
        => request.RateId;

    protected override async Task<GridDataProviderResult<RateDto>> GetDataAsync(GridDataProviderRequest<RateDto> request, string lang) 
        => await RatesDataService.GetDataAsync(request, lang);

    protected override async Task<RateDetailsDto> GetDetailDataAsync(RateDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await RatesDataService.GetDetailDataAsync(key);
    }
    private string GetCustomMessage(RateDetailsDto detail)
    {
        var message = "Кастомное сообщение от ставок: ";
        message += detail.ProxyNodeCode is null
            ? "Ставка без промежуточного узла"
            : "Ставка с промежуточным узлом";
        return message;
    }
}