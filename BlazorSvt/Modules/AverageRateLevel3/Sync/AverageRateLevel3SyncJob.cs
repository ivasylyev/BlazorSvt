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
public sealed class AverageRateLevel3SyncJob() : SnapshotSyncJob(
    "AverageRateLevel3",
    "dbo.PrimitiveEntityData_2057", // AverageRateLevel3 (основная)
    "dbo.PrimitiveEntityData_1014", // LocationsNodes
    "dbo.PrimitiveEntityData_1013", // ProductGroup
    "dbo.PrimitiveEntityData_1015") // MTR (Product)
{
}
