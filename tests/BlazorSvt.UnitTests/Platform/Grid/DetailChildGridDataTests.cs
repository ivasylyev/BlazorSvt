using System.Globalization;
using BlazorBootstrap;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

/// <summary>
/// Фильтр во вложенной таблице дочернего примитива: клиентская фильтрация загруженных строк
/// (<see cref="DetailChildGridData"/>) по типам колонок.
/// </summary>
[Trait("Category", "Unit")]
public class DetailChildGridDataTests
{
    [Fact]
    public void String_ContainsIgnoringCase_SkipsNull()
    {
        var rows = new[]
        {
            new Row { Id = 1, Text = "Rail TANK container" },
            new Row { Id = 2, Text = "Sea" },
            new Row { Id = 3, Text = null }
        };

        Ids(rows, nameof(Row.Text), "tank", FilterOperator.Contains).Should().Equal(1);
        Ids(rows, nameof(Row.Text), "ea", FilterOperator.Contains).Should().Equal(2);
    }

    [Fact]
    public void Date_Nullable_ComparesByDate_SkipsEmpty()
    {
        var rows = new[]
        {
            new Row { Id = 1, NullableDate = new DateOnly(2026, 3, 5) },
            new Row { Id = 2, NullableDate = new DateOnly(2026, 3, 6) },
            new Row { Id = 3, NullableDate = null }
        };

        Ids(rows, nameof(Row.NullableDate), "2026-03-05", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.NullableDate), "2026-03-05", FilterOperator.GreaterThan).Should().Equal(2);
        Ids(rows, nameof(Row.NullableDate), "2026-03-06", FilterOperator.LessThanOrEquals).Should().Equal(1, 2);
        Ids(rows, nameof(Row.NullableDate), "2026-03-05", FilterOperator.NotEquals).Should().Equal(2);
    }

    [Fact]
    public void Date_NonNullable_ComparesByDate()
    {
        var rows = new[]
        {
            new Row { Id = 1, Date = new DateOnly(2026, 3, 5) },
            new Row { Id = 2, Date = new DateOnly(2026, 3, 6) }
        };

        Ids(rows, nameof(Row.Date), "2026-03-06", FilterOperator.Equals).Should().Equal(2);
        Ids(rows, nameof(Row.Date), "2026-03-06", FilterOperator.LessThan).Should().Equal(1);
    }

    [Fact]
    public void DateTime_FilterValueIsMoscowWallClock_ComparedAsUtc()
    {
        // Фильтр вводится по московскому времени, в DTO хранится UTC: так же, как в SQL-фильтре грида справочника.
        var rows = new[]
        {
            new Row { Id = 1, Moment = new DateTime(2026, 3, 5, 0, 0, 0) },
            new Row { Id = 2, Moment = new DateTime(2026, 3, 5, 3, 0, 0) }
        };

        Ids(rows, nameof(Row.Moment), "2026-03-05T03:00", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.Moment), "2026-03-05T03:00", FilterOperator.GreaterThan).Should().Equal(2);
    }

    [Fact]
    public void DateTime_Nullable_NormalizedAndSkipsEmpty()
    {
        var rows = new[]
        {
            new Row { Id = 1, NullableMoment = new DateTime(2026, 3, 5, 0, 0, 0) },
            new Row { Id = 2, NullableMoment = null }
        };

        Ids(rows, nameof(Row.NullableMoment), "2026-03-05T03:00", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.NullableMoment), "2026-03-05T03:00", FilterOperator.LessThan).Should().BeEmpty();
    }

    [Fact]
    public void Enum_EqualsAndNotEquals()
    {
        var rows = new[]
        {
            new Row { Id = 1, Color = SampleColor.Red },
            new Row { Id = 2, Color = SampleColor.Blue }
        };

        Ids(rows, nameof(Row.Color), nameof(SampleColor.Blue), FilterOperator.Equals).Should().Equal(2);
        Ids(rows, nameof(Row.Color), nameof(SampleColor.Blue), FilterOperator.NotEquals).Should().Equal(1);
    }

    [Fact]
    public void Enum_Nullable_EqualsMatchesValue_NotEqualsSkipsEmpty()
    {
        var rows = new[]
        {
            new Row { Id = 1, NullableColor = SampleColor.Red },
            new Row { Id = 2, NullableColor = SampleColor.Blue },
            new Row { Id = 3, NullableColor = null }
        };

        Ids(rows, nameof(Row.NullableColor), nameof(SampleColor.Blue), FilterOperator.Equals).Should().Equal(2);
        Ids(rows, nameof(Row.NullableColor), nameof(SampleColor.Blue), FilterOperator.NotEquals).Should().Equal(1);
    }

    [Fact]
    public void YesNo_Bool_FiltersByValue()
    {
        var rows = new[]
        {
            new Row { Id = 1, Flag = true },
            new Row { Id = 2, Flag = false }
        };

        Ids(rows, nameof(Row.Flag), "True", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.Flag), "False", FilterOperator.Equals).Should().Equal(2);
    }

    [Fact]
    public void YesNo_NullableBool_FiltersByValue_SkipsEmpty()
    {
        var rows = new[]
        {
            new Row { Id = 1, NullableFlag = true },
            new Row { Id = 2, NullableFlag = false },
            new Row { Id = 3, NullableFlag = null }
        };

        Ids(rows, nameof(Row.NullableFlag), "True", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.NullableFlag), "False", FilterOperator.Equals).Should().Equal(2);
    }

    [Fact]
    public void ArchiveStatus_FiltersByIsArchive()
    {
        var rows = new[]
        {
            new Row { Id = 1, IsArchive = false },
            new Row { Id = 2, IsArchive = true }
        };

        Ids(rows, nameof(Row.IsArchive), "False", FilterOperator.Equals).Should().Equal(1);
        Ids(rows, nameof(Row.IsArchive), "True", FilterOperator.Equals).Should().Equal(2);
    }

    [Theory]
    [InlineData("ru-RU")]
    [InlineData("en-US")]
    public void Number_ParsesFilterValueWithInvariantCulture(string culture)
    {
        // Поле type=number отдаёт «5.5» с точкой независимо от языка интерфейса.
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var rows = new[]
            {
                new Row { Id = 1, Amount = 5.5m, NullableAmount = 5.5m },
                new Row { Id = 2, Amount = 0m, NullableAmount = null },
                new Row { Id = 3, Amount = 12m, NullableAmount = 12m }
            };

            Ids(rows, nameof(Row.Amount), "5.5", FilterOperator.Equals).Should().Equal(1);
            Ids(rows, nameof(Row.Amount), "5.5", FilterOperator.GreaterThan).Should().Equal(3);
            Ids(rows, nameof(Row.NullableAmount), "5.5", FilterOperator.Equals).Should().Equal(1);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void NoFilters_ReturnsAllRows_PagedAndSorted()
    {
        var rows = Enumerable.Range(1, 25).Select(i => new Row { Id = i }).ToList();
        var request = new GridDataProviderRequest<Row>
        {
            PageNumber = 2,
            PageSize = 10,
            Filters = [],
            Sorting = []
        };

        var result = DetailChildGridData.Apply(rows, request);

        result.TotalCount.Should().Be(25);
        result.Data!.Select(r => r.Id).Should().Equal(Enumerable.Range(11, 10));
    }

    [Fact]
    public void Filter_NarrowsTotalCount_ForPaging()
    {
        var rows = Enumerable.Range(1, 25).Select(i => new Row { Id = i, Flag = i % 5 == 0 }).ToList();

        var result = Apply(rows, nameof(Row.Flag), "True", FilterOperator.Equals);

        result.TotalCount.Should().Be(5);
    }

    [Fact]
    public void EnumFilterValues_OnlyValuesPresentInColumn_InDeclarationOrder()
    {
        var rows = new[]
        {
            new Row { Id = 1, Color = SampleColor.Blue },
            new Row { Id = 2, Color = SampleColor.Red },
            new Row { Id = 3, Color = SampleColor.Blue }
        };

        DetailChildGridData.EnumFilterValues(rows, nameof(Row.Color))
            .Should().Equal(SampleColor.Red, SampleColor.Blue);

        DetailChildGridData.EnumFilterValues(rows.Where(r => r.Id == 1), nameof(Row.Color))
            .Should().Equal(SampleColor.Blue);
    }

    [Fact]
    public void EnumFilterValues_Nullable_SkipsNull()
    {
        var rows = new[]
        {
            new Row { Id = 1, NullableColor = null },
            new Row { Id = 2, NullableColor = SampleColor.Blue }
        };

        DetailChildGridData.EnumFilterValues(rows, nameof(Row.NullableColor))
            .Should().Equal(SampleColor.Blue);
    }

    [Fact]
    public void EnumFilterValues_NoValuesInColumn_ReturnsEmpty()
    {
        var rows = new[] { new Row { Id = 1, NullableColor = null } };

        DetailChildGridData.EnumFilterValues(rows, nameof(Row.NullableColor)).Should().BeEmpty();
        DetailChildGridData.EnumFilterValues(Array.Empty<Row>(), nameof(Row.Color)).Should().BeEmpty();
    }

    [Fact]
    public void EnumFilterValues_NotEnumColumn_ReturnsNull()
    {
        var rows = new[] { new Row { Id = 1, Text = "x" } };

        DetailChildGridData.EnumFilterValues(rows, nameof(Row.Text)).Should().BeNull();
        DetailChildGridData.EnumFilterValues(rows, nameof(Row.Flag)).Should().BeNull();
        DetailChildGridData.EnumFilterValues(rows, "Missing").Should().BeNull();
    }

    private static List<int> Ids(IEnumerable<Row> rows, string property, string value, FilterOperator op) =>
        Apply(rows, property, value, op).Data!.Select(r => r.Id).ToList();

    private static GridDataProviderResult<Row> Apply(IEnumerable<Row> rows, string property, string value, FilterOperator op)
    {
        var request = new GridDataProviderRequest<Row>
        {
            PageNumber = 1,
            PageSize = 100,
            Filters = [new FilterItem(property, value, op, StringComparison.OrdinalIgnoreCase)],
            Sorting = []
        };

        return DetailChildGridData.Apply(rows.ToList(), request);
    }

    private sealed class Row
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateOnly Date { get; set; }
        public DateOnly? NullableDate { get; set; }
        public DateTime Moment { get; set; }
        public DateTime? NullableMoment { get; set; }
        public SampleColor Color { get; set; }
        public SampleColor? NullableColor { get; set; }
        public bool Flag { get; set; }
        public bool? NullableFlag { get; set; }
        public bool IsArchive { get; set; }
        public decimal Amount { get; set; }
        public decimal? NullableAmount { get; set; }
    }

    private enum SampleColor
    {
        Red = 1,
        Blue = 2
    }
}
