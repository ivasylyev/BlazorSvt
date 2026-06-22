using BlazorBootstrap;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using BlazorSvt.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem, TDetailItem> : SvtComponentBase
{
    #region Fields

    private Grid<TItem> grid = default!;
    private SettingsModal settingsModal = default!;
    private ReportConfirmModal reportConfirmModal = default!;
    private GridColumnSettingsCollection<TItem>? gridSettings;

    #endregion

    #region Injected services

    [Inject]
    public ILogger<GenericGrid<TItem, TDetailItem>> Logger { get; set; } = default!;

    [Inject]
    public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

    [Inject]
    public IGridExcelExporter GridExcelExporter { get; set; } = default!;

    [Inject]
    public IFileDownloadService FileDownloadService { get; set; } = default!;

    [Inject]
    public IOptions<ReportOptions> ReportOptions { get; set; } = default!;

    [Inject]
    public PreloadService PreloadService { get; set; } = default!;

    [Inject]
    public IGridDataService<TItem, TDetailItem> GridDataService { get; set; } = default!;

    [Inject]
    public IDetailSettingsService<TDetailItem> DetailSettingsService { get; set; } = default!;

    #endregion

    #region Parameters

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

    #endregion

    #region Lifecycle

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Logger.LogInformation("Generic grid OnAfterRenderAsync");

        if (firstRender)
        {
            gridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
            StateHasChanged();
        }
    }

    #endregion

    #region Settings

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

    #endregion

    #region Filters

    private async Task ClearFiltersAsync()
    {
        grid.ClearFilters();
        await grid.RefreshDataAsync();
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

    #endregion

    #region Reports

    private Task OnShortReportAsync() =>
        RunReportAsync(
            reportName: "Short",
            confirmationThreshold: ReportOptions.Value.ShortReportConfirmationThreshold,
            generateReport: ExportShortReportAsync);

    private Task OnFullReportAsync() =>
        RunReportAsync(
            reportName: "Full",
            confirmationThreshold: ReportOptions.Value.FullReportConfirmationThreshold,
            generateReport: ExportFullReportAsync);

    private async Task RunReportAsync(
        string reportName,
        int confirmationThreshold,
        Func<int, Task> generateReport)
    {
        if (gridSettings is null)
        {
            Logger.LogWarning("{ReportName} report skipped: grid settings are not loaded yet", reportName);
            return;
        }

        var totalCount = await GetTotalCountAsync();
        if (totalCount == 0)
            return;

        if (totalCount > confirmationThreshold)
        {
            var confirmed = await reportConfirmModal.ConfirmAsync(totalCount);
            if (!confirmed)
                return;
        }

        PreloadService.Show(SpinnerColor.Light, L["GenericGrid.ReportGenerating"]);
        try
        {
            await LoggedOperation.ExecuteAsync(Logger, $"{reportName} report", () => generateReport(totalCount));
        }
        finally
        {
            PreloadService.Hide();
        }
    }

    private async Task ExportShortReportAsync(int totalCount)
    {
        var items = await GetAllGridDataAsync(totalCount);
        var workbook = GridExcelExporter.ExportShortReport(items, gridSettings!.ColumnSettings);
        await DownloadFileAsync(workbook, BuildShortReportFileName());
        Logger.LogInformation("Short report: exported {Count} items", items.Count);
    }

    private async Task ExportFullReportAsync(int totalCount)
    {
        var request = grid.CreateDataProviderRequest(pageNumber: 1, pageSizeOverride: totalCount);
        var items = await GridDataService.GetFullReportDataAsync(request, Lang, totalCount);
        var detailSettings = DetailSettingsService.GetGridDetailSettings(Lang);
        var columns = detailSettings.GroupSettings.Values.SelectMany(settings => settings);
        var workbook = GridExcelExporter.ExportFullReport(items, columns);
        await DownloadFileAsync(workbook, BuildFullReportFileName());
        Logger.LogInformation("Full report: exported {Count} items", items.Count);
    }

    private Task DownloadFileAsync(byte[] content, string fileName) =>
        FileDownloadService.DownloadFromBytesAsync(content, fileName);

    private string BuildShortReportFileName()
    {
        var safeTitle = string.Join("_", PageTitle.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return $"{safeTitle}_short_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
    }

    private string BuildFullReportFileName()
    {
        var safeTitle = string.Join("_", PageTitle.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return $"{safeTitle}_full_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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

    #endregion
}
