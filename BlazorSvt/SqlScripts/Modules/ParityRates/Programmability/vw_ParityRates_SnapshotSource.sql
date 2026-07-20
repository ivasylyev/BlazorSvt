USE [mdm];
GO

/*
    Проекция snapshot ParityRates из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.ParityRates_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Членство в snapshot:
      - Code NOT NULL / непустой
      - StartDate / EndDate / TotalCostTon / TotalCostTransport NOT NULL
      - NodeFrom / NodeTo / ProductGroup / Relevance NOT NULL
      - INNER JOIN на обязательные справочники (валюта, тип транспорта и т.д.)

    Relevance (2108), Currency (2016), TransportType (2023) — стабильные,
    в sync-каскад не входят.
*/

CREATE OR ALTER VIEW v2.vw_ParityRates_SnapshotSource
AS
    SELECT
        CAST(pr.Id AS INT)                                      AS ParityRatesId,
        CASE WHEN ISNULL(pr.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        pr.StartDate                                            AS StartDate,
        pr.EndDate                                              AS EndDate,
        pr.CreationDate                                         AS CreationDate,
        ISNULL(pr.LastChangeDate, pr.CreationDate)              AS LastChangeDate,

        LEFT(pr.Code, 50)                                       AS Code,
        CAST(pr.Relevance AS INT)                               AS RelevanceId,
        CAST(pr.TransportTypeCode AS INT)                       AS TransportTypeId,
        CAST(pr.CurrencyStandard AS INT)                        AS CurrencyId,

        pr.TotalCostTon                                         AS TotalCostTon,
        pr.TotalCostTransport                                   AS TotalCostTransport,
        pr.LoadOfTransport                                      AS LoadOfTransport,
        pr.Level_Danger_Product                                 AS Level_Danger_Product,
        pr.FactRate                                             AS FactRate,
        pr.BusinessPlanningRate                                 AS BusinessPlanningRate,

        LEFT(cur.Code, 3)                                       AS CurrencyCode,
        LEFT(tt.Code, 20)                                       AS TransportTypeCode,

        LEFT(nf.Code, 10)                                       AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                    AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                    AS NodeFromNameRu,

        LEFT(np1.Code, 10)                                      AS ProxyNode1Code,
        LEFT(np1.Name_en, 30)                                   AS ProxyNode1NameEn,
        LEFT(np1.Name_ru, 30)                                   AS ProxyNode1NameRu,

        LEFT(np2.Code, 10)                                      AS ProxyNode2Code,
        LEFT(np2.Name_en, 30)                                   AS ProxyNode2NameEn,
        LEFT(np2.Name_ru, 30)                                   AS ProxyNode2NameRu,

        LEFT(nt.Code, 10)                                       AS NodeToCode,
        LEFT(nt.Name_en, 30)                                    AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                    AS NodeToNameRu,

        LEFT(pg.Code, 5)                                        AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        TRY_CAST(LEFT(p.Code, 7) AS INT)                        AS ProductCode,
        LEFT(p.NameShort_ru, 100)                               AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                               AS ProductNameEn,

        LEFT(pr.Comment, 1000)                                  AS Comment,
        LEFT(pr.DataSource, 4000)                               AS DataSource,
        LEFT(pr.DepartmentResponsibilityArea, 4000)             AS DepartmentResponsibilityArea,
        LEFT(pr.EmployeeResponsibilityArea, 4000)               AS EmployeeResponsibilityArea,
        LEFT(pr.Methodology, 4000)                              AS Methodology,
        LEFT(pr.PriorityText, 4000)                             AS PriorityText,

        CAST(nf.Id AS INT)                                      AS NodeFromId,
        CAST(nt.Id AS INT)                                      AS NodeToId,
        CAST(np1.Id AS INT)                                     AS ProxyNode1Id,
        CAST(np2.Id AS INT)                                     AS ProxyNode2Id,
        CAST(pg.Id AS INT)                                      AS ProductGroupId,
        CAST(p.Id AS INT)                                       AS ProductId

    FROM vw_ParityRates pr (NOLOCK)
    JOIN vw_Relevance rel (NOLOCK) ON pr.Relevance = rel.Id
    JOIN vw_LocationsNodes nf (NOLOCK) ON pr.NodeFromCode = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON pr.NodeToCode = nt.Id
    LEFT JOIN vw_LocationsNodes np1 (NOLOCK) ON pr.ProxyNode1 = np1.Id
    LEFT JOIN vw_LocationsNodes np2 (NOLOCK) ON pr.ProxyNode2 = np2.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON pr.TransportTypeCode = tt.Id
    JOIN vw_ProductGroup pg (NOLOCK) ON pr.ProductGroupCode = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON pr.Product = p.Id
    JOIN vw_Currency cur (NOLOCK) ON pr.CurrencyStandard = cur.Id

    WHERE pr.Code IS NOT NULL
      AND LTRIM(RTRIM(pr.Code)) <> N''
      AND pr.StartDate IS NOT NULL
      AND pr.EndDate IS NOT NULL
      AND pr.TotalCostTon IS NOT NULL
      AND pr.TotalCostTransport IS NOT NULL
      AND pr.LoadOfTransport IS NOT NULL
      AND pr.NodeFromCode IS NOT NULL
      AND pr.NodeToCode IS NOT NULL
      AND pr.ProductGroupCode IS NOT NULL
      AND pr.Relevance IS NOT NULL
GO