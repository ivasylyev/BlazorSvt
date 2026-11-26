USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Region_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Region_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Region_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Region_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Region_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RegionId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Region')
BEGIN
    DROP PARTITION SCHEME v2_ps_Region;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Region_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Region_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Region_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Region
AS PARTITION v2_pf_Region_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RegionId AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Region_Snapshot (
    RegionCode              NVARCHAR(4000) NULL,
    Code_NSI                NVARCHAR(255) NULL,
    RegionCodePlanning      NVARCHAR(4000) NULL,
    RegionCodeRF            NVARCHAR(4000) NULL,
    NameRu                  NVARCHAR(4000) NULL,
    NameEn                  NVARCHAR(4000) NULL,
    CountryNameRu           NVARCHAR(4000) NULL,
    CountryNameEn           NVARCHAR(4000) NULL,
    CountryId               BIGINT NULL,
    Market                  NVARCHAR(4000) NULL,
    StatusId                INT NOT NULL,
    FederalDistrictsId      INT NULL,
    FederalDistrictsCode    NVARCHAR(50) NULL,
    FederalDistrictsNameRu  NVARCHAR(255) NULL,
    FederalDistrictsNameEn  NVARCHAR(255) NULL,
    Id                      INT NOT NULL,
    RegionId                BIGINT NOT NULL,
    Code                    NVARCHAR(50) NOT NULL,
    IsArchive               BIT NOT NULL,
    CreationUserName        NVARCHAR(255) NULL,
    CreationDate            DATETIME NOT NULL,
    LastChangeUserName      NVARCHAR(255) NULL,
    LastChangeDate          DATETIME NOT NULL,

    CONSTRAINT PK_Region_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Region(IsArchive);
GO

ALTER TABLE v2.Region_Snapshot
ADD CONSTRAINT DF_Region_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_RegionId) FOR Id;
GO

ALTER TABLE v2.Region_Snapshot
ADD CONSTRAINT DF_Region_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
