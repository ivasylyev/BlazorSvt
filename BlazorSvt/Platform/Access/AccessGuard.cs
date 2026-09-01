namespace BlazorSvt.Platform.Access;

/// <summary>Fail-closed: без роли read запрещён, Import на проде не выдаётся.</summary>
public sealed class AccessGuard(ICurrentUser currentUser) : IAccessGuard
{
    public void Ensure(AccessAction action)
    {
        if (currentUser.BypassAccessControl)
        {
            return;
        }

        if (currentUser.State == AccessState.DirectoryUnavailable)
        {
            throw new DirectoryUnavailableException("Active Directory is unavailable.");
        }

        switch (action)
        {
            case AccessAction.Read:
                if (currentUser.State == AccessState.Allowed && currentUser.RoleNames.Count > 0)
                {
                    return;
                }

                throw new AccessDeniedException();

            case AccessAction.Import:
                throw new AccessDeniedException();

            default:
                throw new AccessDeniedException();
        }
    }
}
