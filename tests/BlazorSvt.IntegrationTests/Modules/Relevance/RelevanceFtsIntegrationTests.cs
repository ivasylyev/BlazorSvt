using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Relevance.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Relevance;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RelevanceFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RelevanceDto.Name), "Мес"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RelevanceDto>(
            connectionString,
            typeof(RelevanceDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(1);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("Мес", StringComparison.OrdinalIgnoreCase));
    }
}
