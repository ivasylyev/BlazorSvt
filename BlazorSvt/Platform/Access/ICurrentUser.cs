namespace BlazorSvt.Platform.Access;

/// <summary>Права текущего circuit: роли из БД, не Windows-группы.</summary>
public interface ICurrentUser
{
    string Login { get; }

    string? DisplayName { get; }

    IReadOnlyList<string> RoleNames { get; }

    AccessState State { get; }

    bool BypassAccessControl { get; }
}
