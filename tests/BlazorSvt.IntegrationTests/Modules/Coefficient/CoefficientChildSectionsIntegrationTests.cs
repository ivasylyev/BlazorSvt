using System.Data.SqlClient;
using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.Coefficient.Detail;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Modules.Coefficient;

[Collection("Database")]
[Trait("Category", "Integration")]
public class CoefficientChildSectionsIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    private const long CoefficientId = 26857587;

    [SkippableFact]
    public async Task ConditionValues_ReturnRowWithResolvedReferences()
    {
        var page = await CreateService().GetRowsAsync<CoefficientConditionValueDto>(
            CoefficientId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().OnlyContain(r => r.CoefficientId == CoefficientId && r.PrimitiveEntityDataStateId == 1);
        page.Rows.Should().Contain(r => r.TransportKindIdRu != null && r.TransportKindIdEn != null);
        page.Rows.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Code));
    }

    [SkippableFact]
    public async Task ComponentValues_ReturnRowWithComponentName()
    {
        var page = await CreateService().GetRowsAsync<CoefficientComponentValueDto>(
            CoefficientId,
            DetailChildRowStates.For(parentIsArchive: false),
            CancellationToken.None);

        page.TotalCount.Should().BeGreaterThan(0);
        page.Rows.Should().OnlyContain(r =>
            r.CoefficientId == CoefficientId
            && !string.IsNullOrWhiteSpace(r.ComponentName)
            && r.Value != null);
    }

    [SkippableFact]
    public async Task RowLimit_CapsRowsButKeepsTotalCount()
    {
        var all = await CreateService().GetRowsAsync<CoefficientComponentValueDto>(
            await CoefficientWithMostComponentsAsync(),
            DetailChildRowStates.For(parentIsArchive: true),
            CancellationToken.None);
        Skip.If(all.TotalCount < 2, "Need a coefficient with at least two components.");

        var limited = await CreateService(rowLimit: 1).GetRowsAsync<CoefficientComponentValueDto>(
            all.Rows[0].CoefficientId,
            DetailChildRowStates.For(parentIsArchive: true),
            CancellationToken.None);

        limited.Rows.Should().ContainSingle().Which.Id.Should().Be(all.Rows.Min(r => r.Id));
        limited.TotalCount.Should().Be(all.TotalCount);
    }

    private async Task<long> CoefficientWithMostComponentsAsync()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(RequireConnectionString());
#pragma warning restore CS0618 // Type or member is obsolete
        return await connection.ExecuteScalarAsync<long>(
            """
            SELECT TOP 1 CoefficientId
            FROM v2.vw_CoefficientComponentValue_Detail
            GROUP BY CoefficientId
            ORDER BY COUNT(*) DESC
            """);
    }

    private DetailChildDataService CreateService(int rowLimit = 100) =>
        new(
            Options.Create(new DatabaseOptions { MdmDb = RequireConnectionString() }),
            Options.Create(new GridOptions { ChildSectionRowLimit = rowLimit }),
            NullLogger<DetailChildDataService>.Instance,
            Substitute.For<IAccessGuard>());
}
