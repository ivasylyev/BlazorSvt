USE [mdm];
GO

/*
    Таблица «Составляющая по коэффициенту» на карточке Coefficient.
    Источник — dbo.vw_CoefficientComponentValue (dbo.PrimitiveEntityData_2083), дочерний примитив Coefficient.
    Отбор по CoefficientId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_CoefficientComponentValue_Detail
AS
    SELECT
        CAST(cv.CoefficientId AS BIGINT)        AS CoefficientId,
        CAST(cv.Id AS BIGINT)                   AS Id,
        cv.PrimitiveEntityDataStateId           AS PrimitiveEntityDataStateId,
        LEFT(cv.Code, 50)                       AS Code,
        cc.Name                                 AS ComponentName,
        CAST(cv.[Value] AS DECIMAL(19, 4))      AS [Value]
    FROM dbo.vw_CoefficientComponentValue cv (NOLOCK)
    LEFT JOIN dbo.vw_CoefficientComponent cc (NOLOCK)
        ON cc.Id = cv.Component;
GO
