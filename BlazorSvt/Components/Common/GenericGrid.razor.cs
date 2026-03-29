using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using BlazorSvt.Models.Grid;
using BlazorSvt.Models.Page;
using BlazorSvt.Services.Shared;

namespace BlazorSvt.Components.Common;

public partial class GenericGrid<TItem>
{
    private SettingsModal settingsModal = default!;

    [Inject] 
    public ILogger<GenericGrid<TItem>> Logger { get; set; } = default!;

    [Inject] 
    public IGridSettingsService<TItem> GridSettingsService { get; set; } = default!;

    [Parameter, EditorRequired]
    public GridDataProviderDelegate<TItem> DataProvider { get; set; } = default!;

    protected PageSettings? PageSettings;
    protected GridSettings<TItem>? GridSettings;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Generic grid initializing");

        PageSettings = GridSettingsService.GetPageSettings();
        GridSettings = await GridSettingsService.GetGridSettingsAsync();
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
            await GridSettingsService.SaveGridSettingsAsync(GridSettings);
        }

        StateHasChanged();
    }

    private async Task OnCancelClick()
    {
        GridSettings = await GridSettingsService.GetGridSettingsAsync();
        StateHasChanged();
    }

    private async Task OnResetClick()
    {
        await GridSettingsService.ResetGridSettingsAsync();
        GridSettings = await GridSettingsService.GetGridSettingsAsync();
        StateHasChanged();
    }
}