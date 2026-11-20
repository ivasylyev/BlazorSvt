using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class DetailChildRowStatesTests
{
    [Fact]
    public void For_ActiveParent_ReturnsActualOnly() =>
        DetailChildRowStates.For(parentIsArchive: false).Should().Equal(1);

    [Fact]
    public void For_ArchivedParent_ReturnsActualAndArchival() =>
        DetailChildRowStates.For(parentIsArchive: true).Should().Equal(1, 2);
}
