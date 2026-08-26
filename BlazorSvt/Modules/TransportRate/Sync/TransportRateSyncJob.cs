using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportRate.Sync;

/// <summary>
/// Описание синхронизации справочника TransportRate legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.TransportRate_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// Базовые таблицы (dbo.PrimitiveEntityData_*) и их назначение взяты из
/// stg.LoadTransportRate. RowVer на каждой из них добавляется скриптом
/// SqlScripts/Sync/01.Legacy_AddRowVersion.sql.
///
/// ВНИМАНИЕ: членство строки в snapshot зависит от WHERE-фильтра проекции
/// (rt.Code / p.Code), поэтому источник MTR (1015) обязателен в каскаде —
/// иначе вход/выход строки отследит только reconcile.
/// RateType (2048) стабилен и не отслеживается.
/// </summary>
public sealed class TransportRateSyncJob() : SnapshotSyncJob(
    "TransportRate",
    "dbo.PrimitiveEntityData_2012", // TransportRate (основная)
    "dbo.PrimitiveEntityData_1014", // LocationsNodes
    "dbo.PrimitiveEntityData_1013", // ProductGroup
    "dbo.PrimitiveEntityData_1015") // MTR (Product)
{
}
