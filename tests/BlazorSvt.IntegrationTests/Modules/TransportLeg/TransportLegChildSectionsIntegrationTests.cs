using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.TransportLeg;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportLegChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    /// <summary>Плечо с неархивными ставками: код цели, тип ставки, тип транспорта и валюта.</summary>
    private const long LegWithRatesId = 19297637;

    /// <summary>Плечо с неархивными средневзвешенными ставками 3 уровня.</summary>
    private const long LegWithAverageRatesId = 19510517;

    /// <summary>Плечо с неархивными лидтаймами и типом отправки.</summary>
    private const long LegWithLeadtimesId = 23524357;

    [SkippableFact]
    public async Task LegRates_ReturnRowsWithResolvedRateFields()
    {
        var page = await CreateService().GetRowsAsync<LegRateDto>(
            LegWithRatesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().NotBeEmpty();
        page.Rows.Should().OnlyContain(r =>
            r.TransportLegId == LegWithRatesId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.RateCode)
            && IsDefined(r.RateTypeIdRu)
            && IsDefined(r.RateTypeIdEn)
            && IsDefined(r.TransportTypeIdRu)
            && IsDefined(r.TransportTypeIdEn)
            && IsDefined(r.CurrencyId)
            && r.StartDate != null
            && r.EndDate != null);
        page.Rows.Should().Contain(r =>
            !string.IsNullOrWhiteSpace(r.ProductGroupNameRu)
            && !string.IsNullOrWhiteSpace(r.ProductGroupNameEn));
    }

    [SkippableFact]
    public async Task AverageRates_ReturnRowsWithResolvedRateFields()
    {
        var page = await CreateService().GetRowsAsync<LegAverageRateLevel3Dto>(
            LegWithAverageRatesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().NotBeEmpty();
        page.Rows.Should().OnlyContain(r =>
            r.TransportLegId == LegWithAverageRatesId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.AverageRateCode)
            && r.IsDefRate != null
            && IsDefined(r.RateTypeIdRu)
            && IsDefined(r.RateTypeIdEn)
            && IsDefined(r.TransportTypeIdRu)
            && IsDefined(r.TransportTypeIdEn)
            && IsDefined(r.CurrencyId)
            && r.RateLevel3 != null
            && r.StartDate != null
            && r.EndDate != null);
        page.Rows.Should().Contain(r =>
            !string.IsNullOrWhiteSpace(r.ProductGroupNameRu)
            && !string.IsNullOrWhiteSpace(r.ProductGroupNameEn));
    }

    [SkippableFact]
    public async Task Leadtimes_ReturnRowsWithResolvedShipmentType()
    {
        var page = await CreateService().GetRowsAsync<LeadtimeDto>(
            LegWithLeadtimesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().NotBeEmpty();
        page.Rows.Should().OnlyContain(r =>
            r.TransportLegId == LegWithLeadtimesId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.ShipmentTypeNameRu)
            && !string.IsNullOrWhiteSpace(r.ShipmentTypeNameEn)
            && r.StartDate != null
            && r.EndDate != null);
    }

    private static bool IsDefined<TEnum>(TEnum? value) where TEnum : struct, Enum =>
        value is { } v && Enum.IsDefined(v);

    private DetailChildDataService CreateService() =>
        new(
            Options.Create(new DatabaseOptions { MdmDb = RequireConnectionString() }),
            Options.Create(new GridOptions { ChildSectionRowLimit = 100 }),
            NullLogger<DetailChildDataService>.Instance,
            Substitute.For<IAccessGuard>());
}
