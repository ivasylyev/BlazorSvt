using System.Text.RegularExpressions;
using BlazorSvt.Platform;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform;

[Trait("Category", "Unit")]
public class AppBuildInfoTests
{
    [Fact]
    public void Read_ApplicationAssembly_ReturnsFourPartVersionAndUtcBuildTime()
    {
        var prefix = ReadVersionPrefixFromCsproj();
        var info = AppBuildInfo.Read(typeof(AppBuildInfo).Assembly);

        info.Version.Should().MatchRegex(@"^\d+\.\d+\.\d+\.\d+$");
        info.Version.Should().StartWith(prefix + ".");
        info.BuildTimeUtc.Should().NotBe(default);
        info.BuildTimeUtc.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void ParseBuildTimeUtc_ValidValue_ReturnsUtc()
    {
        var result = AppBuildInfo.ParseBuildTimeUtc("2026-09-16T14:30:45Z");

        result.Should().Be(new DateTimeOffset(2026, 9, 16, 14, 30, 45, TimeSpan.Zero));
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-date")]
    [InlineData("2026-13-01T00:00:00Z")]
    [InlineData("2026-09-16T14:30:45")]
    public void ParseBuildTimeUtc_MissingOrInvalid_Throws(string? value)
    {
        var act = () => AppBuildInfo.ParseBuildTimeUtc(value);

        act.Should().Throw<InvalidOperationException>();
    }

    private static string ReadVersionPrefixFromCsproj()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var csproj = Path.Combine(directory.FullName, "BlazorSvt", "BlazorSvt.csproj");
            if (File.Exists(csproj))
            {
                var match = Regex.Match(
                    File.ReadAllText(csproj),
                    @"<VersionPrefix>\s*([^<\s]+)\s*</VersionPrefix>");
                match.Success.Should().BeTrue("BlazorSvt.csproj must contain VersionPrefix");
                return match.Groups[1].Value;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find BlazorSvt/BlazorSvt.csproj.");
    }
}
