using BlazorBootstrap;
using Microsoft.Extensions.Options;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Services.Rates;

public class RatesDataService(
    IOptions<DatabaseOptions> options,
    ILogger<RatesDataService> logger)
    : BaseGridDataService<RateDto>(options, logger), IRatesDataService
{
    protected override string StoredProcedureName
        => "dbo.GetTransportRates";

    public async Task<GridDataProviderResult<RateDto>> GetRatesAsync(GridDataProviderRequest<RateDto> request)
    {
        return await GetDataAsync(request);
    }
}