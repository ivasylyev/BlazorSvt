using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.TransportLeg;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportLegFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private static GridFilter MultimodalTransportKind { get; } =
        FtsFilterTestSupport.IdEquals(
            nameof(TransportLegDto.TransportKindIdRu),
            (int)TransportKindRu.Mix);

    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            MultimodalTransportKind,
            FtsFilterTestSupport.Contains(nameof(TransportLegDto.NodeFromNameRu), "нижнекамск"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithThreeFtsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            MultimodalTransportKind,
            FtsFilterTestSupport.Contains(nameof(TransportLegDto.NodeFromNameRu), "нижнекамск"),
            FtsFilterTestSupport.Contains(nameof(TransportLegDto.ProxyNodeNameRu), "порт"),
            FtsFilterTestSupport.Contains(nameof(TransportLegDto.NodeToNameRu), "китай"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportLegDto>(
            connectionString,
            typeof(TransportLegDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
    }
}
