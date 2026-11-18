USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Coefficient_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Coefficient_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_Coefficient_Snapshot_Id ON [v2].[Coefficient_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Coefficient_Snapshot_Id
    ON v2.Coefficient_Snapshot (Id)
    ON [PRIMARY];
GO

-- Code коэффициента — числовая строка, в full-text не входит (фильтрованные индексы ниже).
CREATE FULLTEXT INDEX ON v2.Coefficient_Snapshot
(
    Name                    LANGUAGE 1049,
    CoefficientTypeNameRu   LANGUAGE 1049,
    RateTypeNameRu          LANGUAGE 1049
)
KEY INDEX UX_Coefficient_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.Coefficient_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Active_Code ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Active_Code
ON v2.Coefficient_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Archive_Code ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Archive_Code
ON v2.Coefficient_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Active_Date ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Active_Date
ON v2.Coefficient_Snapshot (StartDate, FinishDate)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Archive_Date ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Archive_Date
ON v2.Coefficient_Snapshot (StartDate, FinishDate)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Active_CoefficientTypeId ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Active_CoefficientTypeId
ON v2.Coefficient_Snapshot (CoefficientTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Archive_CoefficientTypeId ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Archive_CoefficientTypeId
ON v2.Coefficient_Snapshot (CoefficientTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Active_RateTypeId ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Active_RateTypeId
ON v2.Coefficient_Snapshot (RateTypeId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Coefficient_Snapshot_Archive_RateTypeId ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Coefficient_Snapshot_Archive_RateTypeId
ON v2.Coefficient_Snapshot (RateTypeId)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS [ix_Coefficient_Snapshot_CoefficientId] ON [v2].[Coefficient_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_Coefficient_Snapshot_CoefficientId]
ON v2.Coefficient_Snapshot (CoefficientId);
GO

UPDATE STATISTICS v2.Coefficient_Snapshot WITH FULLSCAN;
GO
