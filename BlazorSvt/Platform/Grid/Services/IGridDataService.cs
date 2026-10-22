using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Read-path справочника: list из snapshot через <c>v2.GetBlazorGridData</c>,
/// detail из view (<see cref="DetailSourceAttribute"/>), отчёты — batch по тем же фильтрам.
/// </summary>
/// <typeparam name="TItem">List-DTO с <see cref="GridSnapshotAttribute"/>.</typeparam>
/// <typeparam name="TDetailItem">Detail-DTO с <see cref="DetailSourceAttribute"/>.</typeparam>
/// <remarks>
/// Перегрузки с <see cref="GridQuery"/> — общий путь для Blazor-грида и API:
/// запрос UI сначала превращается в <see cref="GridQuery"/>, затем идёт тем же кодом.
/// </remarks>
public interface IGridDataService<TItem, TDetailItem>
{
    /// <summary>Страница grid (snapshot + фильтры/сортировка/пагинация).</summary>
    Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request);

    /// <summary>Страница grid по готовому <see cref="GridQuery"/>.</summary>
    Task<GridPage<TItem>> GetPageAsync(GridQuery query, CancellationToken cancellationToken);

    /// <summary>Одна запись detail по бизнес-ключу (<c>SELECT * FROM {DetailSource}</c>).</summary>
    Task<TDetailItem> GetDetailDataAsync(object key, CancellationToken cancellationToken = default);

    /// <summary>Одна запись detail по бизнес-ключу; <c>null</c>, если записи нет.</summary>
    Task<TDetailItem?> FindDetailAsync(object key, CancellationToken cancellationToken);

    /// <summary>Пакет краткого Excel-отчёта (те же колонки, что grid; удлинённый таймаут).</summary>
    Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>Пакет краткого Excel-отчёта; страница и размер пакета — из <paramref name="query"/>.</summary>
    Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(GridQuery query, CancellationToken cancellationToken);

    /// <summary>Пакет полного Excel-отчёта через <c>v2.ExportBlazorGridDetail</c>.</summary>
    Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>Пакет полного Excel-отчёта; страница и размер пакета — из <paramref name="query"/>.</summary>
    Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(GridQuery query, CancellationToken cancellationToken);
}
