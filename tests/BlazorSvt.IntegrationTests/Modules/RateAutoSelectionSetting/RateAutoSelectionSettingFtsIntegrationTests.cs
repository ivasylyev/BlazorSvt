using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.RateAutoSelectionSetting.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.RateAutoSelectionSetting;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RateAutoSelectionSettingFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithCodeContains_ReturnsRows()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RateAutoSelectionSettingDto.Code), "3"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code.Contains("3", StringComparison.OrdinalIgnoreCase));
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithCodeContainsLongerThanThreshold_RunsFullText()
    {
        // Единственный Code — «3». GetBlazorGridData вызывает CONTAINS только при длине больше 2.
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.Contains(nameof(RateAutoSelectionSettingDto.Code), "333"));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            query);

        rows.Should().BeEmpty();
        totalCount.Should().Be(0);
    }
}
