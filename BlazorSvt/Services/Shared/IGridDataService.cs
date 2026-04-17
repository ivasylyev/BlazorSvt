using BlazorBootstrap;

namespace BlazorSvt.Services.Shared;

public interface IGridDataService<TItem>
{
    Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);
}