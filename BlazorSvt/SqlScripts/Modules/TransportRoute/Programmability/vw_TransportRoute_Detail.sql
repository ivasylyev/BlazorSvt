USE [mdm];
GO

/*
    Карточка и полный Excel TransportRoute.
    Общие поля — проекция легаси (vw_TransportRoute_SnapshotSource).
    ShortName* / NameEN и коды плеч — только карточка, из dbo.vw_TransportRoute
    и dbo.vw_TransportLeg. Ключ — TransportRouteId.
*/

CREATE OR ALTER VIEW v2.vw_TransportRoute_Detail
AS
    SELECT
        s.TransportRouteId,
        s.Code,
        s.CodeSAP,
        s.IsActive,
        s.Route,
        s.StartNodeNameRu,
        s.StartNodeNameEn,
        s.StartNodeCode,
        s.RegionFromNameRu,
        s.RegionFromNameEn,
        s.RegionFromCode,
        s.ProxyNodeNameRu,
        s.ProxyNodeNameEn,
        s.ProxyNodeCode,
        s.EndNodeNameRu,
        s.EndNodeNameEn,
        s.EndNodeCode,
        s.RegionToNameRu,
        s.RegionToNameEn,
        s.RegionToCode,
        s.RouteTransportKindId                                              AS RouteTransportKindIdRu,
        s.RouteTransportKindId                                              AS RouteTransportKindIdEn,
        s.Leg1_TransportTypeId                                              AS Leg1_TransportTypeIdRu,
        s.Leg1_TransportTypeId                                              AS Leg1_TransportTypeIdEn,
        s.Leg2_TransportTypeId                                              AS Leg2_TransportTypeIdRu,
        s.Leg2_TransportTypeId                                              AS Leg2_TransportTypeIdEn,
        r.ShortNameRu,
        r.ShortNameEN,
        r.NameEN,
        s.ChainNodes,
        LEFT(l01.Code, 50)                                                  AS Leg_01Code,
        LEFT(l02.Code, 50)                                                  AS Leg_02Code,
        LEFT(l03.Code, 50)                                                  AS Leg_03Code,
        s.IsArchive,
        s.CreationUserName,
        s.CreationDate,
        s.LastChangeUserName,
        s.LastChangeDate
    FROM v2.vw_TransportRoute_SnapshotSource s
    JOIN dbo.vw_TransportRoute r (NOLOCK) ON r.Id = s.TransportRouteId
    LEFT JOIN dbo.vw_TransportLeg l01 (NOLOCK) ON r.Leg_01 = l01.Id
    LEFT JOIN dbo.vw_TransportLeg l02 (NOLOCK) ON r.Leg_02 = l02.Id
    LEFT JOIN dbo.vw_TransportLeg l03 (NOLOCK) ON r.Leg_03 = l03.Id;
GO
