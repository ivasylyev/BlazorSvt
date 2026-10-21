using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Grid.Components;


public partial class GenericDetailView<TItem, TDetailItem> : SvtComponentBase, IDisposable
{
    private DetailSettingsCollection<TDetailItem>? detailSettings;
    private Accordion? accordion;
    private bool allExpanded = true;
    private CancellationTokenSource? loadCancellationTokenSource;

    [Inject]
    public IDetailSettingsService<TDetailItem> DetailSettingsService { get; set; } = default!;

    [Inject]
    public IOptions<GridOptions> GridOptions { get; set; } = default!;

    [Inject]
    public ILogger<GenericDetailView<TItem, TDetailItem>> Logger { get; set; } = default!;

    [Parameter]
    public TItem Item { get; set; } = default!;
    [Parameter]
    public Func<TItem, CancellationToken, Task<TDetailItem>> DetailDataProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment<TDetailItem>? Template { get; set; }

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;

    private bool HideEmptyDetailFields => GridOptions.Value.HideEmptyDetailFields;

    protected override async Task OnParametersSetAsync()
    {
        if (Item is null)
            return;

        loadCancellationTokenSource?.Cancel();
        loadCancellationTokenSource?.Dispose();
        loadCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = loadCancellationTokenSource.Token;

        try
        {
            isLoading = true;
            errorMessage = null;
            allExpanded = true;
            detail = await DetailDataProvider(Item, cancellationToken);
            detailSettings ??= DetailSettingsService.GetGridDetailSettings(Lang);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // superseded by a newer load or component dispose
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

    public void Dispose()
    {
        loadCancellationTokenSource?.Cancel();
        loadCancellationTokenSource?.Dispose();
        loadCancellationTokenSource = null;
    }

    private bool IsFieldVisible(DetailSetting<TDetailItem> setting) =>
        detail is not null && DetailDisplayValue.IsVisible(setting, detail, HideEmptyDetailFields);

    private int GetVisibleGroupCount()
    {
        if (detail is null || detailSettings is null)
            return 0;

        return detailSettings.GroupSettings.Count(g => g.Value.Any(IsFieldVisible));
    }

    private async Task ToggleAllAccordionItemsAsync()
    {
        if (accordion is null)
            return;

        if (allExpanded)
            await accordion.HideAllAccordionItemsAsync();
        else
            await accordion.ShowAllAccordionItemsAsync();

        allExpanded = !allExpanded;
    }
}
