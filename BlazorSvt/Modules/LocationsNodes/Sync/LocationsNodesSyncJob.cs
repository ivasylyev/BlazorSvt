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
public sealed class LocationsNodesSyncJob() : SnapshotSyncJob(
    "LocationsNodes",
    "dbo.PrimitiveEntityData_1014", // LocationsNodes (основная)
    "dbo.PrimitiveEntityData_1008", // Region
    "dbo.PrimitiveEntityData_1009") // Country
{
}
