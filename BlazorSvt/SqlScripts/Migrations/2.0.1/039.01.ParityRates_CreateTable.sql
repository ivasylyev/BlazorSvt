USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ParityRates_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ParityRates_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = '';
SELECT @sql_new = @sql_new + 'DROP INDEX IF EXISTS [' + name + '] ON v2.ParityRates_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.ParityRates_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.ParityRates_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_ParityRatesId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_ParityRates')
BEGIN
    DROP PARTITION SCHEME v2_ps_ParityRates;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_ParityRates_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_ParityRates_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_ParityRates_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_ParityRates
AS PARTITION v2_pf_ParityRates_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_ParityRatesId AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.ParityRates_Snapshot (
    Id                          INT NOT NULL,
    ParityRatesId               INT NOT NULL,
    IsArchive                   BIT NOT NULL,
    StartDate                   DATETIME NOT NULL,
    EndDate                     DATETIME NOT NULL,
    CreationDate                DATETIME NOT NULL,
    LastChangeDate              DATETIME NOT NULL,

    Code                        NVARCHAR(50) NOT NULL,
    RelevanceId                 INT NOT NULL,
    TransportTypeId             INT NOT NULL,
    CurrencyId                  INT NOT NULL,

    TotalCostTon                DECIMAL(15,2) NOT NULL,
    TotalCostTransport          DECIMAL(15,2) NOT NULL,
    LoadOfTransport             DECIMAL(15,2) NOT NULL,
    Level_Danger_Product        DECIMAL(1,0) NULL,
    FactRate                    DECIMAL(19,2) NULL,
    BusinessPlanningRate        DECIMAL(19,2) NULL,

    CurrencyCode                NVARCHAR(3) NOT NULL,
    TransportTypeCode           NVARCHAR(20) NOT NULL,

    NodeFromCode                NVARCHAR(10) NOT NULL,
    NodeFromNameEn              NVARCHAR(30) NOT NULL,
    NodeFromNameRu              NVARCHAR(30) NOT NULL,
    ProxyNode1Code              NVARCHAR(10) NULL,
    ProxyNode1NameEn            NVARCHAR(30) NULL,
    ProxyNode1NameRu            NVARCHAR(30) NULL,
    ProxyNode2Code              NVARCHAR(10) NULL,
    ProxyNode2NameEn            NVARCHAR(30) NULL,
    ProxyNode2NameRu            NVARCHAR(30) NULL,
    NodeToCode                  NVARCHAR(10) NOT NULL,
    NodeToNameEn                NVARCHAR(30) NOT NULL,
    NodeToNameRu                NVARCHAR(30) NOT NULL,

    ProductGroupCode            NVARCHAR(5) NOT NULL,
    ProductGroupNameRu          NVARCHAR(110) NOT NULL,
    ProductGroupNameEn          NVARCHAR(110) NOT NULL,
    ProductCode                 INT NULL,
    ProductNameRu               NVARCHAR(100) NULL,
    ProductNameEn               NVARCHAR(100) NULL,

    Comment                     NVARCHAR(1000) NULL,
    DataSource                  NVARCHAR(4000) NULL,
    DepartmentResponsibilityArea NVARCHAR(4000) NULL,
    EmployeeResponsibilityArea  NVARCHAR(4000) NULL,
    Methodology                 NVARCHAR(4000) NULL,
    PriorityText                NVARCHAR(4000) NULL,
    MarketingDataStructure      NVARCHAR(4000) NULL,

    -- Скрытые FK для каскадной синхронизации
    NodeFromId                  INT NOT NULL,
    NodeToId                    INT NOT NULL,
    ProxyNode1Id                INT NULL,
    ProxyNode2Id                INT NULL,
    ProductGroupId              INT NOT NULL,
    ProductId                   INT NULL

    CONSTRAINT PK_ParityRates_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_ParityRates(IsArchive);
GO

ALTER TABLE v2.ParityRates_Snapshot
ADD CONSTRAINT DF_ParityRates_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_ParityRatesId) FOR Id;
GO

ALTER TABLE v2.ParityRates_Snapshot
ADD CONSTRAINT DF_ParityRates_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO