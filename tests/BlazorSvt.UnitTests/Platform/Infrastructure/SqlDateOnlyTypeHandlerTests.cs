using System.Data;
using System.Globalization;
using BlazorSvt.Platform.Infrastructure.Data;
using FluentAssertions;
using NSubstitute;

namespace BlazorSvt.UnitTests.Platform.Infrastructure;

[Trait("Category", "Unit")]
public class SqlDateOnlyTypeHandlerTests
{
    private readonly SqlDateOnlyTypeHandler handler = new();

    [Fact]
    public void WhenSetValue_WritesMidnightDateTimeAndDateDbType()
    {
        // System.Data.SqlClient.SqlParameter сохраняет DbType.Date как DateTime, поэтому подмена.
        var parameter = Substitute.For<IDbDataParameter>();

        handler.SetValue(parameter, new DateOnly(2026, 9, 22));

        parameter.Value.Should().Be(new DateTime(2026, 9, 22));
        parameter.DbType.Should().Be(DbType.Date);
    }

    [Fact]
    public void WhenParseReceivesDateTime_DropsTime()
    {
        handler.Parse(new DateTime(2026, 9, 22, 15, 45, 30))
            .Should()
            .Be(new DateOnly(2026, 9, 22));
    }

    [Fact]
    public void WhenParseReceivesCultureDateString_ReturnsDate()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            const string text = "22.09.2026";
            DateOnly.TryParse(text, out _).Should().BeTrue();

            handler.Parse(text).Should().Be(new DateOnly(2026, 9, 22));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void WhenParseReceivesUnparseableValue_ReturnsMinValue()
    {
        handler.Parse("not-a-date").Should().Be(DateOnly.MinValue);
    }
}
