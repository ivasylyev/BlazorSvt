namespace BlazorSvt.Platform.Access;

public sealed class DirectoryUnavailableException : Exception
{
    public DirectoryUnavailableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
