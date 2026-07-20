USE [mdm];
GO

/*
Purpose:
  Detail-проекция ParityRates для UI/full Excel.
  Без leadtime / leg1_* / leg2_* секций (их нет у сущности).

Example:
  SELECT * FROM v2.vw_ParityRates_Detail WHERE ParityRatesId = ...
*/

CREATE OR ALTER VIEW v2.vw_ParityRates_Detail
AS
    SELECT
        CAST(pr.Id AS INT)                                      AS ParityRatesId,
        LEFT(pr.Code, 50)                                       AS Code,
        CASE WHEN ISNULL(pr.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        pr.CreationDate                                         AS CreationDate,
        pr.LastChangeDate                                       AS LastChangeDate,
        CAST(pr.StartDate AS DATE)                              AS StartDate,
        CAST(pr.EndDate AS DATE)                                AS EndDate,

        LEFT(rel.Code, 50)                                      AS RelevanceCode,
        LEFT(rel.Name, 100)                                     AS RelevanceName,

        LEFT(nf.Code, 10)                                       AS NodeFromCode,
        LEFT(nf.Name_en, 100)                                   AS NodeFromNameEn,
        LEFT(nf.Name_ru, 100)                                   AS NodeFromNameRu,

        LEFT(np1.Code, 10)                                      AS ProxyNode1Code,
        LEFT(np1.Name_en, 100)                                  AS ProxyNode1NameEn,
        LEFT(np1.Name_ru, 100)                                  AS ProxyNode1NameRu,

        LEFT(np2.Code, 10)                                      AS ProxyNode2Code,
        LEFT(np2.Name_en, 100)                                  AS ProxyNode2NameEn,
        LEFT(np2.Name_ru, 100)                                  AS ProxyNode2NameRu,

        LEFT(nt.Code, 10)                                       AS NodeToCode,
        LEFT(nt.Name_en, 100)                                   AS NodeToNameEn,
        LEFT(nt.Name_ru, 100)                                   AS NodeToNameRu,

        LEFT(tt.Code, 20)                                       AS TransportTypeCode,
        LEFT(tt.Name, 100)                                      AS TransportTypeNameRu,
        LEFT(tt.NameEnRu, 100)                                  AS TransportTypeNameEn,

        LEFT(pg.Code, 5)                                        AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        TRY_CAST(LEFT(p.Code, 7) AS INT)                        AS ProductCode,
        LEFT(p.NameShort_ru, 100)                               AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                               AS ProductNameEn,

        pr.Level_Danger_Product                                 AS Level_Danger_Product,
        ISNULL(pr.Dangerous_Cargo, 0)                           AS Dangerous_Cargo,

        pr.TotalCostTransport                                   AS TotalCostTransport,
        pr.LoadOfTransport                                      AS LoadOfTransport,
        pr.TotalCostTon                                         AS TotalCostTon,
        LEFT(cur.Code, 3)                                       AS CurrencyStandard,

        LEFT(pr.Comment, 1000)                                  AS Comment,
        LEFT(pr.DataSource, 4000)                               AS DataSource,
        pr.FactRate                                             AS FactRate,
        pr.BusinessPlanningRate                                 AS BusinessPlanningRate,
        LEFT(pr.DepartmentResponsibilityArea, 4000)             AS DepartmentResponsibilityArea,
        LEFT(pr.EmployeeResponsibilityArea, 4000)               AS EmployeeResponsibilityArea,
        LEFT(pr.Methodology, 4000)                              AS Methodology,
        LEFT(pr.PriorityText, 4000)                             AS PriorityText

    FROM vw_ParityRates pr (NOLOCK)
    LEFT JOIN vw_Relevance rel (NOLOCK) ON pr.Relevance = rel.Id
    LEFT JOIN vw_LocationsNodes nf (NOLOCK) ON pr.NodeFromCode = nf.Id
    LEFT JOIN vw_LocationsNodes nt (NOLOCK) ON pr.NodeToCode = nt.Id
    LEFT JOIN vw_LocationsNodes np1 (NOLOCK) ON pr.ProxyNode1 = np1.Id
    LEFT JOIN vw_LocationsNodes np2 (NOLOCK) ON pr.ProxyNode2 = np2.Id
    LEFT JOIN vw_TransportType_level_3 tt (NOLOCK) ON pr.TransportTypeCode = tt.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON pr.ProductGroupCode = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON pr.Product = p.Id
    LEFT JOIN vw_Currency cur (NOLOCK) ON pr.CurrencyStandard = cur.Id
GO