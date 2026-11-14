using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.Contractor.Sync;

/// <summary>
/// Синхронизация Contractor: примитив LegalType, таблица dbo.PrimitiveEntityData_2028.
/// vw_Contractor устарело. Address, Status, LegalForm и LevelOrganization не отслеживаются.
/// </summary>
public sealed class ContractorSyncJob() : SnapshotSyncJob(
    "Contractor",
    "dbo.PrimitiveEntityData_2028") // LegalType (основная), реестр Contractor
{
}
