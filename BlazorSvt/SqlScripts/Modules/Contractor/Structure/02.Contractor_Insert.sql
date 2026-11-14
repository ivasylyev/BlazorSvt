USE [mdm];
GO

/*
    Первичная заливка snapshot Contractor из v2.vw_Contractor_SnapshotSource.
*/

INSERT INTO v2.Contractor_Snapshot (
     ContractorId
    ,Code
    ,IsArchive
    ,NameSearch
    ,Consignee
    ,IsTrader
    ,IsProcessing
    ,IsSender
    ,IsDebtor
    ,IsCreditor
    ,INN
    ,CreationDate
    ,LastChangeDate
)
SELECT
     ContractorId
    ,Code
    ,IsArchive
    ,NameSearch
    ,Consignee
    ,IsTrader
    ,IsProcessing
    ,IsSender
    ,IsDebtor
    ,IsCreditor
    ,INN
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Contractor_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2028'   -- LegalType (основная), реестр Contractor
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'Contractor' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
