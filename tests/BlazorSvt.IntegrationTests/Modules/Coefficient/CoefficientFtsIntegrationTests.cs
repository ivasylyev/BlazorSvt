using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Coefficient.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.Coefficient;

[Collection("Database")]
[Trait("Category", "Integration")]
public class CoefficientFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        // Индекс хранит имя целиком (2019_авто): CONTAINS ищет префикс токена, не подстроку в середине.
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(CoefficientDto.Name), "2023_авто"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<CoefficientDto>(
            connectionString,
            typeof(CoefficientDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("2023_авто", StringComparison.OrdinalIgnoreCase));
    }
}
