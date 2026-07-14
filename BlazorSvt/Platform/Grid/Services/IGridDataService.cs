using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Read-path справочника: list из snapshot через <c>v2.GetBlazorGridData</c>,
/// detail из view (<see cref="DetailSourceAttribute"/>), отчёты — batch по тем же фильтрам.
/// </summary>
/// <typeparam name="TItem">List-DTO с <see cref="GridSnapshotAttribute"/>.</typeparam>
/// <typeparam name="TDetailItem">Detail-DTO с <see cref="DetailSourceAttribute"/>.</typeparam>
public interface IGridDataService<TItem, TDetailItem>
{
    /// <summary>Страница grid (snapshot + фильтры/сортировка/пагинация).</summary>
    Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);

    /// <summary>Одна запись detail по бизнес-ключу (<c>SELECT * FROM {DetailSource}</c>).</summary>
    Task<TDetailItem> GetDetailDataAsync(object key);

    /// <summary>Пакет краткого Excel-отчёта (те же колонки, что grid; удлинённый таймаут).</summary>
    Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>Пакет полного Excel-отчёта через <c>v2.ExportBlazorGridDetail</c>.</summary>
    Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
