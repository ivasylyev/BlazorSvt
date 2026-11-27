USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportRoute_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportRoute_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_TransportRoute_Snapshot_Id ON [v2].[TransportRoute_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_TransportRoute_Snapshot_Id
    ON v2.TransportRoute_Snapshot (Id)
    ON [PRIMARY];
GO

-- Code записи — INT, в full-text не входит.
-- Денорм enum (RouteTransportKind*, Leg*_TransportType Code/Name) в full-text не входит.
CREATE FULLTEXT INDEX ON v2.TransportRoute_Snapshot
(
    CodeSAP             LANGUAGE 1033,
    Route               LANGUAGE 1049,
    StartNodeCode       LANGUAGE 1033,
    StartNodeNameRu     LANGUAGE 1049,
    StartNodeNameEn     LANGUAGE 1033,
    RegionFromCode      LANGUAGE 1033,
    RegionFromNameRu    LANGUAGE 1049,
    RegionFromNameEn    LANGUAGE 1033,
    ProxyNodeCode       LANGUAGE 1033,
    ProxyNodeNameRu     LANGUAGE 1049,
    ProxyNodeNameEn     LANGUAGE 1033,
    EndNodeCode         LANGUAGE 1033,
    EndNodeNameRu       LANGUAGE 1049,
    EndNodeNameEn       LANGUAGE 1033,
    RegionToCode        LANGUAGE 1033,
    RegionToNameRu      LANGUAGE 1049,
    RegionToNameEn      LANGUAGE 1033,
    ChainNodes          LANGUAGE 1049
)
KEY INDEX UX_TransportRoute_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.TransportRoute_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Active_Code ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Active_Code
ON v2.TransportRoute_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Archive_Code ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Archive_Code
ON v2.TransportRoute_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Active_RouteTransportKindId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Active_RouteTransportKindId
ON v2.TransportRoute_Snapshot (RouteTransportKindId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Archive_RouteTransportKindId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Archive_RouteTransportKindId
ON v2.TransportRoute_Snapshot (RouteTransportKindId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Active_Leg1_TransportTypeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Active_Leg1_TransportTypeId
ON v2.TransportRoute_Snapshot (Leg1_TransportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Archive_Leg1_TransportTypeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Archive_Leg1_TransportTypeId
ON v2.TransportRoute_Snapshot (Leg1_TransportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Active_Leg2_TransportTypeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Active_Leg2_TransportTypeId
ON v2.TransportRoute_Snapshot (Leg2_TransportTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_Archive_Leg2_TransportTypeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_Archive_Leg2_TransportTypeId
ON v2.TransportRoute_Snapshot (Leg2_TransportTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_TransportRouteId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_TransportRouteId
ON v2.TransportRoute_Snapshot (TransportRouteId);
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_StartNodeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_StartNodeId
ON v2.TransportRoute_Snapshot (StartNodeId);
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_EndNodeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_EndNodeId
ON v2.TransportRoute_Snapshot (EndNodeId);
GO

DROP INDEX IF EXISTS ix_TransportRoute_Snapshot_ProxyNodeId ON [v2].[TransportRoute_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRoute_Snapshot_ProxyNodeId
ON v2.TransportRoute_Snapshot (ProxyNodeId);
GO

UPDATE STATISTICS v2.TransportRoute_Snapshot WITH FULLSCAN;
GO
