USE [mdm];
GO

/*
    Таблица «Применённые коэффициенты» на карточке TransportRate.
    Источник — dbo.vw_TransportRateCoefficient (dbo.PrimitiveEntityData_2106), дочерний примитив TransportRate.
    Код ставки с коэффициентом и исходной ставки — dbo.vw_TransportRate.Code.
    Имя коэффициента — dbo.vw_Coefficient.Name. Вид исходной ставки — dbo.vw_RateKind.Name.
    Отбор по TransportRateId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_TransportRateCoefficient_Detail
AS
    SELECT
        CAST(c.TransportRateId AS BIGINT)       AS TransportRateId,
        CAST(c.Id AS BIGINT)                    AS Id,
        c.PrimitiveEntityDataStateId            AS PrimitiveEntityDataStateId,
        LEFT(c.Code, 50)                        AS Code,
        LEFT(cr.Code, 50)                       AS CoefficientRateCode,
        CAST(c.StartDate AS DATE)               AS StartDate,
        CAST(c.EndDate AS DATE)                 AS EndDate,
        LEFT(cf.Name, 50)                       AS CoefficientName,
        LEFT(sr.Code, 50)                       AS SourceRateCode,
        LEFT(rk.Name, 50)                       AS SourceRateKindName
    FROM dbo.vw_TransportRateCoefficient c (NOLOCK)
    LEFT JOIN dbo.vw_TransportRate cr (NOLOCK)
        ON cr.Id = c.CoefficientRate
    LEFT JOIN dbo.vw_Coefficient cf (NOLOCK)
        ON cf.Id = c.Coefficient
    LEFT JOIN dbo.vw_TransportRate sr (NOLOCK)
        ON sr.Id = c.SourceRate
    LEFT JOIN dbo.vw_RateKind rk (NOLOCK)
        ON rk.Id = c.SourceRateKind;
GO
