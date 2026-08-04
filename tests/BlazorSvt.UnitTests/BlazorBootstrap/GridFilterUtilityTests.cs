using BlazorBootstrap;
using FluentAssertions;

namespace BlazorSvt.UnitTests.BlazorBootstrap;

[Trait("Category", "Unit")]
public class GridFilterUtilityTests
{
    [Theory]
    [InlineData("DateOnly")]
    [InlineData("DateTime")]
    public void IsPendingDateFilter_WhenEmpty_ReturnsFalse(string propertyTypeName)
    {
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, null).Should().BeFalse();
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, "").Should().BeFalse();
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, "   ").Should().BeFalse();
    }

    [Theory]
    [InlineData("DateOnly", "0002-09-30")]
    [InlineData("DateOnly", "0025-09-30")]
    [InlineData("DateOnly", "0202-09-30")]
    [InlineData("DateOnly", "1752-12-31")]
    [InlineData("DateTime", "0002-09-30T12:00")]
    [InlineData("DateTime", "1752-12-31T23:59")]
    public void IsPendingDateFilter_WhenBelowSqlDateTimeMin_ReturnsTrue(string propertyTypeName, string value)
    {
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, value).Should().BeTrue();
    }

    [Theory]
    [InlineData("DateOnly", "not-a-date")]
    [InlineData("DateTime", "abc")]
    public void IsPendingDateFilter_WhenUnparseable_ReturnsTrue(string propertyTypeName, string value)
    {
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, value).Should().BeTrue();
    }

    [Theory]
    [InlineData("DateOnly", "1753-01-01")]
    [InlineData("DateOnly", "2025-09-30")]
    [InlineData("DateOnly", "9999-12-31")]
    [InlineData("DateTime", "1753-01-01T00:00")]
    [InlineData("DateTime", "2025-09-30T14:30")]
    [InlineData("DateTime", "9999-12-31T23:59:59")]
    public void IsPendingDateFilter_WhenWithinSqlDateTimeRange_ReturnsFalse(string propertyTypeName, string value)
    {
        GridFilterUtility.IsPendingDateFilter(propertyTypeName, value).Should().BeFalse();
    }

    [Fact]
    public void IsPendingDateFilter_WhenNotDateProperty_ReturnsFalse()
    {
        GridFilterUtility.IsPendingDateFilter("String", "0002-09-30").Should().BeFalse();
        GridFilterUtility.IsPendingDateFilter("Int32", "0002-09-30").Should().BeFalse();
    }
}
