using BlazorBootstrap;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem> : SvtComponentBase
{
    private Grid<TItem> grid = default!;

    protected GridSettings<TItem>? GridSettings;
    private SettingsModal settingsModal = default!;

    [Inject] public ILogger<GenericGrid<TItem>> Logger { get; set; } = default!;

    [Inject] public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

    [Parameter] [EditorRequired] public GridDataProviderDelegate<TItem> DataProvider { get; set; } = default!;

    [Parameter] [EditorRequired] public string PageTitle { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Logger.LogInformation("Generic grid OnAfterRenderAsync");

        if (firstRender)
        {
            GridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
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
        if (GridSettings is not null)
        {
            GridSettings.ApplyGridColumnSettings(settings);
            await GridSettingsService.SaveGridSettingsAsync(GridSettings, Lang);
        }

        await ClearFiltersAsync();

        StateHasChanged();
    }

    private async Task OnCancelClick()
    {
        GridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
        StateHasChanged();
    }

    private async Task OnResetClick()
    {
        await GridSettingsService.ResetGridSettingsAsync(Lang);
        GridSettings = await GridSettingsService.GetGridSettingsAsync(Lang);
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