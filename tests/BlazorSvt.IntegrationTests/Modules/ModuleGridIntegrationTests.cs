using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.AverageRateLevel3.Detail;
using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Modules.LocationsNodes.Detail;
using BlazorSvt.Modules.LocationsNodes.List;
using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ModuleGridIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task TransportLegGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task TransportRateGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task LocationsNodesGrid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task AverageRateLevel3Grid_ReturnsRows()
    {
        var connectionString = RequireConnectionString();

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery());

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
    }

    [SkippableFact]
    public async Task TransportLegGrid_SecondPageDiffersFromFirstPage()
    {
        var connectionString = RequireConnectionString();
        var sort = new GridSort(nameof(TransportLegDto.TransportLegId), "ASC");
        var filters = new[] { new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator) };

        var (pageOne, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            new GridQuery(1, 10, "ru-RU", sort, filters));

        var (pageTwo, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            new GridQuery(2, 10, "ru-RU", sort, filters));

        pageOne.Should().NotBeEmpty();
        pageTwo.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(10);
        pageOne[0].TransportLegId.Should().NotBe(pageTwo[0].TransportLegId);
    }

    [SkippableFact]
    public async Task TransportLegDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportLegId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportLegDetailDto>(
            connectionString,
            "v2.vw_TransportLeg_Detail",
            "TransportLegId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportLegId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task TransportRateDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].TransportRateId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<TransportRateDetailDto>(
            connectionString,
            "v2.vw_TransportRate_Detail",
            "TransportRateId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.TransportRateId.Should().Be(entityKey);
    }

    [SkippableFact]
    public async Task LocationsNodesDetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<LocationsNodesDto>(
            connectionString,
            typeof(LocationsNodesDto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].LocationsNodesId;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<LocationsNodesDetailDto>(
            connectionString,
            "v2.vw_LocationsNodes_Detail",
            "LocationsNodesId",
            entityKey);

        detail.Should().NotBeNull();
        detail!.LocationsNodesId.Should().Be(entityKey);
        detail.Code.Should().NotBeNullOrWhiteSpace();
    }

    [SkippableFact]
    public async Task AverageRateLevel3DetailView_ReturnsRowForGridEntityKey()
    {
        var connectionString = RequireConnectionString();

        var (rows, _) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            GridSpTestHelper.CreateDefaultQuery(pageSize: 1));

        var entityKey = rows[0].AverageRateLevel3Id;

        var detail = await GridSpTestHelper.QueryDetailViewAsync<AverageRateLevel3DetailDto>(
            connectionString,
            "v2.vw_AverageRateLevel3_Detail",
            "AverageRateLevel3Id",
            entityKey);

        detail.Should().NotBeNull();
        detail!.AverageRateLevel3Id.Should().Be(entityKey);
    }
}
