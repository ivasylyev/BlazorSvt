using BlazorBootstrap;
using BlazorSvt.Models.Dto;

namespace BlazorSvt.Services.Rates;

public interface IRatesDataService 
{
    public Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request, string lang);
}