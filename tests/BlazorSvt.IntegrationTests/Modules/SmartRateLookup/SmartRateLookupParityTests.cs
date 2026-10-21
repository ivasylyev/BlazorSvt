using System.Data.SqlClient;
using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.SmartRateLookup.Contracts;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.SmartRateLookup.Resources;
using BlazorSvt.Modules.SmartRateLookup.Services;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Infrastructure.Config;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit.Abstractions;

namespace BlazorSvt.IntegrationTests.Modules.SmartRateLookup;

/// <summary>
/// Паритет умного подбора с продовой <c>mdm_integ.dbo.CalcTransportRateCRM</c> на запросах CRM из <c>dbo.TransportRateCRM</c>:
/// по каждому коду уровня (и «не найдено») — последние запросы; эталон — пересчёт процедурой (<c>@Debug = 1</c>, «Обновленная ставка»).
/// </summary>
[Collection("Database")]
[Trait("Category", "Integration")]
public class SmartRateLookupParityTests(DatabaseFixture fixture, ITestOutputHelper output) : IntegrationTestBase(fixture)
{
    private const int RequestsPerLevel = 3;

    private const string SampleSql = """
        SELECT ID, RequestDate, NodeFromCode, NodeToCode, ProductCode, TransportTypeCode, CurrencyCode
        FROM (
            SELECT
                ID,
                CAST(DATEADD(hh, DATEDIFF(hh, GETUTCDATE(), GETDATE()), RequestTime) AS DATE) AS RequestDate,
                RateNodeFromCode AS NodeFromCode,
                RateNodeToCode AS NodeToCode,
                RateProductCode AS ProductCode,
                RateTransportTypeCode AS TransportTypeCode,
                RateCurrency AS CurrencyCode,
                ROW_NUMBER() OVER (PARTITION BY RateAggregateCode ORDER BY ID DESC) AS rn
            FROM mdm_integ.dbo.TransportRateCRM
            WHERE RequestStatus IN (0, 1)
              AND RequestTime > DATEADD(MONTH, -3, GETUTCDATE())
        ) s
        WHERE rn <= @PerLevel
        ORDER BY ID DESC
        """;

    [SkippableFact]
    public async Task Lookup_MatchesLegacyCrmProcedure()
    {
        var connectionString = RequireConnectionString();
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();
        Skip.IfNot(
            await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sys.databases WHERE name = 'mdm_integ'") == 1,
            "mdm_integ (CRM integration DB) is not available.");

        var requests = (await connection.QueryAsync<CrmRequest>(SampleSql, new { PerLevel = RequestsPerLevel })).ToList();
        Skip.If(requests.Count == 0, "No recent CRM requests in mdm_integ.dbo.TransportRateCRM.");

        var service = CreateService(connectionString);
        var mismatches = new List<string>();

        foreach (var request in requests)
        {
            var expected = await LegacyRateIdAsync(connection, request.Id);
            long? actual;
            try
            {
                var response = await service.LookupAsync(
                    new SmartRateLookupRequest
                    {
                        NodeFromCode = request.NodeFromCode,
                        NodeToCode = request.NodeToCode,
                        ProductCode = request.ProductCode,
                        TransportTypeCode = request.TransportTypeCode,
                        CurrencyCode = request.CurrencyCode,
                        Date = DateOnly.FromDateTime(request.RequestDate)
                    },
                    CancellationToken.None);
                actual = response.Rate?.AverageRateLevel3Id;
            }
            catch (ApiException ex) when (ex.StatusCode == 400)
            {
                // Неизвестный код — процедура тоже не находит ставку.
                actual = null;
            }

            output.WriteLine($"{request.Id}: legacy {expected?.ToString() ?? "-"}, api {actual?.ToString() ?? "-"}");
            if (expected != actual)
            {
                mismatches.Add($"request {request.Id} ({request.NodeFromCode}->{request.NodeToCode}, {request.ProductCode}, {request.TransportTypeCode}, {request.RequestDate:yyyy-MM-dd}): legacy {expected}, api {actual}");
            }
        }

        mismatches.Should().BeEmpty();
    }

    private static async Task<long?> LegacyRateIdAsync(System.Data.Common.DbConnection connection, int id)
    {
        await using var reader = await connection.ExecuteReaderAsync(
            "mdm_integ.dbo.CalcTransportRateCRM",
            new { ID = id, Debug = true },
            commandType: System.Data.CommandType.StoredProcedure,
            commandTimeout: 120);

        long? rateId = null;
        do
        {
            var hasRequestType = Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i) == "RequestType");
            if (!hasRequestType)
            {
                continue;
            }

            // Последняя строка — «Обновленная ставка»: пересчёт на текущих данных.
            while (await reader.ReadAsync())
            {
                var value = reader["RateID"];
                rateId = value is DBNull ? null : Convert.ToInt64(value);
            }
        }
        while (await reader.NextResultAsync());

        return rateId;
    }

    private static SmartRateLookupService CreateService(string connectionString)
    {
        var repository = new SmartRateLookupRepository(
            Options.Create(new DatabaseOptions { MdmDb = connectionString }),
            NullLogger<SmartRateLookupRepository>.Instance);
        var localizerFactory = new ResourceManagerStringLocalizerFactory(
            Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
        return new SmartRateLookupService(
            repository,
            Substitute.For<IAccessGuard>(),
            new StringLocalizer<SmartRateLookupText>(localizerFactory),
            TimeProvider.System);
    }

    private sealed class CrmRequest
    {
        public int Id { get; init; }
        public DateTime RequestDate { get; init; }
        public string? NodeFromCode { get; init; }
        public string? NodeToCode { get; init; }
        public string? ProductCode { get; init; }
        public string? TransportTypeCode { get; init; }
        public string? CurrencyCode { get; init; }
    }
}
