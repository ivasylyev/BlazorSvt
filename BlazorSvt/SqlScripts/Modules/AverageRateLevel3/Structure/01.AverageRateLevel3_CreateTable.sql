USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.AverageRateLevel3_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.AverageRateLevel3_Snapshot;
END
GO

DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'DROP INDEX IF EXISTS [' + name + '] ON v2.AverageRateLevel3_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.AverageRateLevel3_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql;
GO

DROP TABLE IF EXISTS v2.AverageRateLevel3_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_AverageRateLevel3Id;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_AverageRateLevel3')
BEGIN
    DROP PARTITION SCHEME v2_ps_AverageRateLevel3;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_AverageRateLevel3_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_AverageRateLevel3_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_AverageRateLevel3_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_AverageRateLevel3
AS PARTITION v2_pf_AverageRateLevel3_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_AverageRateLevel3Id AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.AverageRateLevel3_Snapshot (
    Id                          INT NOT NULL,
    AverageRateLevel3Id         INT NOT NULL,
    IsArchive                   BIT NOT NULL,
    IsDefRate                   BIT NOT NULL,
    StartDate                   DATETIME NOT NULL,
    EndDate                     DATETIME NOT NULL,
    CreationDate                DATETIME NOT NULL,
    LastChangeDate              DATETIME NOT NULL,
    RateLevel3                  DECIMAL(15,2) NOT NULL,
    EffectiveLoadOfTransportType DECIMAL(15,2) NOT NULL,
    TransportKindId             INT NOT NULL,
    TransportTypeId             INT NOT NULL,
    RateTypeId                  INT NOT NULL,
    CurrencyId                  INT NOT NULL,
    Code                        INT NOT NULL,
    RateTypeCode                INT NOT NULL,
    ProductCode                 INT NULL,
    CurrencyCode                NVARCHAR(3) NOT NULL,
    NodeFromCode                NVARCHAR(10) NOT NULL,
    NodeFromNameEn              NVARCHAR(30) NOT NULL,
    NodeFromNameRu              NVARCHAR(30) NOT NULL,
    ProxyNodeCode               NVARCHAR(10) NULL,
    ProxyNodeNameEn             NVARCHAR(30) NULL,
    ProxyNodeNameRu             NVARCHAR(30) NULL,
    NodeToCode                  NVARCHAR(10) NOT NULL,
    NodeToNameEn                NVARCHAR(30) NOT NULL,
    NodeToNameRu                NVARCHAR(30) NOT NULL,
    TransportKindCode           NVARCHAR(5) NOT NULL,
    TransportTypeCode           NVARCHAR(20) NOT NULL,
    ProductGroupCode            NVARCHAR(5) NULL,
    ProductGroupNameRu          NVARCHAR(110) NULL,
    ProductGroupNameEn          NVARCHAR(110) NULL,
    ProductNameRu               NVARCHAR(100) NULL,
    ProductNameEn               NVARCHAR(100) NULL,
    NodeFromId                  INT NOT NULL,
    NodeToId                    INT NOT NULL,
    ProxyNodeId                 INT NULL,
    ProductGroupId              INT NULL,
    ProductId                   INT NULL,
    CONSTRAINT PK_AverageRateLevel3_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_AverageRateLevel3(IsArchive);
GO

ALTER TABLE v2.AverageRateLevel3_Snapshot
ADD CONSTRAINT DF_AverageRateLevel3_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_AverageRateLevel3Id) FOR Id;
GO

ALTER TABLE v2.AverageRateLevel3_Snapshot
ADD CONSTRAINT DF_AverageRateLevel3_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
