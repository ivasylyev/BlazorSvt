USE [mdm];
GO

/*
    Первичная полная заливка snapshot Country
    из v2.vw_Country_SnapshotSource.

    RowVer на dbo.PrimitiveEntityData_1009 уже есть.
    StatusElement (1010) в курсор не входит.
*/

INSERT INTO v2.Country_Snapshot (
     Code
    ,NameRu
    ,ISOCode
    ,NameEn
    ,PlanCode
    ,Market
    ,ISOCode3
    ,StatusId
    ,CountryId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
)
SELECT
     Code
    ,NameRu
    ,ISOCode
    ,NameEn
    ,PlanCode
    ,Market
    ,ISOCode3
    ,StatusId
    ,CountryId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Country_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_1009'   -- Country (основная)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'Country' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
