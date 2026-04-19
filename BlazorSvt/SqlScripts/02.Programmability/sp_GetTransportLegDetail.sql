USE mdm
GO

/*
Example:

use mdm
go

-- простой поиск (это когда или нет сортировки, или менее 2х полнотекстовых фильтров)

exec dbo.GetTransportLegDetail @Key=31618174, @Lang='ru'
exec dbo.GetTransportLegDetail @Key=31618174, @Lang='en'
 

*/
CREATE OR ALTER PROCEDURE dbo.GetTransportLegDetail
    @Key            NVARCHAR(50),
    @Lang           NVARCHAR(2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LangSuffix NVARCHAR(2),
            @AllowedColumnsJson NVARCHAR(MAX),
            @SelectList NVARCHAR(MAX)

    SET @LangSuffix = CASE 
                        WHEN @Lang = N'ru' THEN N'Ru' 
                        ELSE N'En' 
                      END

    SELECT 
        CAST(l.Id AS INT)      AS LegId,
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
        CASE 
            WHEN @Lang = N'ru' THEN LEFT(nf.Name_ru, 30) 
            ELSE LEFT(nf.Name_en, 30) 
        END                             AS NodeFromName,
                CASE 
            WHEN @Lang = N'ru' THEN LEFT(nf.Name_ru, 30) 
            ELSE LEFT(nf.Name_en, 30) 
        END                             AS NodeFromName, 
        LEFT(rf.Code, 10)               AS RegionFromCode,   
        CASE 
            WHEN @Lang = N'ru' THEN LEFT(rf.Name_ru, 60) 
            ELSE LEFT(rf.Name_en, 60) 
        END                             AS RegionFromName, 

        LEFT(np.Code, 10)               AS ProxyNodeCode,  
        CASE 
            WHEN @Lang = N'ru' THEN LEFT(np.Name_ru, 30) 
            ELSE LEFT(np.Name_en, 30) 
        END                             AS ProxyNodeName,
        LEFT(rp.Code, 10)               AS ProxyRegionCode,  
        CASE 
            WHEN @Lang = N'ru' THEN LEFT(rp.Name_ru, 30) 
            ELSE LEFT(rp.Name_en, 30) 
        END                             AS ProxyRegionName,
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

    WHERE l.Id = CAST(@Key AS INT)         
        
END
GO
