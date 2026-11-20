using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.RateCalcType.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.RateCalcType;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RateCalcTypeFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithSearchCodeContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RateCalcTypeDto.SearchCode), "Tonne"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateCalcTypeDto>(
            connectionString,
            typeof(RateCalcTypeDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.SearchCode != null && r.SearchCode.Contains("Tonne", StringComparison.OrdinalIgnoreCase));
    }
}
