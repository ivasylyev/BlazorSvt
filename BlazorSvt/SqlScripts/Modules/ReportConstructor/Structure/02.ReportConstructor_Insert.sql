USE [mdm];
GO

/*
    Первичная полная заливка snapshot ReportConstructor
    из v2.vw_ReportConstructor_SnapshotSource.

    Курсор v2.SyncState ставится после RowVer на dbo.PrimitiveEntityData_2130.
    1014, 1013 и 1015 уже имеют RowVer.
*/

INSERT INTO v2.ReportConstructor_Snapshot (
     ReportConstructorId
    ,IsArchive
    ,Code
    ,ReportTypeId
    ,DateFrom
    ,DateTo
    ,NodeFromCode
    ,NodeFromNameRu
    ,NodeFromNameEn
    ,NodeFromId
    ,NodeToCode
    ,NodeToNameRu
    ,NodeToNameEn
    ,NodeToId
    ,TransportKindCode
    ,TransportKindId
    ,TransportTypeCode
    ,TransportTypeId
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductGroupId
    ,ProductCode
    ,ProductNameRu
    ,ProductNameEn
    ,ProductId
    ,CreationDate
    ,LastChangeDate
)
SELECT
     ReportConstructorId
    ,IsArchive
    ,Code
    ,ReportTypeId
    ,DateFrom
    ,DateTo
    ,NodeFromCode
    ,NodeFromNameRu
    ,NodeFromNameEn
    ,NodeFromId
    ,NodeToCode
    ,NodeToNameRu
    ,NodeToNameEn
    ,NodeToId
    ,TransportKindCode
    ,TransportKindId
    ,TransportTypeCode
    ,TransportTypeId
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductGroupId
    ,ProductCode
    ,ProductNameRu
    ,ProductNameEn
    ,ProductId
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_ReportConstructor_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2130'   -- ReportConstructor (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1013'  -- ProductGroup
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1015'  -- MTR (Product)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'ReportConstructor' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
