using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.RateType.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.RateType;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RateTypeFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RateTypeDto.Name), "тендер"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateTypeDto>(
            connectionString,
            typeof(RateTypeDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("ендер", StringComparison.OrdinalIgnoreCase));
    }
}
