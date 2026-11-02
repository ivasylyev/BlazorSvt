using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.ProductGroup.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.ProductGroup;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ProductGroupFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(ProductGroupDto.Name), "каучук"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ProductGroupDto>(
            connectionString,
            typeof(ProductGroupDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("каучук", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNameEnContainsAndRateSyncClassFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(
                nameof(ProductGroupDto.RateSyncClassIdRu),
                (int)RateSyncClassRu.DryNonHazardous),
            FtsFilterTestSupport.Contains(nameof(ProductGroupDto.NameEn), "rubber"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ProductGroupDto>(
            connectionString,
            typeof(ProductGroupDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.RateSyncClassIdRu == RateSyncClassRu.DryNonHazardous);
    }
}
