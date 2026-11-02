USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.MTR_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.MTR_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_MTR_Snapshot_Id ON [v2].[MTR_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_MTR_Snapshot_Id
    ON v2.MTR_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.MTR_Snapshot
(
    Code                LANGUAGE 1033,
    NameFullRu          LANGUAGE 1049,
    NameShortEn         LANGUAGE 1033,
    ProductGroupCode    LANGUAGE 1033,
    ProductGroupNameRu  LANGUAGE 1049,
    ProductGroupNameEn  LANGUAGE 1033,
    EtsngNameRu         LANGUAGE 1049,
    EtsngNameEn         LANGUAGE 1049,
    GngCode             LANGUAGE 1033,
    DpoCode             LANGUAGE 1033,
    MtrType             LANGUAGE 1033
)
KEY INDEX UX_MTR_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.MTR_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Active_Code ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Active_Code
ON v2.MTR_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Archive_Code ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Archive_Code
ON v2.MTR_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Active_UnitId ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Active_UnitId
ON v2.MTR_Snapshot (UnitId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Archive_UnitId ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Archive_UnitId
ON v2.MTR_Snapshot (UnitId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Active_DangerClassId ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Active_DangerClassId
ON v2.MTR_Snapshot (DangerClassId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_MTR_Snapshot_Archive_DangerClassId ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_MTR_Snapshot_Archive_DangerClassId
ON v2.MTR_Snapshot (DangerClassId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS [ix_MTR_Snapshot_MTRId] ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_MTR_Snapshot_MTRId]
ON v2.MTR_Snapshot (MTRId);
GO

DROP INDEX IF EXISTS [ix_MTR_Snapshot_ProductGroupId] ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_MTR_Snapshot_ProductGroupId]
ON v2.MTR_Snapshot (ProductGroupId)
WHERE ProductGroupId IS NOT NULL;
GO

DROP INDEX IF EXISTS [ix_MTR_Snapshot_EtsngId] ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_MTR_Snapshot_EtsngId]
ON v2.MTR_Snapshot (EtsngId)
WHERE EtsngId IS NOT NULL;
GO

UPDATE STATISTICS v2.MTR_Snapshot WITH FULLSCAN;
GO
