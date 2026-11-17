using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using BlazorSvt.Platform.Infrastructure.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BlazorSvt.UnitTests.Platform.Infrastructure;

[Trait("Category", "Unit")]
public class HealthCheckEndpointTests
{
    [Fact]
    public async Task Invoke_Status200_WritesOneDebugLineWithFieldsInOrder()
    {
        var (context, events) = CreateContext(services =>
        {
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, NamedAccountHandler>(
                    IISServerDefaults.AuthenticationScheme,
                    _ => { });
        });

        await HealthCheckEndpoint.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        context.Response.ContentType.Should().Be("text/plain");
        ReadBody(context).Should().Be("healthy");

        var entry = HealthCalls(events).Should().ContainSingle().Subject;
        entry.Level.Should().Be(LogEventLevel.Debug);
        entry.RenderMessage().Should().MatchRegex(
            @"^GET /v2/hc 200 \d+ ms 10\.0\.0\.5 Monitor/1 DOMAIN\\svc$");
        entry.Properties.Should().NotContainKey("ShortCorrelationId");
    }

    [Fact]
    public async Task Invoke_WithoutAccount_LeavesAccountEmptyAndReturnsHealthy()
    {
        var (context, events) = CreateContext();

        await HealthCheckEndpoint.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        ReadBody(context).Should().Be("healthy");
        HealthCalls(events).Should().ContainSingle()
            .Which.RenderMessage().Should().MatchRegex(
                @"^GET /v2/hc 200 \d+ ms 10\.0\.0\.5 Monitor/1 $");
    }

    [Fact]
    public async Task Invoke_WhenAuthenticateSets401_StillReturnsHealthy()
    {
        var (context, events) = CreateContext(services =>
        {
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, StatusOnAuthenticateHandler>(
                    IISServerDefaults.AuthenticationScheme,
                    _ => { });
        });

        await HealthCheckEndpoint.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        ReadBody(context).Should().Be("healthy");
        HealthCalls(events).Should().ContainSingle().Which.Level.Should().Be(LogEventLevel.Debug);
    }

    [Fact]
    public void Write_WithCorrelationHeader_SetsShortCorrelationId()
    {
        var (context, events) = CreateContext();
        context.Request.Headers[MiddlewareDecorator.CorrelationHeaderName] = "abc12345";
        context.Response.StatusCode = StatusCodes.Status200OK;

        HealthCheckEndpoint.Write(context, elapsedMs: 12, account: "");

        var entry = HealthCalls(events).Should().ContainSingle().Subject;
        entry.RenderMessage().Should().Be("GET /v2/hc 200 12 ms 10.0.0.5 Monitor/1 ");
        Scalar(entry, "ShortCorrelationId").Should().Be("abc12345");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Write_WithoutCorrelationHeader_DoesNotSetShortCorrelationId(string? header)
    {
        var (context, events) = CreateContext();
        if (header is not null)
            context.Request.Headers[MiddlewareDecorator.CorrelationHeaderName] = header;
        context.Response.StatusCode = StatusCodes.Status200OK;

        HealthCheckEndpoint.Write(context, elapsedMs: 12, account: "");

        HealthCalls(events).Should().ContainSingle().Which.Properties.Should().NotContainKey("ShortCorrelationId");
    }

    [Fact]
    public void Write_StatusOtherThan200_IsInformation()
    {
        var (context, events) = CreateContext();
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        HealthCheckEndpoint.Write(context, elapsedMs: 15, account: "");

        var entry = HealthCalls(events).Should().ContainSingle().Subject;
        entry.Level.Should().Be(LogEventLevel.Information);
        entry.RenderMessage().Should().Be("GET /v2/hc 503 15 ms 10.0.0.5 Monitor/1 ");
    }

    private static (HttpContext Context, List<LogEvent> Events) CreateContext(Action<IServiceCollection>? configure = null)
    {
        var events = new List<LogEvent>();
        var serilog = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Sink(new ListSink(events))
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddSerilog(serilog, dispose: true);
        });
        configure?.Invoke(services);

        var context = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };
        context.Response.Body = new MemoryStream();
        context.Request.Method = "GET";
        context.Request.PathBase = "/v2";
        context.Request.Path = "/hc";
        context.Request.Headers.UserAgent = "Monitor/1";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.5");
        return (context, events);
    }

    private static IEnumerable<LogEvent> HealthCalls(IEnumerable<LogEvent> events) =>
        events.Where(entry =>
            entry.Properties.TryGetValue("SourceContext", out var source)
            && source.ToString() == "\"BlazorSvt.Platform.Infrastructure.Logging.HealthCheckEndpoint\"");

    private static string ReadBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEnd();
    }

    private static object? Scalar(LogEvent entry, string name)
    {
        entry.Properties.TryGetValue(name, out var value).Should().BeTrue();
        return ((ScalarValue)value!).Value;
    }

    private sealed class ListSink(List<LogEvent> events) : ILogEventSink
    {
        public void Emit(LogEvent logEvent) => events.Add(logEvent);
    }

    private sealed class NamedAccountHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, @"DOMAIN\svc")],
                authenticationType: Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
        }
    }

    private sealed class StatusOnAuthenticateHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
