USE [mdm];
GO

/*
    Первичная полная заливка snapshot TransportLeg.

    Источник — проекция v2.vw_TransportLeg_SnapshotSource (тот же SELECT, что
    использует инкрементальная синхронизация), поэтому полная пересборка и
    инкремент дают идентичный результат.

    Требует предварительно созданной вью:
        Programmability/vw_TransportLeg_SnapshotSource.sql
*/

INSERT INTO v2.TransportLeg_Snapshot (
     TransportLegId
    ,Code

    ,IsArchive
    ,CanBeUsed

    ,ShipmentTypeId

    ,TransportKindId
    ,TransportKindCode

    ,SearchTimeT
    ,LoadTimeT
    ,TravelTimeT
    ,DaysWaitingT
    ,UnLoadTimeT
    ,TransportationTimeT

    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,RegionFromCode
    ,RegionFromNameEn
    ,RegionFromNameRu

    ,ProxyNodeCode
    ,ProxyNodeNameEn
    ,ProxyNodeNameRu
    ,ProxyRegionCode
    ,ProxyRegionNameEn
    ,ProxyRegionNameRu

    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,RegionToCode
    ,RegionToNameEn
    ,RegionToNameRu

    ,CreationDate
    ,LastChangeDate

    ,NodeFromId
    ,NodeToId
    ,ProxyNodeId
    ,RegionFromId
    ,RegionToId
    ,ProxyRegionId
)
SELECT
     TransportLegId
    ,Code

    ,IsArchive
    ,CanBeUsed

    ,ShipmentTypeId

    ,TransportKindId
    ,TransportKindCode

    ,SearchTimeT
    ,LoadTimeT
    ,TravelTimeT
    ,DaysWaitingT
    ,UnLoadTimeT
    ,TransportationTimeT

    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,RegionFromCode
    ,RegionFromNameEn
    ,RegionFromNameRu

    ,ProxyNodeCode
    ,ProxyNodeNameEn
    ,ProxyNodeNameRu
    ,ProxyRegionCode
    ,ProxyRegionNameEn
    ,ProxyRegionNameRu

    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,RegionToCode
    ,RegionToNameEn
    ,RegionToNameRu

    ,CreationDate
    ,LastChangeDate

    ,NodeFromId
    ,NodeToId
    ,ProxyNodeId
    ,RegionFromId
    ,RegionToId
    ,ProxyRegionId
FROM v2.vw_TransportLeg_SnapshotSource;
GO

/*
    Инициализация курсоров синхронизации на текущую границу версий.
    После полной заливки инкремент должен подхватывать только изменения,
    случившиеся ПОСЛЕ первичной загрузки.

    @Hi = наибольшая гарантированно закоммиченная версия
        = MIN_ACTIVE_ROWVERSION() - 1.
*/
IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2007'   -- TransportLeg (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1008'  -- Region
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'TransportLeg' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
