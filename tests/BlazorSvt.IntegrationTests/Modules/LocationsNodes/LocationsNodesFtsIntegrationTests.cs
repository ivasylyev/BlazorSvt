using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.LocationsNodes.List.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.LocationsNodes;

[Collection("Database")]
[Trait("Category", "Integration")]
public class LocationsNodesFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static GridFilter WarehouseTypeNode { get; } =
        FtsFilterTestSupport.IdEquals(
            nameof(LocationsNodesDto.TypeNodeIdRu),
            (int)TypeNodeRu.Warehouse);

    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndTypeNodeFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            WarehouseTypeNode,
            FtsFilterTestSupport.Contains(nameof(LocationsNodesDto.NameRu), "нижнекамск"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TypeNodeIdRu == TypeNodeRu.Warehouse);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithThreeFtsAndTypeNodeFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            WarehouseTypeNode,
            FtsFilterTestSupport.Contains(nameof(LocationsNodesDto.NameRu), "нижнекамск"),
            FtsFilterTestSupport.Contains(nameof(LocationsNodesDto.RegionNameRu), "татарстан"),
            FtsFilterTestSupport.Contains(nameof(LocationsNodesDto.CountryNameRu), "российская"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TypeNodeIdRu == TypeNodeRu.Warehouse);
    }
}
