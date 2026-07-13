using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.AverageRateLevel3.Sync;

/// <summary>
/// Описание синхронизации справочника AverageRateLevel3 legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.AverageRateLevel3_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Базовые таблицы (dbo.PrimitiveEntityData_*) и их назначение —
/// по аналогии с TransportRate. RowVer на каждой из них добавляется скриптом
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
///
/// ВНИМАНИЕ: членство строки в snapshot зависит от WHERE-фильтра проекции
/// (rt.Code / p.Code), поэтому источник MTR (1015) обязателен в каскаде —
/// иначе вход/выход строки отследит только reconcile.
/// RateType (2048) стабилен и не отслеживается.
/// RatesOfAverageRate (2058) и TransportRate (2012) не отслеживаются.
/// </summary>
public sealed class AverageRateLevel3SyncJob : ISnapshotSyncJob
{
    // Базовые таблицы legacy для каждого справочника-источника
    // (имя = ключ v2.SyncState.SourceName и значение @Source процедуры детекции).
    private const string AverageRateTable = "dbo.PrimitiveEntityData_2057"; // AverageRateLevel3 (основная)
    private const string NodeTable = "dbo.PrimitiveEntityData_1014";       // LocationsNodes
    private const string ProductGroupTable = "dbo.PrimitiveEntityData_1013"; // ProductGroup
    private const string ProductTable = "dbo.PrimitiveEntityData_1015";    // MTR (Product)

    public string Entity => "AverageRateLevel3";

    public string SnapshotTable => "v2.AverageRateLevel3_Snapshot";

    public string EntityKeyColumn => "AverageRateLevel3Id";

    public string SourceProjectionView => "v2.vw_AverageRateLevel3_SnapshotSource";

    public string PopulateAffectedKeysProc => "v2.AverageRateLevel3_PopulateAffectedKeys";

    public IReadOnlyList<SnapshotSyncSource> Sources { get; } = new List<SnapshotSyncSource>
    {
        new(AverageRateTable),  // основная: изменившиеся средние
        new(NodeTable),         // каскад: NodeFrom/To/Proxy
        new(ProductGroupTable), // каскад: ProductGroupId
        new(ProductTable),      // каскад: ProductId (важно для WHERE-членства)
    };
}
