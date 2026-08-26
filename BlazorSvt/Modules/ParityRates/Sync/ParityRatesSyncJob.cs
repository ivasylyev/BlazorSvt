using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.ParityRates.Sync;

/// <summary>
/// Синхронизация ParityRates legacy -> v2 snapshot.
/// Источники: основная PED_2109 + LocationsNodes / ProductGroup / MTR.
/// Relevance (2108), Currency (2016), TransportType (2023) стабильны — не в каскаде.
/// </summary>
public sealed class ParityRatesSyncJob() : SnapshotSyncJob(
    "ParityRates",
    "dbo.PrimitiveEntityData_2109", // ParityRates (основная)
    "dbo.PrimitiveEntityData_1014", // LocationsNodes
    "dbo.PrimitiveEntityData_1013", // ProductGroup
    "dbo.PrimitiveEntityData_1015") // MTR (Product)
{
}
