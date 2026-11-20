using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportRoute.Sync;

/// <summary>
/// Описание синхронизации справочника TransportRoute legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.TransportRoute_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// RowVer основной таблицы добавляется скриптом SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
/// TransportKind (2008) и TransportType_level_3 (2023) стабильны и в каскад не входят.
/// TransportLeg в снимок не проецируется.
/// </summary>
public sealed class TransportRouteSyncJob() : SnapshotSyncJob(
    "TransportRoute",
    "dbo.PrimitiveEntityData_2010", // TransportRoute (основная)
    "dbo.PrimitiveEntityData_1014", // LocationsNodes
    "dbo.PrimitiveEntityData_1008") // Region
{
}
