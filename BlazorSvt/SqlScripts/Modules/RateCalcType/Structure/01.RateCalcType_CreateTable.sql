USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateCalcType_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateCalcType_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.RateCalcType_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.RateCalcType_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.RateCalcType_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RateCalcTypeId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_RateCalcType')
BEGIN
    DROP PARTITION SCHEME v2_ps_RateCalcType;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_RateCalcType_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_RateCalcType_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_RateCalcType_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_RateCalcType
AS PARTITION v2_pf_RateCalcType_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RateCalcTypeId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.RateCalcType_Snapshot (
    Id                  BIGINT NOT NULL,
    RateCalcTypeId      BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    Code                NVARCHAR(50) NOT NULL,
    Name                NVARCHAR(50) NULL,
    SearchCode          NVARCHAR(50) NULL,
    CreationUserName    NVARCHAR(255) NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeUserName  NVARCHAR(255) NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_RateCalcType_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_RateCalcType(IsArchive);
GO

ALTER TABLE v2.RateCalcType_Snapshot
ADD CONSTRAINT DF_RateCalcType_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_RateCalcTypeId) FOR Id;
GO

ALTER TABLE v2.RateCalcType_Snapshot
ADD CONSTRAINT DF_RateCalcType_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
