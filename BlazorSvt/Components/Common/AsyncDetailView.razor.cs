using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;


public partial class AsyncDetailView<TItem, TDetailItem> : ComponentBase
{
    [Parameter] 
    public TItem Item { get; set; } = default!;
    [Parameter] 
    public Func<TItem, object> KeySelector { get; set; } = default!;
    [Parameter] 
    public Func<TItem, Task<TDetailItem>> DetailsDataProvider { get; set; } = default!;
    [Parameter] 
    public RenderFragment<TDetailItem>? Template { get; set; }

    private static readonly Dictionary<object, TDetailItem> Cache = new();

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null)
            return;
        var key = KeySelector(Item);

        if (Cache.TryGetValue(key, out var cached))
        {
            detail = cached;
            return;
        }

        try
        {
            isLoading = true;
            errorMessage = null;

            detail = await DetailsDataProvider(Item);
            Cache[key] = detail;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}