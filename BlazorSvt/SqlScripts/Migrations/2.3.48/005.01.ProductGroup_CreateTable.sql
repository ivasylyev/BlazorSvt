USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ProductGroup_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ProductGroup_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.ProductGroup_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.ProductGroup_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.ProductGroup_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_ProductGroupId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_ProductGroup')
BEGIN
    DROP PARTITION SCHEME v2_ps_ProductGroup;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_ProductGroup_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_ProductGroup_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_ProductGroup_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_ProductGroup
AS PARTITION v2_pf_ProductGroup_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_ProductGroupId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.ProductGroup_Snapshot (
    Id                                          BIGINT NOT NULL,
    ProductGroupId                              BIGINT NOT NULL,
    Code                                        NVARCHAR(50) NOT NULL,

    IsArchive                                   BIT NOT NULL,

    Name                                        NVARCHAR(4000) NULL,
    ShortName                                   NVARCHAR(4000) NULL,
    GroupCode                                   NVARCHAR(4000) NULL,
    ProductGroupCBD                             NVARCHAR(4000) NULL,
    NameEnCode                                  NVARCHAR(4000) NULL,
    NameEnRu                                    NVARCHAR(4000) NULL,
    Code_NSI                                    NVARCHAR(50) NULL,
    SVTUseProductGroupWhenNoProductRateExists   BIT NULL,
    NameEn                                      NVARCHAR(4000) NULL,

    RateSyncClassId                             BIGINT NULL,
    RateSyncClassCode                           NVARCHAR(50) NULL,
    RateSyncClassNameRu                         NVARCHAR(2000) NULL,

    CreationDate                                DATETIME NOT NULL,
    LastChangeDate                              DATETIME NOT NULL,

    CONSTRAINT PK_ProductGroup_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_ProductGroup(IsArchive);
GO

ALTER TABLE v2.ProductGroup_Snapshot
ADD CONSTRAINT DF_ProductGroup_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_ProductGroupId) FOR Id;
GO

ALTER TABLE v2.ProductGroup_Snapshot
ADD CONSTRAINT DF_ProductGroup_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
