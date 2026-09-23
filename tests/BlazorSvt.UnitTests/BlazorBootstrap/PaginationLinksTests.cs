using BlazorBootstrap;
using FluentAssertions;

namespace BlazorSvt.UnitTests.BlazorBootstrap;

[Trait("Category", "Unit")]
public class PaginationLinksTests
{
    [Theory]
    [InlineData(1, 100, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(2, 100, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(3, 100, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(4, 100, new[] { 2, 3, 4, 5, 6 })]
    [InlineData(23, 5000, new[] { 21, 22, 23, 24, 25 })]
    [InlineData(100, 100, new[] { 96, 97, 98, 99, 100 })]
    [InlineData(4, 4, new[] { 1, 2, 3, 4 })]
    [InlineData(1, 1, new[] { 1 })]
    [InlineData(100, 2, new[] { 1, 2 })]
    public void GetPageNumbers_CentersActivePageInsideTheExistingRange(int activePage, int totalPages, int[] expected)
    {
        PaginationLinks.GetPageNumbers(activePage, totalPages).Should().Equal(expected);
    }

    [Fact]
    public void GetBackwardJumps_WhenPageIs2500Of5000_ShowsEveryStep()
    {
        PaginationLinks.GetBackwardJumps(2500, 5000).Select(jump => (jump.Label, jump.Page)).Should().Equal(
            ("-1000", 1500),
            ("-100", 2400),
            ("-20", 2480),
            ("-5", 2495));
    }

    [Fact]
    public void GetBackwardJumps_WhenPageIs1000_OmitsTheStepThatWouldLeaveTheRange()
    {
        var jumps = PaginationLinks.GetBackwardJumps(1000, 1000);

        jumps.Select(jump => (jump.Label, jump.Page)).Should().Equal(
            ("-100", 900),
            ("-20", 980),
            ("-5", 995));
    }

    [Fact]
    public void GetForwardJumps_WhenPageIs23Of5000_ShowsEveryStep()
    {
        PaginationLinks.GetForwardJumps(23, 5000).Select(jump => (jump.Label, jump.Page)).Should().Equal(
            ("+5", 28),
            ("+20", 43),
            ("+100", 123),
            ("+1000", 1023));
    }

    [Theory]
    [InlineData(1, 100)]
    [InlineData(5, 100)]
    public void GetBackwardJumps_WhenFivePagesCannotBeSubtracted_HidesMinusFive(int activePage, int totalPages)
    {
        PaginationLinks.GetBackwardJumps(activePage, totalPages).Select(jump => jump.Label).Should().NotContain("-5");
    }

    [Fact]
    public void GetForwardJumps_WhenOnlySmallerStepsStillFit_HidesTheRest()
    {
        PaginationLinks.GetForwardJumps(80, 100).Select(jump => (jump.Label, jump.Page)).Should().Equal(
            ("+5", 85),
            ("+20", 100));
    }

    [Fact]
    public void GetForwardJumps_WhenFivePagesCannotBeAdded_HidesEveryForwardStep()
    {
        PaginationLinks.GetForwardJumps(96, 100).Should().BeEmpty();
    }

    [Fact]
    public void GetJumps_WhenActivePageIsPastTheEnd_HidesEveryStep()
    {
        PaginationLinks.GetBackwardJumps(100, 2).Should().BeEmpty();
        PaginationLinks.GetForwardJumps(100, 2).Should().BeEmpty();
    }
}
