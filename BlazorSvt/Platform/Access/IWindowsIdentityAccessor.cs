namespace BlazorSvt.Platform.Access;

/// <summary>Windows identity из IIS; <c>DOMAIN\user</c>.</summary>
public interface IWindowsIdentityAccessor
{
    /// <summary>Windows identity as DOMAIN\user, or null if the request is not authenticated.</summary>
    string? GetLogin();
}
