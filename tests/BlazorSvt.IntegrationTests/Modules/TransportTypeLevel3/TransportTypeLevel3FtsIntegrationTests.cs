using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportTypeLevel3.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.TransportTypeLevel3;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportTypeLevel3FtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContainsAndTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(nameof(TransportTypeLevel3Dto.TransportKindIdRu), (int)TransportKindRu.Mix),
            FtsFilterTestSupport.Contains(nameof(TransportTypeLevel3Dto.Name), "контейнер"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportTypeLevel3Dto>(
            connectionString,
            typeof(TransportTypeLevel3Dto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Mix);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("контейнер", StringComparison.OrdinalIgnoreCase));
    }
}
