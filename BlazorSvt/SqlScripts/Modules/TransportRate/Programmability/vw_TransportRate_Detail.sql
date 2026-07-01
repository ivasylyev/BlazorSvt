USE mdm
GO

/*
Example:

use mdm
go

SELECT *
FROM v2.vw_TransportRate_Detail
WHERE TransportRateId = 34041206

*/
DROP VIEW IF EXISTS v2.vw_TransportRateDetail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetTransportRateDetail;
GO

DROP VIEW IF EXISTS v2.vw_TransportRates_Detail;
GO

CREATE OR ALTER VIEW v2.vw_TransportRate_Detail
AS
    SELECT
     CAST(r.Id AS INT)              AS TransportRateId
    ,CAST(r.Code AS BIGINT)	        AS Code
    ,CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END 
                                    AS IsArchive
    ,ISNULL(r.IsDefRate, 0)         AS IsDefRate
    ,r.CreationDate 	            AS CreationDate
    ,r.LastChangeDate 	            AS LastChangeDate
    ,ar.Code		                AS AverageRateCode
    ,ar.RateLevel3 		            AS AverageRateLevel3TotalCostTon
    ,r.TotalCostTon 	            AS TotalCostTon
    ,r.TotalCostTransport 	        AS TotalCostTransport
    ,rct.Code 		                AS CalcType
    ,curSt.Code 		            AS CurrencyStandard
    ,CAST(r.CurrencyRateMonth AS DATE)	        AS CurrencyRateMonth
    ,r.EffectiveLoadOfTransportType AS EffectiveLoadOfTransportType
    ,CAST(r.StartDate AS DATE) 	    AS StartDate
    ,CAST(r.EndDate  AS DATE)	    AS EndDate
    ,rtp.Code		                AS TypeCode
    ,rtp.[Name]		                AS TypeName
    ,r.TotalCostTonRUB 	            AS TotalCostTonRUB
    ,r.TotalCostTonEUR 	            AS TotalCostTonEUR
    ,r.TotalCostTonCNY 	            AS TotalCostTonCNY
    ,r.TotalCostTonUSD 	            AS TotalCostTonUSD
    ,r.TotalCostTransportRUB 	    AS TotalCostTransportRUB
    ,r.TotalCostTransportEUR 	    AS TotalCostTransportEUR
    ,r.TotalCostTransportCNY 	    AS TotalCostTransportCNY
    ,r.TotalCostTransportUSD 	    AS TotalCostTransportUSD
    ,r.EmptyRFSize 	                AS EmptyRFSize
    ,curEmptyRF.Code 		        AS EmptyRFCurrency
    ,r.EmptyCISSize  	            AS EmptyCISSize
    ,curEmptyCIS.Code		        AS EmptyCISCurrency
    ,r.ProvisionTransportSize	    AS ProvisionTransportSize
    ,curProvisionTransport.Code	    AS ProvisionTransportCurrency
    ,r.FerryboatSize	            AS FerryboatSize
    ,curFerryboat.Code		        AS FerryboatCurrency
    ,r.TEFromSize	                AS TEFromSize
    ,curTEFrom.Code		            AS TEFromCurrency
    ,r.PNPFromSize	                AS RatePNPFromSize
    ,curPNPFrom.Code		        AS PNPFromCurrency
    ,r.TEToSize	                    AS TEToSize
    ,curTETo.Code		            AS TEToCurrency
    ,r.PNPToSize	                AS PNPToSize
    ,curPNPTo.Code		            AS PNPToCurrency
    ,r.DrainLoadingSize	            AS DrainLoadingSize
    ,curDrainLoading.Code	        AS DrainLoadingCurrency
    ,r.TransshipmentSize	        AS TransshipmentSize
    ,curTransshipment.Code		    AS TransshipmentCurrency
    ,r.FreightSize	                AS FreightSize
    ,curFreight.Code		        AS FreightCurrency
    ,r.AdditionalFeesCISSize	    AS AdditionalFeesCISSize
    ,curAdditionalFeesCIS.Code	    AS AdditionalFeesCISCurrency
    ,r.LoadedCISSize	            AS LoadedCISSize
    ,curLoadedCIS.Code		        AS LoadedCISCurrency
    ,r.LoadedRFSize	                AS LoadedRFSize
    ,curLoadedRF.Code		        AS LoadedRFCurrency
    ,r.TEFromSize_fix	            AS TEFromSize_fix
    ,curTEFrom_fix.Code		        AS TEFromCurrency_fix
    ,r.TEToSize_fix	                AS TEToSize_fix
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
    ,con.Code		                AS ContractorCode
    ,con.NameSearch		            AS ContractorNameSearch
    ,con.ShortNameEGRUL		        AS ContractorEGRUL
    ,r.Nomination	                AS Nomination
    ,tsp.TenderServicePack	        AS TenderServicePack
    ,r.TenderNumber	                AS TenderNumber
    ,r.AdditionalAgreementNumber    AS AdditionalAgreementNumber
    ,r.Comment	                    AS Comment
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
    ,r.Leg1_EffectiveLoad	        AS Leg1_EffectiveLoad
    ,r.Leg1_TotalCostTon	        AS Leg1_TotalCostTon
    ,r.Leg1_TotalCostTransport	    AS Leg1_TotalCostTransport
    ,leg1_cur.Code		            AS Leg1_BaseCurrency
    ,r.Leg1_TotalCostTonRUB	        AS Leg1_TotalCostTonRUB
    ,r.Leg1_TotalCostTonUSD	        AS Leg1_TotalCostTonUSD
    ,r.Leg1_TotalCostTonEUR	        AS Leg1_TotalCostTonEUR
    ,r.Leg1_TotalCostTonCNY	        AS Leg1_TotalCostTonCNY
    ,r.Leg1_TotalCostTransportRUB	AS Leg1_TotalCostTransportRUB
    ,r.Leg1_TotalCostTransportUSD	AS Leg1_TotalCostTransportUSD
    ,r.Leg1_TotalCostTransportEUR	AS Leg1_TotalCostTransportEUR
    ,r.Leg1_TotalCostTransportCNY	AS Leg1_TotalCostTransportCNY
    ,lt.Leg1_SearchTime 		    AS LeadTimeLeg1_SearchTime
    ,lt.Leg1_LoadTime 		        AS LeadTimeLeg1_LoadTime
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg1_TravelTime
    ,lt.Leg1_DaysWaiting 		    AS LeadTimeLeg1_DaysWaiting
    ,lt.Leg1_TransportationTime     AS LeadTimeLeg1_TransportationTime
    ,lt.Leg1_Distance 		        AS LeadTimeLeg1_Distance
    ,tt2.Code		                AS Leg2_TransportTypeCode
    ,tt2.[Name]		                AS Leg2_TransportTypeNameRu
    ,tt2.NameEnRu		            AS Leg2_TransportTypeNameRuEn
    ,r.Leg2_EffectiveLoad	        AS Leg2_EffectiveLoad
    ,r.Leg2_TotalCostTon	        AS Leg2_TotalCostTon
    ,r.Leg2_TotalCostTransport	    AS Leg2_TotalCostTransport
    ,leg2_cur.Code		            AS Leg2_BaseCurrency
    ,r.Leg2_TotalCostTonRUB	        AS Leg2_TotalCostTonRUB
    ,r.Leg2_TotalCostTonUSD	        AS Leg2_TotalCostTonUSD
    ,r.Leg2_TotalCostTonEUR	        AS Leg2_TotalCostTonEUR
    ,r.Leg2_TotalCostTonCNY	        AS Leg2_TotalCostTonCNY
    ,r.Leg2_TotalCostTransportRUB	AS Leg2_TotalCostTransportRUB
    ,r.Leg2_TotalCostTransportUSD	AS Leg2_TotalCostTransportUSD
    ,r.Leg2_TotalCostTransportEUR	AS Leg2_TotalCostTransportEUR
    ,r.Leg2_TotalCostTransportCNY	AS Leg2_TotalCostTransportCNY
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg2_TravelTime
    ,lt.Leg2_DaysWaiting 		    AS LeadTimeLeg2_DaysWaiting
    ,lt.Leg2_UploadTime 		    AS LeadTimeLeg2_UploadTime
    ,lt.Leg2_TransportationTime 	AS LeadTimeLeg2_TransportationTime
    ,lt.Leg2_Distance 		        AS LeadTimeLeg2_Distance
    FROM mdm.dbo.vw_TransportRate AS r 
    CROSS APPLY (
        SELECT TOP 1
            rar.AverageRateLevel3Id
        FROM mdm.dbo.vw_RatesOfAverageRate rar
        WHERE rar.Rate = r.Id
        ORDER BY rar.Id DESC
    ) rar -- чтобы избежать дублирования 

    CROSS APPLY (
        SELECT TOP 1
            ar.Code,
            ar.RateLevel3
        FROM mdm.dbo.vw_AverageRateLevel3 ar
        WHERE ar.Id = rar.AverageRateLevel3Id
        ORDER BY ar.Id DESC
    ) ar -- чтобы избежать дублирования 
    
    JOIN mdm.dbo.vw_RateType rtp 
        ON r.RateType = rtp.Id			
			
    JOIN mdm.dbo.vw_RateCalcType rct 
        ON rct.Id = r.RateCalcType			
    JOIN mdm.dbo.vw_Currency curSt 
        ON curSt.Id = 	r.CurrencyStandard		
    LEFT JOIN mdm.dbo.vw_Currency curEmptyRF 
        ON curEmptyRF.Id = 	r.EmptyRFCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curEmptyCIS 
        ON curEmptyCIS.Id =	r.EmptyCISCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curProvisionTransport 
        ON curProvisionTransport.Id=	r.ProvisionTransportCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curFerryboat 
        ON curFerryboat.Id=	r.FerryboatCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curTEFrom 
        ON curTEFrom.Id=	r.TEFromCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curPNPFrom 
        ON curPNPFrom.Id=	r.PNPFromCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curTETo 
        ON curTETo.Id=	r.TEToCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curPNPTo 
        ON curPNPTo.Id=	r.PNPToCurrency	
    LEFT JOIN mdm.dbo.vw_Currency curDrainLoading 
        ON curDrainLoading.Id=	r.DrainLoadingCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curTransshipment 
        ON curTransshipment.Id=	r.TransshipmentCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curFreight  
        ON curFreight.Id=	r.FreightCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curAdditionalFeesCIS  
        ON curAdditionalFeesCIS.Id=	r.AdditionalFeesCISCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curLoadedCIS 
        ON curLoadedCIS.Id=	r.LoadedCISCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curLoadedRF  
        ON curLoadedRF.Id=	r.LoadedRFCurrency		
    LEFT JOIN mdm.dbo.vw_Currency curTEFrom_fix 
        ON curTEFrom_fix.Id=	r.TEFromCurrency_fix		
    LEFT JOIN mdm.dbo.vw_Currency curTETo_fix 
        ON curTETo_fix.Id=	r.TEToCurrency_fix		

 			
    JOIN mdm.dbo.vw_LocationsNodes nf 
        ON nf.Id = r.NodeFrom			
    LEFT JOIN mdm.dbo.vw_Region rf 
        ON rf.Id = nf.Region	

    LEFT JOIN mdm.dbo.vw_LocationsNodes np 
        ON np.Id = r.ProxyNode			
    LEFT JOIN mdm.dbo.vw_Region rp 
        ON rp.Id = np.Region			

    JOIN mdm.dbo.vw_LocationsNodes nt 
        ON nt.Id = r.NodeTo			
    LEFT JOIN mdm.dbo.vw_Region rt 
        ON rt.Id = nt.Region			
			
    LEFT JOIN mdm.dbo.vw_Basis bas 
        ON bas.Id=r.basis			
    LEFT JOIN mdm.dbo.vw_LocationsNodes nb 
        ON nb.Id = r.BasisNode			
			
    JOIN mdm.dbo.vw_TransportKind tk 
        ON tk.Id = r.TransportKind			
			
    JOIN mdm.dbo.vw_TransportType_level_3 tt 
        ON tt.Id = r.TransportType			
			
    LEFT JOIN mdm.dbo.vw_ProductGroup pg 
        ON pg.Id = r.ProductGroup			
    LEFT JOIN mdm.dbo.vw_MTR p  
        ON p.Id = r.Product			
			
			
    LEFT JOIN mdm.dbo.vw_Contractor con 
        ON con.Id=r.Counterparty			

    CROSS APPLY (
        SELECT TOP 1
            lr.TransportLegId
        FROM mdm.dbo.vw_LegRate lr
        WHERE lr.Rate = r.Id
        ORDER BY lr.Id DESC
    ) lr -- чтобы избежать дублирования 

    CROSS APPLY (
        SELECT TOP 1
            lg.Id,
            lg.Code,
            lg.LastChangeDate
        FROM mdm.dbo.vw_TransportLeg lg
        WHERE lg.Id = lr.TransportLegId
        ORDER BY lg.Id DESC
    ) lg -- чтобы избежать дублирования 
    
    OUTER APPLY (
        SELECT TOP (1)
            lit.*
        FROM mdm.dbo.vw_Leadtime AS lit
        INNER JOIN mdm.dbo.vw_ShipmentType AS st
            ON st.Id = lit.ShipmentType AND st.IsMain = 1
        WHERE lit.TransportLegId = lg.Id
          AND CAST(r.EndDate AS DATE) >= CAST(lit.StartDate AS DATE)
          AND CAST(lit.EndDate AS DATE) >= CAST(r.StartDate AS DATE)
        ORDER BY lit.PrimitiveEntityDataStateId ASC,
                 lit.StartDate ASC,
                 lit.LastChangeDate DESC
    ) AS lt
			
    LEFT JOIN mdm.dbo.vw_TransportType_level_3 tt1 
        ON tt1.Id = r.Leg1_TransportType			
    LEFT JOIN mdm.dbo.vw_TransportType_level_3 tt2 
        ON tt2.Id = r.Leg2_TransportType			
			
    LEFT JOIN mdm.dbo.vw_Currency leg1_cur 
        ON leg1_cur.Id=	r.Leg1_BaseCurrency		
    LEFT JOIN mdm.dbo.vw_Currency leg2_cur 
        ON leg2_cur.Id=	r.Leg2_BaseCurrency		

    LEFT JOIN mdm.dbo.vw_TransportRateTenderServicePack tsp 
        ON r.Id = tsp.TransportRateId
GO
