using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

public interface IGridDataService<TItem, TDetailItem>
{
    Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);

    Task<TDetailItem> GetDetailDataAsync(object key);

    Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
