using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Rates;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class RatesGrid : BaseGridPage<RateDto>
{
    [Inject]
    public IRatesDataService RatesDataService { get; set; } = default!;

    protected override async Task<GridDataProviderResult<RateDto>> GetDataAsync(GridDataProviderRequest<RateDto> request)
    {
        return await RatesDataService.GetRatesAsync(request);
    }

}