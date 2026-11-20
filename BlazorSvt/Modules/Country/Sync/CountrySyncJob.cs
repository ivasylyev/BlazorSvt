using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.Country.Sync;

/// <summary>
/// Описание синхронизации справочника Country legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию — процедура v2.Country_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// StatusElement (dbo.PrimitiveEntityData_1010) — короткий словарь, в гриде enum;
/// не отслеживается. RowVer на dbo.PrimitiveEntityData_1009 уже есть.
/// </summary>
public sealed class CountrySyncJob() : SnapshotSyncJob(
    "Country",
    "dbo.PrimitiveEntityData_1009") // Country (основная)
{
}
