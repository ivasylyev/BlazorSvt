using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;


public partial class GenericDetailView<TItem, TDetailItem> : SvtComponentBase
{
    private DetailSettingsCollection<TDetailItem>? detailSettings;

    [Inject]
    public IDetailSettingsService<TDetailItem> DetailSettingsService { get; set; } = default!;

    [Parameter] 
    public TItem Item { get; set; } = default!;
    [Parameter] 
    public Func<TItem, object> KeySelector { get; set; } = default!;
    [Parameter] 
    public Func<TItem, Task<TDetailItem>> DetailDataProvider { get; set; } = default!;

    [Parameter] 
    public RenderFragment<TDetailItem>? Template { get; set; }


    private static readonly Dictionary<object, TDetailItem> ItemsCache = new();

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null)
            return;

        var key = KeySelector(Item);

        try
        {
            isLoading = true;
            errorMessage = null;
            if (ItemsCache.TryGetValue(key, out var cached))
            {
                detail = cached;
            }
            else
            {
                detail = await DetailDataProvider(Item);
                ItemsCache[key] = detail;
            }
            
            detailSettings ??= DetailSettingsService.GetGridDetailSettings(Lang);
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