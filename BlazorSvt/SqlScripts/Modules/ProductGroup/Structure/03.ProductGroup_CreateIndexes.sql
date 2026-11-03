USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ProductGroup_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ProductGroup_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_ProductGroup_Snapshot_Id ON [v2].[ProductGroup_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_ProductGroup_Snapshot_Id
    ON v2.ProductGroup_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.ProductGroup_Snapshot
(
    Code                LANGUAGE 1033,
    Name                LANGUAGE 1049,
    ShortName           LANGUAGE 1049,
    GroupCode           LANGUAGE 1033,
    ProductGroupCBD     LANGUAGE 1033,
    NameEnCode          LANGUAGE 1033,
    NameEnRu            LANGUAGE 1049,
    Code_NSI            LANGUAGE 1033,
    NameEn              LANGUAGE 1033,
    RateSyncClassNameRu LANGUAGE 1049
)
KEY INDEX UX_ProductGroup_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.ProductGroup_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_ProductGroup_Snapshot_Active_Code ON [v2].[ProductGroup_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ProductGroup_Snapshot_Active_Code
ON v2.ProductGroup_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ProductGroup_Snapshot_Archive_Code ON [v2].[ProductGroup_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ProductGroup_Snapshot_Archive_Code
ON v2.ProductGroup_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ProductGroup_Snapshot_Active_RateSyncClassId ON [v2].[ProductGroup_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ProductGroup_Snapshot_Active_RateSyncClassId
ON v2.ProductGroup_Snapshot (RateSyncClassId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ProductGroup_Snapshot_Archive_RateSyncClassId ON [v2].[ProductGroup_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ProductGroup_Snapshot_Archive_RateSyncClassId
ON v2.ProductGroup_Snapshot (RateSyncClassId)
WHERE IsArchive = 1;
GO

-- Индекс на бизнес-ключ для синхронизации: upsert (MERGE ON ProductGroupId),
-- pre-delete смены партиции и reconciliation ищут строки по ключу без скана таблицы.
DROP INDEX IF EXISTS [ix_ProductGroup_Snapshot_ProductGroupId] ON [v2].[ProductGroup_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_ProductGroup_Snapshot_ProductGroupId]
ON v2.ProductGroup_Snapshot (ProductGroupId);
GO

UPDATE STATISTICS v2.ProductGroup_Snapshot WITH FULLSCAN;
GO
