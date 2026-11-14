USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Contractor_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Contractor_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Contractor_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Contractor_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Contractor_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_ContractorId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Contractor')
BEGIN
    DROP PARTITION SCHEME v2_ps_Contractor;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Contractor_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Contractor_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Contractor_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Contractor
AS PARTITION v2_pf_Contractor_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_ContractorId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Contractor_Snapshot (
    Id              BIGINT NOT NULL,
    ContractorId    BIGINT NOT NULL,
    Code            NVARCHAR(50) NOT NULL,
    IsArchive       BIT NOT NULL,

    NameSearch      NVARCHAR(4000) NULL,
    Consignee       BIT NOT NULL,
    IsTrader        BIT NOT NULL,
    IsProcessing    BIT NOT NULL,
    IsSender        BIT NOT NULL,
    IsDebtor        BIT NOT NULL,
    IsCreditor      BIT NOT NULL,
    INN             NVARCHAR(4000) NULL,

    CreationDate    DATETIME NOT NULL,
    LastChangeDate  DATETIME NOT NULL,

    CONSTRAINT PK_Contractor_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Contractor(IsArchive);
GO

ALTER TABLE v2.Contractor_Snapshot
ADD CONSTRAINT DF_Contractor_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_ContractorId) FOR Id;
GO

ALTER TABLE v2.Contractor_Snapshot
ADD CONSTRAINT DF_Contractor_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
