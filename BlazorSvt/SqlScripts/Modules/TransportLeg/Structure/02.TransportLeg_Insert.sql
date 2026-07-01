USE [mdm];
GO

INSERT INTO v2.TransportLeg_Snapshot (
     TransportLegId
    ,Code               
    
    ,IsArchive          
    ,CanBeUsed          
    
    ,ShipmentTypeId     

    ,TransportKindId    
    ,TransportKindCode  

    ,SearchTimeT         
    ,LoadTimeT           
    ,TravelTimeT         
    ,DaysWaitingT        
    ,UnLoadTimeT         
    ,TransportationTimeT 

    ,NodeFromCode       
    ,NodeFromNameEn     
    ,NodeFromNameRu     
    ,RegionFromCode     
    ,RegionFromNameEn   
    ,RegionFromNameRu   
    
    ,ProxyNodeCode      
    ,ProxyNodeNameEn    
    ,ProxyNodeNameRu    
    ,ProxyRegionCode    
    ,ProxyRegionNameEn  
    ,ProxyRegionNameRu  
    
    ,NodeToCode         
    ,NodeToNameEn       
    ,NodeToNameRu       
    ,RegionToCode       
    ,RegionToNameEn     
    ,RegionToNameRu     
    
    ,CreationDate       
    ,LastChangeDate     
)
SELECT 
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        CAST(st.Id AS INT)              AS ShipmentTypeId,     

        CAST(l.TransportKind AS INT)    AS TransportKindId,     
        LEFT(tk.Code, 5)                AS TransportKindCode,

        LEFT(l.SearchTimeT, 20)         AS SearchTimeT,
        LEFT(l.LoadTimeT, 20)           AS LoadTimeT,
        LEFT(l.TravelTimeT, 20)         AS TravelTimeT,
        LEFT(l.DaysWaitingT, 20)        AS DaysWaitingT,
        LEFT(l.UnLoadTimeT, 20)         AS UnLoadTimeT,
        LEFT(l.TransportationTimeT, 20) AS TransportationTimeT,
        
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
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate

    
    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id
    JOIN vw_ShipmentType st (NOLOCK) ON l.ShipmentTypeCodeT = st.Code

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
 
 
