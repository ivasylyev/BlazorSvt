using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.MTR.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.MTR;

[Collection("Database")]
[Trait("Category", "Integration")]
public class MTRFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(MTRDto.NameFullRu), "полипропилен"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<MTRDto>(
            connectionString,
            typeof(MTRDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.NameFullRu != null && r.NameFullRu.Contains("полипропилен", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContainsAndUnitFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(nameof(MTRDto.UnitIdRu), (int)UnitRu.Ton),
            FtsFilterTestSupport.Contains(nameof(MTRDto.NameFullRu), "полипропилен"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<MTRDto>(
            connectionString,
            typeof(MTRDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.UnitIdRu == UnitRu.Ton);
        rows.Should().OnlyContain(r => r.NameFullRu != null && r.NameFullRu.Contains("полипропилен", StringComparison.OrdinalIgnoreCase));
    }
}
