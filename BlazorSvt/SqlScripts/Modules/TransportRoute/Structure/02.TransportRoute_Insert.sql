USE [mdm];
GO

/*
    Первичная полная заливка snapshot TransportRoute
    из v2.vw_TransportRoute_SnapshotSource.

    Курсор v2.SyncState ставится после RowVer на dbo.PrimitiveEntityData_2010.
    1014 и 1008 уже имеют RowVer.
    TransportKind (2008) и TransportType_level_3 (2023) в курсор не входят.
*/

INSERT INTO v2.TransportRoute_Snapshot (
     Code
    ,CodeSAP
    ,Route
    ,StartNodeId
    ,StartNodeCode
    ,StartNodeNameRu
    ,StartNodeNameEn
    ,RegionFromId
    ,RegionFromCode
    ,RegionFromNameRu
    ,RegionFromNameEn
    ,ProxyNodeId
    ,ProxyNodeCode
    ,ProxyNodeNameRu
    ,ProxyNodeNameEn
    ,EndNodeId
    ,EndNodeCode
    ,EndNodeNameRu
    ,EndNodeNameEn
    ,RegionToId
    ,RegionToCode
    ,RegionToNameRu
    ,RegionToNameEn
    ,RouteTransportKindId
    ,RouteTransportKindCode
    ,RouteTransportKindName
    ,RouteTransportKindNameEnRu
    ,IsActive
    ,Leg1_TransportTypeId
    ,Leg1_TransportTypeCode
    ,Leg1_TransportTypeName
    ,Leg1_TransportTypeNameEnRu
    ,Leg2_TransportTypeId
    ,Leg2_TransportTypeCode
    ,Leg2_TransportTypeName
    ,Leg2_TransportTypeNameEnRu
    ,ChainNodes
    ,TransportRouteId
    ,IsArchive
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
)
SELECT
     Code
    ,CodeSAP
    ,Route
    ,StartNodeId
    ,StartNodeCode
    ,StartNodeNameRu
    ,StartNodeNameEn
    ,RegionFromId
    ,RegionFromCode
    ,RegionFromNameRu
    ,RegionFromNameEn
    ,ProxyNodeId
    ,ProxyNodeCode
    ,ProxyNodeNameRu
    ,ProxyNodeNameEn
    ,EndNodeId
    ,EndNodeCode
    ,EndNodeNameRu
    ,EndNodeNameEn
    ,RegionToId
    ,RegionToCode
    ,RegionToNameRu
    ,RegionToNameEn
    ,RouteTransportKindId
    ,RouteTransportKindCode
    ,RouteTransportKindName
    ,RouteTransportKindNameEnRu
    ,IsActive
    ,Leg1_TransportTypeId
    ,Leg1_TransportTypeCode
    ,Leg1_TransportTypeName
    ,Leg1_TransportTypeNameEnRu
    ,Leg2_TransportTypeId
    ,Leg2_TransportTypeCode
    ,Leg2_TransportTypeName
    ,Leg2_TransportTypeNameEnRu
    ,ChainNodes
    ,TransportRouteId
    ,IsArchive
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
FROM v2.vw_TransportRoute_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2010'   -- TransportRoute (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1008'  -- Region
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'TransportRoute' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
