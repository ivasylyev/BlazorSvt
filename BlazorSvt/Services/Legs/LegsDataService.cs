using BlazorBootstrap;
using Microsoft.Extensions.Options;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Services.Legs;

public class LegsDataService(
    IOptions<DatabaseOptions> options,
    ILogger<LegsDataService> logger)
    : BaseGridDataService<LegDto>(options, logger), ILegsDataService
{
    protected override string StoredProcedureName
        => "dbo.GetTransportLegs";

    public async Task<GridDataProviderResult<LegDto>> GetLegsAsync(GridDataProviderRequest<LegDto> request, string lang)
    {
        return await GetDataAsync(request, lang);
    }
}