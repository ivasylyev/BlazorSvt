USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportKind_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportKind_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_TransportKind_Snapshot_Id ON [v2].[TransportKind_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_TransportKind_Snapshot_Id
    ON v2.TransportKind_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.TransportKind_Snapshot
(
    Code      LANGUAGE 1033,
    Name      LANGUAGE 1049,
    NameEnRu  LANGUAGE 1049,
    Level     LANGUAGE 1049,
    KindCode  LANGUAGE 1033
)
KEY INDEX UX_TransportKind_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.TransportKind_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_TransportKind_Snapshot_Active_Code ON [v2].[TransportKind_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportKind_Snapshot_Active_Code
ON v2.TransportKind_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportKind_Snapshot_Archive_Code ON [v2].[TransportKind_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportKind_Snapshot_Archive_Code
ON v2.TransportKind_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportKind_Snapshot_Active_StatusId ON [v2].[TransportKind_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportKind_Snapshot_Active_StatusId
ON v2.TransportKind_Snapshot (StatusId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportKind_Snapshot_Archive_StatusId ON [v2].[TransportKind_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportKind_Snapshot_Archive_StatusId
ON v2.TransportKind_Snapshot (StatusId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportKind_Snapshot_TransportKindId ON [v2].[TransportKind_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportKind_Snapshot_TransportKindId
ON v2.TransportKind_Snapshot (TransportKindId);
GO

UPDATE STATISTICS v2.TransportKind_Snapshot WITH FULLSCAN;
GO
