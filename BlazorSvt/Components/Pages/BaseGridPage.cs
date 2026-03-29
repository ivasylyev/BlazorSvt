
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Components.Pages;

public abstract class BaseGridPage<TItem> : ComponentBase
{
    [Inject]
    protected PageTimingService TimingService { get; set; } = default!;

    // 👉 Публичный DataProvider, который пойдёт в GenericGrid
    protected async Task<GridDataProviderResult<TItem>> DataProvider(GridDataProviderRequest<TItem> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            return await GetDataAsync(request);
        }
    }

    // 👉 Абстрактный метод — бизнес-логика уходит в наследника
    protected abstract Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request);
}