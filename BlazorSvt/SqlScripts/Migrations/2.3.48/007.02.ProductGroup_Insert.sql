USE [mdm];
GO

/*
    Первичная полная заливка snapshot ProductGroup.

    Источник — проекция v2.vw_ProductGroup_SnapshotSource (тот же SELECT, что
    использует инкрементальная синхронизация), поэтому полная пересборка и
    инкремент дают идентичный результат.

    Требует предварительно созданной вью:
        Programmability/vw_ProductGroup_SnapshotSource.sql
*/

INSERT INTO v2.ProductGroup_Snapshot (
     ProductGroupId
    ,Code
    ,IsArchive

    ,Name
    ,ShortName
    ,GroupCode
    ,ProductGroupCBD
    ,NameEnCode
    ,NameEnRu
    ,Code_NSI
    ,SVTUseProductGroupWhenNoProductRateExists
    ,NameEn

    ,RateSyncClassId
    ,RateSyncClassCode
    ,RateSyncClassNameRu

    ,CreationDate
    ,LastChangeDate
)
SELECT
     ProductGroupId
    ,Code
    ,IsArchive

    ,Name
    ,ShortName
    ,GroupCode
    ,ProductGroupCBD
    ,NameEnCode
    ,NameEnRu
    ,Code_NSI
    ,SVTUseProductGroupWhenNoProductRateExists
    ,NameEn

    ,RateSyncClassId
    ,RateSyncClassCode
    ,RateSyncClassNameRu

    ,CreationDate
    ,LastChangeDate
FROM v2.vw_ProductGroup_SnapshotSource;
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
        SELECT N'dbo.PrimitiveEntityData_1013'   -- ProductGroup (основная)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'ProductGroup' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
