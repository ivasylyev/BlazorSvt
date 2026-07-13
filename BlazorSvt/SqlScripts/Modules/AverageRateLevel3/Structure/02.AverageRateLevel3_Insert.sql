USE [mdm];
GO

INSERT INTO v2.AverageRateLevel3_Snapshot (
     AverageRateLevel3Id
    ,IsArchive
    ,IsDefRate
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate
    ,RateLevel3
    ,EffectiveLoadOfTransportType
    ,TransportKindId
    ,TransportTypeId
    ,RateTypeId
    ,CurrencyId
    ,Code
    ,RateTypeCode
    ,ProductCode
    ,CurrencyCode
    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,ProxyNodeCode
    ,ProxyNodeNameEn
    ,ProxyNodeNameRu
    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,TransportKindCode
    ,TransportTypeCode
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductNameRu
    ,ProductNameEn
    ,NodeFromId
    ,NodeToId
    ,ProxyNodeId
    ,ProductGroupId
    ,ProductId
)
SELECT
     AverageRateLevel3Id
    ,IsArchive
    ,IsDefRate
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate
    ,RateLevel3
    ,EffectiveLoadOfTransportType
    ,TransportKindId
    ,TransportTypeId
    ,RateTypeId
    ,CurrencyId
    ,Code
    ,RateTypeCode
    ,ProductCode
    ,CurrencyCode
    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,ProxyNodeCode
    ,ProxyNodeNameEn
    ,ProxyNodeNameRu
    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,TransportKindCode
    ,TransportTypeCode
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductNameRu
    ,ProductNameEn
    ,NodeFromId
    ,NodeToId
    ,ProxyNodeId
    ,ProductGroupId
    ,ProductId
FROM v2.vw_AverageRateLevel3_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2057'   -- AverageRateLevel3 (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1013'  -- ProductGroup
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1015'  -- MTR (Product)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'AverageRateLevel3' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
