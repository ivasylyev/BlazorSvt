using System.Text.Json;
using BlazorSvt.Modules.RateType.List;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Api;

[Trait("Category", "Unit")]
public class CatalogQueryTranslatorTests
{
    private static readonly CatalogPeriod Period = new("StartDate", "EndDate");

    [Fact]
    public void Translate_WhenNoFilters_AddsActiveOnlyFilter()
    {
        var query = Translate(new CatalogSearchRequest());

        query.Filters.Should().ContainSingle()
            .Which.Should().Be(new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator));
        query.Sort.Should().Be(new GridSort(null, "ASC"));
    }

    [Fact]
    public void Translate_WhenIncludeArchived_DoesNotFilterArchive()
    {
        var query = Translate(new CatalogSearchRequest { IncludeArchived = true });

        query.Filters.Should().BeEmpty();
    }

    [Fact]
    public void Translate_WhenArchiveFilteredExplicitly_DoesNotAddDefault()
    {
        var query = Translate(new CatalogSearchRequest { Filters = [Filter("isArchive", "eq", true)] });

        query.Filters.Should().ContainSingle()
            .Which.Should().Be(new GridFilter("IsArchive", "True", GridFilterOperators.EqualsOperator));
    }

    [Fact]
    public void Translate_TextContains_PassesValueAndMapsFieldCaseInsensitive()
    {
        var query = Translate(new CatalogSearchRequest { Filters = [Filter("NODEFROMCODE", "contains", "A56")] });

        query.Filters.Should().Contain(new GridFilter("NodeFromCode", "A56", GridFilterOperators.ContainsOperator));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("  a*  ")]
    [InlineData("(\"x\")")]
    public void Translate_TextShorterThanFullTextMinimum_Returns400(string value)
    {
        var errors = Errors(new CatalogSearchRequest { Filters = [Filter("nodeFromCode", "contains", value)] });

        errors.Should().ContainKey("filters[0].value").WhoseValue.Should().Equal(CatalogValidationCodes.ValueTooShort);
    }

    [Fact]
    public void Translate_ApostropheCountsDoubled_AsInStoredProcedure()
    {
        CatalogQueryTranslator.CleanFullText("a'").Should().Be("a''");
        var query = Translate(new CatalogSearchRequest { Filters = [Filter("nodeFromCode", "contains", "a'")] });

        query.Filters.Should().Contain(f => f.PropertyName == "NodeFromCode");
    }

    [Theory]
    [InlineData("nodeFromCode", "eq")]
    [InlineData("isArchive", "gt")]
    [InlineData("currencyId", "lt")]
    [InlineData("startDate", "contains")]
    public void Translate_OperatorNotSupportedForType_Returns400(string field, string op)
    {
        var errors = Errors(new CatalogSearchRequest { Filters = [Filter(field, op, "2026-01-01")] });

        errors.Should().ContainKey("filters[0].operator").WhoseValue.Should().Equal(CatalogValidationCodes.OperatorNotAllowed);
    }

    [Fact]
    public void Translate_UnknownField_Returns400()
    {
        var errors = Errors(new CatalogSearchRequest { Filters = [Filter("nope", "eq", 1)] });

        errors.Should().ContainKey("filters[0].field").WhoseValue.Should().Equal(CatalogValidationCodes.UnknownField);
    }

    [Theory]
    [InlineData("code", "abc")]
    [InlineData("totalCostTon", "1,5")]
    [InlineData("startDate", "01.10.2026")]
    [InlineData("startDate", "1700-01-01")]
    [InlineData("isArchive", "yes")]
    [InlineData("currencyId", "XXX")]
    public void Translate_InvalidValue_Returns400(string field, string value)
    {
        var op = field is "isArchive" or "currencyId" ? "eq" : "ge";
        var errors = Errors(new CatalogSearchRequest { Filters = [Filter(field, op, value)] });

        errors.Should().ContainKey("filters[0].value").WhoseValue.Should().Equal(CatalogValidationCodes.InvalidValue);
    }

    [Fact]
    public void Translate_TypedValues_UseInvariantSqlFormat()
    {
        var query = Translate(new CatalogSearchRequest
        {
            Filters =
            [
                Filter("code", "eq", 2966338),
                Filter("totalCostTon", "le", 1500.5m),
                Filter("startDate", "ge", "2026-10-01"),
                Filter("currencyId", "eq", "rub"),
                Filter("rateTypeIdEn", "ne", "Tender")
            ]
        });

        query.Filters.Should().Contain(
        [
            new GridFilter("Code", "2966338", GridFilterOperators.EqualsOperator),
            new GridFilter("TotalCostTon", "1500.5", GridFilterOperators.LessThanOrEqualsOperator),
            new GridFilter("StartDate", "2026-10-01", GridFilterOperators.GreaterThanOrEqualsOperator),
            new GridFilter("CurrencyId", "32694", GridFilterOperators.EqualsOperator),
            new GridFilter("RateTypeIdEn", "543746", GridFilterOperators.NotEqualsOperator)
        ]);
    }

    [Fact]
    public void Translate_AsOf_AddsPeriodConditions()
    {
        var query = Translate(new CatalogSearchRequest { AsOf = new DateOnly(2026, 10, 20) });

        query.Filters.Should().Contain(
        [
            new GridFilter("StartDate", "2026-10-20", GridFilterOperators.LessThanOrEqualsOperator),
            new GridFilter("EndDate", "2026-10-20", GridFilterOperators.GreaterThanOrEqualsOperator)
        ]);
    }

    [Fact]
    public void Translate_AsOfWithoutPeriod_Returns400()
    {
        var act = () => CatalogQueryTranslator.Translate(
            typeof(RateTypeDto), new CatalogSearchRequest { AsOf = new DateOnly(2026, 1, 1) }, null, 1, 10);

        act.Should().Throw<ApiException>()
            .Which.Errors.Should().ContainKey("asOf").WhoseValue.Should().Equal(CatalogValidationCodes.NotSupported);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void Translate_PageSizeOutOfRange_Returns400(int pageSize)
    {
        var act = () => CatalogQueryTranslator.Translate(
            typeof(TransportRateDto), new CatalogSearchRequest(), Period, 1, pageSize);

        act.Should().Throw<ApiException>().Which.Errors.Should().ContainKey("pageSize");
    }

    [Fact]
    public void Translate_Sort_MapsFieldAndDirection()
    {
        var query = Translate(new CatalogSearchRequest { Sort = new CatalogSortRequest { Field = "startDate", Direction = "DESC" } });

        query.Sort.Should().Be(new GridSort("StartDate", "DESC"));
    }

    [Fact]
    public void Translate_SortUnknownFieldOrDirection_Returns400()
    {
        var errors = Errors(new CatalogSearchRequest { Sort = new CatalogSortRequest { Field = "nope", Direction = "up" } });

        errors.Should().ContainKeys("sort.field", "sort.direction");
    }

    [Fact]
    public void Translate_CollectsAllErrors()
    {
        var errors = Errors(new CatalogSearchRequest
        {
            Filters = [Filter("nope", "eq", 1), new CatalogFilterRequest { Field = "code", Operator = "eq" }]
        });

        errors.Should().ContainKeys("filters[0].field", "filters[1].value");
    }

    private static GridQuery Translate(CatalogSearchRequest request) =>
        CatalogQueryTranslator.Translate(typeof(TransportRateDto), request, Period, 1, 100);

    private static IReadOnlyDictionary<string, string[]> Errors(CatalogSearchRequest request)
    {
        var act = () => Translate(request);
        var exception = act.Should().Throw<ApiException>().Which;
        exception.Code.Should().Be(ApiErrorCodes.ValidationFailed);
        return exception.Errors!;
    }

    private static CatalogFilterRequest Filter(string field, string op, object value) =>
        new() { Field = field, Operator = op, Value = JsonSerializer.SerializeToElement(value) };
}
