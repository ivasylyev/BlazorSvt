using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportLeg.Sync;

/// <summary>
/// Описание синхронизации справочника TransportLeg legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.TransportLeg_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Базовые таблицы (dbo.PrimitiveEntityData_*) и их назначение взяты из
/// stg.LoadTransportLeg. RowVer на каждой из них добавляется скриптом
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
/// </summary>
public sealed class TransportLegSyncJob : ISnapshotSyncJob
{
    // Базовые таблицы legacy для каждого справочника-источника
    // (имя = ключ v2.SyncState.SourceName и значение @Source процедуры детекции).
    private const string LegTable = "dbo.PrimitiveEntityData_2007";    // TransportLeg (основная)
    private const string NodeTable = "dbo.PrimitiveEntityData_1014";   // LocationsNodes
    private const string RegionTable = "dbo.PrimitiveEntityData_1008"; // Region

    public string Entity => "TransportLeg";

    public string SnapshotTable => "v2.TransportLeg_Snapshot";

    public string EntityKeyColumn => "TransportLegId";

    public string SourceProjectionView => "v2.vw_TransportLeg_SnapshotSource";

    public string PopulateAffectedKeysProc => "v2.TransportLeg_PopulateAffectedKeys";

    public IReadOnlyList<SnapshotSyncSource> Sources { get; } = new List<SnapshotSyncSource>
    {
        new(LegTable),    // основная: изменившиеся плечи
        new(RegionTable), // каскад: RegionFrom/To/Proxy
        new(NodeTable),   // каскад: NodeFrom/To/Proxy
    };
}
