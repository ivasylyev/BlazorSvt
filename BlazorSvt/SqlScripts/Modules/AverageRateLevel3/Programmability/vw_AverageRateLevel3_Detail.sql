USE mdm
GO

/*
Example:

use mdm
go

SELECT *
FROM v2.vw_AverageRateLevel3_Detail
WHERE AverageRateLevel3Id = 34041206

*/
DROP VIEW IF EXISTS v2.vw_AverageRateLevel3Detail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetAverageRateLevel3Detail;
GO

DROP VIEW IF EXISTS v2.vw_AverageRateLevel3s_Detail;
GO

CREATE OR ALTER VIEW v2.vw_AverageRateLevel3_Detail
AS
    SELECT
     CAST(ar.Id AS INT)              AS AverageRateLevel3Id
    ,CAST(ar.Code AS BIGINT)	        AS Code
    ,CASE WHEN ISNULL(ar.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END 
                                    AS IsArchive
    ,ISNULL(ar.IsDefRate, 0)         AS IsDefRate
    ,ar.CreationDate 	            AS CreationDate
    ,ar.LastChangeDate 	            AS LastChangeDate
    ,trc.TransportRateCodes          AS TransportRateCodes
    ,CAST(ar.CalcDate AS DATE)       AS CalcDate
    ,ar.RateLevel3 		            AS RateLevel3
    ,curSt.Code 		            AS CurrencyStandard
    ,CAST(ar.CurrencyRateMonth AS DATE)	        AS CurrencyRateMonth
    ,ar.EffectiveLoadOfTransportType AS EffectiveLoadOfTransportType
    ,ar.MinDailyTransportation       AS MinDailyTransportation
    ,ar.MaxDailyTransportation       AS MaxDailyTransportation
    ,CAST(ar.StartDate AS DATE) 	    AS StartDate
    ,CAST(ar.EndDate  AS DATE)	    AS EndDate
    ,rtp.Code		                AS TypeCode
    ,rtp.[Name]		                AS TypeName
    ,ar.TotalCostTonRUB 	            AS TotalCostTonRUB
    ,ar.TotalCostTonEUR 	            AS TotalCostTonEUR
    ,ar.TotalCostTonCNY 	            AS TotalCostTonCNY
    ,ar.TotalCostTonUSD 	            AS TotalCostTonUSD
    ,ar.TotalCostTransportRUB 	    AS TotalCostTransportRUB
    ,ar.TotalCostTransportEUR 	    AS TotalCostTransportEUR
    ,ar.TotalCostTransportCNY 	    AS TotalCostTransportCNY
    ,ar.TotalCostTransportUSD 	    AS TotalCostTransportUSD
    ,ar.EmptyRFSize 	                AS EmptyRFSize
    ,curEmptyRF.Code 		        AS EmptyRFCurrency
    ,ar.EmptyCISSize  	            AS EmptyCISSize
    ,curEmptyCIS.Code		        AS EmptyCISCurrency
    ,ar.ProvisionTransportSize	    AS ProvisionTransportSize
    ,curProvisionTransport.Code	    AS ProvisionTransportCurrency
    ,ar.FerryboatSize	            AS FerryboatSize
    ,curFerryboat.Code		        AS FerryboatCurrency
    ,ar.TEFromSize	                AS TEFromSize
    ,curTEFrom.Code		            AS TEFromCurrency
    ,ar.PNPFromSize	                AS PNPFromSize
    ,curPNPFrom.Code		        AS PNPFromCurrency
    ,ar.TEToSize	                    AS TEToSize
    ,curTETo.Code		            AS TEToCurrency
    ,ar.PNPToSize	                AS PNPToSize
    ,curPNPTo.Code		            AS PNPToCurrency
    ,ar.DrainLoadingSize	            AS DrainLoadingSize
    ,curDrainLoading.Code	        AS DrainLoadingCurrency
    ,ar.TransshipmentSize	        AS TransshipmentSize
    ,curTransshipment.Code		    AS TransshipmentCurrency
    ,ar.FreightSize	                AS FreightSize
    ,curFreight.Code		        AS FreightCurrency
    ,ar.AdditionalFeesCISSize	    AS AdditionalFeesCISSize
    ,curAdditionalFeesCIS.Code	    AS AdditionalFeesCISCurrency
    ,ar.LoadedCISSize	            AS LoadedCISSize
    ,curLoadedCIS.Code		        AS LoadedCISCurrency
    ,ar.LoadedRFSize	                AS LoadedRFSize
    ,curLoadedRF.Code		        AS LoadedRFCurrency
    ,ar.TEFromSize_fix	            AS TEFromSize_fix
    ,curTEFrom_fix.Code		        AS TEFromCurrency_fix
    ,ar.TEToSize_fix	                AS TEToSize_fix
    ,curTETo_fix.Code		        AS TEToCurrency_fix
    ,nf.Code		                AS NodeFromCode
    ,nf.Name_ru		                AS NodeFromNameRu
    ,nf.Name_en		                AS NodeFromNameEn
    ,rf.Code		                AS RegionFromCode
    ,rf.Name_ru		                AS RegionFromNameRu
    ,rf.Name_en		                AS RegionFromNameEn
    ,np.Code		                AS ProxyNodeCode
    ,np.Name_ru		                AS ProxyNodeNameRu
    ,np.Name_en		                AS ProxyNodeNameEn
    ,rp.Code		                AS ProxyRegionCode
    ,rp.Name_ru		                AS ProxyRegionNameRu
    ,rp.Name_en		                AS ProxyRegionNameEn
    ,nt.Code		                AS NodeToCode
    ,nt.Name_ru		                AS NodeToNameRu
    ,nt.Name_en		                AS NodeToNameEn
    ,rt.Code		                AS RegionToCode
    ,rt.Name_ru		                AS RegionToNameRu
    ,rt.Name_en		                AS RegionToNameEn
    ,bas.[Name]		                AS Basis
    ,nb.Code		                AS BasisNodeCode
    ,nb.Name_ru		                AS BasisNodeNameRu
    ,tk.Code		                AS TransportKindCode
    ,tk.[Name]		                AS TransportKindNameRu
    ,tk.NameEnRu		            AS TransportKindNameRuEn
    ,tt.Code		                AS TransportTypeCode
    ,tt.[Name]		                AS TransportTypeNameRu
    ,tt.NameEnRu		            AS TransportTypeNameRuEn
    ,pg.Code		                AS ProductGroupCode
    ,pg.NameEnRu 		            AS ProductGroupNameEnRu
    ,p.Code 		                AS ProductCode
    ,p.NameFull_ru		            AS ProductNameRu
    ,p.NameFull_en 		            AS ProductNameEn
    ,p.DPOCode 		                AS ProductDPOCOde
    ,ar.Comment	                    AS Comment
    ,lg.Code		                AS LegCode
    ,lg.LastChangeDate 		        AS LegChangeDate
    ,lt.LastChangeDate 		        AS LeadTimeChangeDate
    ,lt.StartDate 		            AS LeadTimeStartDate
    ,lt.EndDate		                AS LeadTimeEndDate
    ,lt.Code 		                AS LeadTimeCode
    ,lt.SearchTime 		            AS LeadTimeSearchTime
    ,lt.LoadTime 		            AS LeadTimeLoadTime
    ,lt.TravelTime 		            AS LeadTimeTravelTime
    ,lt.DaysWaiting		            AS LeadTimeDaysWaiting
    ,lt.UnLoadTime 		            AS LeadTimeUnLoadTime
    ,lt.TransportationTime 		    AS LeadTimeTransportationTime
    ,lt.Distance 		            AS LeadTimeDistance
    ,tt1.Code		                AS Leg1_TransportTypeCode
    ,tt1.[Name]		                AS Leg1_TransportTypeNameRu
    ,tt1.NameEnRu		            AS Leg1_TransportTypeNameRuEn
    ,ar.Leg1_EffectiveLoad	        AS Leg1_EffectiveLoad
    ,ar.Leg1_TotalCostTon	        AS Leg1_TotalCostTon
    ,ar.Leg1_TotalCostTransport	    AS Leg1_TotalCostTransport
    ,leg1_cur.Code		            AS Leg1_BaseCurrency
    ,ar.Leg1_TotalCostTonRUB	        AS Leg1_TotalCostTonRUB
    ,ar.Leg1_TotalCostTonUSD	        AS Leg1_TotalCostTonUSD
    ,ar.Leg1_TotalCostTonEUR	        AS Leg1_TotalCostTonEUR
    ,ar.Leg1_TotalCostTonCNY	        AS Leg1_TotalCostTonCNY
    ,ar.Leg1_TotalCostTransportRUB	AS Leg1_TotalCostTransportRUB
    ,ar.Leg1_TotalCostTransportUSD	AS Leg1_TotalCostTransportUSD
    ,ar.Leg1_TotalCostTransportEUR	AS Leg1_TotalCostTransportEUR
    ,ar.Leg1_TotalCostTransportCNY	AS Leg1_TotalCostTransportCNY
    ,lt.Leg1_SearchTime 		    AS LeadTimeLeg1_SearchTime
    ,lt.Leg1_LoadTime 		        AS LeadTimeLeg1_LoadTime
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg1_TravelTime
    ,lt.Leg1_DaysWaiting 		    AS LeadTimeLeg1_DaysWaiting
    ,lt.Leg1_TransportationTime     AS LeadTimeLeg1_TransportationTime
    ,lt.Leg1_Distance 		        AS LeadTimeLeg1_Distance
    ,tt2.Code		                AS Leg2_TransportTypeCode
    ,tt2.[Name]		                AS Leg2_TransportTypeNameRu
    ,tt2.NameEnRu		            AS Leg2_TransportTypeNameRuEn
    ,ar.Leg2_EffectiveLoad	        AS Leg2_EffectiveLoad
    ,ar.Leg2_TotalCostTon	        AS Leg2_TotalCostTon
    ,ar.Leg2_TotalCostTransport	    AS Leg2_TotalCostTransport
    ,leg2_cur.Code		            AS Leg2_BaseCurrency
    ,ar.Leg2_TotalCostTonRUB	        AS Leg2_TotalCostTonRUB
    ,ar.Leg2_TotalCostTonUSD	        AS Leg2_TotalCostTonUSD
    ,ar.Leg2_TotalCostTonEUR	        AS Leg2_TotalCostTonEUR
    ,ar.Leg2_TotalCostTonCNY	        AS Leg2_TotalCostTonCNY
    ,ar.Leg2_TotalCostTransportRUB	AS Leg2_TotalCostTransportRUB
    ,ar.Leg2_TotalCostTransportUSD	AS Leg2_TotalCostTransportUSD
    ,ar.Leg2_TotalCostTransportEUR	AS Leg2_TotalCostTransportEUR
    ,ar.Leg2_TotalCostTransportCNY	AS Leg2_TotalCostTransportCNY
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg2_TravelTime
    ,lt.Leg2_DaysWaiting 		    AS LeadTimeLeg2_DaysWaiting
    ,lt.Leg2_UploadTime 		    AS LeadTimeLeg2_UploadTime
    ,lt.Leg2_TransportationTime 	AS LeadTimeLeg2_TransportationTime
    ,lt.Leg2_Distance 		        AS LeadTimeLeg2_Distance
    FROM vw_AverageRateLevel3 AS ar

    OUTER APPLY (
        SELECT STRING_AGG(CAST(r.Code AS NVARCHAR(50)), ', ') AS TransportRateCodes
        FROM vw_RatesOfAverageRate rar
        JOIN vw_TransportRate r ON r.Id = rar.Rate
        WHERE rar.AverageRateLevel3Id = ar.Id
          AND rar.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
          AND r.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
    ) trc

    JOIN vw_RateType rtp 
        ON ar.RateType = rtp.Id			

    JOIN vw_Currency curSt 
        ON curSt.Id = ar.CurrencyStandard		
    LEFT JOIN vw_Currency curEmptyRF 
        ON curEmptyRF.Id = ar.EmptyRFCurrency		
    LEFT JOIN vw_Currency curEmptyCIS 
        ON curEmptyCIS.Id = ar.EmptyCISCurrency		
    LEFT JOIN vw_Currency curProvisionTransport 
        ON curProvisionTransport.Id = ar.ProvisionTransportCurrency		
    LEFT JOIN vw_Currency curFerryboat 
        ON curFerryboat.Id = ar.FerryboatCurrency		
    LEFT JOIN vw_Currency curTEFrom 
        ON curTEFrom.Id = ar.TEFromCurrency		
    LEFT JOIN vw_Currency curPNPFrom 
        ON curPNPFrom.Id = ar.PNPFromCurrency		
    LEFT JOIN vw_Currency curTETo 
        ON curTETo.Id = ar.TEToCurrency		
    LEFT JOIN vw_Currency curPNPTo 
        ON curPNPTo.Id = ar.PNPToCurrency	
    LEFT JOIN vw_Currency curDrainLoading 
        ON curDrainLoading.Id = ar.DrainLoadingCurrency		
    LEFT JOIN vw_Currency curTransshipment 
        ON curTransshipment.Id = ar.TransshipmentCurrency		
    LEFT JOIN vw_Currency curFreight  
        ON curFreight.Id = ar.FreightCurrency		
    LEFT JOIN vw_Currency curAdditionalFeesCIS  
        ON curAdditionalFeesCIS.Id = ar.AdditionalFeesCISCurrency		
    LEFT JOIN vw_Currency curLoadedCIS 
        ON curLoadedCIS.Id = ar.LoadedCISCurrency		
    LEFT JOIN vw_Currency curLoadedRF  
        ON curLoadedRF.Id = ar.LoadedRFCurrency		
    LEFT JOIN vw_Currency curTEFrom_fix 
        ON curTEFrom_fix.Id = ar.TEFromCurrency_fix		
    LEFT JOIN vw_Currency curTETo_fix 
        ON curTETo_fix.Id = ar.TEToCurrency_fix		

    JOIN vw_LocationsNodes nf 
        ON nf.Id = ar.NodeFrom			
    LEFT JOIN vw_Region rf 
        ON rf.Id = nf.Region	

    LEFT JOIN vw_LocationsNodes np 
        ON np.Id = ar.ProxyNode			
    LEFT JOIN vw_Region rp 
        ON rp.Id = np.Region			

    JOIN vw_LocationsNodes nt 
        ON nt.Id = ar.NodeTo			
    LEFT JOIN vw_Region rt 
        ON rt.Id = nt.Region			
			
    LEFT JOIN vw_Basis bas 
        ON bas.Id = ar.basis			
    LEFT JOIN vw_LocationsNodes nb 
        ON nb.Id = ar.BasisNode			
			
    JOIN vw_TransportKind tk 
        ON tk.Id = ar.TransportKind			
			
    JOIN vw_TransportType_level_3 tt 
        ON tt.Id = ar.TransportType			
			
    LEFT JOIN vw_ProductGroup pg 
        ON pg.Id = ar.ProductGroup			
    LEFT JOIN vw_MTR p  
        ON p.Id = ar.Product			

    CROSS APPLY (
        SELECT TOP 1
            lar.TransportLegId
        FROM vw_LegAverageRateLevel3 lar
        WHERE lar.Rate = ar.Id
        ORDER BY lar.Id DESC
    ) lar

    CROSS APPLY (
        SELECT TOP 1
            lg.Id,
            lg.Code,
            lg.LastChangeDate
        FROM vw_TransportLeg lg
        WHERE lg.Id = lar.TransportLegId
        ORDER BY lg.Id DESC
    ) lg
    
    OUTER APPLY (
        SELECT TOP (1)
            lit.*
        FROM vw_Leadtime AS lit
        INNER JOIN vw_ShipmentType AS st
            ON st.Id = lit.ShipmentType AND st.IsMain = 1
        WHERE lit.TransportLegId = lg.Id
          AND CAST(ar.EndDate AS DATE) >= CAST(lit.StartDate AS DATE)
          AND CAST(lit.EndDate AS DATE) >= CAST(ar.StartDate AS DATE)
        ORDER BY lit.PrimitiveEntityDataStateId ASC,
                 lit.StartDate ASC,
                 lit.LastChangeDate DESC
    ) AS lt
			
    LEFT JOIN vw_TransportType_level_3 tt1 
        ON tt1.Id = ar.Leg1_TransportType			
    LEFT JOIN vw_TransportType_level_3 tt2 
        ON tt2.Id = ar.Leg2_TransportType			
			
    LEFT JOIN vw_Currency leg1_cur 
        ON leg1_cur.Id = ar.Leg1_BaseCurrency		
    LEFT JOIN vw_Currency leg2_cur 
        ON leg2_cur.Id = ar.Leg2_BaseCurrency		
GO
