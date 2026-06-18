namespace BlazorSvt.Services.Shared;

public interface IFileDownloadService
{
    Task DownloadFromBytesAsync(byte[] content, string fileName, CancellationToken cancellationToken = default);
}
