using System.Diagnostics;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.Grid.Components;

/// <summary>
/// Базовая страница справочника: grid + detail через <see cref="IGridDataService{TItem,TDetailItem}"/>,
/// замер времени через <see cref="PageTimingService"/>.
/// </summary>
public abstract class BaseGridPage<TItem, TDetailItem> : SvtComponentBase
{
    [Inject]
    protected PageTimingService TimingService { get; set; } = default!;

    [Inject]
    protected IGridDataService<TItem, TDetailItem> DataService { get; set; } = default!;

    [Inject]
    protected ILogger<BaseGridPage<TItem, TDetailItem>> Logger { get; set; } = default!;

    protected async Task<GridDataProviderResult<TItem>> DataProvider(GridDataProviderRequest<TItem> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await GetDataAsync(request, Lang);

            Logger.LogDebug(
                "Grid {ItemType} page {PageNumber}/{PageSize} loaded {Count}/{TotalCount} in {ElapsedMs} ms",
                typeof(TItem).Name,
                request.PageNumber,
                request.PageSize,
                result.Data?.Count() ?? 0,
                result.TotalCount,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
    }

    protected async Task<TDetailItem?> DetailDataProvider(TItem request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            var stopwatch = Stopwatch.StartNew();
            var detail = await GetDetailDataAsync(request, Lang);

            Logger.LogDebug(
                "Detail {DetailType} for {ItemType} loaded in {ElapsedMs} ms",
                typeof(TDetailItem).Name,
                typeof(TItem).Name,
                stopwatch.ElapsedMilliseconds);

            return detail;
        }
    }

    /// <summary>Бизнес-ключ строки для detail/export (обычно {Entity}Id).</summary>
    protected abstract object DetailKeySelector(TItem request);

    protected virtual Task<GridDataProviderResult<TItem>> GetDataAsync(
        GridDataProviderRequest<TItem> request,
        string lang) =>
        DataService.GetDataAsync(request, lang);

    protected virtual Task<TDetailItem> GetDetailDataAsync(TItem request, string lang)
    {
        var key = DetailKeySelector(request);
        return DataService.GetDetailDataAsync(key);
    }
}
