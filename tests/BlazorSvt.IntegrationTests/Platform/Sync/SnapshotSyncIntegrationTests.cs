using System.Data;
using System.Data.SqlClient;
using BlazorSvt.IntegrationTests.Infrastructure;
using BlazorSvt.Modules.LocationsNodes.Sync;
using BlazorSvt.Modules.TransportLeg.Sync;
using BlazorSvt.Modules.TransportRate.Sync;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Sync;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlazorSvt.IntegrationTests.Platform.Sync;

/// <summary>
/// Инкрементальная синхронизация: COMMIT-стимул в legacy → RunIncrementalAsync → assert → cleanup.
/// </summary>
[Collection("Database")]
[Trait("Category", "Integration")]
public sealed class SnapshotSyncIntegrationTests(DatabaseFixture fixture) : IntegrationTestBase(fixture)
{
    [SkippableFact]
    public async Task TransportLeg_IncrementalSync_PropagatesLegacyChange()
    {
        await RunCommitStimulusScenarioAsync(new SyncStimulusCase(
            new TransportLegSyncJob(),
            LegacyTable: "dbo.PrimitiveEntityData_2007",
            LegacyColumn: "a_4976", // vw_TransportLeg.SearchTimeT
            SnapshotColumn: "SearchTimeT",
            CreateStimulusValue: original => $"SVT{Random.Shared.Next(100_000, 999_999):D6}",
            ParseLegacyValue: value => (string)value!,
            FormatLegacyValue: value => (string)value));
    }

    [SkippableFact]
    public async Task LocationsNodes_IncrementalSync_PropagatesLegacyChange()
    {
        await RunCommitStimulusScenarioAsync(new SyncStimulusCase(
            new LocationsNodesSyncJob(),
            LegacyTable: "dbo.PrimitiveEntityData_1014",
            LegacyColumn: "a_1020", // vw_LocationsNodes.Name_ru
            SnapshotColumn: "NameRu",
            CreateStimulusValue: original => $"SVT_TST_{Random.Shared.Next(100_000, 999_999)}",
            ParseLegacyValue: value => (string)value!,
            FormatLegacyValue: value => (string)value));
    }

    [SkippableFact]
    public async Task TransportRate_IncrementalSync_PropagatesLegacyChange()
    {
        await RunCommitStimulusScenarioAsync(new SyncStimulusCase(
            new TransportRateSyncJob(),
            LegacyTable: "dbo.PrimitiveEntityData_2012",
            LegacyColumn: "a_3283", // vw_TransportRate.TotalCostTon
            SnapshotColumn: "TotalCostTon",
            CreateStimulusValue: original => decimal.Round(Convert.ToDecimal(original), 2) + 10m,
            ParseLegacyValue: value => decimal.Round(Convert.ToDecimal(value), 2),
            FormatLegacyValue: value => (decimal)value!));
    }

    private async Task RunCommitStimulusScenarioAsync(SyncStimulusCase testCase)
    {
        var connectionString = RequireConnectionString();
        await EnsureSyncInfrastructureReadyAsync(connectionString, testCase.Job);

        var entityKey = await SnapshotSyncTestHelper.PickNonArchiveEntityKeyAsync(
            connectionString,
            testCase.Job.SnapshotTable,
            testCase.Job.EntityKeyColumn);

        var originalLegacyValue = await SnapshotSyncTestHelper.GetLegacyColumnValueAsync(
            connectionString,
            testCase.LegacyTable,
            testCase.LegacyColumn,
            entityKey);

        originalLegacyValue.Should().NotBeNull(
            $"legacy row for {testCase.Job.Entity} key {entityKey} must exist in {testCase.LegacyTable}");

        await SnapshotSyncTestHelper.AssertSnapshotMatchesSourceAsync(
            connectionString,
            testCase.Job,
            entityKey,
            testCase.SnapshotColumn);

        var stimulusValue = testCase.CreateStimulusValue(originalLegacyValue!);
        var executor = SnapshotSyncTestHelper.CreateExecutor(connectionString);

        try
        {
            await SnapshotSyncTestHelper.UpdateLegacyColumnAsync(
                connectionString,
                testCase.LegacyTable,
                testCase.LegacyColumn,
                entityKey,
                testCase.FormatLegacyValue(stimulusValue));

            await executor.RunIncrementalAsync(testCase.Job, CancellationToken.None);

            var affected = await SnapshotSyncTestHelper.GetLastAffectedCountAsync(
                connectionString,
                testCase.Job.Entity,
                testCase.LegacyTable);

            affected.Should().BeGreaterThan(0,
                $"sync should detect committed change in {testCase.LegacyTable}");

            var snapshotValue = await SnapshotSyncTestHelper.GetSnapshotColumnValueAsync(
                connectionString,
                testCase.Job.SnapshotTable,
                testCase.Job.EntityKeyColumn,
                entityKey,
                testCase.SnapshotColumn);

            var sourceValue = await SnapshotSyncTestHelper.GetSourceColumnValueAsync(
                connectionString,
                testCase.Job.SourceProjectionView,
                testCase.Job.EntityKeyColumn,
                entityKey,
                testCase.SnapshotColumn);

            testCase.ParseLegacyValue(snapshotValue).Should().Be(testCase.ParseLegacyValue(sourceValue));
            testCase.ParseLegacyValue(sourceValue).Should().Be(testCase.ParseLegacyValue(stimulusValue));
            testCase.ParseLegacyValue(snapshotValue).Should().NotBe(testCase.ParseLegacyValue(originalLegacyValue));
        }
        finally
        {
            await SnapshotSyncTestHelper.UpdateLegacyColumnAsync(
                connectionString,
                testCase.LegacyTable,
                testCase.LegacyColumn,
                entityKey,
                testCase.FormatLegacyValue(originalLegacyValue!));

            await executor.RunIncrementalAsync(testCase.Job, CancellationToken.None);

            await SnapshotSyncTestHelper.AssertSnapshotMatchesSourceAsync(
                connectionString,
                testCase.Job,
                entityKey,
                testCase.SnapshotColumn);
        }
    }

    private static async Task EnsureSyncInfrastructureReadyAsync(string connectionString, ISnapshotSyncJob job)
    {
        var mainSource = job.Sources[0].Name;
        var cursor = await SnapshotSyncTestHelper.GetCursorAsync(connectionString, job.Entity, mainSource);

        if (cursor is not null)
        {
            return;
        }

        var executor = SnapshotSyncTestHelper.CreateExecutor(connectionString);
        await executor.RunIncrementalAsync(job, CancellationToken.None);

        cursor = await SnapshotSyncTestHelper.GetCursorAsync(connectionString, job.Entity, mainSource);
        Skip.If(cursor is null,
            $"v2.SyncState cursor for {job.Entity}/{mainSource} is not initialized.");
    }

    private sealed record SyncStimulusCase(
        ISnapshotSyncJob Job,
        string LegacyTable,
        string LegacyColumn,
        string SnapshotColumn,
        Func<object?, object> CreateStimulusValue,
        Func<object?, object> ParseLegacyValue,
        Func<object, object> FormatLegacyValue);
}

internal static class SnapshotSyncTestHelper
{
    public static SnapshotSyncExecutor CreateExecutor(string connectionString)
    {
        var databaseOptions = Options.Create(new DatabaseOptions { MdmDb = connectionString });
        var syncOptions = Options.Create(new SnapshotSyncOptions { CommandTimeoutSeconds = 300 });

        return new SnapshotSyncExecutor(
            databaseOptions,
            syncOptions,
            new SyncStateStore(),
            NullLogger<SnapshotSyncExecutor>.Instance);
    }

    public static async Task<long> PickNonArchiveEntityKeyAsync(
        string connectionString,
        string snapshotTable,
        string entityKeyColumn)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        var sql = $"""
            SELECT TOP (1) {QuoteIdentifier(entityKeyColumn)}
            FROM {snapshotTable}
            WHERE IsArchive = 0
            ORDER BY {QuoteIdentifier(entityKeyColumn)}
            """;

        var key = await connection.QuerySingleOrDefaultAsync<long?>(sql);
        key.Should().NotBeNull($"non-archive row required in {snapshotTable}");
        return key!.Value;
    }

    public static async Task<byte[]?> GetCursorAsync(
        string connectionString,
        string entity,
        string sourceName)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<byte[]?>(
            "v2.SyncState_Get",
            new { Entity = entity, SourceName = sourceName },
            commandType: CommandType.StoredProcedure);
    }

    public static async Task<int?> GetLastAffectedCountAsync(
        string connectionString,
        string entity,
        string sourceName)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<int?>(
            """
            SELECT LastAffectedCount
            FROM v2.SyncState
            WHERE Entity = @Entity AND SourceName = @SourceName
            """,
            new { Entity = entity, SourceName = sourceName });
    }

    public static async Task<object?> GetLegacyColumnValueAsync(
        string connectionString,
        string legacyTable,
        string legacyColumn,
        long entityKey)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        var sql = $"""
            SELECT {QuoteIdentifier(legacyColumn)}
            FROM {legacyTable}
            WHERE PrimitiveEntityItemId = @EntityKey
            """;

        return await connection.ExecuteScalarAsync<object?>(sql, new { EntityKey = entityKey });
    }

    public static async Task UpdateLegacyColumnAsync(
        string connectionString,
        string legacyTable,
        string legacyColumn,
        long entityKey,
        object value)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        var sql = $"""
            UPDATE {legacyTable}
            SET {QuoteIdentifier(legacyColumn)} = @Value
            WHERE PrimitiveEntityItemId = @EntityKey
            """;

        var affected = await connection.ExecuteAsync(sql, new { EntityKey = entityKey, Value = value });
        affected.Should().Be(1, $"legacy update must touch exactly one row in {legacyTable}");
    }

    public static async Task<object?> GetSnapshotColumnValueAsync(
        string connectionString,
        string snapshotTable,
        string entityKeyColumn,
        long entityKey,
        string column)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        var sql = $"""
            SELECT {QuoteIdentifier(column)}
            FROM {snapshotTable}
            WHERE {QuoteIdentifier(entityKeyColumn)} = @EntityKey
            """;

        return await connection.ExecuteScalarAsync<object?>(sql, new { EntityKey = entityKey });
    }

    public static async Task<object?> GetSourceColumnValueAsync(
        string connectionString,
        string sourceView,
        string entityKeyColumn,
        long entityKey,
        string column)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync();

        var sql = $"""
            SELECT {QuoteIdentifier(column)}
            FROM {sourceView}
            WHERE {QuoteIdentifier(entityKeyColumn)} = @EntityKey
            """;

        return await connection.ExecuteScalarAsync<object?>(sql, new { EntityKey = entityKey });
    }

    public static async Task AssertSnapshotMatchesSourceAsync(
        string connectionString,
        ISnapshotSyncJob job,
        long entityKey,
        string column)
    {
        var snapshotValue = await GetSnapshotColumnValueAsync(
            connectionString,
            job.SnapshotTable,
            job.EntityKeyColumn,
            entityKey,
            column);

        var sourceValue = await GetSourceColumnValueAsync(
            connectionString,
            job.SourceProjectionView,
            job.EntityKeyColumn,
            entityKey,
            column);

        NormalizeValue(snapshotValue).Should().Be(
            NormalizeValue(sourceValue),
            $"before stimulus, {job.Entity} snapshot and source must match on {column}");
    }

    private static object? NormalizeValue(object? value) =>
        value switch
        {
            null => null,
            decimal d => decimal.Round(d, 4),
            double d => decimal.Round((decimal)d, 4),
            float f => decimal.Round((decimal)f, 4),
            _ => value
        };

    private static string QuoteIdentifier(string identifier) =>
        $"[{identifier.Replace("]", "]]", StringComparison.Ordinal)}]";
}
