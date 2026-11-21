USE [mdm];
GO

/*
    Таблица «Средневзвешенные ставки 3 уровня» на карточке TransportLeg.
    Источник — dbo.vw_LegAverageRateLevel3 (dbo.PrimitiveEntityData_2059), дочерний примитив TransportLeg.
    Поля цели — dbo.vw_AverageRateLevel3. Группа продуктов — dbo.vw_ProductGroup. Продукт — dbo.vw_MTR.
    Отбор по TransportLegId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_LegAverageRateLevel3_Detail
AS
    SELECT
        CAST(lr.TransportLegId AS BIGINT)                  AS TransportLegId,
        CAST(lr.Id AS BIGINT)                              AS Id,
        lr.PrimitiveEntityDataStateId                      AS PrimitiveEntityDataStateId,
        LEFT(lr.Code, 50)                                  AS Code,
        LEFT(ar.Code, 50)                                  AS AverageRateCode,
        CAST(ar.IsDefRate AS BIT)                          AS IsDefRate,
        CAST(ar.RateType AS BIGINT)                        AS RateTypeIdRu,
        CAST(ar.RateType AS BIGINT)                        AS RateTypeIdEn,
        CAST(ar.TransportType AS BIGINT)                   AS TransportTypeIdRu,
        CAST(ar.TransportType AS BIGINT)                   AS TransportTypeIdEn,
        pg.Name                                            AS ProductGroupNameRu,
        pg.NameEn                                          AS ProductGroupNameEn,
        m.NameShort_ru                                     AS ProductNameRu,
        m.NameShort_en                                     AS ProductNameEn,
        CAST(ar.StartDate AS DATE)                         AS StartDate,
        CAST(ar.EndDate AS DATE)                           AS EndDate,
        CAST(ar.RateLevel3 AS DECIMAL(15, 2))              AS RateLevel3,
        CAST(ar.EffectiveLoadOfTransportType AS DECIMAL(10, 3)) AS EffectiveLoadOfTransportType,
        CAST(ar.CurrencyStandard AS BIGINT)                AS CurrencyId
    FROM dbo.vw_LegAverageRateLevel3 lr (NOLOCK)
    LEFT JOIN dbo.vw_AverageRateLevel3 ar (NOLOCK)
        ON ar.Id = lr.Rate
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK)
        ON pg.Id = ar.ProductGroup
    LEFT JOIN dbo.vw_MTR m (NOLOCK)
        ON m.Id = ar.Product;
GO
