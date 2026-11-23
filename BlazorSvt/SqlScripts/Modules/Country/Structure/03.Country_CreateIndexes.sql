USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Country_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Country_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_Country_Snapshot_Id ON [v2].[Country_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Country_Snapshot_Id
    ON v2.Country_Snapshot (Id)
    ON [PRIMARY];
GO

-- Code записи — короткие цифровые строки, в full-text не входит.
-- ISOCode — decimal, в full-text не входит.
CREATE FULLTEXT INDEX ON v2.Country_Snapshot
(
    NameRu      LANGUAGE 1049,
    NameEn      LANGUAGE 1033,
    PlanCode    LANGUAGE 1033,
    Market      LANGUAGE 1049,
    ISOCode3    LANGUAGE 1033
)
KEY INDEX UX_Country_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.Country_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_Country_Snapshot_Active_Code ON [v2].[Country_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Country_Snapshot_Active_Code
ON v2.Country_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Country_Snapshot_Archive_Code ON [v2].[Country_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Country_Snapshot_Archive_Code
ON v2.Country_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Country_Snapshot_Active_StatusId ON [v2].[Country_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Country_Snapshot_Active_StatusId
ON v2.Country_Snapshot (StatusId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Country_Snapshot_Archive_StatusId ON [v2].[Country_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Country_Snapshot_Archive_StatusId
ON v2.Country_Snapshot (StatusId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Country_Snapshot_CountryId ON [v2].[Country_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Country_Snapshot_CountryId
ON v2.Country_Snapshot (CountryId);
GO

UPDATE STATISTICS v2.Country_Snapshot WITH FULLSCAN;
GO
