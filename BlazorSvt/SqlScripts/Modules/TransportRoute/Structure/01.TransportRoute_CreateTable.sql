USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportRoute_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportRoute_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.TransportRoute_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.TransportRoute_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.TransportRoute_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_TransportRouteId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_TransportRoute')
BEGIN
    DROP PARTITION SCHEME v2_ps_TransportRoute;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_TransportRoute_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_TransportRoute_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_TransportRoute_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_TransportRoute
AS PARTITION v2_pf_TransportRoute_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_TransportRouteId AS BIGINT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.TransportRoute_Snapshot (
    Code                            NVARCHAR(50) NOT NULL,
    CodeSAP                         NVARCHAR(50) NULL,
    Route                           NVARCHAR(4000) NULL,
    StartNodeId                     BIGINT NULL,
    StartNodeCode                   NVARCHAR(50) NULL,
    StartNodeNameRu                 NVARCHAR(4000) NULL,
    StartNodeNameEn                 NVARCHAR(4000) NULL,
    RegionFromId                    BIGINT NULL,
    RegionFromCode                  NVARCHAR(50) NULL,
    RegionFromNameRu                NVARCHAR(4000) NULL,
    RegionFromNameEn                NVARCHAR(4000) NULL,
    ProxyNodeId                     BIGINT NULL,
    ProxyNodeCode                   NVARCHAR(50) NULL,
    ProxyNodeNameRu                 NVARCHAR(4000) NULL,
    ProxyNodeNameEn                 NVARCHAR(4000) NULL,
    EndNodeId                       BIGINT NULL,
    EndNodeCode                     NVARCHAR(50) NULL,
    EndNodeNameRu                   NVARCHAR(4000) NULL,
    EndNodeNameEn                   NVARCHAR(4000) NULL,
    RegionToId                      BIGINT NULL,
    RegionToCode                    NVARCHAR(50) NULL,
    RegionToNameRu                  NVARCHAR(4000) NULL,
    RegionToNameEn                  NVARCHAR(4000) NULL,
    RouteTransportKindId            BIGINT NULL,
    RouteTransportKindCode          NVARCHAR(50) NULL,
    RouteTransportKindName          NVARCHAR(4000) NULL,
    RouteTransportKindNameEnRu      NVARCHAR(4000) NULL,
    IsActive                        BIT NULL,
    Leg1_TransportTypeId            BIGINT NULL,
    Leg1_TransportTypeCode          NVARCHAR(50) NULL,
    Leg1_TransportTypeName          NVARCHAR(4000) NULL,
    Leg1_TransportTypeNameEnRu      NVARCHAR(4000) NULL,
    Leg2_TransportTypeId            BIGINT NULL,
    Leg2_TransportTypeCode          NVARCHAR(50) NULL,
    Leg2_TransportTypeName          NVARCHAR(4000) NULL,
    Leg2_TransportTypeNameEnRu      NVARCHAR(4000) NULL,
    ChainNodes                      NVARCHAR(4000) NULL,
    Id                              BIGINT NOT NULL,
    TransportRouteId                BIGINT NOT NULL,
    IsArchive                       BIT NOT NULL,
    CreationDate                    DATETIME NOT NULL,
    LastChangeDate                  DATETIME NOT NULL,

    CONSTRAINT PK_TransportRoute_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_TransportRoute(IsArchive);
GO

ALTER TABLE v2.TransportRoute_Snapshot
ADD CONSTRAINT DF_TransportRoute_Snapshot_Id
    DEFAULT (NEXT VALUE FOR v2.seq_TransportRouteId) FOR Id;
GO

ALTER TABLE v2.TransportRoute_Snapshot
ADD CONSTRAINT DF_TransportRoute_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
