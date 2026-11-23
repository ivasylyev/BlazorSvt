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

    [Inject]
    public IEnumerable<IDetailChildSection<TDetailItem>> ChildSections { get; set; } = default!;

    [Parameter]
    public TItem Item { get; set; } = default!;
    [Parameter]
    public Func<TItem, CancellationToken, Task<TDetailItem>> DetailDataProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment<TDetailItem>? Template { get; set; }

    private TDetailItem? detail;
    private bool isLoading;
    private string? errorMessage;
    private IReadOnlyList<ChildSectionState> childSections = [];

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
            childSections = [];
            var loadedDetail = await DetailDataProvider(Item, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return;

            detail = loadedDetail;
            detailSettings ??= DetailSettingsService.GetGridDetailSettings(Lang);

            if (detail is not null && ChildSections.Any())
            {
                isLoading = false;
                StateHasChanged();
                var loadedSections = await LoadChildSectionsAsync(detail, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return;

                childSections = loadedSections;
                allExpanded = DetailAccordionPlan.AllExpanded(BuildAccordion().Sections);
            }
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
            if (!cancellationToken.IsCancellationRequested)
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

    private static Type ChildTableViewType(DetailChildTable table) =>
        typeof(DetailChildTableView<>).MakeGenericType(table.RowType);

    private static IDictionary<string, object?> ChildTableViewParameters(DetailChildTable table) =>
        new Dictionary<string, object?> { [nameof(DetailChildTableView<object>.Table)] = table };

    private AccordionLayout BuildAccordion()
    {
        var children = childSections.Where(section => section.IsVisible).ToList();
        var fieldGroups = detailSettings!
            .GroupSettings
            .Where(group => group.Value.Any(IsFieldVisible))
            .ToList();
        var sections = DetailAccordionPlan.Build(
            children.Select(child => new DetailChildAccordionInput(child.ErrorMessage is not null)).ToList(),
            fieldGroups.Count);
        return new AccordionLayout(children, fieldGroups, sections);
    }

    private async Task<IReadOnlyList<ChildSectionState>> LoadChildSectionsAsync(
        TDetailItem parent,
        CancellationToken cancellationToken)
    {
        var lang = Lang;
        var loads = ChildSections.Select(section => LoadChildSectionAsync(section, parent, lang, cancellationToken));
        return await Task.WhenAll(loads);
    }

    private async Task<ChildSectionState> LoadChildSectionAsync(
        IDetailChildSection<TDetailItem> section,
        TDetailItem parent,
        string lang,
        CancellationToken cancellationToken)
    {
        try
        {
            return new ChildSectionState(section.Title, await section.LoadAsync(parent, lang, cancellationToken), null);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogError(ex, "Failed to load child section {Section} for {ItemType}", section.GetType().Name, typeof(TItem).Name);
            return new ChildSectionState(section.Title, null, ex.Message);
        }
    }

    private sealed record ChildSectionState(string Title, DetailChildTable? Table, string? ErrorMessage)
    {
        public bool IsVisible => ErrorMessage is not null || Table?.HasRows == true;
    }

    private sealed record AccordionLayout(
        IReadOnlyList<ChildSectionState> Children,
        IReadOnlyList<KeyValuePair<string, IReadOnlyCollection<DetailSetting<TDetailItem>>>> FieldGroups,
        IReadOnlyList<DetailAccordionSection> Sections);

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
