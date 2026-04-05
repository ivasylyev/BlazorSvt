using BlazorBootstrap;
using BlazorSvt.Models.Grid;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem> : SvtComponentBase
{
    private Grid<TItem> grid = default!;
    private SettingsModal settingsModal = default!;

    [Inject] 
    public ILogger<GenericGrid<TItem>> Logger { get; set; } = default!;

    [Inject] 
    public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

    [Parameter, EditorRequired]
    public GridDataProviderDelegate<TItem> DataProvider { get; set; } = default!;

    [Parameter, EditorRequired]
    public string PageTitle { get; set; } = default!;

    protected GridSettings<TItem>? GridSettings;

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
        StateHasChanged();
    }
}