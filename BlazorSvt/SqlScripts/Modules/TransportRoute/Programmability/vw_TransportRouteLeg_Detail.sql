USE [mdm];
GO

/*
    Таблица «Плечо маршрута» на карточке TransportRoute.
    Источник — dbo.vw_TransportRouteLeg (dbo.PrimitiveEntityData_2035), дочерний примитив TransportRoute.
    Поля плеча — dbo.vw_TransportLeg: код, имена узлов без кода, enum вида транспорта и типов плеча.
    Отбор по TransportRouteId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_TransportRouteLeg_Detail
AS
    SELECT
        CAST(rl.TransportRouteId AS BIGINT)     AS TransportRouteId,
        CAST(rl.Id AS BIGINT)                   AS Id,
        rl.PrimitiveEntityDataStateId           AS PrimitiveEntityDataStateId,
        rl.Code                                 AS Code,
        rl.Step                                 AS Step,
        LEFT(leg.Code, 50)                      AS LegCode,
        nf.Name_ru                              AS NodeFromNameRu,
        nf.Name_en                              AS NodeFromNameEn,
        np.Name_ru                              AS ProxyNodeNameRu,
        np.Name_en                              AS ProxyNodeNameEn,
        nt.Name_ru                              AS NodeToNameRu,
        nt.Name_en                              AS NodeToNameEn,
        CAST(leg.TransportKind AS BIGINT)       AS TransportKindIdRu,
        CAST(leg.TransportKind AS BIGINT)       AS TransportKindIdEn,
        CAST(leg.Leg1_TransportType AS BIGINT)  AS Leg1_TransportTypeIdRu,
        CAST(leg.Leg1_TransportType AS BIGINT)  AS Leg1_TransportTypeIdEn,
        CAST(leg.Leg2_TransportType AS BIGINT)  AS Leg2_TransportTypeIdRu,
        CAST(leg.Leg2_TransportType AS BIGINT)  AS Leg2_TransportTypeIdEn
    FROM dbo.vw_TransportRouteLeg rl (NOLOCK)
    LEFT JOIN dbo.vw_TransportLeg leg (NOLOCK)
        ON leg.Id = rl.Leg
    LEFT JOIN dbo.vw_LocationsNodes nf (NOLOCK)
        ON nf.Id = leg.NodeFrom
    LEFT JOIN dbo.vw_LocationsNodes np (NOLOCK)
        ON np.Id = leg.ProxyNode
    LEFT JOIN dbo.vw_LocationsNodes nt (NOLOCK)
        ON nt.Id = leg.NodeTo;
GO
