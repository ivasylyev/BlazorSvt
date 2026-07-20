USE [mdm];
GO

/*
    Первичная полная заливка snapshot ParityRates.
    Источник — v2.vw_ParityRates_SnapshotSource.
*/

INSERT INTO v2.ParityRates_Snapshot (
     ParityRatesId
    ,IsArchive
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate
    ,Code
    ,RelevanceId
    ,TransportTypeId
    ,CurrencyId
    ,TotalCostTon
    ,TotalCostTransport
    ,LoadOfTransport
    ,Level_Danger_Product
    ,FactRate
    ,BusinessPlanningRate
    ,CurrencyCode
    ,TransportTypeCode
    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,ProxyNode1Code
    ,ProxyNode1NameEn
    ,ProxyNode1NameRu
    ,ProxyNode2Code
    ,ProxyNode2NameEn
    ,ProxyNode2NameRu
    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductCode
    ,ProductNameRu
    ,ProductNameEn
    ,Comment
    ,DataSource
    ,DepartmentResponsibilityArea
    ,EmployeeResponsibilityArea
    ,Methodology
    ,PriorityText
    ,NodeFromId
    ,NodeToId
    ,ProxyNode1Id
    ,ProxyNode2Id
    ,ProductGroupId
    ,ProductId
)
SELECT
     ParityRatesId
    ,IsArchive
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate
    ,Code
    ,RelevanceId
    ,TransportTypeId
    ,CurrencyId
    ,TotalCostTon
    ,TotalCostTransport
    ,LoadOfTransport
    ,Level_Danger_Product
    ,FactRate
    ,BusinessPlanningRate
    ,CurrencyCode
    ,TransportTypeCode
    ,NodeFromCode
    ,NodeFromNameEn
    ,NodeFromNameRu
    ,ProxyNode1Code
    ,ProxyNode1NameEn
    ,ProxyNode1NameRu
    ,ProxyNode2Code
    ,ProxyNode2NameEn
    ,ProxyNode2NameRu
    ,NodeToCode
    ,NodeToNameEn
    ,NodeToNameRu
    ,ProductGroupCode
    ,ProductGroupNameRu
    ,ProductGroupNameEn
    ,ProductCode
    ,ProductNameRu
    ,ProductNameEn
    ,Comment
    ,DataSource
    ,DepartmentResponsibilityArea
    ,EmployeeResponsibilityArea
    ,Methodology
    ,PriorityText
    ,NodeFromId
    ,NodeToId
    ,ProxyNode1Id
    ,ProxyNode2Id
    ,ProductGroupId
    ,ProductId
FROM v2.vw_ParityRates_SnapshotSource;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2109'   -- ParityRates (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1013'  -- ProductGroup
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1015'  -- MTR (Product)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'ParityRates' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO