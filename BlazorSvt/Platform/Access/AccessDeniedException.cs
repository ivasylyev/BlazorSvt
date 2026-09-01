namespace BlazorSvt.Platform.Access;

public sealed class AccessDeniedException : InvalidOperationException
{
    public AccessDeniedException()
        : base("Access denied.")
    {
    }
}
