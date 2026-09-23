using BlazorSvt.Modules.HomeRatePivot;
using BlazorSvt.Modules.HomeRatePivot.Data;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Modules.HomeRatePivot;

[Trait("Category", "Unit")]
public class HomeRatePivotServiceTests
{
    private static readonly DateOnly September = new(2026, 9, 1);

    [Theory]
    [InlineData(22)]
    [InlineData(1)]
    public void WhenAnchorIsSeptember_WindowIsJuneThroughNovember(int day)
    {
        var months = HomeRatePivotService.BuildMonthWindow(new DateOnly(2026, 9, day));

        months.Should().Equal(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 7, 1),
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 10, 1),
            new DateOnly(2026, 11, 1));
    }

    [Fact]
    public void WhenAnchorIsJanuary_WindowCrossesIntoPreviousYear()
    {
        var months = HomeRatePivotService.BuildMonthWindow(new DateOnly(2026, 1, 15));

        months.Should().Equal(
            new DateOnly(2025, 10, 1),
            new DateOnly(2025, 11, 1),
            new DateOnly(2025, 12, 1),
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 3, 1));
    }

    [Fact]
    public void WhenThereAreNoFacts_BuildsOneRowPerDirectionWithNullRates()
    {
        var table = HomeRatePivotService.Pivot(Window(), [], useRussianNames: true);

        table.Rows.Should().HaveCount(HomeRatePivotDirections.Pairs.Count);
        table.Rows[0].DirectionLabel.Should().Be("A56543 - REG74");
        table.Rows.Should().Equal(
            HomeRatePivotDirections.Pairs,
            (row, pair) =>
                row.DirectionLabel == $"{pair.FromCode} - {pair.ToCode}"
                && row.RatesByMonth.Length == HomeRatePivotService.MonthWindowSize
                && row.RatesByMonth.All(rate => rate is null));
    }

    [Fact]
    public void WhenRussianNamesRequested_UsesRuThenEnThenCode()
    {
        var facts = new[]
        {
            Fact(1, fromRu: "Нижнекамск", fromEn: "Nizhnekamsk", toRu: "   ", toEn: "Tatarstan"),
            Fact(2, fromRu: " ", fromEn: null, toRu: null, toEn: " ")
        };

        var table = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true);

        table.Rows[0].DirectionLabel.Should().Be("Нижнекамск - Tatarstan");
        table.Rows[1].DirectionLabel.Should().Be("A56543 - REG47");
    }

    [Fact]
    public void WhenEnglishNamesRequested_UsesEnThenRuThenCode()
    {
        var facts = new[]
        {
            Fact(1, fromRu: "Нижнекамск", fromEn: "Nizhnekamsk", toRu: "Татарстан", toEn: "  "),
            Fact(2, fromRu: null, fromEn: null, toRu: " ", toEn: null)
        };

        var table = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: false);

        table.Rows[0].DirectionLabel.Should().Be("Nizhnekamsk - Татарстан");
        table.Rows[1].DirectionLabel.Should().Be("A56543 - REG47");
    }

    [Fact]
    public void WhenFactMonthIsInsideWindow_WritesRateOnTheMatchingRow()
    {
        var facts = new[]
        {
            Fact(3, fromRu: "Казань", toRu: "Татарстан", year: 2026, month: 9, rate: 15.5m)
        };

        var table = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true);

        table.Rows.Should().HaveCount(HomeRatePivotDirections.Pairs.Count);
        table.Rows[2].DirectionLabel.Should().Be("Казань - Татарстан");
        table.Rows[2].RatesByMonth[3].Should().Be(15.5m);
        table.Rows[2].RatesByMonth.Where((_, index) => index != 3).Should().AllSatisfy(rate => rate.Should().BeNull());
        table.Rows.Where((_, index) => index != 2).Should().AllSatisfy(row =>
            row.RatesByMonth.Should().AllSatisfy(rate => rate.Should().BeNull()));
    }

    [Fact]
    public void WhenPairHasSeveralFacts_NodeNamesComeFromTheFirst()
    {
        var facts = new[]
        {
            Fact(1, fromRu: "FirstRu", toRu: "FirstTo", year: 2026, month: 9, rate: 10m),
            Fact(1, fromRu: "SecondRu", toRu: "SecondTo", year: 2026, month: 10, rate: 20m)
        };

        var row = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true).Rows[0];

        row.DirectionLabel.Should().Be("FirstRu - FirstTo");
        row.RatesByMonth[3].Should().Be(10m);
        row.RatesByMonth[4].Should().Be(20m);
    }

    [Fact]
    public void WhenFactMonthIsOutsideWindow_DoesNotWriteRate()
    {
        var facts = new[]
        {
            Fact(1, year: 2026, month: 5, rate: 99m),
            Fact(1, year: 2026, month: 12, rate: 98m)
        };

        var row = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true).Rows[0];

        row.RatesByMonth.Should().AllSatisfy(rate => rate.Should().BeNull());
    }

    [Fact]
    public void WhenYearMonthOrRateIsMissing_SkipsFact()
    {
        var facts = new[]
        {
            Fact(1, year: null, month: 9, rate: 1m),
            Fact(1, year: 2026, month: null, rate: 2m),
            Fact(1, year: 2026, month: 9, rate: null),
            Fact(1, year: 2026, month: 10, rate: 3m)
        };

        var rates = HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true).Rows[0].RatesByMonth;

        rates[3].Should().BeNull();
        rates[4].Should().Be(3m);
    }

    [Fact]
    public void WhenTwoFactsShareMonth_KeepsTheLastRate()
    {
        var facts = new[]
        {
            Fact(1, year: 2026, month: 9, rate: 1.1m),
            Fact(1, year: 2026, month: 9, rate: 2.2m)
        };

        HomeRatePivotService.Pivot(Window(), facts, useRussianNames: true)
            .Rows[0].RatesByMonth[3].Should().Be(2.2m);
    }

    private static IReadOnlyList<DateOnly> Window() =>
        HomeRatePivotService.BuildMonthWindow(September);

    private static HomeRatePivotFactRow Fact(
        int sortOrder,
        string? fromRu = null,
        string? fromEn = null,
        string? toRu = null,
        string? toEn = null,
        int? year = null,
        int? month = null,
        decimal? rate = null)
    {
        var (fromCode, toCode) = HomeRatePivotDirections.Pairs[sortOrder - 1];
        return new HomeRatePivotFactRow
        {
            SortOrder = sortOrder,
            NodeFromCode = fromCode,
            NodeToCode = toCode,
            NodeFromNameRu = fromRu,
            NodeFromNameEn = fromEn,
            NodeToNameRu = toRu,
            NodeToNameEn = toEn,
            Year = year,
            Month = month,
            RateLevel3 = rate
        };
    }
}
