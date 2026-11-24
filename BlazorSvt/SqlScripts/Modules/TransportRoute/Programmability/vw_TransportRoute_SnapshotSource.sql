USE [mdm];
GO

/*
    Проекция snapshot TransportRoute из dbo.vw_TransportRoute.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo TransportRoute = 2037. Примитив = 2010,
    таблица dbo.PrimitiveEntityData_2010.

    RegionFrom / RegionTo — не колонки view: узел -> LocationsNodes.Region -> vw_Region.
    TransportKind (2008) и TransportType_level_3 (2023) в каскад не входят.
    Плечи Leg_01..03 в снимок не денормализуются.
*/

CREATE OR ALTER VIEW v2.vw_TransportRoute_SnapshotSource
AS
    SELECT
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        LEFT(r.CodeSAP, 50)                                                      AS CodeSAP,
        LEFT(r.Route, 4000)                                                      AS Route,

        CAST(r.StartNode AS BIGINT)                                              AS StartNodeId,
        LEFT(sn.Code, 50)                                                        AS StartNodeCode,
        LEFT(sn.Name_ru, 4000)                                                   AS StartNodeNameRu,
        LEFT(sn.Name_en, 4000)                                                   AS StartNodeNameEn,

        CAST(sn.Region AS BIGINT)                                                AS RegionFromId,
        LEFT(rf.Code, 50)                                                        AS RegionFromCode,
        LEFT(rf.Name_ru, 4000)                                                   AS RegionFromNameRu,
        LEFT(rf.Name_en, 4000)                                                   AS RegionFromNameEn,

        CAST(r.ProxyNode AS BIGINT)                                              AS ProxyNodeId,
        LEFT(pn.Code, 50)                                                        AS ProxyNodeCode,
        LEFT(pn.Name_ru, 4000)                                                   AS ProxyNodeNameRu,
        LEFT(pn.Name_en, 4000)                                                   AS ProxyNodeNameEn,

        CAST(r.EndNode AS BIGINT)                                                AS EndNodeId,
        LEFT(en.Code, 50)                                                        AS EndNodeCode,
        LEFT(en.Name_ru, 4000)                                                   AS EndNodeNameRu,
        LEFT(en.Name_en, 4000)                                                   AS EndNodeNameEn,

        CAST(en.Region AS BIGINT)                                                AS RegionToId,
        LEFT(rt.Code, 50)                                                        AS RegionToCode,
        LEFT(rt.Name_ru, 4000)                                                   AS RegionToNameRu,
        LEFT(rt.Name_en, 4000)                                                   AS RegionToNameEn,

        CAST(r.RouteTransportKind AS BIGINT)                                    AS RouteTransportKindId,
        LEFT(tk.Code, 50)                                                        AS RouteTransportKindCode,
        LEFT(tk.Name, 4000)                                                      AS RouteTransportKindName,
        LEFT(tk.NameEnRu, 4000)                                                  AS RouteTransportKindNameEnRu,

        r.IsActive                                                               AS IsActive,

        CAST(r.Leg1_TransportType AS BIGINT)                                    AS Leg1_TransportTypeId,
        LEFT(l1.Code, 50)                                                        AS Leg1_TransportTypeCode,
        LEFT(l1.Name, 4000)                                                      AS Leg1_TransportTypeName,
        LEFT(l1.NameEnRu, 4000)                                                  AS Leg1_TransportTypeNameEnRu,

        CAST(r.Leg2_TransportType AS BIGINT)                                    AS Leg2_TransportTypeId,
        LEFT(l2.Code, 50)                                                        AS Leg2_TransportTypeCode,
        LEFT(l2.Name, 4000)                                                      AS Leg2_TransportTypeName,
        LEFT(l2.NameEnRu, 4000)                                                  AS Leg2_TransportTypeNameEnRu,

        LEFT(r.ChainNodes, 4000)                                                 AS ChainNodes,

        CAST(r.Id AS BIGINT)                                                     AS TransportRouteId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        r.CreationDate                                                           AS CreationDate,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportRoute r (NOLOCK)
    LEFT JOIN dbo.vw_LocationsNodes sn (NOLOCK) ON r.StartNode = sn.Id
    LEFT JOIN dbo.vw_Region rf (NOLOCK) ON sn.Region = rf.Id
    LEFT JOIN dbo.vw_LocationsNodes pn (NOLOCK) ON r.ProxyNode = pn.Id
    LEFT JOIN dbo.vw_LocationsNodes en (NOLOCK) ON r.EndNode = en.Id
    LEFT JOIN dbo.vw_Region rt (NOLOCK) ON en.Region = rt.Id
    LEFT JOIN dbo.vw_TransportKind tk (NOLOCK) ON r.RouteTransportKind = tk.Id
    LEFT JOIN dbo.vw_TransportType_level_3 l1 (NOLOCK) ON r.Leg1_TransportType = l1.Id
    LEFT JOIN dbo.vw_TransportType_level_3 l2 (NOLOCK) ON r.Leg2_TransportType = l2.Id;
GO
