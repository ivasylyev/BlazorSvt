using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.RateSyncClass.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.RateSyncClass;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RateSyncClassFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RateSyncClassDto.Name), "Сухие"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateSyncClassDto>(
            connectionString,
            typeof(RateSyncClassDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("ухие", StringComparison.OrdinalIgnoreCase));
    }
}
