using BlazorBootstrap;
using BlazorSvt.Components.Common;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public abstract class BaseGridPage<TItem, TDetailItem> : SvtComponentBase
{
    [Inject] 
    protected PageTimingService TimingService { get; set; } = default!;

    protected async Task<GridDataProviderResult<TItem>> DataProvider(GridDataProviderRequest<TItem> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await GetDataAsync(request, Lang);
        }
    }

    protected async Task<TDetailItem?> DetailsDataProvider(TItem request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await GetDetailDataAsync(request, Lang);
        }
    }

    protected abstract Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);

    protected abstract Task<TDetailItem> GetDetailDataAsync(TItem request, string lang);

    protected abstract object DetailKeySelector(TItem request);
}