USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateSyncClass_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateSyncClass_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.RateSyncClass_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.RateSyncClass_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.RateSyncClass_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RateSyncClassId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_RateSyncClass')
BEGIN
    DROP PARTITION SCHEME v2_ps_RateSyncClass;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_RateSyncClass_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_RateSyncClass_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_RateSyncClass_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_RateSyncClass
AS PARTITION v2_pf_RateSyncClass_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RateSyncClassId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.RateSyncClass_Snapshot (
    Id                  BIGINT NOT NULL,
    RateSyncClassId     BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    Code                INT          NOT NULL,
    Name                NVARCHAR(2000) NULL,
    CreationUserName    NVARCHAR(255) NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeUserName  NVARCHAR(255) NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_RateSyncClass_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_RateSyncClass(IsArchive);
GO

ALTER TABLE v2.RateSyncClass_Snapshot
ADD CONSTRAINT DF_RateSyncClass_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_RateSyncClassId) FOR Id;
GO

ALTER TABLE v2.RateSyncClass_Snapshot
ADD CONSTRAINT DF_RateSyncClass_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
