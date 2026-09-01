namespace BlazorSvt.Platform.Access;

/// <summary>Прямые группы AD в виде <c>DOMAIN\Group</c> (не вложенные из Windows-токена).</summary>
public interface IActiveDirectoryClient
{
    Task<DirectoryUser> GetUserAsync(string login, CancellationToken cancellationToken = default);
}
