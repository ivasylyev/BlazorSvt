using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Contractor.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Contractor;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ContractorFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameSearchContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(ContractorDto.NameSearch), "ООО"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ContractorDto>(
            connectionString,
            typeof(ContractorDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.NameSearch != null && r.NameSearch.Contains("ООО", StringComparison.OrdinalIgnoreCase));
    }
}
