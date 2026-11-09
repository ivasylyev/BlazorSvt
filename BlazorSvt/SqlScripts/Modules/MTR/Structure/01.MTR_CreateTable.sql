USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.MTR_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.MTR_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.MTR_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.MTR_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.MTR_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_MTRId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_MTR')
BEGIN
    DROP PARTITION SCHEME v2_ps_MTR;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_MTR_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_MTR_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_MTR_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_MTR
AS PARTITION v2_pf_MTR_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_MTRId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.MTR_Snapshot (
    Id                   BIGINT NOT NULL,
    MTRId                BIGINT NOT NULL,
    Code                 NVARCHAR(50) NOT NULL,
    IsArchive            BIT NOT NULL,

    NameFullRu           NVARCHAR(4000) NULL,
    NameShortEn          NVARCHAR(4000) NULL,

    ProductGroupId       BIGINT NULL,
    ProductGroupCode     NVARCHAR(50) NULL,
    ProductGroupNameRu   NVARCHAR(4000) NULL,
    ProductGroupNameEn   NVARCHAR(4000) NULL,

    EtsngId              BIGINT NULL,
    EtsngNameRu          NVARCHAR(4000) NULL,
    EtsngNameEn          NVARCHAR(4000) NULL,

    GngCode              NVARCHAR(4000) NULL,

    UnitId               BIGINT NULL,
    UnitCode             NVARCHAR(50) NULL,
    UnitNameRu           NVARCHAR(4000) NULL,
    UnitNameEn           NVARCHAR(4000) NULL,

    DpoCode              NVARCHAR(4000) NULL,
    MtrType              NVARCHAR(4000) NULL,

    DangerClassId        BIGINT NULL,
    DangerClassCode      NVARCHAR(50) NULL,
    DangerClassNameRu    NVARCHAR(4000) NULL,

    CreationDate         DATETIME NOT NULL,
    LastChangeDate       DATETIME NOT NULL,

    CONSTRAINT PK_MTR_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_MTR(IsArchive);
GO

ALTER TABLE v2.MTR_Snapshot
ADD CONSTRAINT DF_MTR_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_MTRId) FOR Id;
GO

ALTER TABLE v2.MTR_Snapshot
ADD CONSTRAINT DF_MTR_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
