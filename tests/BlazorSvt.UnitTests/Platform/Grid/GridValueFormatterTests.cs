using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridValueFormatterTests
{
    [Fact]
    public void Format_WhenNull_ReturnsEmptyString()
    {
        GridValueFormatter.Format<string?>(null).Should().Be(string.Empty);
        GridValueFormatter.Format<DateTime?>(null).Should().Be(string.Empty);
    }

    [Fact]
    public void Format_WhenDateTime_ReturnsMoscowWallClock()
    {
        GridValueFormatter.Format(new DateTime(2026, 11, 20, 9, 49, 18))
            .Should().Be(new DateTime(2026, 11, 20, 12, 49, 18));
        GridValueFormatter.Format<DateTime?>(new DateTime(2026, 11, 20, 22, 0, 0))
            .Should().Be(new DateTime(2026, 11, 21, 1, 0, 0));
    }

    [Fact]
    public void Format_WhenOtherType_ReturnsValueAsIs()
    {
        GridValueFormatter.Format("abc").Should().Be("abc");
        GridValueFormatter.Format(12.5m).Should().Be(12.5m);
        GridValueFormatter.Format(new DateOnly(2026, 11, 20)).Should().Be(new DateOnly(2026, 11, 20));
    }
}
