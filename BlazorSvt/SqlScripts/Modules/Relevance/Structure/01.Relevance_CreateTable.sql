USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Relevance_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Relevance_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Relevance_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Relevance_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Relevance_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RelevanceId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Relevance')
BEGIN
    DROP PARTITION SCHEME v2_ps_Relevance;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Relevance_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Relevance_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Relevance_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Relevance
AS PARTITION v2_pf_Relevance_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RelevanceId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Relevance_Snapshot (
    Code                NVARCHAR(50) NOT NULL,
    Name                NVARCHAR(100) NULL,
    Id                  BIGINT NOT NULL,
    RelevanceId         BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_Relevance_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Relevance(IsArchive);
GO

ALTER TABLE v2.Relevance_Snapshot
ADD CONSTRAINT DF_Relevance_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_RelevanceId) FOR Id;
GO

ALTER TABLE v2.Relevance_Snapshot
ADD CONSTRAINT DF_Relevance_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
