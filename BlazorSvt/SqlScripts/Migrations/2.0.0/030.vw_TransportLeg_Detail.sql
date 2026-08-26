USE mdm
GO

/*
Purpose:
  Detail-проекция для UI/full Excel (не snapshot grid).
  Strangler Fig: может присоединять legacy dbo.vw_* / ped.a_* — богаче, чем v2.*_Snapshot.
  Grid list → snapshot; detail/export → этот view ([DetailSource] на detail-DTO).

Example:

use mdm
go

SELECT *
FROM v2.vw_TransportLeg_Detail
WHERE TransportLegId = 2845464

*/
DROP VIEW IF EXISTS v2.vw_TransportLegDetail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetTransportLegDetail;
GO

DROP VIEW IF EXISTS v2.vw_TransportLegs_Detail;
GO

CREATE OR ALTER VIEW v2.vw_TransportLeg_Detail
AS
    SELECT 
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        CAST(st.Id AS INT)              AS ShipmentTypeIdRu,  
        CAST(st.Id AS INT)              AS ShipmentTypeIdEn,  

        CAST(l.TransportKind AS INT)    AS TransportKindIdRu,     
        CAST(l.TransportKind AS INT)    AS TransportKindIdEn,     
        tk.Code                         AS TransportKindCode,

        l.SearchTimeT                   AS SearchTimeT,
        l.LoadTimeT                     AS LoadTimeT,
        l.TravelTimeT                   AS TravelTimeT,
        l.DaysWaitingT                  AS DaysWaitingT,
        l.UnLoadTimeT                   AS UnLoadTimeT,
        l.TransportationTimeT           AS TransportationTimeT,

        l.Distance                      AS Distance,
        nf.Code                         AS NodeFromCode,   
        nf.Name_en                      AS NodeFromNameEn, 
        nf.Name_ru                      AS NodeFromNameRu, 
        rf.Code                         AS RegionFromCode,   
        rf.Name_en                      AS RegionFromNameEn, 
        rf.Name_ru                      AS RegionFromNameRu, 

        np.Code                         AS ProxyNodeCode,  
        np.Name_en                      AS ProxyNodeNameEn,
        np.Name_ru                      AS ProxyNodeNameRu,
        rp.Code                         AS ProxyRegionCode,  
        rp.Name_en                      AS ProxyRegionNameEn,
        rp.Name_ru                      AS ProxyRegionNameRu,

        nt.Code                         AS NodeToCode,     
        nt.Name_en                      AS NodeToNameEn,   
        nt.Name_ru                      AS NodeToNameRu,   
        rt.Code                         AS RegionToCode,     
        rt.Name_en                      AS RegionToNameEn,   
        rt.Name_ru                      AS RegionToNameRu,   

        l.CreationDate                  AS CreationDate,
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate,
        l.Leg1_TransportType            AS Leg1_TransportTypeIdRu,
        l.Leg1_TransportType            AS Leg1_TransportTypeIdEn,
        l.Leg1_SearchTime,
        l.Leg1_LoadTime,
        l.Leg1_TravelTime,
        l.Leg1_DaysWaiting,
        l.Leg1_TransportationTime,
        l.Leg1_Distance,
        l.Leg2_TransportType            AS Leg2_TransportTypeIdRu,
        l.Leg2_TransportType            AS Leg2_TransportTypeIdEn,
        l.Leg2_UpLoadTime,
        l.Leg2_TravelTime,
        l.Leg2_DaysWaiting,
        l.Leg2_TransportationTime,
        l.Leg2_Distance
    

    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id
    JOIN vw_ShipmentType st (NOLOCK) ON l.ShipmentTypeCodeT = st.Code

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
GO
