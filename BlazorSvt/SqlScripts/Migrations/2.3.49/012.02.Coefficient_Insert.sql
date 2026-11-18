USE [mdm];
GO

/*
    Первичная полная заливка snapshot Coefficient из v2.vw_Coefficient_SnapshotSource
    (тот же SELECT, что у инкрементальной синхронизации).

    Требует Programmability/vw_Coefficient_SnapshotSource.sql.
*/

INSERT INTO v2.Coefficient_Snapshot (
     CoefficientId
    ,Code
    ,IsArchive
    ,CoefficientTypeId
    ,CoefficientTypeCode
    ,CoefficientTypeNameRu
    ,Name
    ,StartDate
    ,FinishDate
    ,Active
    ,OverallValue
    ,RateTypeId
    ,RateTypeCode
    ,RateTypeNameRu
    ,CreationDate
    ,LastChangeDate
)
SELECT
     CoefficientId
    ,Code
    ,IsArchive
    ,CoefficientTypeId
    ,CoefficientTypeCode
    ,CoefficientTypeNameRu
    ,Name
    ,StartDate
    ,FinishDate
    ,Active
    ,OverallValue
    ,RateTypeId
    ,RateTypeCode
    ,RateTypeNameRu
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Coefficient_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2081'   -- Coefficient (основная)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'Coefficient' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
