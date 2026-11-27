using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Country.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Country;

[Collection("Database")]
[Trait("Category", "Integration")]
public class CountryFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameRuContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(CountryDto.NameRu), "Афганистан"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CountryDto>(
            connectionString,
            typeof(CountryDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(1);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.NameRu != null && r.NameRu.Contains("Афганистан", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNameRuAndMarket_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(CountryDto.NameRu), "Афганистан"),
            FtsFilterTestSupport.Contains(nameof(CountryDto.Market), "Азия"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CountryDto>(
            connectionString,
            typeof(CountryDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(1);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Market != null && r.Market.Contains("Азия", StringComparison.OrdinalIgnoreCase));
    }
}
