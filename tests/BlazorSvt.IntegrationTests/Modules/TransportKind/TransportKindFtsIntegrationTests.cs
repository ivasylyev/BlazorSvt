using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportKind.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.TransportKind;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportKindFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithNameContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(TransportKindDto.Name), "Автомобильный"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportKindDto>(
            connectionString,
            typeof(TransportKindDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(1);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("Автомобильный", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithNameAndLevelContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(TransportKindDto.Name), "транспорт"),
            FtsFilterTestSupport.Contains(nameof(TransportKindDto.Level), "уровень"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<TransportKindDto>(
            connectionString,
            typeof(TransportKindDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().Be(3);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Name != null && r.Name.Contains("транспорт", StringComparison.OrdinalIgnoreCase));
        rows.Should().OnlyContain(r => r.Level != null && r.Level.Contains("уровень", StringComparison.OrdinalIgnoreCase));
    }
}
