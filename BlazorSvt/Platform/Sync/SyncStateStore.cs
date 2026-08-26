using System.Data;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Доступ к таблице-курсору v2.SyncState через generic-процедуры v2.SyncState_*.
/// Все методы работают на переданном декораторе (обёртка над уже открытым
/// соединением), чтобы разделять сессию с временной таблицей и upsert.
/// </summary>
public sealed class SyncStateStore
{
    private const string GetProc = "v2.SyncState_Get";
    private const string UpsertProc = "v2.SyncState_Upsert";
    private const string MarkReconciledProc = "v2.SyncState_MarkReconciled";

    /// <summary>Нулевой курсор (обработка "с самого начала"), если строки ещё нет.</summary>
    public static readonly byte[] ZeroCursor = new byte[8];

    /// <summary>
    /// Возвращает сохранённый курсор источника или <c>null</c>, если его ещё нет
    /// (справочник не проинициализирован — требуется первичная заливка/seed).
    /// </summary>
    public Task<byte[]?> GetCursorAsync(
        DbConnectionLogDecorator db,
        string entity,
        string sourceName)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Entity", entity);
        parameters.Add("SourceName", sourceName);

        return db.QuerySingleOrDefaultAsync<byte[]?>(GetProc, parameters, CommandType.StoredProcedure);
    }

    /// <summary>Создаёт/обновляет курсор источника после успешной обработки.</summary>
    public Task UpsertCursorAsync(
        DbConnectionLogDecorator db,
        string entity,
        string sourceName,
        byte[] rowVersion,
        int? affectedCount,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Entity", entity);
        parameters.Add("SourceName", sourceName);
        parameters.Add("RowVersion", rowVersion, DbType.Binary, size: 8);
        parameters.Add("AffectedCount", affectedCount);

        return db.ExecuteAsync(UpsertProc, parameters, CommandType.StoredProcedure, cancellationToken);
    }

    /// <summary>Фиксирует время последней сверки reconciliation по всем источникам справочника.</summary>
    public Task MarkReconciledAsync(
        DbConnectionLogDecorator db,
        string entity,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Entity", entity);

        return db.ExecuteAsync(MarkReconciledProc, parameters, CommandType.StoredProcedure, cancellationToken);
    }
}
