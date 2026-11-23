USE [mdm];
GO

/*
    Первичная полная заливка snapshot Region
    из v2.vw_Region_SnapshotSource.

    RowVer на dbo.PrimitiveEntityData_1008 и dbo.PrimitiveEntityData_1009 уже есть.
    StatusElement (1010) и Federal_Districts (2120) в курсор не входят.
*/

INSERT INTO v2.Region_Snapshot (
     RegionCode
    ,Code_NSI
    ,RegionCodePlanning
    ,RegionCodeRF
    ,NameRu
    ,NameEn
    ,CountryNameRu
    ,CountryNameEn
    ,CountryId
    ,Market
    ,StatusId
    ,FederalDistrictsId
    ,FederalDistrictsCode
    ,FederalDistrictsNameRu
    ,FederalDistrictsNameEn
    ,RegionId
    ,Code
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
)
SELECT
     RegionCode
    ,Code_NSI
    ,RegionCodePlanning
    ,RegionCodeRF
    ,NameRu
    ,NameEn
    ,CountryNameRu
    ,CountryNameEn
    ,CountryId
    ,Market
    ,StatusId
    ,FederalDistrictsId
    ,FederalDistrictsCode
    ,FederalDistrictsNameRu
    ,FederalDistrictsNameEn
    ,RegionId
    ,Code
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Region_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_1008'   -- Region (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1009'  -- Country
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'Region' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
