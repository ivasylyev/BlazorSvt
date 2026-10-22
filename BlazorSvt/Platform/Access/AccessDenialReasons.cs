namespace BlazorSvt.Platform.Access;

/// <summary>Коды причин отказа в доступе (лог и ответы API).</summary>
public static class AccessDenialReasons
{
    public const string NoWindowsIdentity = "no_windows_identity";
    public const string NoUser = "no_user";
    public const string NoRoles = "no_roles";
}
