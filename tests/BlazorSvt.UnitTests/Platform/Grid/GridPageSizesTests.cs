using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridPageSizesTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(30)]
    [InlineData(50)]
    public void Normalize_WhenValueIsAllowed_KeepsIt(int pageSize)
    {
        GridPageSizes.Normalize(pageSize).Should().Be(pageSize);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(25)]
    [InlineData(100)]
    public void Normalize_WhenValueIsMissingOrNotAllowed_ReturnsDefault(int? pageSize)
    {
        GridPageSizes.Normalize(pageSize).Should().Be(GridPageSizes.Default);
    }
}
