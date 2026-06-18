using Microsoft.JSInterop;

namespace BlazorSvt.Services.Shared;

public class FileDownloadService(IJSRuntime js) : IFileDownloadService
{
    public async Task DownloadFromBytesAsync(byte[] content, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        using var streamRef = new DotNetStreamReference(stream);
        await js.InvokeVoidAsync("downloadFileFromStream", cancellationToken, fileName, streamRef);
    }
}
