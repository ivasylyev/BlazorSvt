USE [mdm];
GO

/*
    Проекция snapshot Coefficient из dbo.vw_Coefficient — единый источник
    для первичной заливки (02.Coefficient_Insert.sql) и инкрементального MERGE.

    EntityInfo Coefficient = 2108. Примитив Coefficient = 2081,
    таблица dbo.PrimitiveEntityData_2081.

    CoefficientType (2077) и RateType (2048) — короткие словари с enum,
    в Sources не входят. Дочерние CoefficientConditionValue (2080) и
    CoefficientComponentValue (2083) в снимок не входят.
*/

CREATE OR ALTER VIEW v2.vw_Coefficient_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS CoefficientId,
        TRY_CAST(c.Code AS INT)                                                 AS Code,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        CAST(c.[Type] AS BIGINT)                                                 AS CoefficientTypeId,
        LEFT(ct.Code, 50)                                                        AS CoefficientTypeCode,
        LEFT(ct.Name, 50)                                                        AS CoefficientTypeNameRu,

        LEFT(c.Name, 50)                                                         AS Name,
        CAST(c.StartDate AS DATETIME)                                            AS StartDate,
        CAST(c.FinishDate AS DATETIME)                                           AS FinishDate,
        ISNULL(c.Active, 0)                                                      AS Active,
        ISNULL(c.OverallValue, 0)                                                AS OverallValue,

        CAST(c.RateType AS BIGINT)                                               AS RateTypeId,
        LEFT(rt.Code, 50)                                                        AS RateTypeCode,
        LEFT(rt.Name, 100)                                                       AS RateTypeNameRu,

        c.CreationDate                                                           AS CreationDate,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Coefficient c (NOLOCK)
    LEFT JOIN dbo.vw_CoefficientType ct (NOLOCK) ON c.[Type] = ct.Id
    LEFT JOIN dbo.vw_RateType rt (NOLOCK) ON c.RateType = rt.Id;
GO
