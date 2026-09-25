USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateType_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateType_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.RateType_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.RateType_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.RateType_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RateTypeId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_RateType')
BEGIN
    DROP PARTITION SCHEME v2_ps_RateType;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_RateType_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_RateType_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_RateType_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_RateType
AS PARTITION v2_pf_RateType_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RateTypeId AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.RateType_Snapshot (
    Id                  INT NOT NULL,
    RateTypeId          BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    Code                NVARCHAR(50) NOT NULL,
    Name                NVARCHAR(4000) NOT NULL,
    IsUseNomination     BIT NOT NULL,
    IsDeflated          BIT NOT NULL,
    Comment             NVARCHAR(4000) NULL,
    IsIntoSyncClass     BIT NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_RateType_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_RateType(IsArchive);
GO

ALTER TABLE v2.RateType_Snapshot
ADD CONSTRAINT DF_RateType_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_RateTypeId) FOR Id;
GO

ALTER TABLE v2.RateType_Snapshot
ADD CONSTRAINT DF_RateType_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
