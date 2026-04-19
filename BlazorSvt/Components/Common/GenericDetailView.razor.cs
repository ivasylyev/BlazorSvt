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


    private static readonly Dictionary<object, TDetailItem> Cache = new();

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null)
            return;

        detailSettings ??= DetailSettingsService.GetGridDetailSettings(Lang);

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

            detail = await DetailDataProvider(Item);
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