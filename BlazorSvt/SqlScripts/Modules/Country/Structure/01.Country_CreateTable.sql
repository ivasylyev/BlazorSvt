USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Country_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Country_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Country_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Country_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Country_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_CountryId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Country')
BEGIN
    DROP PARTITION SCHEME v2_ps_Country;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Country_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Country_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Country_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Country
AS PARTITION v2_pf_Country_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_CountryId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Country_Snapshot (
    Code                INT          NOT NULL,
    NameRu              NVARCHAR(4000) NULL,
    ISOCode             INT NULL,
    NameEn              NVARCHAR(4000) NULL,
    PlanCode            NVARCHAR(4000) NULL,
    Market              NVARCHAR(4000) NULL,
    ISOCode3            NVARCHAR(4000) NULL,
    StatusId            BIGINT NULL,
    Id                  BIGINT NOT NULL,
    CountryId           BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NULL,

    CONSTRAINT PK_Country_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Country(IsArchive);
GO

ALTER TABLE v2.Country_Snapshot
ADD CONSTRAINT DF_Country_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_CountryId) FOR Id;
GO

ALTER TABLE v2.Country_Snapshot
ADD CONSTRAINT DF_Country_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
