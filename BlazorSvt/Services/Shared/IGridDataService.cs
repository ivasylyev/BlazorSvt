using BlazorBootstrap;

namespace BlazorSvt.Services.Shared;

public interface IGridDataService<TItem, TDetailItem>
{
    Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);
    Task<TDetailItem> GetDetailDataAsync(object key);
    Task<IReadOnlyList<TItem>> GetShortReportDataAsync(GridDataProviderRequest<TItem> request, string lang, int totalCount);
    Task<IReadOnlyList<TDetailItem>> GetFullReportDataAsync(GridDataProviderRequest<TItem> request, string lang, int totalCount);
}