USE [mdm];
GO

/*
    Первичная полная заливка snapshot TransportRate.

    Источник — проекция v2.vw_TransportRate_SnapshotSource (тот же SELECT и
    WHERE-фильтр, что использует инкрементальная синхронизация), поэтому полная
    пересборка и инкремент дают идентичный результат.

    Требует предварительно созданной вью:
        Programmability/vw_TransportRate_SnapshotSource.sql
*/

INSERT INTO v2.TransportRate_Snapshot (
     TransportRateId
    ,IsArchive
    ,IsDefRate
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate

    ,TotalCostTon
    ,TotalCostTransport

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
     TransportRateId
    ,IsArchive
    ,IsDefRate
    ,StartDate
    ,EndDate
    ,CreationDate
    ,LastChangeDate

    ,TotalCostTon
    ,TotalCostTransport

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
FROM v2.vw_TransportRate_SnapshotSource;
GO

/*
    Инициализация курсоров синхронизации на текущую границу версий.
    После полной заливки инкремент должен подхватывать только изменения,
    случившиеся ПОСЛЕ первичной загрузки.

    @Hi = наибольшая гарантированно закоммиченная версия
        = MIN_ACTIVE_ROWVERSION() - 1.
*/
IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    ;WITH Sources (SourceName) AS (
        SELECT N'dbo.PrimitiveEntityData_2012'   -- TransportRate (основная)
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1014'  -- LocationsNodes
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1013'  -- ProductGroup
        UNION ALL SELECT N'dbo.PrimitiveEntityData_1015'  -- MTR (Product)
    )
    MERGE v2.SyncState AS tgt
    USING (SELECT N'TransportRate' AS Entity, SourceName FROM Sources) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @Hi, LastRunUtc = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
