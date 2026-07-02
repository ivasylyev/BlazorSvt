USE [mdm];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.LocationNodesSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.LocationNodesSnapshot;
END
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.LocationsNodes_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.LocationsNodes_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_LocationNodesSnapshot_Id ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS UX_LocationsNodes_Snapshot_Id ON [v2].[LocationsNodes_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_LocationsNodes_Snapshot_Id
    ON v2.LocationsNodes_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.LocationsNodes_Snapshot
(
    Code                   LANGUAGE 1033,
    NameEn                 LANGUAGE 1033,
    NameRu                 LANGUAGE 1049,
    LocationTypeNameRu     LANGUAGE 1049,
    LocationTypeNameEn     LANGUAGE 1033,
    TypeNodeNameRu         LANGUAGE 1049,
    TypeNodeNameEn         LANGUAGE 1033,
    RegionNameRu           LANGUAGE 1049,
    RegionNameEn           LANGUAGE 1033,
    RegionRU               LANGUAGE 1049,
    CountryNameRu          LANGUAGE 1049,
    CountryNameEn          LANGUAGE 1033
)
KEY INDEX UX_LocationsNodes_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON mdm.v2.LocationsNodes_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_Code ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Active_Code ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Active_Code
ON v2.LocationsNodes_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_Code ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Archive_Code ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Archive_Code
ON v2.LocationsNodes_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_LocationTypeId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Active_LocationTypeId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Active_LocationTypeId ON v2.LocationsNodes_Snapshot
(
    LocationTypeId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_LocationTypeId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Archive_LocationTypeId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Archive_LocationTypeId ON v2.LocationsNodes_Snapshot
(
    LocationTypeId
)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_TypeNodeId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Active_TypeNodeId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Active_TypeNodeId ON v2.LocationsNodes_Snapshot
(
    TypeNodeId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_TypeNodeId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Archive_TypeNodeId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Archive_TypeNodeId ON v2.LocationsNodes_Snapshot
(
    TypeNodeId
)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_RegionId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Active_RegionId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Active_RegionId ON v2.LocationsNodes_Snapshot
(
    RegionId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_RegionId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Archive_RegionId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Archive_RegionId ON v2.LocationsNodes_Snapshot
(
    RegionId
)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_CountryId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Active_CountryId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Active_CountryId ON v2.LocationsNodes_Snapshot
(
    CountryId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_CountryId ON [v2].[LocationNodesSnapshot];
DROP INDEX IF EXISTS ix_LocationsNodes_Snapshot_Archive_CountryId ON [v2].[LocationsNodes_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationsNodes_Snapshot_Archive_CountryId ON v2.LocationsNodes_Snapshot
(
    CountryId
)
WHERE IsArchive = 1;
GO

UPDATE STATISTICS v2.LocationsNodes_Snapshot WITH FULLSCAN;
GO
