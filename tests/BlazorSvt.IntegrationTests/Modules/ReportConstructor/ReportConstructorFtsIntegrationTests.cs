using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.ReportConstructor.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.ReportConstructor;

[Collection("Database")]
[Trait("Category", "Integration")]
public class ReportConstructorFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithProductGroupNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(ReportConstructorDto.ProductGroupNameRu), "Каучуки"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ReportConstructorDto>(
            connectionString,
            typeof(ReportConstructorDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r =>
            r.ProductGroupNameRu != null
            && r.ProductGroupNameRu.Contains("Каучуки", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithTransportKindFilter_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(
                nameof(ReportConstructorDto.TransportKindIdRu),
                (int)TransportKindRu.Auto));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<ReportConstructorDto>(
            connectionString,
            typeof(ReportConstructorDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.TransportKindIdRu == TransportKindRu.Auto);
    }
}
