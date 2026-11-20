USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Region_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Region_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_Region_Snapshot_Id ON [v2].[Region_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Region_Snapshot_Id
    ON v2.Region_Snapshot (Id)
    ON [PRIMARY];
GO

-- Денорм FederalDistricts* в full-text не входит.
CREATE FULLTEXT INDEX ON v2.Region_Snapshot
(
    RegionCode          LANGUAGE 1033,
    Code_NSI            LANGUAGE 1033,
    RegionCodePlanning  LANGUAGE 1033,
    RegionCodeRF        LANGUAGE 1033,
    NameRu              LANGUAGE 1049,
    NameEn              LANGUAGE 1033,
    CountryNameRu       LANGUAGE 1049,
    CountryNameEn       LANGUAGE 1033,
    Market              LANGUAGE 1049,
    Code                LANGUAGE 1033
)
KEY INDEX UX_Region_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.Region_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Active_Code ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Active_Code
ON v2.Region_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Archive_Code ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Archive_Code
ON v2.Region_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Active_StatusId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Active_StatusId
ON v2.Region_Snapshot (StatusId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Archive_StatusId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Archive_StatusId
ON v2.Region_Snapshot (StatusId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Active_FederalDistrictsId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Active_FederalDistrictsId
ON v2.Region_Snapshot (FederalDistrictsId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_Archive_FederalDistrictsId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_Archive_FederalDistrictsId
ON v2.Region_Snapshot (FederalDistrictsId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_RegionId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_RegionId
ON v2.Region_Snapshot (RegionId);
GO

DROP INDEX IF EXISTS ix_Region_Snapshot_CountryId ON [v2].[Region_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Region_Snapshot_CountryId
ON v2.Region_Snapshot (CountryId);
GO

UPDATE STATISTICS v2.Region_Snapshot WITH FULLSCAN;
GO
