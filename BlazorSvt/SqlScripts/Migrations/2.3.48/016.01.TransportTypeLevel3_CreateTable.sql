USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportTypeLevel3_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportTypeLevel3_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.TransportTypeLevel3_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.TransportTypeLevel3_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.TransportTypeLevel3_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_TransportTypeLevel3Id;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_TransportTypeLevel3')
BEGIN
    DROP PARTITION SCHEME v2_ps_TransportTypeLevel3;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_TransportTypeLevel3_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_TransportTypeLevel3_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_TransportTypeLevel3_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_TransportTypeLevel3
AS PARTITION v2_pf_TransportTypeLevel3_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_TransportTypeLevel3Id AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.TransportTypeLevel3_Snapshot (
    Id                      INT NOT NULL,
    TransportTypeLevel3Id   BIGINT NOT NULL,
    IsArchive               BIT NOT NULL,
    IsBasic_forCBD          BIT NULL,
    Code                    NVARCHAR(100) NOT NULL,
    TransportKindId         INT NOT NULL,
    TransportTypeId         INT NOT NULL,
    Name                    NVARCHAR(4000) NULL,
    NameEnRu                NVARCHAR(4000) NULL,
    TypeCode                NVARCHAR(4000) NULL,
    SAPCode                 NVARCHAR(4000) NULL,
    OrionCode               NVARCHAR(4000) NULL,
    StatusId                INT NOT NULL,
    CreationDate            DATETIME NOT NULL,
    LastChangeDate          DATETIME NOT NULL,

    CONSTRAINT PK_TransportTypeLevel3_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_TransportTypeLevel3(IsArchive);
GO

ALTER TABLE v2.TransportTypeLevel3_Snapshot
ADD CONSTRAINT DF_TransportTypeLevel3_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_TransportTypeLevel3Id) FOR Id;
GO

ALTER TABLE v2.TransportTypeLevel3_Snapshot
ADD CONSTRAINT DF_TransportTypeLevel3_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
