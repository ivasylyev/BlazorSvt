USE [mdm]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ParityRates_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ParityRates_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_ParityRates_Snapshot_Id ON [v2].[ParityRates_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_ParityRates_Snapshot_Id
    ON v2.ParityRates_Snapshot (Id)
    ON [PRIMARY];
GO

-- FTS: без Code (GUID). Name* + whitelist Codes + текстовые поля грида.
CREATE FULLTEXT INDEX ON v2.ParityRates_Snapshot
(
    NodeFromCode       LANGUAGE 1033,
    NodeFromNameEn     LANGUAGE 1033,
    NodeFromNameRu     LANGUAGE 1049,
    ProxyNode1Code     LANGUAGE 1033,
    ProxyNode1NameEn   LANGUAGE 1033,
    ProxyNode1NameRu   LANGUAGE 1049,
    ProxyNode2Code     LANGUAGE 1033,
    ProxyNode2NameEn   LANGUAGE 1033,
    ProxyNode2NameRu   LANGUAGE 1049,
    NodeToCode         LANGUAGE 1033,
    NodeToNameEn       LANGUAGE 1033,
    NodeToNameRu       LANGUAGE 1049,
    TransportTypeCode  LANGUAGE 1033,
    ProductGroupCode   LANGUAGE 1033,
    ProductGroupNameRu LANGUAGE 1049,
    ProductGroupNameEn LANGUAGE 1033,
    ProductNameRu      LANGUAGE 1049,
    ProductNameEn      LANGUAGE 1033,
    Comment            LANGUAGE 1049,
    DataSource         LANGUAGE 1049,
    DepartmentResponsibilityArea LANGUAGE 1049,
    EmployeeResponsibilityArea   LANGUAGE 1049,
    Methodology        LANGUAGE 1049,
    PriorityText       LANGUAGE 1049
)
KEY INDEX UX_ParityRates_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON mdm.v2.ParityRates_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Active_Code ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Active_Code
ON v2.ParityRates_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Archive_Code ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Archive_Code
ON v2.ParityRates_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Active_Date ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Active_Date
ON v2.ParityRates_Snapshot (StartDate, EndDate)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Archive_Date ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Archive_Date
ON v2.ParityRates_Snapshot (StartDate, EndDate)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Active_RelevanceId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Active_RelevanceId
ON v2.ParityRates_Snapshot (RelevanceId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Archive_RelevanceId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Archive_RelevanceId
ON v2.ParityRates_Snapshot (RelevanceId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Active_TransportTypeId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Active_TransportTypeId
ON v2.ParityRates_Snapshot (TransportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_Archive_TransportTypeId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_Archive_TransportTypeId
ON v2.ParityRates_Snapshot (TransportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_ParityRatesId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_ParityRatesId
ON v2.ParityRates_Snapshot (ParityRatesId);
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_NodeFromId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_NodeFromId
ON v2.ParityRates_Snapshot (NodeFromId);
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_NodeToId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_NodeToId
ON v2.ParityRates_Snapshot (NodeToId);
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_ProxyNode1Id ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_ProxyNode1Id
ON v2.ParityRates_Snapshot (ProxyNode1Id)
WHERE ProxyNode1Id IS NOT NULL;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_ProxyNode2Id ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_ProxyNode2Id
ON v2.ParityRates_Snapshot (ProxyNode2Id)
WHERE ProxyNode2Id IS NOT NULL;
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_ProductGroupId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_ProductGroupId
ON v2.ParityRates_Snapshot (ProductGroupId);
GO

DROP INDEX IF EXISTS ix_ParityRates_Snapshot_ProductId ON [v2].[ParityRates_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_ParityRates_Snapshot_ProductId
ON v2.ParityRates_Snapshot (ProductId)
WHERE ProductId IS NOT NULL;
GO

UPDATE STATISTICS v2.ParityRates_Snapshot WITH FULLSCAN;
GO