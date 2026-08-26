using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.ParityRates.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.ParityRates;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ParityRatesFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithOneFtsAndRelevanceFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(
                nameof(ParityRatesDto.RelevanceIdRu),
                (int)RelevanceRu.Month),
            FtsFilterTestSupport.Contains(nameof(ParityRatesDto.ProductGroupNameRu), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ParityRatesDto>(
            connectionString,
            typeof(ParityRatesDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.RelevanceIdRu == RelevanceRu.Month);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNodeNameFts_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(ParityRatesDto.NodeFromNameRu), "нижнекамск"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ParityRatesDto>(
            connectionString,
            typeof(ParityRatesDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
    }
}
