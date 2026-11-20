using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportRoute.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.TransportRoute;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportRouteFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static GridFilter AutoTransportKind { get; } =
        FtsFilterTestSupport.IdEquals(
            nameof(TransportRouteDto.RouteTransportKindIdRu),
            (int)TransportKindRu.Auto);

    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            AutoTransportKind,
            FtsFilterTestSupport.Contains(nameof(TransportRouteDto.StartNodeNameRu), "Красноярск"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRouteDto>(
            connectionString,
            typeof(TransportRouteDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.RouteTransportKindIdRu == TransportKindRu.Auto);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithTwoFtsFilters_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(TransportRouteDto.StartNodeNameRu), "Красноярск"),
            FtsFilterTestSupport.Contains(nameof(TransportRouteDto.Route), "Тюмен"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRouteDto>(
            connectionString,
            typeof(TransportRouteDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
    }
}
