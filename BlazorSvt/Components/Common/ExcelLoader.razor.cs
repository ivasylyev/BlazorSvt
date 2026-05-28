using BlazorSvt.Models.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorSvt.Components.Common;

public partial class ExcelLoader : SvtComponentBase
{
    private const int MaxFileSize = 10 * 1024 * 1024;

    [Inject]
    public ILogger<ExcelLoader> Logger { get; set; } = default!;

    [Parameter]
    public EventCallback<ExcelFile> OnLoaded { get; set; }

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

        try
        {
            await using var uploadStream = file.OpenReadStream(maxAllowedSize: MaxFileSize);
            using var buffer = new MemoryStream();
            await uploadStream.CopyToAsync(buffer);

            var content = buffer.ToArray();
            await OnLoaded.InvokeAsync(new ExcelFile(file.Name, content));
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
}
