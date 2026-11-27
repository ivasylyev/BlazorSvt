using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Platform;

[Collection("Database")]
[Trait("Category", "Integration")]
public class GetBlazorGridDataTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task Execute_ReturnsRowsAndPositiveTotalCount()
    {
        var connectionString = RequireConnectionString();
        var query = GridSpTestHelper.CreateDefaultQuery();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(rows.Count);
    }

    [SkippableFact]
    public async Task Execute_WhenPageSizeIsTen_ReturnsAtMostTenRows()
    {
        var connectionString = RequireConnectionString();
        var query = GridSpTestHelper.CreateDefaultQuery(pageNumber: 1, pageSize: 10);

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        rows.Should().HaveCountLessThanOrEqualTo(10);
        totalCount.Should().BeGreaterThan(10);
    }

    [SkippableFact]
    public async Task Execute_WhenSortedByCodeAsc_ReturnsDeterministicFirstRowOnRepeatedCalls()
    {
        var connectionString = RequireConnectionString();
        var query = new GridQuery(
            1,
            1,
            new GridSort(nameof(TransportLegDto.Code), "ASC"),
            [new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator)]);

        var (firstCall, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        var (secondCall, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        firstCall.Should().ContainSingle();
        secondCall.Should().ContainSingle();
        firstCall[0].Code.Should().Be(secondCall[0].Code);
    }

    [SkippableFact]
    public async Task Execute_WhenInFilterOnEnumColumn_ReturnsOnlyListedValues()
    {
        var connectionString = RequireConnectionString();
        var ids = $"{(int)TransportKindRu.Auto},{(int)TransportKindRu.Rail}";
        var query = new GridQuery(
            1,
            20,
            new GridSort(null, "ASC"),
            [
                new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator),
                new GridFilter(nameof(TransportLegDto.TransportKindIdRu), ids, GridFilterOperators.InOperator)
            ]);

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        var allowed = new[] { TransportKindRu.Auto, TransportKindRu.Rail };
        rows.Should().NotBeEmpty();
        rows.Should().OnlyContain(row => allowed.Contains(row.TransportKindIdRu));
        totalCount.Should().BeGreaterThanOrEqualTo(rows.Count);
    }

    [SkippableFact]
    public async Task Execute_WhenIsArchiveTrueFilter_ReturnsNoMoreRowsThanActiveFilter()
    {
        var connectionString = RequireConnectionString();
        var activeQuery = GridSpTestHelper.CreateDefaultQuery(pageNumber: 1, pageSize: 1);
        var archivedQuery = new GridQuery(
            1,
            1,
            new GridSort(null, "ASC"),
            [new GridFilter("IsArchive", "True", GridFilterOperators.EqualsOperator)]);

        var (_, activeCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            activeQuery);

        var (_, archivedCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            archivedQuery);

        archivedCount.Should().BeGreaterThan(0);
        activeCount.Should().BeGreaterThan(archivedCount);
    }
}
