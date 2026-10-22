using BlazorSvt.Platform.Infrastructure.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Serilog.Events;

namespace BlazorSvt.UnitTests.Platform.Infrastructure;

[Trait("Category", "Unit")]
public class SerilogRequestLogLevelTests
{
    [Theory]
    [InlineData("/hc")]
    [InlineData("/HC")]
    public void GetLevel_HealthCheck_IsVerbose(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Response.StatusCode = 200;

        SerilogRequestLogLevel.GetLevel(context, elapsedMs: 1, ex: null)
            .Should().Be(LogEventLevel.Verbose);
    }

    [Fact]
    public void GetLevel_NormalPath_IsInformation()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/transportleg";
        context.Response.StatusCode = 200;

        SerilogRequestLogLevel.GetLevel(context, elapsedMs: 1, ex: null)
            .Should().Be(LogEventLevel.Information);
    }
}
