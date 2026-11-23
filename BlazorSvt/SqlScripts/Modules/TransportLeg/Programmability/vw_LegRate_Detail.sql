USE [mdm];
GO

/*
    Таблица «Ставки плеча» на карточке TransportLeg.
    Источник — dbo.vw_LegRate (dbo.PrimitiveEntityData_2044), дочерний примитив TransportLeg.
    Поля цели — dbo.vw_TransportRate. Группа продуктов — dbo.vw_ProductGroup. Продукт — dbo.vw_MTR.
    Отбор по TransportLegId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_LegRate_Detail
AS
    SELECT
        CAST(lr.TransportLegId AS BIGINT)             AS TransportLegId,
        CAST(lr.Id AS BIGINT)                         AS Id,
        lr.PrimitiveEntityDataStateId                 AS PrimitiveEntityDataStateId,
        LEFT(lr.Code, 50)                             AS Code,
        LEFT(tr.Code, 50)                             AS RateCode,
        ISNULL(CAST(tr.IsDefRate AS BIT), 0)          AS IsDefRate,
        CAST(tr.RateType AS BIGINT)                   AS RateTypeIdRu,
        CAST(tr.RateType AS BIGINT)                   AS RateTypeIdEn,
        CAST(tr.TransportType AS BIGINT)              AS TransportTypeIdRu,
        CAST(tr.TransportType AS BIGINT)              AS TransportTypeIdEn,
        pg.Name                                       AS ProductGroupNameRu,
        pg.NameEn                                     AS ProductGroupNameEn,
        m.NameShort_ru                                AS ProductNameRu,
        m.NameShort_en                                AS ProductNameEn,
        CAST(tr.StartDate AS DATE)                    AS StartDate,
        CAST(tr.EndDate AS DATE)                      AS EndDate,
        CAST(tr.TotalCostTon AS DECIMAL(19, 2))       AS TotalCostTon,
        CAST(tr.TotalCostTransport AS DECIMAL(19, 2)) AS TotalCostTransport,
        CAST(tr.CurrencyStandard AS BIGINT)           AS CurrencyId
    FROM dbo.vw_LegRate lr (NOLOCK)
    LEFT JOIN dbo.vw_TransportRate tr (NOLOCK)
        ON tr.Id = lr.Rate
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK)
        ON pg.Id = tr.ProductGroup
    LEFT JOIN dbo.vw_MTR m (NOLOCK)
        ON m.Id = tr.Product;
GO
