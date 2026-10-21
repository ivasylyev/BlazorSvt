namespace BlazorSvt.Platform.Access;

/// <summary>Scoped-состояние пользователя; заполняет <see cref="UserAccessSynchronizer"/>.</summary>
public sealed class CurrentUser : ICurrentUser
{
    public string Login { get; private set; } = string.Empty;

    public string? DisplayName { get; private set; }

    public IReadOnlyList<string> RoleNames { get; private set; } = [];

    public AccessState State { get; private set; } = AccessState.Uninitialized;

    public bool BypassAccessControl { get; private set; }

    /// <summary>Код причины отказа (<see cref="AccessDenialReasons"/>); <c>null</c>, если доступ не отклонён.</summary>
    public string? DenialReason { get; private set; }

    public void ApplyBypass(string login, string? displayName)
    {
        Login = login;
        DisplayName = displayName;
        RoleNames = [];
        State = AccessState.Allowed;
        BypassAccessControl = true;
        DenialReason = null;
    }

    public void ApplyAllowed(string login, string? displayName, IReadOnlyList<string> roleNames)
    {
        Login = login;
        DisplayName = displayName;
        RoleNames = roleNames;
        State = AccessState.Allowed;
        BypassAccessControl = false;
        DenialReason = null;
    }

    public void ApplyDenied(
        string login,
        string? displayName,
        IReadOnlyList<string>? roleNames = null,
        string? reason = null)
    {
        Login = login;
        DisplayName = displayName;
        RoleNames = roleNames ?? [];
        State = AccessState.Denied;
        BypassAccessControl = false;
        DenialReason = reason;
    }

    public void ApplyDirectoryUnavailable(string login)
    {
        Login = login;
        DisplayName = null;
        RoleNames = [];
        State = AccessState.DirectoryUnavailable;
        BypassAccessControl = false;
        DenialReason = null;
    }
}
