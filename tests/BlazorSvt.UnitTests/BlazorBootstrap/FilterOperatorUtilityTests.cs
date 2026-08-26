using BlazorBootstrap;
using FluentAssertions;

namespace BlazorSvt.UnitTests.BlazorBootstrap;

[Trait("Category", "Unit")]
public class FilterOperatorUtilityTests
{
    private static readonly FilterOperator[] ComparisonAndClearOperators =
    [
        FilterOperator.Equals,
        FilterOperator.NotEquals,
        FilterOperator.LessThan,
        FilterOperator.LessThanOrEquals,
        FilterOperator.GreaterThan,
        FilterOperator.GreaterThanOrEquals,
        FilterOperator.Clear
    ];

    [Theory]
    [InlineData("Int16")]
    [InlineData("Int32")]
    [InlineData("Int64")]
    [InlineData("Single")]
    [InlineData("Decimal")]
    [InlineData("Double")]
    public void GetFilterOperators_ForNumberTypes_IncludesComparisonOperatorsLikeDates(string propertyTypeName)
    {
        var numberOps = FilterOperatorUtility.GetFilterOperators(propertyTypeName)
            .Select(x => x.FilterOperator)
            .ToArray();
        var dateOps = FilterOperatorUtility.GetFilterOperators("DateOnly")
            .Select(x => x.FilterOperator)
            .ToArray();

        numberOps.Should().Equal(ComparisonAndClearOperators);
        numberOps.Should().Equal(dateOps);
    }

    [Fact]
    public void GetNumberFilterOperators_MatchesDateFilterOperators()
    {
        var numberOps = FilterOperatorUtility.GetNumberFilterOperators()
            .Select(x => x.FilterOperator)
            .ToArray();
        var dateOps = FilterOperatorUtility.GetDateFilterOperators()
            .Select(x => x.FilterOperator)
            .ToArray();

        numberOps.Should().Equal(dateOps);
    }

    [Fact]
    public void GetFilterOperators_WithTranslations_ReplacesTextAndSymbolForNumberOperators()
    {
        var translations = new[]
        {
            new FilterOperatorInfo(">", "Больше", FilterOperator.GreaterThan),
            new FilterOperatorInfo("<", "Меньше", FilterOperator.LessThan)
        };

        var result = FilterOperatorUtility.GetFilterOperators("Decimal", translations).ToList();

        result.Should().ContainSingle(x => x.FilterOperator == FilterOperator.GreaterThan)
            .Which.Should().BeEquivalentTo(new FilterOperatorInfo(">", "Больше", FilterOperator.GreaterThan));
        result.Should().ContainSingle(x => x.FilterOperator == FilterOperator.LessThan)
            .Which.Should().BeEquivalentTo(new FilterOperatorInfo("<", "Меньше", FilterOperator.LessThan));
        result.Should().Contain(x => x.FilterOperator == FilterOperator.Equals);
        result.Select(x => x.FilterOperator).Should().Equal(ComparisonAndClearOperators);
    }
}
