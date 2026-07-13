USE [mdm];
GO

/*
    Проекция snapshot AverageRateLevel3 из legacy-вью — единый источник истины
    для read-модели. Используется первичной заливкой и инкрементальной синхронизацией.

    Членство в snapshot:
      - RateLevel3 IS NOT NULL
      - валидные Code / RateType / Product (как TransportRate)
      - EXISTS хотя бы одной связанной TransportRate с тем же PrimitiveEntityDataStateId
*/

CREATE OR ALTER VIEW v2.vw_AverageRateLevel3_SnapshotSource
AS
    SELECT
        CAST(ar.Id AS INT)                                              AS AverageRateLevel3Id,
        CASE WHEN ISNULL(ar.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(ar.IsDefRate, 0)                                         AS IsDefRate,
        ar.StartDate                                                    AS StartDate,
        ar.EndDate                                                      AS EndDate,
        ar.CreationDate                                                 AS CreationDate,
        ISNULL(ar.LastChangeDate, ar.CreationDate)                        AS LastChangeDate,
        ar.RateLevel3                                                   AS RateLevel3,
        ISNULL(ar.EffectiveLoadOfTransportType, 0)                      AS EffectiveLoadOfTransportType,
        CAST(ar.TransportKind AS INT)                                   AS TransportKindId,
        CAST(ar.TransportType AS INT)                                   AS TransportTypeId,
        CAST(ar.RateType AS INT)                                        AS RateTypeId,
        CAST(ar.CurrencyStandard AS INT)                                AS CurrencyId,
        TRY_CAST(LEFT(ar.Code, 10) AS INT)                              AS Code,
        TRY_CAST(LEFT(rt.Code, 2) AS INT)                               AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                                AS ProductCode,
        LEFT(cur.Code, 3)                                               AS CurrencyCode,
        LEFT(nf.Code, 10)                                               AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                            AS NodeFromNameRu,
        LEFT(np.Code, 10)                                               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                            AS ProxyNodeNameRu,
        LEFT(nt.Code, 10)                                               AS NodeToCode,
        LEFT(nt.Name_en, 30)                                            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                            AS NodeToNameRu,
        LEFT(tk.Code, 5)                                                AS TransportKindCode,
        LEFT(tt.Code, 20)                                               AS TransportTypeCode,
        LEFT(pg.Code, 5)                                                AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100)         AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)             AS ProductGroupNameEn,
        LEFT(p.NameShort_ru, 100)                                       AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                                       AS ProductNameEn,
        CAST(nf.Id AS INT)                                              AS NodeFromId,
        CAST(nt.Id AS INT)                                              AS NodeToId,
        CAST(np.Id AS INT)                                              AS ProxyNodeId,
        CAST(pg.Id AS INT)                                              AS ProductGroupId,
        CAST(p.Id AS INT)                                               AS ProductId
    FROM vw_AverageRateLevel3 ar (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON ar.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON ar.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON ar.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON ar.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON ar.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON ar.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON ar.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON ar.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON ar.CurrencyStandard = cur.Id
    WHERE ar.RateLevel3 IS NOT NULL
      AND TRY_CAST(LEFT(ar.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
      AND EXISTS (
          SELECT 1
          FROM vw_RatesOfAverageRate rar (NOLOCK)
          JOIN vw_TransportRate r (NOLOCK) ON r.Id = rar.Rate
          WHERE rar.AverageRateLevel3Id = ar.Id
            AND rar.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
            AND r.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
      );
GO
