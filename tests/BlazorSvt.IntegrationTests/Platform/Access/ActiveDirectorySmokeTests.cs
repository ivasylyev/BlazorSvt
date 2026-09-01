using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Platform.Access;

[Trait("Category", "Integration")]
[SupportedOSPlatform("windows")]
public class ActiveDirectorySmokeTests
{
    [SkippableFact]
    public void CurrentUser_CanReadDisplayNameAndDirectGroups()
    {
        Skip.IfNot(OperatingSystem.IsWindows(), "Active Directory client is Windows-only.");

        UserPrincipal? user = null;
        try
        {
            user = UserPrincipal.Current;
        }
        catch (Exception ex) when (ex is PrincipalServerDownException or PrincipalOperationException or InvalidOperationException)
        {
            Skip.If(true, $"Active Directory is not available: {ex.Message}");
        }

        Skip.If(user is null, "No domain user for the test process.");

        using (user)
        {
            user.SamAccountName.Should().NotBeNullOrWhiteSpace();

            using var groups = user.GetGroups();
            _ = groups.Cast<Principal>().Take(5).ToList();
        }
    }
}
