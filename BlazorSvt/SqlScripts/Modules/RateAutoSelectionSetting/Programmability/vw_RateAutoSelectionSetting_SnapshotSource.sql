USE [mdm];
GO

/*
    Проекция snapshot RateAutoSelectionSetting из dbo.vw_RateAutoSelectionSetting.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo RateAutoSelectionSetting = 2122. Примитив = 2101,
    таблица dbo.PrimitiveEntityData_2101. Других источников job нет.
*/

CREATE OR ALTER VIEW v2.vw_RateAutoSelectionSetting_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateAutoSelectionSettingId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        r.DestinationNodeToRegion                                                AS DestinationNodeToRegion,
        r.DestinationNodeToCountry                                               AS DestinationNodeToCountry,
        r.ProductToProductGroup                                                  AS ProductToProductGroup,
        r.AnyTransportKind                                                       AS AnyTransportKind,
        r.CreationDate                                                           AS CreationDate,
        r.LastChangeDate                                                         AS LastChangeDate
    FROM dbo.vw_RateAutoSelectionSetting r (NOLOCK);
GO
