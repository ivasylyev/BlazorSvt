using BlazorBootstrap;
using BlazorSvt.Components.Common;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace BlazorSvt.Components.Pages;

public abstract class BaseGridPage<TItem> : SvtComponentBase
{
    [Inject] protected PageTimingService TimingService { get; set; } = default!;

    protected async Task<GridDataProviderResult<TItem>> DataProvider(GridDataProviderRequest<TItem> request)
    {
        using (new StopwatchTransaction(TimingService))
        {
            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return await GetDataAsync(request, lang);
        }
    }

    protected abstract Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang);
}