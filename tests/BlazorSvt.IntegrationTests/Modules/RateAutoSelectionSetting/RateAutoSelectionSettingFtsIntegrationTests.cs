using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.RateAutoSelectionSetting.List;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Modules.RateAutoSelectionSetting;

[Collection("Database")]
[Trait("Category", "Integration")]
public class RateAutoSelectionSettingFtsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task GetBlazorGridData_WithCodeEquals_ReturnsRows()
    {
        // Code — INT (SVT-5317): фильтр точный, работает с одной цифры.
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(nameof(RateAutoSelectionSettingDto.Code), 3));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            query);

        rows.Should().NotBeEmpty();
        totalCount.Should().BeGreaterThan(0);
        rows.Should().OnlyContain(r => !r.IsArchive);
        rows.Should().OnlyContain(r => r.Code == 3);
    }

    [SkippableFact]
    public async Task GetBlazorGridData_WithMissingCode_ReturnsEmpty()
    {
        var connectionString = RequireConnectionString();
        var query = FtsFilterTestSupport.CreateQuery(
            FtsFilterTestSupport.IdEquals(nameof(RateAutoSelectionSettingDto.Code), 333));

        var (rows, totalCount) = await GridSpTestHelper.ExecuteGetBlazorGridDataAsync<RateAutoSelectionSettingDto>(
            connectionString,
            typeof(RateAutoSelectionSettingDto),
            query);

        rows.Should().BeEmpty();
        totalCount.Should().Be(0);
    }
}
