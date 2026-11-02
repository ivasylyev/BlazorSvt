USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Currency_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Currency_Snapshot;
END
GO

DROP INDEX IF EXISTS UX_Currency_Snapshot_Id ON [v2].[Currency_Snapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_Currency_Snapshot_Id
    ON v2.Currency_Snapshot (Id)
    ON [PRIMARY];
GO

CREATE FULLTEXT INDEX ON v2.Currency_Snapshot
(
    Name     LANGUAGE 1049,
    Symbolic LANGUAGE 1033,
    Code     LANGUAGE 1033,
    Code_NSI LANGUAGE 1033
)
KEY INDEX UX_Currency_Snapshot_Id
WITH STOPLIST = SYSTEM,
     CHANGE_TRACKING = AUTO;
GO

ALTER FULLTEXT INDEX ON v2.Currency_Snapshot
SET STOPLIST = OFF;
GO

DROP INDEX IF EXISTS ix_Currency_Snapshot_Active_Code ON [v2].[Currency_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Currency_Snapshot_Active_Code
ON v2.Currency_Snapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Currency_Snapshot_Archive_Code ON [v2].[Currency_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Currency_Snapshot_Archive_Code
ON v2.Currency_Snapshot (Code)
WHERE IsArchive = 1;
GO

DROP INDEX IF EXISTS ix_Currency_Snapshot_Active_CurrencyId ON [v2].[Currency_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Currency_Snapshot_Active_CurrencyId
ON v2.Currency_Snapshot (CurrencyId)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_Currency_Snapshot_Archive_CurrencyId ON [v2].[Currency_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_Currency_Snapshot_Archive_CurrencyId
ON v2.Currency_Snapshot (CurrencyId)
WHERE IsArchive = 1;
GO

UPDATE STATISTICS v2.Currency_Snapshot WITH FULLSCAN;
GO
