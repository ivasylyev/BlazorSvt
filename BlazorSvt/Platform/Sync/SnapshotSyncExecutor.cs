using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Универсальная механика инкрементальной синхронизации legacy -> v2 snapshot.
/// Одинакова для всех справочников; специфика описана в <see cref="ISnapshotSyncJob"/>.
///
/// C# отвечает только за оркестрацию цикла (границы, seed, продвижение курсоров,
/// изоляция ошибок); вся SQL-логика вынесена в generic-процедуры v2.Sync_* и
/// per-entity процедуру детекции. Все обращения к БД идут через
/// <see cref="DbConnectionLogDecorator"/> (единое логирование/тайминги).
///
/// Один цикл (на справочник, одно соединение):
///   1. @Hi = v2.Sync_GetHighWatermark — наибольшая гарантированно закоммиченная
///      версия (строки незавершённых транзакций исключаются -> нет пропусков и
///      не нужен RCSI).
///   2. Для каждого источника по его курсору @Lo и общему @Hi процедура детекции
///      добавляет в #AffectedKeys бизнес-ключи затронутых записей (основная
///      таблица + каскад).
///   3. v2.Sync_UpsertAffected — партиционно-безопасный upsert из проекции.
///   4. Продвигаем курсоры всех источников на @Hi (идемпотентно при повторе).
/// Чтение источников — NOLOCK; запись — только в v2.* -> писателей legacy не блокируем.
/// </summary>
public sealed class SnapshotSyncExecutor(
    IOptions<DatabaseOptions> databaseOptions,
    IOptions<SnapshotSyncOptions> syncOptions,
    SyncStateStore stateStore,
    ILogger<SnapshotSyncExecutor> logger)
{
    private const string GetHighWatermarkProc = "v2.Sync_GetHighWatermark";
    private const string UpsertAffectedProc = "v2.Sync_UpsertAffected";
    private const string ReconcileProc = "v2.Sync_Reconcile";

    private readonly string connectionString = databaseOptions.Value.MdmDb;
    private readonly int commandTimeout = syncOptions.Value.CommandTimeoutSeconds;

    /// <summary>Один инкрементальный цикл для справочника.</summary>
    public async Task RunIncrementalAsync(ISnapshotSyncJob job, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureSqlSessionSettingsAsync(connection, cancellationToken);
        var db = new DbConnectionLogDecorator(connection, logger, commandTimeout);

        var hi = await GetHighWatermarkAsync(db, cancellationToken);
        var hiValue = RowVersionToUInt64(hi);

        var sourcesToProcess = new List<(SnapshotSyncSource Source, byte[] Cursor)>();

        foreach (var source in job.Sources)
        {
            var cursor = await stateStore.GetCursorAsync(db, job.Entity, source.Name);

            if (cursor is null)
            {
                // Курсора нет -> справочник ещё не проинициализирован.
                // Считаем, что первичная заливка (02.*_Insert) выполнена,
                // и сдвигаем курсор на текущую границу без обработки истории.
                await stateStore.UpsertCursorAsync(
                    db, job.Entity, source.Name, hi, affectedCount: null, cancellationToken);

                logger.LogInformation(
                    "Sync {Entity}/{Source}: курсор инициализирован на текущую границу (seed).",
                    job.Entity, source.Name);

                continue;
            }

            var lag = hiValue - RowVersionToUInt64(cursor);
            if (lag > 0)
            {
                logger.LogDebug(
                    "Sync {Entity}/{Source}: cursor lag {CursorLag} (Lo={Lo:X16}, Hi={Hi:X16}).",
                    job.Entity, source.Name, lag, RowVersionToUInt64(cursor), hiValue);
            }

            sourcesToProcess.Add((source, cursor));
        }

        if (sourcesToProcess.Count == 0)
        {
            logger.LogDebug(
                "Sync {Entity}: incremental skipped (no sources to process) in {ElapsedMs} ms.",
                job.Entity, stopwatch.ElapsedMilliseconds);
            return;
        }

        await CreateAffectedKeysTableAsync(db, cancellationToken);

        try
        {
            ulong maxLag = 0;

            foreach (var (source, cursor) in sourcesToProcess)
            {
                maxLag = Math.Max(maxLag, hiValue - RowVersionToUInt64(cursor));

                var parameters = new DynamicParameters();
                parameters.Add("Source", source.Name);
                parameters.Add("Lo", cursor, DbType.Binary, size: 8);
                parameters.Add("Hi", hi, DbType.Binary, size: 8);

                await db.ExecuteAsync(
                    job.PopulateAffectedKeysProc,
                    parameters,
                    CommandType.StoredProcedure,
                    cancellationToken);
            }

            var affected = await UpsertAffectedAsync(db, job, cancellationToken);

            foreach (var (source, _) in sourcesToProcess)
            {
                await stateStore.UpsertCursorAsync(
                    db, job.Entity, source.Name, hi, affected, cancellationToken);
            }

            if (affected > 0)
            {
                logger.LogInformation(
                    "Sync {Entity}: обновлено {Affected} строк snapshot, maxCursorLag {CursorLag}, {ElapsedMs} ms.",
                    job.Entity, affected, maxLag, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogDebug(
                    "Sync {Entity}: incremental idle (0 rows), maxCursorLag {CursorLag}, {ElapsedMs} ms.",
                    job.Entity, maxLag, stopwatch.ElapsedMilliseconds);
            }
        }
        finally
        {
            await DropAffectedKeysTableAsync(db, cancellationToken);
        }
    }

    /// <summary>
    /// Reconciliation: удаляет из snapshot "фантомы" — записи, физически удалённые
    /// в legacy (их нет в проекции). Ловит то, что инкремент по rowversion не видит
    /// (жёсткие delete). Запускается редко (по умолчанию раз в сутки, ночью).
    /// </summary>
    public async Task RunReconcileAsync(ISnapshotSyncJob job, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        await EnsureSqlSessionSettingsAsync(connection, cancellationToken);
        var db = new DbConnectionLogDecorator(connection, logger, commandTimeout);

        var parameters = new DynamicParameters();
        parameters.Add("SnapshotTable", job.SnapshotTable);
        parameters.Add("SourceView", job.SourceProjectionView);
        parameters.Add("KeyColumn", job.EntityKeyColumn);

        var deleted = await db.QuerySingleAsync<int>(
            ReconcileProc, parameters, CommandType.StoredProcedure, cancellationToken);

        await stateStore.MarkReconciledAsync(db, job.Entity, cancellationToken);

        logger.LogInformation(
            "Reconcile {Entity}: удалено фантомов {Deleted} за {ElapsedMs} ms.",
            job.Entity, deleted, stopwatch.ElapsedMilliseconds);
    }

    private Task<byte[]> GetHighWatermarkAsync(
        DbConnectionLogDecorator db, CancellationToken cancellationToken) =>
        db.QuerySingleAsync<byte[]>(
            GetHighWatermarkProc, new DynamicParameters(), CommandType.StoredProcedure, cancellationToken);

    private Task<int> UpsertAffectedAsync(
        DbConnectionLogDecorator db, ISnapshotSyncJob job, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("SnapshotTable", job.SnapshotTable);
        parameters.Add("SourceView", job.SourceProjectionView);
        parameters.Add("KeyColumn", job.EntityKeyColumn);

        return db.QuerySingleAsync<int>(
            UpsertAffectedProc, parameters, CommandType.StoredProcedure, cancellationToken);
    }

    private Task CreateAffectedKeysTableAsync(
        DbConnectionLogDecorator db, CancellationToken cancellationToken) =>
        db.ExecuteAsync(
            "IF OBJECT_ID('tempdb..#AffectedKeys') IS NOT NULL DROP TABLE #AffectedKeys;" +
            "CREATE TABLE #AffectedKeys (EntityKey BIGINT NOT NULL PRIMARY KEY);",
            new DynamicParameters(),
            CommandType.Text,
            cancellationToken);

    private Task DropAffectedKeysTableAsync(
        DbConnectionLogDecorator db, CancellationToken cancellationToken) =>
        db.ExecuteAsync(
            "IF OBJECT_ID('tempdb..#AffectedKeys') IS NOT NULL DROP TABLE #AffectedKeys;",
            new DynamicParameters(),
            CommandType.Text,
            cancellationToken);

    private SqlConnection OpenConnection()
    {
#pragma warning disable CS0618 // System.Data.SqlClient — как в остальном коде проекта
        return new SqlConnection(connectionString);
#pragma warning restore CS0618
    }

    /// <summary>
    /// Проекции snapshot join'ят indexed views legacy — без этих SET MERGE падает.
    /// </summary>
    private static async Task EnsureSqlSessionSettingsAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            new CommandDefinition(
                "SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; SET ANSI_WARNINGS ON;",
                cancellationToken: cancellationToken));
    }

    /// <summary>SQL rowversion — big-endian 8 байт.</summary>
    internal static ulong RowVersionToUInt64(byte[] value)
    {
        if (value.Length == 0)
        {
            return 0;
        }

        ulong result = 0;
        foreach (var b in value)
        {
            result = (result << 8) | b;
        }

        return result;
    }
}
