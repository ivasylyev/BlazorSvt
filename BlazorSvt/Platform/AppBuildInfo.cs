using System.Globalization;
using System.Reflection;

namespace BlazorSvt.Platform;

public sealed record AppBuildInfo(string Version, DateTimeOffset BuildTimeUtc)
{
    internal static AppBuildInfo Read(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var version = assembly.GetName().Version?.ToString()
            ?? throw new InvalidOperationException($"Assembly '{assembly.GetName().Name}' has no version.");

        var buildTimeRaw = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(static attribute => attribute.Key == "BuildTimeUtc")
            ?.Value;

        return new AppBuildInfo(version, ParseBuildTimeUtc(buildTimeRaw));
    }

    internal static DateTimeOffset ParseBuildTimeUtc(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("BuildTimeUtc assembly metadata is missing.");
        }

        if (!DateTimeOffset.TryParseExact(
                value,
                "yyyy-MM-ddTHH:mm:ssZ",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var buildTimeUtc))
        {
            throw new InvalidOperationException($"BuildTimeUtc assembly metadata is invalid: '{value}'.");
        }

        return buildTimeUtc;
    }
}

public interface IAppBuildInfo
{
    AppBuildInfo Current { get; }
}

public sealed class AppBuildInfoProvider : IAppBuildInfo
{
    public AppBuildInfo Current { get; } = AppBuildInfo.Read(typeof(AppBuildInfoProvider).Assembly);
}
