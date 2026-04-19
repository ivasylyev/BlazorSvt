using BlazorBootstrap;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem, TDetailItem> : SvtComponentBase
{
    private Grid<TItem> grid = default!;
    private SettingsModal settingsModal = default!;

    private GridColumnSettingsCollection<TItem>? gridSettings;

    [Inject] 
    public ILogger<GenericGrid<TItem, TDetailItem>> Logger { get; set; } = default!;

    [Inject] 
    public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

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