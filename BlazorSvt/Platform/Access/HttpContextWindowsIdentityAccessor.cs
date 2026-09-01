namespace BlazorSvt.Platform.Access;

/// <summary>Берёт <c>Identity.Name</c> с HTTP-запроса (IIS Windows Auth).</summary>
public sealed class HttpContextWindowsIdentityAccessor(IHttpContextAccessor httpContextAccessor)
    : IWindowsIdentityAccessor
{
    public string? GetLogin()
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity;
        if (identity is not { IsAuthenticated: true })
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(identity.Name) ? null : identity.Name;
    }
}
