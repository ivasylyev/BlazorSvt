using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.LocationsNodes.Sync;

/// <summary>
/// Описание синхронизации справочника LocationsNodes legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.LocationsNodes_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Базовые таблицы (dbo.PrimitiveEntityData_*) и их назначение взяты из
/// stg.LoadLocationsNodes. RowVer на каждой из них добавляется скриптом
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
/// </summary>
public sealed class LocationsNodesSyncJob : ISnapshotSyncJob
{
    // Базовые таблицы legacy для каждого справочника-источника
    // (имя = ключ v2.SyncState.SourceName и значение @Source процедуры детекции).
    private const string NodeTable = "dbo.PrimitiveEntityData_1014";   // LocationsNodes (основная)
    private const string RegionTable = "dbo.PrimitiveEntityData_1008"; // Region
    private const string CountryTable = "dbo.PrimitiveEntityData_1009"; // Country

    public string Entity => "LocationsNodes";

    public string SnapshotTable => "v2.LocationsNodes_Snapshot";

    public string EntityKeyColumn => "LocationsNodesId";

    public string SourceProjectionView => "v2.vw_LocationsNodes_SnapshotSource";

    public string PopulateAffectedKeysProc => "v2.LocationsNodes_PopulateAffectedKeys";

    public IReadOnlyList<SnapshotSyncSource> Sources { get; } = new List<SnapshotSyncSource>
    {
        new(NodeTable),    // основная: изменившиеся узлы
        new(RegionTable),  // каскад: RegionId
        new(CountryTable), // каскад: CountryId
    };
}
