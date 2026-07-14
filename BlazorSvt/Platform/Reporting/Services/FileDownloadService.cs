using Microsoft.JSInterop;

namespace BlazorSvt.Platform.Reporting.Services;

public class FileDownloadService(IJSRuntime js, ILogger<FileDownloadService> logger) : IFileDownloadService
{
    public async Task DownloadFromBytesAsync(byte[] content, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content);
        await DownloadFromStreamAsync(stream, fileName, cancellationToken);
    }

    public async Task DownloadFromStreamAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var streamRef = new DotNetStreamReference(content, leaveOpen: true);
            await js.InvokeVoidAsync("downloadFileFromStream", cancellationToken, fileName, streamRef);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download file {FileName}", fileName);
            throw;
        }
    }
}
