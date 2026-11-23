using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Region.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Region;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RegionFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameRuContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RegionDto.NameRu), "Кабул"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RegionDto>(
            connectionString,
            typeof(RegionDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.NameRu != null && r.NameRu.Contains("Кабул", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNameRuMarketAndStatusFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(nameof(RegionDto.StatusIdRu), (int)StatusElementRu.Active),
            FtsFilterTestSupport.Contains(nameof(RegionDto.NameRu), "Кабул"),
            FtsFilterTestSupport.Contains(nameof(RegionDto.Market), "Азия"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RegionDto>(
            connectionString,
            typeof(RegionDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.StatusIdRu == StatusElementRu.Active);
        rows.Should().OnlyContain(r => r.NameRu != null && r.NameRu.Contains("Кабул", StringComparison.OrdinalIgnoreCase));
        rows.Should().OnlyContain(r => r.Market != null && r.Market.Contains("Азия", StringComparison.OrdinalIgnoreCase));
    }
}
