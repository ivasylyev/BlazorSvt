USE mdm
GO

/*
Example:

use mdm
go


exec dbo.GetTransportLegDetail '2845464'
 

*/
CREATE OR ALTER PROCEDURE dbo.GetTransportLegDetail
    @Key            NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CAST(l.Id AS INT)      AS LegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        CAST(st.Id AS INT)              AS ShipmentTypeIdRu,  
        CAST(st.Id AS INT)              AS ShipmentTypeIdEn,  

        CAST(l.TransportKind AS INT)    AS TransportKindIdRu,     
        CAST(l.TransportKind AS INT)    AS TransportKindIdEn,     
        LEFT(tk.Code, 5)                AS TransportKindCode,

        LEFT(l.SearchTimeT, 20)         AS SearchTimeT,
        LEFT(l.LoadTimeT, 20)           AS LoadTimeT,
        LEFT(l.TravelTimeT, 20)         AS TravelTimeT,
        LEFT(l.DaysWaitingT, 20)        AS DaysWaitingT,
        LEFT(l.UnLoadTimeT, 20)         AS UnLoadTimeT,
        LEFT(l.TransportationTimeT, 20) AS TransportationTimeT,

        l.Distance                      AS Distance,
        LEFT(nf.Code, 10)               AS NodeFromCode,   
        LEFT(nf.Name_en, 30)            AS NodeFromNameEn, 
        LEFT(nf.Name_ru, 30)            AS NodeFromNameRu, 
        LEFT(rf.Code, 10)               AS RegionFromCode,   
        LEFT(rf.Name_en, 60)            AS RegionFromNameEn, 
        LEFT(rf.Name_ru, 60)            AS RegionFromNameRu, 

        LEFT(np.Code, 10)               AS ProxyNodeCode,  
        LEFT(np.Name_en, 30)            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)            AS ProxyNodeNameRu,
        LEFT(rp.Code, 10)               AS ProxyRegionCode,  
        LEFT(rp.Name_en, 30)            AS ProxyRegionNameEn,
        LEFT(rp.Name_ru, 30)            AS ProxyRegionNameRu,

        LEFT(nt.Code, 10)               AS NodeToCode,     
        LEFT(nt.Name_en, 30)            AS NodeToNameEn,   
        LEFT(nt.Name_ru, 30)            AS NodeToNameRu,   
        LEFT(rt.Code, 10)               AS RegionToCode,     
        LEFT(rt.Name_en, 30)            AS RegionToNameEn,   
        LEFT(rt.Name_ru, 30)            AS RegionToNameRu,   

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
    
    WHERE l.Id = CAST(@Key AS INT)        
        
END
GO
