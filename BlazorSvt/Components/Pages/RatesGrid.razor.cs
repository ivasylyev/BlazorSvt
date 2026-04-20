using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class RatesGrid : BaseGridPage<RateDto, RateDetailDto>
{
    [Inject] 
    public IGridDataService<RateDto, RateDetailDto> RatesDataService { get; set; } = default!;

    protected override object DetailKeySelector(RateDto request)
        => request.RateId;

    protected override async Task<GridDataProviderResult<RateDto>> GetDataAsync(GridDataProviderRequest<RateDto> request, string lang) 
        => await RatesDataService.GetDataAsync(request, lang);

    protected override async Task<RateDetailDto> GetDetailDataAsync(RateDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await RatesDataService.GetDetailDataAsync(key);
    }
    private string GetCustomMessage(RateDetailDto detail)
    {
        var message = L["RateDetailDto.CustomMessage"].Value;
        message += detail.ProxyNodeCode is null
            ? L["RateDetailDto.CustomMessageWithoutProxy"].Value 
            : L["RateDetailDto.CustomMessageWithProxy"].Value; 
        return message;
    }
}