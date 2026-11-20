using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.RateAutoSelectionSetting.Sync;

/// <summary>
/// Синхронизация RateAutoSelectionSetting: таблица dbo.PrimitiveEntityData_2101.
/// Каскада нет.
/// </summary>
public sealed class RateAutoSelectionSettingSyncJob() : SnapshotSyncJob(
    "RateAutoSelectionSetting",
    "dbo.PrimitiveEntityData_2101") // RateAutoSelectionSetting (основная)
{
}
