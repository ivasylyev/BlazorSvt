using BlazorSvt.Platform.Access;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Access;

[Trait("Category", "Unit")]
public class AccessGuardTests
{
    [Fact]
    public void EnsureRead_WhenBypass_DoesNotThrow()
    {
        var user = new CurrentUser();
        user.ApplyBypass(AccessDefaults.LocalDevLogin, "LocalDev");
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Read);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureImport_WhenBypass_DoesNotThrow()
    {
        var user = new CurrentUser();
        user.ApplyBypass(AccessDefaults.LocalDevLogin, "LocalDev");
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Import);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureRead_WhenHasRole_DoesNotThrow()
    {
        var user = new CurrentUser();
        user.ApplyAllowed(@"SIBUR\ivanov", "Ivanov", ["Window Users"]);
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Read);

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureImport_WhenHasRole_Throws()
    {
        var user = new CurrentUser();
        user.ApplyAllowed(@"SIBUR\ivanov", "Ivanov", ["Window Users"]);
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Import);

        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void EnsureRead_WhenNoRoles_Throws()
    {
        var user = new CurrentUser();
        user.ApplyDenied(@"SIBUR\ivanov", "Ivanov");
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Read);

        act.Should().Throw<AccessDeniedException>();
    }

    [Fact]
    public void EnsureRead_WhenDirectoryUnavailable_ThrowsDirectoryException()
    {
        var user = new CurrentUser();
        user.ApplyDirectoryUnavailable(@"SIBUR\ivanov");
        var guard = new AccessGuard(user);

        var act = () => guard.Ensure(AccessAction.Read);

        act.Should().Throw<DirectoryUnavailableException>();
    }
}
