using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.TransportRoute.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.TransportRoute;

[Collection("Database")]
[Trait("Category", "Integration")]
public class TransportRouteChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    /// <summary>Маршрут с шестью неархивными плечами: код, узлы отправления и назначения, вид транспорта.</summary>
    private const long RouteWithNodesId = 18133141;

    /// <summary>Маршрут, у плеча которого заполнены вид транспорта и оба типа плеча.</summary>
    private const long RouteWithLegTypesId = 39392689;

    [SkippableFact]
    public async Task Legs_ReturnRowsWithResolvedNodesAndTransportKind()
    {
        var page = await CreateService().GetRowsAsync<TransportRouteLegDto>(
            RouteWithNodesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().HaveCount(page.TotalCount);
        page.Rows.Should().OnlyContain(r =>
            r.TransportRouteId == RouteWithNodesId
            && r.PrimitiveEntityDataStateId == 1
            && !string.IsNullOrWhiteSpace(r.Code)
            && !string.IsNullOrWhiteSpace(r.LegCode)
            && !string.IsNullOrWhiteSpace(r.NodeFromNameRu)
            && !string.IsNullOrWhiteSpace(r.NodeFromNameEn)
            && !string.IsNullOrWhiteSpace(r.NodeToNameRu)
            && !string.IsNullOrWhiteSpace(r.NodeToNameEn)
            && IsDefined(r.TransportKindIdRu)
            && IsDefined(r.TransportKindIdEn));
    }

    [SkippableFact]
    public async Task Legs_FullyFilledRow_ResolvesLegTypes()
    {
        var page = await CreateService().GetRowsAsync<TransportRouteLegDto>(
            RouteWithLegTypesId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);
        Skip.If(page.TotalCount == 0, $"TransportRoute {RouteWithLegTypesId} has no legs in this database.");

        var row = page.Rows.First(r =>
            r.TransportKindIdRu == TransportKindRu.Mix
            && r.Leg1_TransportTypeIdRu == TransportTypeLevel3Ru.Mix_rail_cont_40
            && r.Leg2_TransportTypeIdRu == TransportTypeLevel3Ru.Mix_sea);
        IsDefined(row.TransportKindIdEn).Should().BeTrue();
        IsDefined(row.Leg1_TransportTypeIdEn).Should().BeTrue();
        IsDefined(row.Leg2_TransportTypeIdEn).Should().BeTrue();
        row.LegCode.Should().NotBeNullOrWhiteSpace();
        row.NodeFromNameRu.Should().NotBeNullOrWhiteSpace();
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
