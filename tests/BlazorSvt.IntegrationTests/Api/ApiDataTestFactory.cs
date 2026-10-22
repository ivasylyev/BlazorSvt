using System.Security.Claims;
using System.Text.Encodings.Web;
using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Platform.Access;
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
/// Хост СВТ в памяти с реальной БД: Windows-учётка и on-behalf — тестовые,
/// <see cref="IGridDataService{TItem,TDetail}"/> и freshness — боевые.
/// </summary>
public sealed class ApiDataTestFactory : WebApplicationFactory<Program>
{
    private readonly string logDirectory = Path.Combine(Path.GetTempPath(), "svt-api-data-tests", Guid.NewGuid().ToString("N"));

    public IUserAccessRepository UserAccess { get; } = Substitute.For<IUserAccessRepository>();

    public string? ConnectionString { get; } = DatabaseFixture.ResolveConnectionString();

    public bool IsDatabaseAvailable { get; }

    public ApiDataTestFactory()
    {
        IsDatabaseAvailable = !string.IsNullOrWhiteSpace(ConnectionString) && CanConnect(ConnectionString);

        UserAccess.FindByLoginAsync(ApiTestFactory.RegisteredUser, Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, ApiTestFactory.RegisteredUser, "Ivanov", null));
        UserAccess.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>()).Returns(["Window Users"]);
    }

    public HttpClient CreateApiClient(
        string? caller = ApiTestFactory.TrustedClient,
        string? onBehalfOf = ApiTestFactory.RegisteredUser)
    {
        var client = CreateClient();
        if (caller is not null)
        {
            client.DefaultRequestHeaders.Add(ApiTestFactory.UserHeader, caller);
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
        builder.UseSetting("Api:TrustedClients:0", ApiTestFactory.TrustedClient);
        builder.UseSetting("Api:SwaggerUiEnabled", "false");
        builder.UseSetting("Serilog:WriteTo:1:Args:path", Path.Combine(logDirectory, "api-.txt"));

        if (!string.IsNullOrWhiteSpace(ConnectionString))
        {
            builder.UseSetting("Database:MdmDb", ConnectionString);
        }

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(ApiDataTestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, ApiDataTestAuthHandler>(
                    ApiDataTestAuthHandler.SchemeName, _ => { });
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultScheme = ApiDataTestAuthHandler.SchemeName;
                options.DefaultAuthenticateScheme = ApiDataTestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = ApiDataTestAuthHandler.SchemeName;
            });

            services.RemoveAll<IHostedService>();
            services.Replace(ServiceDescriptor.Scoped(_ => UserAccess));
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

    private static bool CanConnect(string connectionString)
    {
        try
        {
#pragma warning disable CS0618
            using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
#pragma warning restore CS0618
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private sealed class ApiDataTestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var login = Request.Headers[ApiTestFactory.UserHeader].ToString();
            if (string.IsNullOrEmpty(login))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, login)], SchemeName);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
        }
    }
}
