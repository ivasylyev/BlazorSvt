using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Currency.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Currency;

[Collection("Database")]
[Trait("Category", "Integration")]
public class CurrencyFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(CurrencyDto.Name), "рубл"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CurrencyDto>(
            connectionString,
            typeof(CurrencyDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("рубл", StringComparison.OrdinalIgnoreCase));
    }
}
