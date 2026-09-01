namespace BlazorSvt.Platform.Access;

/// <summary>Легаси-таблицы <c>dbo.User</c>, <c>dbo.Role</c>, <c>dbo.UserRole</c>.</summary>
public interface IUserAccessRepository
{
    Task<UserRecord?> FindByLoginAsync(string login, CancellationToken cancellationToken = default);

    Task<UserRecord> GetOrCreateUserAsync(
        string login,
        string? name,
        string? email,
        CancellationToken cancellationToken = default);

    Task UpdateProfileAsync(
        int userId,
        string? name,
        string? email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoleRecord>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetUserRoleIdsAsync(int userId, CancellationToken cancellationToken = default);

    Task ReconcileUserRolesAsync(
        int userId,
        IReadOnlyList<int> desiredRoleIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(int userId, CancellationToken cancellationToken = default);
}
