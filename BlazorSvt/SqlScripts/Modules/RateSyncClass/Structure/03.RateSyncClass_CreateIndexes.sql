USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateSyncClass_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateSyncClass_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_RateSyncClass_Snapshot_Id ON [v2].[RateSyncClass_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_RateSyncClass_Snapshot_Id
    ON v2.RateSyncClass_Snapshot (Id)
    ON [PRIMARY];
GO

-- Code — INT, в full-text не входит.
CREATE FULLTEXT INDEX ON v2.RateSyncClass_Snapshot
(
    Name LANGUAGE 1049
)
KEY INDEX UX_RateSyncClass_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.RateSyncClass_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_RateSyncClass_Snapshot_Active_Code ON [v2].[RateSyncClass_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateSyncClass_Snapshot_Active_Code
ON v2.RateSyncClass_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_RateSyncClass_Snapshot_Archive_Code ON [v2].[RateSyncClass_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateSyncClass_Snapshot_Archive_Code
ON v2.RateSyncClass_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_RateSyncClass_Snapshot_RateSyncClassId ON [v2].[RateSyncClass_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateSyncClass_Snapshot_RateSyncClassId
ON v2.RateSyncClass_Snapshot (RateSyncClassId);
GO

UPDATE STATISTICS v2.RateSyncClass_Snapshot WITH FULLSCAN;
GO
