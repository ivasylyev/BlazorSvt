USE [mdm];
GO

/*
    Первичная полная заливка snapshot RateAutoSelectionSetting
    из v2.vw_RateAutoSelectionSetting_SnapshotSource.

    Курсор v2.SyncState ставится после RowVer на dbo.PrimitiveEntityData_2101.
*/

INSERT INTO v2.RateAutoSelectionSetting_Snapshot (
     RateAutoSelectionSettingId
    ,IsArchive
    ,Code
    ,DestinationNodeToRegion
    ,DestinationNodeToCountry
    ,ProductToProductGroup
    ,AnyTransportKind
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
)
SELECT
     RateAutoSelectionSettingId
    ,IsArchive
    ,Code
    ,DestinationNodeToRegion
    ,DestinationNodeToCountry
    ,ProductToProductGroup
    ,AnyTransportKind
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
FROM v2.vw_RateAutoSelectionSetting_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2101'   -- RateAutoSelectionSetting (основная)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'RateAutoSelectionSetting' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
