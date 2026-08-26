using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.Grid.Components;

public partial class SettingsModal
{
    private Modal modal = default!;

    [Parameter]
    public IReadOnlyCollection<IGridColumnSetting>? ColumnSettings { get; set; }

    [Parameter]
    public EventCallback<IReadOnlyCollection<IGridColumnSetting>> OnOk { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public EventCallback OnReset { get; set; }

    public async Task ShowAsync()
    {
        await modal.ShowAsync();
    }

    private async Task HandleOk()
    {
        if (ColumnSettings is not null)
        {
            await OnOk.InvokeAsync(ColumnSettings);
        }

        await modal.HideAsync();
    }

    private async Task HandleCancel()
    {
        await OnCancel.InvokeAsync();
        await modal.HideAsync();
    }

    private async Task HandleReset()
    {
        await OnReset.InvokeAsync();
        await modal.HideAsync();
    }
}