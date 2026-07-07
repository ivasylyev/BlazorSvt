using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportRate.Sync;

/// <summary>
/// Описание синхронизации справочника TransportRate legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.TransportRate_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Базовые таблицы (dbo.PrimitiveEntityData_*) и их назначение взяты из
/// stg.LoadTransportRate. RowVer на каждой из них добавляется скриптом
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
///
/// ВНИМАНИЕ: членство строки в snapshot зависит от WHERE-фильтра проекции
/// (rt.Code / p.Code), поэтому источник MTR (1015) обязателен в каскаде —
/// иначе вход/выход строки отследит только reconcile.
/// RateType (2048) стабилен и не отслеживается.
/// </summary>
public sealed class TransportRateSyncJob : ISnapshotSyncJob
{
    // Базовые таблицы legacy для каждого справочника-источника
    // (имя = ключ v2.SyncState.SourceName и значение @Source процедуры детекции).
    private const string RateTable = "dbo.PrimitiveEntityData_2012";         // TransportRate (основная)
    private const string NodeTable = "dbo.PrimitiveEntityData_1014";       // LocationsNodes
    private const string ProductGroupTable = "dbo.PrimitiveEntityData_1013"; // ProductGroup
    private const string ProductTable = "dbo.PrimitiveEntityData_1015";    // MTR (Product)

    public string Entity => "TransportRate";

    public string SnapshotTable => "v2.TransportRate_Snapshot";

    public string EntityKeyColumn => "TransportRateId";

    public string SourceProjectionView => "v2.vw_TransportRate_SnapshotSource";

    public string PopulateAffectedKeysProc => "v2.TransportRate_PopulateAffectedKeys";

    public IReadOnlyList<SnapshotSyncSource> Sources { get; } = new List<SnapshotSyncSource>
    {
        new(RateTable),         // основная: изменившиеся рейты
        new(NodeTable),         // каскад: NodeFrom/To/Proxy
        new(ProductGroupTable), // каскад: ProductGroupId
        new(ProductTable),      // каскад: ProductId (важно для WHERE-членства)
    };
}
