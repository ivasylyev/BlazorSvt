USE [mdm]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.AverageRateLevel3_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.AverageRateLevel3_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_AverageRateLevel3_Snapshot_Id ON [v2].[AverageRateLevel3_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_AverageRateLevel3_Snapshot_Id
    ON v2.AverageRateLevel3_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.AverageRateLevel3_Snapshot
(
    NodeFromCode       LANGUAGE 1033,
    NodeFromNameEn     LANGUAGE 1033,
    NodeFromNameRu     LANGUAGE 1049,
    ProxyNodeCode      LANGUAGE 1033,
    ProxyNodeNameEn    LANGUAGE 1033,
    ProxyNodeNameRu    LANGUAGE 1049,
    NodeToCode         LANGUAGE 1033,
    NodeToNameEn       LANGUAGE 1033,
    NodeToNameRu       LANGUAGE 1049,
    ProductGroupCode   LANGUAGE 1033,
    ProductGroupNameRu LANGUAGE 1049,
    ProductGroupNameEn LANGUAGE 1033,
    ProductNameRu      LANGUAGE 1049,
    ProductNameEn      LANGUAGE 1033
)
KEY INDEX UX_AverageRateLevel3_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON mdm.v2.AverageRateLevel3_Snapshot
SET STOPLIST = OFF;
GO

CREATE NONCLUSTERED INDEX ix_AverageRateLevel3_Snapshot_Active_Code
ON v2.AverageRateLevel3_Snapshot (Code)
WHERE IsArchive = 0;
GO

CREATE NONCLUSTERED INDEX ix_AverageRateLevel3_Snapshot_Archive_Code
ON v2.AverageRateLevel3_Snapshot (Code)
WHERE IsArchive = 1;
GO

CREATE NONCLUSTERED INDEX ix_AverageRateLevel3_Snapshot_Active_Date
ON v2.AverageRateLevel3_Snapshot (StartDate, EndDate)
WHERE IsArchive = 0;
GO

CREATE NONCLUSTERED INDEX ix_AverageRateLevel3_Snapshot_Archive_Date
ON v2.AverageRateLevel3_Snapshot (StartDate, EndDate)
WHERE IsArchive = 1;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_Active_TransportKindId_TransportTypeId]
ON [v2].AverageRateLevel3_Snapshot ([TransportKindId], [TransportTypeId])
WHERE IsArchive = 0;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_Archive_TransportKindId_TransportTypeId]
ON [v2].AverageRateLevel3_Snapshot ([TransportKindId], [TransportTypeId])
WHERE IsArchive = 1;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_Active_RateTypeId]
ON [v2].AverageRateLevel3_Snapshot ([RateTypeId])
WHERE IsArchive = 0;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_Archive_RateTypeId]
ON [v2].AverageRateLevel3_Snapshot ([RateTypeId])
WHERE IsArchive = 1;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_AverageRateLevel3Id]
ON v2.AverageRateLevel3_Snapshot (AverageRateLevel3Id);
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_NodeFromId]
ON v2.AverageRateLevel3_Snapshot (NodeFromId);
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_NodeToId]
ON v2.AverageRateLevel3_Snapshot (NodeToId);
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_ProxyNodeId]
ON v2.AverageRateLevel3_Snapshot (ProxyNodeId)
WHERE ProxyNodeId IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_ProductGroupId]
ON v2.AverageRateLevel3_Snapshot (ProductGroupId)
WHERE ProductGroupId IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_ProductId]
ON v2.AverageRateLevel3_Snapshot (ProductId)
WHERE ProductId IS NOT NULL;
GO

CREATE NONCLUSTERED INDEX [ix_AverageRateLevel3_Snapshot_CurrencyId]
ON v2.AverageRateLevel3_Snapshot (CurrencyId);
GO

UPDATE STATISTICS v2.AverageRateLevel3_Snapshot WITH FULLSCAN;
GO
