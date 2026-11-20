using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.ReportConstructor.Sync;

/// <summary>
/// Описание синхронизации справочника ReportConstructor legacy -> v2 snapshot.
/// Механику выполняет <see cref="SnapshotSyncExecutor"/> через generic-процедуры
/// v2.Sync_*; детекцию/каскад — процедура v2.ReportConstructor_PopulateAffectedKeys.
/// Здесь только описание snapshot, проекции и списка источников изменений.
///
/// ReportDictionaryTypes (2129), TransportKind (2008) и TransportType_level_3 (2023)
/// стабильны и не отслеживаются. ProxyNode в снимок не входит.
/// </summary>
public sealed class ReportConstructorSyncJob() : SnapshotSyncJob(
    "ReportConstructor",
    "dbo.PrimitiveEntityData_2130", // ReportConstructor (основная)
    "dbo.PrimitiveEntityData_1014", // LocationsNodes
    "dbo.PrimitiveEntityData_1013", // ProductGroup
    "dbo.PrimitiveEntityData_1015") // MTR (Product)
{
}
