using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.Coefficient.Sync;

/// <summary>
/// Синхронизация Coefficient: таблица dbo.PrimitiveEntityData_2081.
/// CoefficientType (2077) и RateType (2048) — короткие словари с enum, не отслеживаются.
/// Дочерние CoefficientConditionValue (2080) и CoefficientComponentValue (2083) в снимок не входят.
/// </summary>
public sealed class CoefficientSyncJob() : SnapshotSyncJob(
    "Coefficient",
    "dbo.PrimitiveEntityData_2081") // Coefficient (основная)
{
}
