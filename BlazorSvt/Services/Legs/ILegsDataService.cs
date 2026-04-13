using BlazorBootstrap;
using BlazorSvt.Models.Dto;

namespace BlazorSvt.Services.Legs;

public interface ILegsDataService 
{
    public Task<GridDataProviderResult<LegDto>> GetLegsAsync(GridDataProviderRequest<LegDto> request, string lang);
}