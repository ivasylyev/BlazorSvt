using Microsoft.JSInterop;

namespace BlazorSvt.Platform.Reporting.Services;

public class FileDownloadService(IJSRuntime js) : IFileDownloadService
{
    public async Task DownloadFromBytesAsync(byte[] content, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        await DownloadFromStreamAsync(stream, fileName, cancellationToken);
    }

    public async Task DownloadFromStreamAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(content, leaveOpen: true);
        await js.InvokeVoidAsync("downloadFileFromStream", cancellationToken, fileName, streamRef);
    }
}
