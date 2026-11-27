USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.RateAutoSelectionSetting_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.RateAutoSelectionSetting_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.RateAutoSelectionSetting_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.RateAutoSelectionSetting_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.RateAutoSelectionSetting_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_RateAutoSelectionSettingId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_RateAutoSelectionSetting')
BEGIN
    DROP PARTITION SCHEME v2_ps_RateAutoSelectionSetting;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_RateAutoSelectionSetting_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_RateAutoSelectionSetting_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_RateAutoSelectionSetting_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_RateAutoSelectionSetting
AS PARTITION v2_pf_RateAutoSelectionSetting_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_RateAutoSelectionSettingId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.RateAutoSelectionSetting_Snapshot (
    Id                          BIGINT NOT NULL,
    RateAutoSelectionSettingId  BIGINT NOT NULL,
    IsArchive                   BIT NOT NULL,
    Code                        INT          NOT NULL,
    DestinationNodeToRegion     BIT NULL,
    DestinationNodeToCountry    BIT NULL,
    ProductToProductGroup       BIT NULL,
    AnyTransportKind            BIT NULL,
    CreationUserName            NVARCHAR(255) NULL,
    CreationDate                DATETIME NOT NULL,
    LastChangeUserName          NVARCHAR(255) NULL,
    LastChangeDate              DATETIME NULL,

    CONSTRAINT PK_RateAutoSelectionSetting_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_RateAutoSelectionSetting(IsArchive);
GO

ALTER TABLE v2.RateAutoSelectionSetting_Snapshot
ADD CONSTRAINT DF_RateAutoSelectionSetting_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_RateAutoSelectionSettingId) FOR Id;
GO

ALTER TABLE v2.RateAutoSelectionSetting_Snapshot
ADD CONSTRAINT DF_RateAutoSelectionSetting_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
