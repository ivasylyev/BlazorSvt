namespace BlazorSvt.Platform.Access;

/// <summary>Не вызывает AD: нет домена / не Windows.</summary>
public sealed class UnsupportedDirectoryClient : IActiveDirectoryClient
{
    public Task<DirectoryUser> GetUserAsync(string login, CancellationToken cancellationToken = default)
    {
        throw new DirectoryUnavailableException("Active Directory is only available on Windows.");
    }
}
