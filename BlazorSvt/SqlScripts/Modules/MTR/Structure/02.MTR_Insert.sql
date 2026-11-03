USE [mdm];
GO

/*
    Первичная полная заливка snapshot MTR.

    Источник — проекция v2.vw_MTR_SnapshotSource (тот же SELECT, что
    использует инкрементальная синхронизация), поэтому полная пересборка и
    инкремент дают идентичный результат.

    Требует предварительно созданной вью:
        Programmability/vw_MTR_SnapshotSource.sql
*/

INSERT INTO v2.MTR_Snapshot (
     MTRId
    ,Code
    ,IsArchive

    ,NameFullRu
    ,NameShortEn

    ,ProductGroupId
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn

    ,EtsngId
    ,EtsngNameRu
    ,EtsngNameEn

    ,GngCode

    ,UnitId
    ,UnitCode
    ,UnitNameRu
    ,UnitNameEn

    ,DpoCode
    ,MtrType

    ,DangerClassId
    ,DangerClassCode
    ,DangerClassNameRu

    ,CreationDate
    ,LastChangeDate
)
SELECT
     MTRId
    ,Code
    ,IsArchive

    ,NameFullRu
    ,NameShortEn

    ,ProductGroupId
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn

    ,EtsngId
    ,EtsngNameRu
    ,EtsngNameEn

    ,GngCode

    ,UnitId
    ,UnitCode
    ,UnitNameRu
    ,UnitNameEn

    ,DpoCode
    ,MtrType

    ,DangerClassId
    ,DangerClassCode
    ,DangerClassNameRu

    ,CreationDate
    ,LastChangeDate
FROM v2.vw_MTR_SnapshotSource;
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
        SELECT N'dbo.PrimitiveEntityData_1015'   -- MTR (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1013'  -- ProductGroup
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'MTR' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
