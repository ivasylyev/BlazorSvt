using BlazorSvt.Platform.Access;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.UnitTests.Platform.Access;

[Trait("Category", "Unit")]
public class UserAccessSynchronizerTests
{
    [Fact]
    public async Task Synchronize_WhenIgnoreAccessControlInDevelopment_DoesNotCallDirectoryOrRepository()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns((string?)null);
        var directory = Substitute.For<IActiveDirectoryClient>();
        var repository = Substitute.For<IUserAccessRepository>();
        var sut = Create(
            currentUser,
            identity,
            directory,
            repository,
            ignoreAccessControl: true,
            environmentName: Environments.Development);

        await sut.SynchronizeAsync();

        currentUser.BypassAccessControl.Should().BeTrue();
        currentUser.Login.Should().Be(AccessDefaults.LocalDevLogin);
        currentUser.State.Should().Be(AccessState.Allowed);
        await directory.DidNotReceiveWithAnyArgs().GetUserAsync(default!, default);
        await repository.DidNotReceiveWithAnyArgs().GetOrCreateUserAsync(default!, default, default, default);
    }

    [Fact]
    public async Task Synchronize_WhenIgnoreAccessControlInProduction_DoesNotBypass()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        directory.GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns(new DirectoryUser("Ivanov", "ivanov@sibur.ru", [@"SIBUR\G001-SVT_WINDOW_USERS"]));
        var repository = Substitute.For<IUserAccessRepository>();
        repository.GetOrCreateUserAsync(@"SIBUR\ivanov", "Ivanov", "ivanov@sibur.ru", Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, @"SIBUR\ivanov", "Ivanov", "ivanov@sibur.ru"));
        repository.GetRolesAsync(Arg.Any<CancellationToken>())
            .Returns([new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS")]);
        repository.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>())
            .Returns(["Window Users"]);
        var sut = Create(
            currentUser,
            identity,
            directory,
            repository,
            ignoreAccessControl: true,
            environmentName: Environments.Production,
            synchronizeUserRoles: true);

        await sut.SynchronizeAsync();

        currentUser.BypassAccessControl.Should().BeFalse();
        currentUser.State.Should().Be(AccessState.Allowed);
        currentUser.RoleNames.Should().Equal("Window Users");
        await directory.Received(1).GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Synchronize_WhenDirectoryThrows_DoesNotCreateUser()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        directory.GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns<DirectoryUser>(_ => throw new DirectoryUnavailableException("down"));
        var repository = Substitute.For<IUserAccessRepository>();
        var sut = Create(currentUser, identity, directory, repository, synchronizeUserRoles: true);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.DirectoryUnavailable);
        await repository.DidNotReceiveWithAnyArgs().GetOrCreateUserAsync(default!, default, default, default);
    }

    [Fact]
    public async Task Synchronize_WhenNoMatchingGroups_CreatesUserAndDenies()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        directory.GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns(new DirectoryUser("Ivanov", null, [@"SIBUR\OTHER"]));
        var repository = Substitute.For<IUserAccessRepository>();
        repository.GetOrCreateUserAsync(@"SIBUR\ivanov", "Ivanov", null, Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, @"SIBUR\ivanov", "Ivanov", null));
        repository.GetRolesAsync(Arg.Any<CancellationToken>())
            .Returns([new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS")]);
        repository.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>())
            .Returns([]);
        var sut = Create(currentUser, identity, directory, repository, synchronizeUserRoles: true);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Denied);
        currentUser.Login.Should().Be(@"SIBUR\ivanov");
        await repository.Received(1).ReconcileUserRolesAsync(10, Arg.Is<IReadOnlyList<int>>(ids => ids.Count == 0), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Synchronize_WhenMatchingGroup_GrantsAccess()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        directory.GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns(new DirectoryUser("Ivanov", "ivanov@sibur.ru", [@"SIBUR\G001-SVT_WINDOW_USERS"]));
        var repository = Substitute.For<IUserAccessRepository>();
        repository.GetOrCreateUserAsync(@"SIBUR\ivanov", "Ivanov", "ivanov@sibur.ru", Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, @"SIBUR\ivanov", "Ivanov", "ivanov@sibur.ru"));
        repository.GetRolesAsync(Arg.Any<CancellationToken>())
            .Returns([new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS")]);
        repository.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>())
            .Returns(["Window Users"]);
        var sut = Create(currentUser, identity, directory, repository, synchronizeUserRoles: true);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Allowed);
        currentUser.RoleNames.Should().Equal("Window Users");
        await repository.Received(1).ReconcileUserRolesAsync(
            10,
            Arg.Is<IReadOnlyList<int>>(ids => ids.Count == 1 && ids[0] == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Synchronize_WhenRolesRevoked_ReconcilesEmptyAndDenies()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        directory.GetUserAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns(new DirectoryUser("Ivanov", null, []));
        var repository = Substitute.For<IUserAccessRepository>();
        repository.GetOrCreateUserAsync(@"SIBUR\ivanov", "Ivanov", null, Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, @"SIBUR\ivanov", "Ivanov", null));
        repository.GetRolesAsync(Arg.Any<CancellationToken>())
            .Returns([new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS")]);
        repository.GetUserRoleIdsAsync(10, Arg.Any<CancellationToken>()).Returns([3]);
        repository.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>()).Returns([]);
        var sut = Create(currentUser, identity, directory, repository, synchronizeUserRoles: true);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Denied);
        await repository.Received(1).ReconcileUserRolesAsync(10, Arg.Is<IReadOnlyList<int>>(ids => ids.Count == 0), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Synchronize_WhenNoWindowsIdentity_DeniesWithoutDirectory()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns((string?)null);
        var directory = Substitute.For<IActiveDirectoryClient>();
        var repository = Substitute.For<IUserAccessRepository>();
        var sut = Create(
            currentUser,
            identity,
            directory,
            repository,
            ignoreAccessControl: false,
            environmentName: Environments.Production);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Denied);
        await directory.DidNotReceiveWithAnyArgs().GetUserAsync(default!, default);
    }

    [Fact]
    public async Task Synchronize_WhenSynchronizeUserRolesOff_ReadsExistingRolesWithoutDirectory()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        var repository = Substitute.For<IUserAccessRepository>();
        repository.FindByLoginAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns(new UserRecord(10, @"SIBUR\ivanov", "Ivanov", null));
        repository.GetUserRoleNamesAsync(10, Arg.Any<CancellationToken>())
            .Returns(["Window Users"]);
        var sut = Create(currentUser, identity, directory, repository, synchronizeUserRoles: false);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Allowed);
        currentUser.RoleNames.Should().Equal("Window Users");
        currentUser.DisplayName.Should().Be("Ivanov");
        await directory.DidNotReceiveWithAnyArgs().GetUserAsync(default!, default);
        await repository.DidNotReceiveWithAnyArgs().GetOrCreateUserAsync(default!, default, default, default);
        await repository.DidNotReceiveWithAnyArgs().ReconcileUserRolesAsync(default, default!, default);
    }

    [Fact]
    public async Task Synchronize_WhenSynchronizeUserRolesOffAndNoUser_DeniesWithoutDirectory()
    {
        var currentUser = new CurrentUser();
        var identity = Substitute.For<IWindowsIdentityAccessor>();
        identity.GetLogin().Returns(@"SIBUR\ivanov");
        var directory = Substitute.For<IActiveDirectoryClient>();
        var repository = Substitute.For<IUserAccessRepository>();
        repository.FindByLoginAsync(@"SIBUR\ivanov", Arg.Any<CancellationToken>())
            .Returns((UserRecord?)null);
        var sut = Create(currentUser, identity, directory, repository);

        await sut.SynchronizeAsync();

        currentUser.State.Should().Be(AccessState.Denied);
        await directory.DidNotReceiveWithAnyArgs().GetUserAsync(default!, default);
        await repository.DidNotReceiveWithAnyArgs().GetOrCreateUserAsync(default!, default, default, default);
    }

    private static UserAccessSynchronizer Create(
        CurrentUser currentUser,
        IWindowsIdentityAccessor identity,
        IActiveDirectoryClient directory,
        IUserAccessRepository repository,
        bool ignoreAccessControl = false,
        string environmentName = "Production",
        bool synchronizeUserRoles = false)
    {
        var env = Substitute.For<IHostEnvironment>();
        env.EnvironmentName.Returns(environmentName);

        return new UserAccessSynchronizer(
            currentUser,
            identity,
            directory,
            repository,
            Options.Create(new AccessOptions
            {
                IgnoreAccessControl = ignoreAccessControl,
                SynchronizeUserRoles = synchronizeUserRoles
            }),
            env,
            NullLogger<UserAccessSynchronizer>.Instance);
    }
}
