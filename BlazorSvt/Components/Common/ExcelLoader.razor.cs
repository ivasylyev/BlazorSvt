using BlazorSvt.Models.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorSvt.Components.Common;

public partial class ExcelLoader : SvtComponentBase
{
    private const int MaxFileSize = 10 * 1024 * 1024;
    private const double CopyBufferProgressPercent = 50;

    [Inject]
    public ILogger<ExcelLoader> Logger { get; set; } = default!;

    [Parameter]
    public EventCallback<ExcelFile> OnLoaded { get; set; }

    private bool _isLoading;
    private bool _showProgress;
    private double _progressPercent;

    public async Task OnFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file is null)
        {
            return;
        }

        await StartProgressAsync();

        try
        {
            await using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var buffer = new MemoryStream();
            await uploadStream.CopyToAsync(buffer);
            await SetProgressAsync(CopyBufferProgressPercent);

            var content = buffer.ToArray();
            await CompleteProgressAsync();

            await OnLoaded.InvokeAsync(new ExcelFile(file.Name, content));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Excel file {FileName}", file.Name);
            throw;
        }
        finally
        {
            await FinishProgressAsync();
        }
    }

    private async Task StartProgressAsync()
    {
        _isLoading = true;
        _showProgress = true;
        _progressPercent = 0;
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetProgressAsync(double percent)
    {
        _progressPercent = percent;
        await InvokeAsync(StateHasChanged);
    }

    private async Task CompleteProgressAsync()
    {
        _progressPercent = 100;
        await InvokeAsync(StateHasChanged);
    }

    private async Task FinishProgressAsync()
    {
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(500);
        _showProgress = false;
        await InvokeAsync(StateHasChanged);
    }
}
