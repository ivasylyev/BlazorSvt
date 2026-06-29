namespace BlazorSvt.Platform.Reporting.Services;

public interface IFileDownloadService
{
    Task DownloadFromBytesAsync(byte[] content, string fileName, CancellationToken cancellationToken = default);
}
