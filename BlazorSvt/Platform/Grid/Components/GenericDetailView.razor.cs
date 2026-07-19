using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.Grid.Components;


public partial class GenericDetailView<TItem, TDetailItem> : SvtComponentBase
{
    private DetailSettingsCollection<TDetailItem>? detailSettings;
    private Accordion? accordion;
    private bool allExpanded = true;

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
            allExpanded = true;
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

    private int GetVisibleGroupCount()
    {
        if (detail is null || detailSettings is null)
            return 0;

        return detailSettings.GroupSettings.Count(g => g.Value.Any(s => s.VisibleSelector(detail)));
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
