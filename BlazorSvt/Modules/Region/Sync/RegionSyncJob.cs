using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.Region.Sync;

/// <summary>
/// Описание синхронизации справочника Region legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию — процедура v2.Region_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// StatusElement (dbo.PrimitiveEntityData_1010) и Federal_Districts
/// (dbo.PrimitiveEntityData_2120) в гриде — enum, не отслеживаются.
/// RowVer на основной таблице уже есть.
/// </summary>
public sealed class RegionSyncJob() : SnapshotSyncJob(
    "Region",
    "dbo.PrimitiveEntityData_1008", // Region (основная)
    "dbo.PrimitiveEntityData_1009") // Country
{
}
