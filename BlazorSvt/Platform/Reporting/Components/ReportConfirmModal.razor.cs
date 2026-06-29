using BlazorBootstrap;

namespace BlazorSvt.Platform.Reporting.Components;

public partial class ReportConfirmModal
{
    private Modal modal = default!;
    private int rowCount;
    private TaskCompletionSource<bool>? confirmationTcs;
    private bool resolvedByButton;

    public async Task<bool> ConfirmAsync(int rowCount)
    {
        this.rowCount = rowCount;
        resolvedByButton = false;
        confirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        await modal.ShowAsync();
        return await confirmationTcs.Task;
    }

    private async Task HandleYesAsync()
    {
        resolvedByButton = true;
        confirmationTcs?.TrySetResult(true);
        await modal.HideAsync();
    }

    private async Task HandleNoAsync()
    {
        resolvedByButton = true;
        confirmationTcs?.TrySetResult(false);
        await modal.HideAsync();
    }

    private Task HandleHiddenAsync()
    {
        if (!resolvedByButton)
            confirmationTcs?.TrySetResult(false);

        return Task.CompletedTask;
    }
}
