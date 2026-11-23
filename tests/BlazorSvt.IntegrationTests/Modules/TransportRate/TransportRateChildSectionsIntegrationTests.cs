using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.TransportRate;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportRateChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    /// <summary>Ставка с одним неархивным тендерным пакетом и заполненной валютой лота.</summary>
    private const long RateWithTenderPackId = 39010792;

    /// <summary>Ставка с двумя неархивными коэффициентами: коды ставок, имя коэффициента и вид исходной ставки.</summary>
    private const long RateWithCoefficientsId = 32747452;

    [SkippableFact]
    public async Task TenderPacks_ReturnRowWithResolvedCurrency()
    {
        var page = await CreateService().GetRowsAsync<TransportRateTenderServicePackDto>(
            RateWithTenderPackId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);
        Skip.If(page.TotalCount == 0, $"TransportRate {RateWithTenderPackId} has no tender packs in this database.");

        page.Rows.Should().HaveCount(page.TotalCount);
        page.Rows.Should().OnlyContain(r =>
            r.TransportRateId == RateWithTenderPackId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.TenderServicePack)
            && r.LotRate != null
            && IsDefined(r.LotCurrencyId));
    }

    [SkippableFact]
    public async Task Coefficients_ReturnRowsWithResolvedNames()
    {
        var page = await CreateService().GetRowsAsync<TransportRateCoefficientDto>(
            RateWithCoefficientsId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);
        Skip.If(page.TotalCount == 0, $"TransportRate {RateWithCoefficientsId} has no coefficients in this database.");

        page.Rows.Should().HaveCount(page.TotalCount);
        page.Rows.Should().OnlyContain(r =>
            r.TransportRateId == RateWithCoefficientsId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.CoefficientRateCode)
            && !string.IsNullOrWhiteSpace(r.CoefficientName)
            && !string.IsNullOrWhiteSpace(r.SourceRateCode)
            && !string.IsNullOrWhiteSpace(r.SourceRateKindName)
            && r.StartDate != null
            && r.EndDate != null);
    }

    private static bool IsDefined<TEnum>(TEnum? value) where TEnum : struct, Enum =>
        value is { } v && Enum.IsDefined(v);

    private DetailChildDataService CreateService(int rowLimit = 100) =>
        new(
            Options.Create(new DatabaseOptions { MdmDb = RequireConnectionString() }),
            Options.Create(new GridOptions { ChildSectionRowLimit = rowLimit }),
            NullLogger<DetailChildDataService>.Instance,
            Substitute.For<IAccessGuard>());
}
