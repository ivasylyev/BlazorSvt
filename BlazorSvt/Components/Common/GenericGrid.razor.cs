using BlazorBootstrap;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem, TDetailItem> : SvtComponentBase
{
    private Grid<TItem> grid = default!;
    private SettingsModal settingsModal = default!;
    private ReportConfirmModal reportConfirmModal = default!;

    private GridColumnSettingsCollection<TItem>? gridSettings;

    [Inject] 
    public ILogger<GenericGrid<TItem, TDetailItem>> Logger { get; set; } = default!;

    [Inject] 
    public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

    [Inject]
    public IGridExcelExporter GridExcelExporter { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public IOptions<ReportOptions> ReportOptions { get; set; } = default!;

    [Inject]
    public PreloadService PreloadService { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string PageTitle { get; set; } = default!;

    [Parameter]
    public bool AllowDetailView { get; set; } = default!;

    [Parameter] 
    [EditorRequired] 
    public GridDataProviderDelegate<TItem> DataProvider { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public Func<TItem, Task<TDetailItem>> DetailDataProvider { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public Func<TItem, object> DetailKeySelector { get; set; } = default!;
    [Parameter] 
    public RenderFragment<TDetailItem>? DetailViewTemplate { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Logger.LogInformation("Generic grid OnAfterRenderAsync");

        if (firstRender)
        {
            gridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
            StateHasChanged();
        }
    }

    private async Task OnShortReportAsync()
    {
        if (gridSettings is null)
        {
            Logger.LogWarning("Short report skipped: grid settings are not loaded yet");
            return;
        }

        var totalCount = await GetTotalCountAsync();
        if (totalCount == 0)
            return;

        if (totalCount > ReportOptions.Value.ShortReportConfirmationThreshold)
        {
            var confirmed = await reportConfirmModal.ConfirmAsync(totalCount);
            if (!confirmed)
                return;
        }

        await RunReportGenerationAsync(() => ExportShortReportAsync(totalCount));
    }

    private async Task RunReportGenerationAsync(Func<Task> generateReport)
    {
        PreloadService.Show(SpinnerColor.Light, L["GenericGrid.ReportGenerating"]);
        try
        {
            await generateReport();
        }
        finally
        {
            PreloadService.Hide();
        }
    }

    private async Task ExportShortReportAsync(int totalCount)
    {
        var items = await GetAllGridDataAsync(totalCount);
        var workbook = GridExcelExporter.Export(items, gridSettings!.ColumnSettings);
        await DownloadFileAsync(workbook, BuildShortReportFileName());
        Logger.LogInformation("Short report: exported {Count} items", items.Count);
    }

    private async Task DownloadFileAsync(byte[] content, string fileName)
    {
        using var stream = new MemoryStream(content);
        using var streamRef = new DotNetStreamReference(stream);
        await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }

    private string BuildShortReportFileName()
    {
        var safeTitle = string.Join("_", PageTitle.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return $"{safeTitle}_short_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
    }

    private async Task<int> GetTotalCountAsync()
    {
        var countResult = await DataProvider.Invoke(grid.CreateDataProviderRequest(pageNumber: 1, pageSizeOverride: 1));
        return countResult.TotalCount ?? 0;
    }

    private async Task<IReadOnlyList<TItem>> GetAllGridDataAsync(int totalCount)
    {
        if (totalCount == 0)
            return [];

        var dataResult = await DataProvider.Invoke(grid.CreateDataProviderRequest(pageNumber: 1, pageSizeOverride: totalCount));
        return dataResult.Data?.ToList() ?? [];
    }

    private async Task OnFullReportAsync()
    {
        if (gridSettings is null)
        {
            Logger.LogWarning("Full report skipped: grid settings are not loaded yet");
            return;
        }

        var totalCount = await GetTotalCountAsync();
        if (totalCount == 0)
            return;

        if (totalCount > ReportOptions.Value.FullReportConfirmationThreshold)
        {
            var confirmed = await reportConfirmModal.ConfirmAsync(totalCount);
            if (!confirmed)
                return;
        }

        await Task.CompletedTask;
    }

    private async Task ClearFiltersAsync()
    {
        grid.ClearFilters();
        await grid.RefreshDataAsync();
    }

    private async Task ShowSettingsAsync()
    {
        await settingsModal.ShowAsync();
    }

    private async Task OnOkClick(IReadOnlyCollection<IGridColumnSetting> settings)
    {
        if (gridSettings is not null)
        {
            gridSettings.ApplyGridColumnSettings(settings);
            await GridSettingsService.SaveGridSettingsAsync(gridSettings, Lang);
        }

        await ClearFiltersAsync();

        StateHasChanged();
    }

    private async Task OnCancelClick()
    {
        gridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
        StateHasChanged();
    }

    private async Task OnResetClick()
    {
        await GridSettingsService.ResetGridSettingsAsync(Lang);
        gridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
        await ClearFiltersAsync();
        StateHasChanged();
    }

    private async Task<IEnumerable<FilterOperatorInfo>> GridFiltersTranslationProvider()
    {
        var filters = new List<FilterOperatorInfo>
        {
            // Текстовые фильтры
            new("*a*", L["FilterOperator.Contains"], FilterOperator.Contains),
            new("!*a*", L["FilterOperator.DoesNotContain"], FilterOperator.DoesNotContain),
            new("a**", L["FilterOperator.StartsWith"], FilterOperator.StartsWith),
            new("**a", L["FilterOperator.EndsWith"], FilterOperator.EndsWith),
            new("=", L["FilterOperator.Equals"], FilterOperator.Equals),
            new("!=", L["FilterOperator.NotEquals"], FilterOperator.NotEquals),
            // Числовые фильтры и даты
            new("<", L["FilterOperator.LessThan"], FilterOperator.LessThan),
            new("<=", L["FilterOperator.LessThanOrEquals"], FilterOperator.LessThanOrEquals),
            new(">", L["FilterOperator.GreaterThan"], FilterOperator.GreaterThan),
            new(">=", L["FilterOperator.GreaterThanOrEquals"], FilterOperator.GreaterThanOrEquals),
            // Общие
            new("x", L["FilterOperator.Clear"], FilterOperator.Clear)
        };
        return await Task.FromResult(filters);
    }
}