USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.ReportConstructor_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.ReportConstructor_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.ReportConstructor_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.ReportConstructor_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.ReportConstructor_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_ReportConstructorId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_ReportConstructor')
BEGIN
    DROP PARTITION SCHEME v2_ps_ReportConstructor;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_ReportConstructor_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_ReportConstructor_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_ReportConstructor_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_ReportConstructor
AS PARTITION v2_pf_ReportConstructor_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_ReportConstructorId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.ReportConstructor_Snapshot (
    Id                      BIGINT NOT NULL,
    ReportConstructorId     BIGINT NOT NULL,
    IsArchive               BIT NOT NULL,
    Code                    INT          NOT NULL,
    ReportTypeId            BIGINT NOT NULL,
    DateFrom                DATETIME2(7) NULL,
    DateTo                  DATETIME2(7) NULL,
    NodeFromCode            NVARCHAR(50) NULL,
    NodeFromNameRu          NVARCHAR(255) NULL,
    NodeFromNameEn          NVARCHAR(255) NULL,
    NodeFromId              BIGINT NULL,
    NodeToCode              NVARCHAR(50) NULL,
    NodeToNameRu            NVARCHAR(255) NULL,
    NodeToNameEn            NVARCHAR(255) NULL,
    NodeToId                BIGINT NULL,
    TransportKindCode       NVARCHAR(50) NULL,
    TransportKindId         BIGINT NULL,
    TransportTypeCode       NVARCHAR(50) NULL,
    TransportTypeId         BIGINT NULL,
    ProductGroupCode        NVARCHAR(50) NULL,
    ProductGroupNameRu      NVARCHAR(255) NULL,
    ProductGroupNameEn      NVARCHAR(255) NULL,
    ProductGroupId          BIGINT NULL,
    ProductCode             NVARCHAR(50) NULL,
    ProductNameRu           NVARCHAR(255) NULL,
    ProductNameEn           NVARCHAR(255) NULL,
    ProductId               BIGINT NULL,
    CreationDate            DATETIME NOT NULL,
    LastChangeDate          DATETIME NOT NULL,

    CONSTRAINT PK_ReportConstructor_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_ReportConstructor(IsArchive);
GO

ALTER TABLE v2.ReportConstructor_Snapshot
ADD CONSTRAINT DF_ReportConstructor_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_ReportConstructorId) FOR Id;
GO

ALTER TABLE v2.ReportConstructor_Snapshot
ADD CONSTRAINT DF_ReportConstructor_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
