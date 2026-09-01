using BlazorSvt.Platform.Access;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Access;

[Trait("Category", "Unit")]
public class UserRoleReconcilerTests
{
    [Fact]
    public void Match_WhenGroupEqualsDomainGroup_IgnoresCaseAndReturnsRoleId()
    {
        var roles = new[]
        {
            new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS"),
            new RoleRecord(4, "Initiators", @"SIBUR\G001GG-SVT_INITIATOR_USERS")
        };

        var matched = UserRoleReconciler.Match(roles, [@"sibur\g001-svt_window_users"]);

        matched.Should().Equal(3);
    }

    [Fact]
    public void Match_WhenDomainDiffers_DoesNotMatchSameGroupName()
    {
        var roles = new[] { new RoleRecord(3, "Window Users", @"SIBUR\G001-SVT_WINDOW_USERS") };

        var matched = UserRoleReconciler.Match(roles, [@"DEV002\G001-SVT_WINDOW_USERS"]);

        matched.Should().BeEmpty();
    }

    [Fact]
    public void Match_WhenOneGroupMapsToSeveralRoles_ReturnsAll()
    {
        var roles = new[]
        {
            new RoleRecord(1, "A", @"DEV002\SameGroup"),
            new RoleRecord(2, "B", @"DEV002\SameGroup")
        };

        var matched = UserRoleReconciler.Match(roles, [@"DEV002\SameGroup"]);

        matched.Should().BeEquivalentTo([1, 2]);
    }

    [Fact]
    public void Match_WhenDomainGroupIsEmpty_SkipsRole()
    {
        var roles = new[] { new RoleRecord(1, "Empty", "  ") };

        UserRoleReconciler.Match(roles, [@"SIBUR\G"]).Should().BeEmpty();
    }

    [Fact]
    public void Diff_AddsAndRemoves()
    {
        var diff = UserRoleReconciler.Diff(currentRoleIds: [1, 2], desiredRoleIds: [2, 3]);

        diff.Add.Should().Equal(3);
        diff.Remove.Should().Equal(1);
    }

    [Fact]
    public void Diff_WhenEqual_IsEmpty()
    {
        var diff = UserRoleReconciler.Diff([1, 2], [2, 1]);

        diff.Add.Should().BeEmpty();
        diff.Remove.Should().BeEmpty();
    }
}
