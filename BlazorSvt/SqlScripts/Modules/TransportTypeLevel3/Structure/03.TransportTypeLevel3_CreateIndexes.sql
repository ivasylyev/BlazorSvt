USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportTypeLevel3_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportTypeLevel3_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_TransportTypeLevel3_Snapshot_Id ON [v2].[TransportTypeLevel3_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_TransportTypeLevel3_Snapshot_Id
    ON v2.TransportTypeLevel3_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.TransportTypeLevel3_Snapshot
(
    Name      LANGUAGE 1049,
    NameEnRu  LANGUAGE 1049,
    Code      LANGUAGE 1033,
    TypeCode  LANGUAGE 1033,
    SAPCode   LANGUAGE 1033,
    OrionCode LANGUAGE 1033
)
KEY INDEX UX_TransportTypeLevel3_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.TransportTypeLevel3_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Active_Code ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Active_Code
ON v2.TransportTypeLevel3_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Archive_Code ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Archive_Code
ON v2.TransportTypeLevel3_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Active_TransportKindId_TransportTypeId ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Active_TransportKindId_TransportTypeId
ON v2.TransportTypeLevel3_Snapshot (TransportKindId, TransportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Archive_TransportKindId_TransportTypeId ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Archive_TransportKindId_TransportTypeId
ON v2.TransportTypeLevel3_Snapshot (TransportKindId, TransportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Active_TransportTypeLevel3Id ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Active_TransportTypeLevel3Id
ON v2.TransportTypeLevel3_Snapshot (TransportTypeLevel3Id)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportTypeLevel3_Snapshot_Archive_TransportTypeLevel3Id ON [v2].[TransportTypeLevel3_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportTypeLevel3_Snapshot_Archive_TransportTypeLevel3Id
ON v2.TransportTypeLevel3_Snapshot (TransportTypeLevel3Id)
WHERE IsArchive = 1;
GO

UPDATE STATISTICS v2.TransportTypeLevel3_Snapshot WITH FULLSCAN;
GO
