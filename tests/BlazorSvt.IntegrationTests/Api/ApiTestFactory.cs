using System.Security.Claims;
using System.Text.Encodings.Web;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Api;

/// <summary>
/// Хост СВТ в памяти без БД: доступ, данные и подбор подменены, Windows-учётка — из заголовка <see cref="UserHeader"/>.
/// </summary>
public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    public const string UserHeader = "X-Test-User";
    public const string TrustedClient = @"TEST\svc-atlas";
    public const string RegisteredUser = @"TEST\ivanov";
    public const string UserWithoutRoles = @"TEST\noroles";

    private readonly string logDirectory = Path.Combine(Path.GetTempPath(), "svt-api-tests", Guid.NewGuid().ToString("N"));

    public IUserAccessRepository UserAccess { get; } = Substitute.For<IUserAccessRepository>();

    public ICatalogFreshnessReader Freshness { get; } = Substitute.For<ICatalogFreshnessReader>();

    public IGridDataService<TransportRateDto, TransportRateDetailDto> TransportRates { get; } =
        Substitute.For<IGridDataService<TransportRateDto, TransportRateDetailDto>>();

    public ISmartRateLookupRepository SmartLookup { get; } = Substitute.For<ISmartRateLookupRepository>();

    public ApiTestFactory()
    {
        UserAccess.FindByLoginAsync(RegisteredUser, Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, RegisteredUser, "Ivanov", null));
        UserAccess.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>()).Returns(["Window Users"]);
        UserAccess.FindByLoginAsync(UserWithoutRoles, Arg.Any<CancellationToken>())
            .Returns(new UserRecord(11, UserWithoutRoles, "NoRoles", null));
        UserAccess.GetUserRoleNamesAsync(11, Arg.Any<CancellationToken>()).Returns([]);
    }

    public HttpClient CreateApiClient(string? caller = TrustedClient, string? onBehalfOf = RegisteredUser)
    {
        var client = CreateClient();
        if (caller is not null)
        {
            client.DefaultRequestHeaders.Add(UserHeader, caller);
        }

        if (onBehalfOf is not null)
        {
            client.DefaultRequestHeaders.Add("X-On-Behalf-Of", onBehalfOf);
        }

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("PathBase", "/");
        builder.UseSetting("Sync:Enabled", "false");
        builder.UseSetting("Auth:IgnoreAccessControl", "false");
        builder.UseSetting("Auth:SynchronizeUserRoles", "false");
        builder.UseSetting("Api:TrustedClients:0", TrustedClient);
        builder.UseSetting("Api:SwaggerUiEnabled", "false");
        builder.UseSetting("Serilog:WriteTo:1:Args:path", Path.Combine(logDirectory, "api-.txt"));

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = TestAuthHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            });

            services.RemoveAll<IHostedService>();
            services.Replace(ServiceDescriptor.Scoped(_ => UserAccess));
            services.Replace(ServiceDescriptor.Singleton(Freshness));
            services.Replace(ServiceDescriptor.Scoped(_ => TransportRates));
            services.Replace(ServiceDescriptor.Scoped(_ => SmartLookup));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            Directory.Delete(logDirectory, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var login = Request.Headers[UserHeader].ToString();
            if (string.IsNullOrEmpty(login))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, login)], SchemeName);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
        }
    }
}
