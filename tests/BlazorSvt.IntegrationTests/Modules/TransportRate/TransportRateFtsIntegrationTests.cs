using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.TransportRate;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportRateFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static GridFilter MultimodalTransportKind { get; } =
        FtsFilterTestSupport.IdEquals(
            nameof(TransportRateDto.TransportKindIdRu),
            (int)TransportKindRu.Mix);

    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            MultimodalTransportKind,
            FtsFilterTestSupport.Contains(nameof(TransportRateDto.ProductGroupNameRu), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
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
            FtsFilterTestSupport.Contains(nameof(TransportRateDto.NodeFromNameRu), "нижнекамск"),
            FtsFilterTestSupport.Contains(nameof(TransportRateDto.ProxyNodeNameRu), "порт"),
            FtsFilterTestSupport.Contains(nameof(TransportRateDto.NodeToNameRu), "китай"),
            FtsFilterTestSupport.Contains(nameof(TransportRateDto.ProductGroupNameRu), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportRateDto>(
            connectionString,
            typeof(TransportRateDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
    }
}
