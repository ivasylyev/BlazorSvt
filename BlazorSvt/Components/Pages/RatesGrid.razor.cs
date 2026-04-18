using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class RatesGrid : BaseGridPage<RateDto, RateDetailsDto>
{
    [Inject] public IGridDataService<RateDto> RatesDataService { get; set; } = default!;

    protected override async Task<GridDataProviderResult<RateDto>> GetDataAsync(GridDataProviderRequest<RateDto> request, string lang)
    {
        return await RatesDataService.GetDataAsync(request, lang);
    }

    protected override async Task<RateDetailsDto> GetDetailDataAsync(RateDto request, string lang)
    {
        await Task.CompletedTask;
        return new RateDetailsDto();
    }


}