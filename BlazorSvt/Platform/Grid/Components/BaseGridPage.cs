using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.Grid.Components;

public abstract class BaseGridPage<TItem, TDetailItem> : SvtComponentBase
{
    [Inject]
    protected PageTimingService TimingService { get; set; } = default!;

    [Inject]
    protected IGridDataService<TItem, TDetailItem> DataService { get; set; } = default!;

    protected async Task<GridDataProviderResult<TItem>> DataProvider(GridDataProviderRequest<TItem> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await GetDataAsync(request, Lang);
        }
    }

    protected async Task<TDetailItem?> DetailDataProvider(TItem request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await GetDetailDataAsync(request, Lang);
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
