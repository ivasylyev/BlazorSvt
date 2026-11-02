using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.MTR.Sync;

/// <summary>
/// Описание синхронизации справочника MTR legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию — процедура v2.MTR_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Unit, DangerClass и StatusElement в гриде — enum, не отслеживаются.
/// RowVer на каждой таблице добавляет SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
/// </summary>
public sealed class MTRSyncJob() : SnapshotSyncJob(
    "MTR",
    "dbo.PrimitiveEntityData_1015", // MTR (основная)
    "dbo.PrimitiveEntityData_1013", // ProductGroup
    "dbo.PrimitiveEntityData_2019") // ETSNG
{
}
