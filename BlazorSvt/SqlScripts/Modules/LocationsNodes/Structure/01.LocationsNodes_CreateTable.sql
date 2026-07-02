USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.LocationNodesSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.LocationNodesSnapshot;
END
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.LocationsNodes_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.LocationsNodes_Snapshot;
END
GO

DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'DROP INDEX IF EXISTS [' + name + '] ON v2.LocationNodesSnapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.LocationNodesSnapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql;
GO

DECLARE @sql_new NVARCHAR(MAX) = '';
SELECT @sql_new = @sql_new + 'DROP INDEX IF EXISTS [' + name + '] ON v2.LocationsNodes_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.LocationsNodes_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.LocationNodesSnapshot;
GO

DROP TABLE IF EXISTS v2.LocationsNodes_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_LocationsNodesId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_LocationsNodes')
BEGIN
    DROP PARTITION SCHEME v2_ps_LocationsNodes;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_LocationsNodes_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_LocationsNodes_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_LocationsNodes_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_LocationsNodes
AS PARTITION v2_pf_LocationsNodes_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_LocationsNodesId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.LocationsNodes_Snapshot (
    Id                      BIGINT NOT NULL,
    LocationsNodesId        BIGINT NOT NULL,
    Code                    NVARCHAR(50) NOT NULL,

    IsArchive               BIT NOT NULL,

    NameRu                  NVARCHAR(100) NULL,
    NameEn                  NVARCHAR(100) NULL,

    LocationTypeId          BIGINT NULL,
    LocationTypeCode        NVARCHAR(50) NULL,
    LocationTypeNameRu      NVARCHAR(100) NULL,
    LocationTypeNameEn      NVARCHAR(100) NULL,

    TypeNodeId              BIGINT NULL,
    TypeNodeCode            NVARCHAR(50) NULL,
    TypeNodeNameRu          NVARCHAR(100) NULL,
    TypeNodeNameEn          NVARCHAR(100) NULL,

    RegionId                BIGINT NULL,
    RegionCode              NVARCHAR(50) NULL,
    RegionNameRu            NVARCHAR(100) NULL,
    RegionNameEn            NVARCHAR(100) NULL,
    RegionRU                NVARCHAR(100) NULL,

    CountryId               BIGINT NULL,
    CountryNameRu           NVARCHAR(100) NULL,
    CountryNameEn           NVARCHAR(100) NULL,

    CreationDate            DATETIME NOT NULL,
    LastChangeDate          DATETIME NOT NULL,

    CONSTRAINT PK_LocationsNodes_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_LocationsNodes(IsArchive);
GO

ALTER TABLE v2.LocationsNodes_Snapshot
ADD CONSTRAINT DF_LocationsNodes_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_LocationsNodesId) FOR Id;
GO

ALTER TABLE v2.LocationsNodes_Snapshot
ADD CONSTRAINT DF_LocationsNodes_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
