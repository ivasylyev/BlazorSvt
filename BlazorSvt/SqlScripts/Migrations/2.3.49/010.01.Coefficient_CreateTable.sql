USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Coefficient_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Coefficient_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Coefficient_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Coefficient_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Coefficient_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_CoefficientId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Coefficient')
BEGIN
    DROP PARTITION SCHEME v2_ps_Coefficient;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Coefficient_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Coefficient_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Coefficient_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Coefficient
AS PARTITION v2_pf_Coefficient_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_CoefficientId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Coefficient_Snapshot (
    Id                      BIGINT NOT NULL,
    CoefficientId           BIGINT NOT NULL,
    Code                    NVARCHAR(50) NOT NULL,
    IsArchive               BIT NOT NULL,

    CoefficientTypeId       BIGINT NULL,
    CoefficientTypeCode     NVARCHAR(50) NULL,
    CoefficientTypeNameRu   NVARCHAR(50) NULL,

    Name                    NVARCHAR(50) NULL,
    StartDate               DATETIME NULL,
    FinishDate              DATETIME NULL,
    Active                  BIT NOT NULL,
    OverallValue            BIT NOT NULL,

    RateTypeId              BIGINT NULL,
    RateTypeCode            NVARCHAR(50) NULL,
    RateTypeNameRu          NVARCHAR(100) NULL,

    CreationDate            DATETIME NOT NULL,
    LastChangeDate          DATETIME NOT NULL,

    CONSTRAINT PK_Coefficient_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Coefficient(IsArchive);
GO

ALTER TABLE v2.Coefficient_Snapshot
ADD CONSTRAINT DF_Coefficient_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_CoefficientId) FOR Id;
GO

ALTER TABLE v2.Coefficient_Snapshot
ADD CONSTRAINT DF_Coefficient_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
