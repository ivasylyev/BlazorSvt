using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.AverageRateLevel3.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.AverageRateLevel3;

[Collection("Database")]
[Trait("Category", "Integration")]
public class AverageRateLevel3FtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static GridFilter MultimodalTransportKind { get; } =
        FtsFilterTestSupport.IdEquals(
            nameof(AverageRateLevel3Dto.TransportKindIdRu),
            (int)TransportKindRu.Mix);

    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            MultimodalTransportKind,
            FtsFilterTestSupport.Contains(nameof(AverageRateLevel3Dto.ProductGroupNameRu), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithFourFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            MultimodalTransportKind,
            FtsFilterTestSupport.Contains(nameof(AverageRateLevel3Dto.NodeFromNameRu), "нижнекамск"),
            FtsFilterTestSupport.Contains(nameof(AverageRateLevel3Dto.ProxyNodeNameRu), "порт"),
            FtsFilterTestSupport.Contains(nameof(AverageRateLevel3Dto.NodeToNameRu), "китай"),
            FtsFilterTestSupport.Contains(nameof(AverageRateLevel3Dto.ProductGroupNameRu), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<AverageRateLevel3Dto>(
            connectionString,
            typeof(AverageRateLevel3Dto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
    }
}
