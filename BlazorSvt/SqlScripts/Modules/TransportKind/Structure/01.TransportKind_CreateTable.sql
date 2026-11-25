USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportKind_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportKind_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.TransportKind_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.TransportKind_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.TransportKind_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_TransportKindId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_TransportKind')
BEGIN
    DROP PARTITION SCHEME v2_ps_TransportKind;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_TransportKind_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_TransportKind_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_TransportKind_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_TransportKind
AS PARTITION v2_pf_TransportKind_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_TransportKindId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.TransportKind_Snapshot (
    Code                NVARCHAR(50) NOT NULL,
    Name                NVARCHAR(4000) NULL,
    NameEnRu            NVARCHAR(4000) NULL,
    StatusId            BIGINT NULL,
    Level               NVARCHAR(4000) NULL,
    KindCode            NVARCHAR(4000) NULL,
    Id                  BIGINT NOT NULL,
    TransportKindId     BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    CreationUserName    NVARCHAR(255) NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeUserName  NVARCHAR(255) NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_TransportKind_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_TransportKind(IsArchive);
GO

ALTER TABLE v2.TransportKind_Snapshot
ADD CONSTRAINT DF_TransportKind_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_TransportKindId) FOR Id;
GO

ALTER TABLE v2.TransportKind_Snapshot
ADD CONSTRAINT DF_TransportKind_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
