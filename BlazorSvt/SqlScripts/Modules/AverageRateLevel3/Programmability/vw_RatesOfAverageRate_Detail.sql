USE [mdm];
GO

/*
    Таблица «Ставки, вошедшие в расчет» на карточке AverageRateLevel3.
    Источник — dbo.vw_RatesOfAverageRate (dbo.PrimitiveEntityData_2058), дочерний примитив AverageRateLevel3.
    Код ставки и стоимости — dbo.vw_TransportRate.
    Отбор по AverageRateLevel3Id и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_RatesOfAverageRate_Detail
AS
    SELECT
        CAST(r.AverageRateLevel3Id AS BIGINT)        AS AverageRateLevel3Id,
        CAST(r.Id AS BIGINT)                         AS Id,
        r.PrimitiveEntityDataStateId                 AS PrimitiveEntityDataStateId,
        LEFT(r.Code, 50)                             AS Code,
        LEFT(tr.Code, 50)                            AS TransportRateCode,
        CAST(tr.TotalCostTon AS DECIMAL(19, 2))      AS TotalCostTon,
        CAST(tr.TotalCostTransport AS DECIMAL(19, 2)) AS TotalCostTransport
    FROM dbo.vw_RatesOfAverageRate r (NOLOCK)
    LEFT JOIN dbo.vw_TransportRate tr (NOLOCK)
        ON tr.Id = r.Rate;
GO
