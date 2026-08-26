USE [mdm];
GO

/*
    Первичная полная заливка snapshot LocationsNodes.

    Источник — проекция v2.vw_LocationsNodes_SnapshotSource (тот же SELECT, что
    использует инкрементальная синхронизация), поэтому полная пересборка и
    инкремент дают идентичный результат.

    Требует предварительно созданной вью:
        Programmability/vw_LocationsNodes_SnapshotSource.sql
*/

INSERT INTO v2.LocationsNodes_Snapshot (
     LocationsNodesId
    ,Code
    ,IsArchive

    ,NameRu
    ,NameEn

    ,LocationTypeId
    ,LocationTypeCode
    ,LocationTypeNameRu
    ,LocationTypeNameEn

    ,TypeNodeId
    ,TypeNodeCode
    ,TypeNodeNameRu
    ,TypeNodeNameEn

    ,RegionId
    ,RegionCode
    ,RegionNameRu
    ,RegionNameEn
    ,RegionRU

    ,CountryId
    ,CountryNameRu
    ,CountryNameEn

    ,CreationDate
    ,LastChangeDate
)
SELECT
     LocationsNodesId
    ,Code
    ,IsArchive

    ,NameRu
    ,NameEn

    ,LocationTypeId
    ,LocationTypeCode
    ,LocationTypeNameRu
    ,LocationTypeNameEn

    ,TypeNodeId
    ,TypeNodeCode
    ,TypeNodeNameRu
    ,TypeNodeNameEn

    ,RegionId
    ,RegionCode
    ,RegionNameRu
    ,RegionNameEn
    ,RegionRU

    ,CountryId
    ,CountryNameRu
    ,CountryNameEn

    ,CreationDate
    ,LastChangeDate
FROM v2.vw_LocationsNodes_SnapshotSource;
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
        SELECT N'dbo.PrimitiveEntityData_1014'   -- LocationsNodes (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1008'  -- Region
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1009'  -- Country
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'LocationsNodes' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
