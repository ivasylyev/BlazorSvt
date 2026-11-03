using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.ProductGroup.Sync;

/// <summary>
/// Описание синхронизации справочника ProductGroup legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию — процедура v2.ProductGroup_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// RateSyncClass (dbo.PrimitiveEntityData_2127) — короткий словарь, в гриде enum;
/// не отслеживается. RowVer на основной таблице добавляет
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
/// </summary>
public sealed class ProductGroupSyncJob() : SnapshotSyncJob(
    "ProductGroup",
    "dbo.PrimitiveEntityData_1013") // ProductGroup (основная)
{
}
