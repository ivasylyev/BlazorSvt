USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateAutoSelectionSetting_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateAutoSelectionSetting_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_RateAutoSelectionSetting_Snapshot_Id ON [v2].[RateAutoSelectionSetting_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_RateAutoSelectionSetting_Snapshot_Id
    ON v2.RateAutoSelectionSetting_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.RateAutoSelectionSetting_Snapshot
(
    Code LANGUAGE 1033
)
KEY INDEX UX_RateAutoSelectionSetting_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.RateAutoSelectionSetting_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_RateAutoSelectionSetting_Snapshot_Active_Code ON [v2].[RateAutoSelectionSetting_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateAutoSelectionSetting_Snapshot_Active_Code
ON v2.RateAutoSelectionSetting_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_RateAutoSelectionSetting_Snapshot_Archive_Code ON [v2].[RateAutoSelectionSetting_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_RateAutoSelectionSetting_Snapshot_Archive_Code
ON v2.RateAutoSelectionSetting_Snapshot (Code)
WHERE IsArchive = 1;
GO

UPDATE STATISTICS v2.RateAutoSelectionSetting_Snapshot WITH FULLSCAN;
GO
