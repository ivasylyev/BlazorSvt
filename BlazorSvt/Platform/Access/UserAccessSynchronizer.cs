using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Access;

/// <summary>
/// Идентификация Windows-пользователя и загрузка ролей из <c>dbo.User</c>/<c>dbo.UserRole</c>.
/// Сверка с AD — только при <see cref="AccessOptions.SynchronizeUserRoles"/>.
/// </summary>
public sealed class UserAccessSynchronizer(
    CurrentUser currentUser,
    IWindowsIdentityAccessor identityAccessor,
    IActiveDirectoryClient directoryClient,
    IUserAccessRepository repository,
    IOptions<AccessOptions> options,
    IHostEnvironment hostEnvironment,
    ILogger<UserAccessSynchronizer> logger)
{
    public bool IsBypassActive => options.Value.IgnoreAccessControl && hostEnvironment.IsDevelopment();

    public async Task SynchronizeAsync(CancellationToken cancellationToken = default)
    {
        if (IsBypassActive)
        {
            var bypassLogin = identityAccessor.GetLogin() ?? AccessDefaults.LocalDevLogin;
            var displayName = SplitSam(bypassLogin);
            currentUser.ApplyBypass(bypassLogin, displayName);
            logger.LogInformation(
                "Access control bypassed for {UserLogin} (Development IgnoreAccessControl)",
                bypassLogin);
            return;
        }

        var login = identityAccessor.GetLogin();
        if (string.IsNullOrWhiteSpace(login))
        {
            currentUser.ApplyDenied(string.Empty, displayName: null, reason: AccessDenialReasons.NoWindowsIdentity);
            logger.LogWarning("Access denied: {UserLogin} {Reason}", string.Empty, AccessDenialReasons.NoWindowsIdentity);
            return;
        }

        if (options.Value.SynchronizeUserRoles)
        {
            await SynchronizeFromDirectoryAsync(login, cancellationToken);
            return;
        }

        await LoadExistingRolesAsync(login, cancellationToken);
    }

    /// <summary>
    /// Права пользователя, от имени которого действует доверенный клиент API.
    /// Только сохранённые роли <c>dbo.UserRole</c>: AD на каждый запрос не опрашивается.
    /// </summary>
    public async Task SynchronizeOnBehalfAsync(string login, CancellationToken cancellationToken = default)
    {
        if (IsBypassActive)
        {
            currentUser.ApplyBypass(login, SplitSam(login));
            return;
        }

        await LoadExistingRolesAsync(login, cancellationToken);
    }

    private async Task LoadExistingRolesAsync(string login, CancellationToken cancellationToken)
    {
        var user = await repository.FindByLoginAsync(login, cancellationToken);
        if (user is null)
        {
            currentUser.ApplyDenied(login, displayName: null, reason: AccessDenialReasons.NoUser);
            logger.LogInformation("Access denied: {UserLogin} {Reason}", login, AccessDenialReasons.NoUser);
            return;
        }

        ApplyRoles(login, user.Name, await repository.GetUserRoleNamesAsync(user.Id, cancellationToken));
    }

    private async Task SynchronizeFromDirectoryAsync(string login, CancellationToken cancellationToken)
    {
        DirectoryUser directoryUser;
        try
        {
            directoryUser = await directoryClient.GetUserAsync(login, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Directory unavailable for {UserLogin}", login);
            currentUser.ApplyDirectoryUnavailable(login);
            return;
        }

        var email = string.IsNullOrWhiteSpace(directoryUser.Email) ? null : directoryUser.Email;
        var user = await repository.GetOrCreateUserAsync(
            login,
            directoryUser.DisplayName,
            email,
            cancellationToken);

        await repository.UpdateProfileAsync(user.Id, directoryUser.DisplayName, email, cancellationToken);

        var roles = await repository.GetRolesAsync(cancellationToken);
        var desiredRoleIds = UserRoleReconciler.Match(roles, directoryUser.Groups);
        await repository.ReconcileUserRolesAsync(user.Id, desiredRoleIds, cancellationToken);

        ApplyRoles(
            login,
            directoryUser.DisplayName,
            await repository.GetUserRoleNamesAsync(user.Id, cancellationToken));
    }

    private void ApplyRoles(string login, string? displayName, IReadOnlyList<string> roleNames)
    {
        if (roleNames.Count == 0)
        {
            currentUser.ApplyDenied(login, displayName, reason: AccessDenialReasons.NoRoles);
            logger.LogInformation("Access denied: {UserLogin} {Reason}", login, AccessDenialReasons.NoRoles);
            return;
        }

        currentUser.ApplyAllowed(login, displayName, roleNames);
        logger.LogInformation("Access granted: {UserLogin}", login);
    }

    private static string SplitSam(string login)
    {
        var separator = login.IndexOf('\\');
        return separator >= 0 && separator < login.Length - 1
            ? login[(separator + 1)..]
            : login;
    }
}
