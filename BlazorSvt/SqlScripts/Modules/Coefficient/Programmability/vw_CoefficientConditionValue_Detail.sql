USE [mdm];
GO

/*
    Таблица «Условие по коэффициенту» на карточке Coefficient.
    Источник — dbo.vw_CoefficientConditionValue (dbo.PrimitiveEntityData_2080), дочерний примитив Coefficient.
    Отбор по CoefficientId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_CoefficientConditionValue_Detail
AS
    SELECT
        CAST(cv.CoefficientId AS BIGINT)        AS CoefficientId,
        CAST(cv.Id AS BIGINT)                   AS Id,
        cv.PrimitiveEntityDataStateId           AS PrimitiveEntityDataStateId,
        LEFT(cv.Code, 50)                       AS Code,
        CAST(cv.[Except] AS BIT)                AS [Except],

        CAST(cv.TransportKind AS BIGINT)        AS TransportKindIdRu,
        CAST(cv.TransportKind AS BIGINT)        AS TransportKindIdEn,

        CAST(cv.TransportTypeLevel2 AS BIGINT)  AS TransportTypeLevel2IdRu,
        CAST(cv.TransportTypeLevel2 AS BIGINT)  AS TransportTypeLevel2IdEn,

        vt.Name                                 AS VehicleTypeName,

        nf.Name_ru                              AS FromLocationNameRu,
        nf.Name_en                              AS FromLocationNameEn,
        nt.Name_ru                              AS ToLocationNameRu,
        nt.Name_en                              AS ToLocationNameEn
    FROM dbo.vw_CoefficientConditionValue cv (NOLOCK)
    LEFT JOIN dbo.vw_VehicleType vt (NOLOCK)
        ON vt.Id = cv.VehicleType
    LEFT JOIN dbo.vw_LocationsNodes nf (NOLOCK)
        ON nf.Id = cv.FromLocation
    LEFT JOIN dbo.vw_LocationsNodes nt (NOLOCK)
        ON nt.Id = cv.ToLocation;
GO
