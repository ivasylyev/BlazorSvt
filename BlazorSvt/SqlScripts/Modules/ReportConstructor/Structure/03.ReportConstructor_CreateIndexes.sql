USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ReportConstructor_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ReportConstructor_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_ReportConstructor_Snapshot_Id ON [v2].[ReportConstructor_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_ReportConstructor_Snapshot_Id
    ON v2.ReportConstructor_Snapshot (Id)
    ON [PRIMARY];
GO

-- Code записи — INT, в full-text не входит.
CREATE FULLTEXT INDEX ON v2.ReportConstructor_Snapshot
(
    NodeFromCode        LANGUAGE 1033,
    NodeFromNameRu      LANGUAGE 1049,
    NodeFromNameEn      LANGUAGE 1033,
    NodeToCode          LANGUAGE 1033,
    NodeToNameRu        LANGUAGE 1049,
    NodeToNameEn        LANGUAGE 1033,
    TransportKindCode   LANGUAGE 1033,
    TransportTypeCode   LANGUAGE 1033,
    ProductGroupCode    LANGUAGE 1033,
    ProductGroupNameRu  LANGUAGE 1049,
    ProductGroupNameEn  LANGUAGE 1033,
    ProductCode         LANGUAGE 1033,
    ProductNameRu       LANGUAGE 1049,
    ProductNameEn       LANGUAGE 1033
)
KEY INDEX UX_ReportConstructor_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.ReportConstructor_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Active_Code ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Active_Code
ON v2.ReportConstructor_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Archive_Code ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Archive_Code
ON v2.ReportConstructor_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Active_ReportTypeId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Active_ReportTypeId
ON v2.ReportConstructor_Snapshot (ReportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Archive_ReportTypeId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Archive_ReportTypeId
ON v2.ReportConstructor_Snapshot (ReportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Active_TransportKindId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Active_TransportKindId
ON v2.ReportConstructor_Snapshot (TransportKindId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Archive_TransportKindId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Archive_TransportKindId
ON v2.ReportConstructor_Snapshot (TransportKindId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Active_TransportTypeId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Active_TransportTypeId
ON v2.ReportConstructor_Snapshot (TransportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_Archive_TransportTypeId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_Archive_TransportTypeId
ON v2.ReportConstructor_Snapshot (TransportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_ReportConstructorId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_ReportConstructorId
ON v2.ReportConstructor_Snapshot (ReportConstructorId);
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_NodeFromId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_NodeFromId
ON v2.ReportConstructor_Snapshot (NodeFromId);
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_NodeToId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_NodeToId
ON v2.ReportConstructor_Snapshot (NodeToId);
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_ProductGroupId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_ProductGroupId
ON v2.ReportConstructor_Snapshot (ProductGroupId);
GO

DROP INDEX IF EXISTS ix_ReportConstructor_Snapshot_ProductId ON [v2].[ReportConstructor_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ReportConstructor_Snapshot_ProductId
ON v2.ReportConstructor_Snapshot (ProductId);
GO

UPDATE STATISTICS v2.ReportConstructor_Snapshot WITH FULLSCAN;
GO
