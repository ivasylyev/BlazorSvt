using BlazorBootstrap;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Grid;

[Trait("Category", "Unit")]
public class GridQueryFactoryTests
{
    private readonly GridQueryFactory<TransportLegDto> _factory = new();
    private readonly GridQueryFactory<TransportRateDto> _rateFactory = new();

    [Fact]
    public void Create_WhenSortingIsEmpty_UsesDefaultAscWithNullSortKey()
    {
        var request = CreateRequest(sorting: []);

        var query = _factory.Create(request);

        query.Sort.PropertyName.Should().BeNull();
        query.Sort.Direction.Should().Be("ASC");
    }

    [Fact]
    public void Create_WhenSortingIsDescending_MapsToDesc()
    {
        var request = CreateRequest(sorting:
        [
            new SortingItem<TransportLegDto>("Code", x => x.Code, SortDirection.Descending)
        ]);

        var query = _factory.Create(request);

        query.Sort.PropertyName.Should().Be("Code");
        query.Sort.Direction.Should().Be("DESC");
    }

    [Fact]
    public void Create_WhenFiltersAreNull_AddsDefaultIsArchiveFalseFilter()
    {
        var request = CreateRequest(filters: null);

        var query = _factory.Create(request);

        query.Filters.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator));
    }

    [Fact]
    public void Create_WhenContainsFilterProvided_MapsOperator()
    {
        var request = CreateRequest(filters:
        [
            new FilterItem(nameof(TransportLegDto.NodeFromNameRu), "казань", FilterOperator.Contains, StringComparison.OrdinalIgnoreCase)
        ]);

        var query = _factory.Create(request);

        query.Filters.Should().Contain(f =>
            f.PropertyName == nameof(TransportLegDto.NodeFromNameRu)
            && f.Value == "казань"
            && f.Operator == GridFilterOperators.ContainsOperator);
    }

    [Fact]
    public void Create_WhenEnumFilterProvided_ConvertsValueToNumericString()
    {
        var request = CreateRequest(filters:
        [
            new FilterItem(nameof(TransportLegDto.TransportKindIdRu), nameof(TransportKindRu.Rail), FilterOperator.Equals, StringComparison.Ordinal)
        ]);

        var query = _factory.Create(request);

        query.Filters.Should().Contain(f =>
            f.PropertyName == nameof(TransportLegDto.TransportKindIdRu)
            && f.Value == ((int)TransportKindRu.Rail).ToString()
            && f.Operator == GridFilterOperators.EqualsOperator);
    }

    [Fact]
    public void Create_WhenPageNumberAndPageSizeOverridden_UsesOverrideValues()
    {
        var request = CreateRequest(pageNumber: 5, pageSize: 50);

        var query = _factory.Create(request, pageNumber: 1, pageSize: 10);

        query.PageNumber.Should().Be(1);
        query.PageSize.Should().Be(10);
    }

    [Theory]
    [InlineData(FilterOperator.GreaterThan, GridFilterOperators.GreaterThanOperator)]
    [InlineData(FilterOperator.LessThanOrEquals, GridFilterOperators.LessThanOrEqualsOperator)]
    [InlineData(FilterOperator.NotEquals, GridFilterOperators.NotEqualsOperator)]
    [InlineData(FilterOperator.Clear, GridFilterOperators.ClearOperator)]
    public void Create_MapsFilterOperators(FilterOperator sourceOperator, string expectedOperator)
    {
        var request = CreateRequest(filters:
        [
            new FilterItem(nameof(TransportLegDto.Code), "LEG-1", sourceOperator, StringComparison.Ordinal)
        ]);

        var query = _factory.Create(request);

        query.Filters.Should().Contain(f => f.Operator == expectedOperator);
    }

    [Theory]
    [InlineData(FilterOperator.Equals, GridFilterOperators.EqualsOperator)]
    [InlineData(FilterOperator.NotEquals, GridFilterOperators.NotEqualsOperator)]
    [InlineData(FilterOperator.LessThan, GridFilterOperators.LessThanOperator)]
    [InlineData(FilterOperator.LessThanOrEquals, GridFilterOperators.LessThanOrEqualsOperator)]
    [InlineData(FilterOperator.GreaterThan, GridFilterOperators.GreaterThanOperator)]
    [InlineData(FilterOperator.GreaterThanOrEquals, GridFilterOperators.GreaterThanOrEqualsOperator)]
    public void Create_MapsDecimalComparisonOperators(FilterOperator sourceOperator, string expectedOperator)
    {
        var request = new GridDataProviderRequest<TransportRateDto>
        {
            PageNumber = 1,
            PageSize = 20,
            Filters =
            [
                new FilterItem(nameof(TransportRateDto.TotalCostTon), "100.50", sourceOperator, StringComparison.Ordinal)
            ],
            Sorting = []
        };

        var query = _rateFactory.Create(request);

        query.Filters.Should().ContainSingle(f =>
            f.PropertyName == nameof(TransportRateDto.TotalCostTon)
            && f.Value == "100.50"
            && f.Operator == expectedOperator);
    }

    private static GridDataProviderRequest<TransportLegDto> CreateRequest(
        IEnumerable<FilterItem>? filters = null,
        IEnumerable<SortingItem<TransportLegDto>>? sorting = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        return new GridDataProviderRequest<TransportLegDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Filters = filters!, // null is intentional for default-filter scenario
            Sorting = sorting ?? []
        };
    }
}
