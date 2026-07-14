using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.Grid.Components;


public partial class GenericDetailView<TItem, TDetailItem> : SvtComponentBase
{
    private DetailSettingsCollection<TDetailItem>? detailSettings;

    [Inject]
    public IDetailSettingsService<TDetailItem> DetailSettingsService { get; set; } = default!;

    [Inject]
    public ILogger<GenericDetailView<TItem, TDetailItem>> Logger { get; set; } = default!;

    [Parameter]
    public TItem Item { get; set; } = default!;
    [Parameter]
    public Func<TItem, Task<TDetailItem>> DetailDataProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment<TDetailItem>? Template { get; set; }

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null)
            return;

        try
        {
            isLoading = true;
            errorMessage = null;
            detail = await DetailDataProvider(Item);
            detailSettings ??= DetailSettingsService.GetGridDetailSettings(Lang);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load detail for {ItemType}", typeof(TItem).Name);
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}
