using BlazorSvt.Import.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorSvt.Import.Components;

public partial class ExcelLoader : SvtComponentBase, IDisposable
{
    private const int MaxFileSize = 10 * 1024 * 1024;

    [Inject]
    public ILogger<ExcelLoader> Logger { get; set; } = default!;

    [Parameter]
    public EventCallback<ExcelFile> OnLoaded { get; set; }

    private readonly CancellationTokenSource _loadCancellationTokenSource = new();
    private bool _isLoading;

    public async Task OnFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file is null)
        {
            return;
        }

        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        var cancellationToken = _loadCancellationTokenSource.Token;
        try
        {
            await using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var buffer = new MemoryStream();
            await uploadStream.CopyToAsync(buffer, cancellationToken);

            var content = buffer.ToArray();
            await OnLoaded.InvokeAsync(new ExcelFile(file.Name, content));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // component disposed while uploading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Excel file {FileName}", file.Name);
            throw;
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        _loadCancellationTokenSource.Cancel();
        _loadCancellationTokenSource.Dispose();
    }
}
