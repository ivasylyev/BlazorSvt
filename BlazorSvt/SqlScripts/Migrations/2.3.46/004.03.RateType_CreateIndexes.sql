USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateType_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateType_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_RateType_Snapshot_Id ON [v2].[RateType_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_RateType_Snapshot_Id
    ON v2.RateType_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.RateType_Snapshot
(
    Name    LANGUAGE 1049,
    Comment LANGUAGE 1049
)
KEY INDEX UX_RateType_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.RateType_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_RateType_Snapshot_Active_Code ON [v2].[RateType_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateType_Snapshot_Active_Code
ON v2.RateType_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_RateType_Snapshot_Archive_Code ON [v2].[RateType_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateType_Snapshot_Archive_Code
ON v2.RateType_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_RateType_Snapshot_Active_RateTypeId ON [v2].[RateType_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateType_Snapshot_Active_RateTypeId
ON v2.RateType_Snapshot (RateTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_RateType_Snapshot_Archive_RateTypeId ON [v2].[RateType_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateType_Snapshot_Archive_RateTypeId
ON v2.RateType_Snapshot (RateTypeId)
WHERE IsArchive = 1;
GO

UPDATE STATISTICS v2.RateType_Snapshot WITH FULLSCAN;
GO
