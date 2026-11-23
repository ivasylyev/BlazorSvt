using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.AverageRateLevel3.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.AverageRateLevel3;

[Collection("Database")]
[Trait("Category", "Integration")]
public class AverageRateLevel3ChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    /// <summary>Ставка 3 уровня с одной неархивной строкой RatesOfAverageRate: код ребёнка, код ставки и обе стоимости заполнены.</summary>
    private const long ParentWithRateId = 35906580;

    [SkippableFact]
    public async Task Rates_ReturnRowsWithResolvedRateCodeAndCosts()
    {
        var page = await CreateService().GetRowsAsync<RatesOfAverageRateDto>(
            ParentWithRateId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().HaveCount(page.TotalCount);
        page.Rows.Should().OnlyContain(r =>
            r.AverageRateLevel3Id == ParentWithRateId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.TransportRateCode)
            && r.TotalCostTon != null
            && r.TotalCostTransport != null);
    }

    private DetailChildDataService CreateService(int rowLimit = 100) =>
        new(
            Options.Create(new DatabaseOptions { MdmDb = RequireConnectionString() }),
            Options.Create(new GridOptions { ChildSectionRowLimit = rowLimit }),
            NullLogger<DetailChildDataService>.Instance,
            Substitute.For<IAccessGuard>());
}
