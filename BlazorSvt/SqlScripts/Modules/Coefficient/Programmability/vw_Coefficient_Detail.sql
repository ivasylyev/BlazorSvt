USE [mdm];
GO

/*
    Карточка и полный Excel Coefficient.
    Источник — dbo.vw_Coefficient (dbo.PrimitiveEntityData_2081).
    Дочерние CoefficientConditionValue и CoefficientComponentValue — свои представления
    v2.vw_CoefficientConditionValue_Detail и v2.vw_CoefficientComponentValue_Detail;
    на карточке это таблицы, в полный Excel не входят.
*/

CREATE OR ALTER VIEW v2.vw_Coefficient_Detail
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS CoefficientId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate,
        TRY_CAST(c.Code AS INT)                                                 AS Code,

        CAST(c.[Type] AS BIGINT)                                                 AS CoefficientTypeIdRu,
        CAST(c.[Type] AS BIGINT)                                                 AS CoefficientTypeIdEn,

        LEFT(c.Name, 50)                                                         AS Name,
        CAST(c.StartDate AS DATE)                                                AS StartDate,
        CAST(c.FinishDate AS DATE)                                               AS FinishDate,
        ISNULL(c.Active, 0)                                                      AS Active,
        ISNULL(c.OverallValue, 0)                                                AS OverallValue,

        CAST(c.RateType AS BIGINT)                                               AS RateTypeIdRu,
        CAST(c.RateType AS BIGINT)                                               AS RateTypeIdEn
    FROM dbo.vw_Coefficient c (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO
