USE [mdm];
GO

/*
    Проекция snapshot TransportRate из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportRate_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT (включая WHERE-фильтр членства) для обоих путей
    гарантирует, что инкремент и полная пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportRate_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение LocationsNodes / ProductGroup / MTR -> затронутые рейты).
    В грид и DTO не отдаются.

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE. Поэтому источники 2048 и 1015 обязательны в каскаде
    процедуры детекции — иначе вход/выход строки отследит только суточный reconcile.
*/

CREATE OR ALTER VIEW v2.vw_TransportRate_SnapshotSource
AS
    SELECT
        CAST(r.Id AS INT)                                   AS TransportRateId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(r.IsDefRate, 0)                              AS IsDefRate,
        r.StartDate                                         AS StartDate,
        r.EndDate                                           AS EndDate,
        r.CreationDate                                      AS CreationDate,
        ISNULL(r.LastChangeDate, r.CreationDate)            AS LastChangeDate,

        r.TotalCostTon                                      AS TotalCostTon,
        r.TotalCostTransport                                AS TotalCostTransport,

        CAST(r.TransportKind AS INT)                        AS TransportKindId,
        CAST(r.TransportType AS INT)                        AS TransportTypeId,
        CAST(r.RateType AS INT)                             AS RateTypeId,
        CAST(r.CurrencyStandard AS INT)                     AS CurrencyId,

        TRY_CAST(LEFT(r.Code, 10) AS INT)                   AS Code,

        TRY_CAST(LEFT(rt.Code, 2) AS INT)                   AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                    AS ProductCode,
        LEFT(cur.Code, 3)                                   AS CurrencyCode,

        LEFT(nf.Code, 10)                                   AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                AS NodeFromNameRu,

        LEFT(np.Code, 10)                                   AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                AS ProxyNodeNameRu,

        LEFT(nt.Code, 10)                                   AS NodeToCode,
        LEFT(nt.Name_en, 30)                                AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                AS NodeToNameRu,

        LEFT(tk.Code, 5)                                    AS TransportKindCode,
        LEFT(tt.Code, 20)                                   AS TransportTypeCode,

        LEFT(pg.Code, 5)                                    AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        LEFT(p.NameShort_ru, 100)                           AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                           AS ProductNameEn,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)                                  AS NodeFromId,
        CAST(nt.Id AS INT)                                  AS NodeToId,
        CAST(np.Id AS INT)                                  AS ProxyNodeId,
        CAST(pg.Id AS INT)                                  AS ProductGroupId,
        CAST(p.Id AS INT)                                   AS ProductId

    FROM vw_TransportRate r (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON r.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON r.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON r.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON r.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id

    WHERE r.TotalCostTon IS NOT NULL
      AND r.TotalCostTransport IS NOT NULL
      AND TRY_CAST(LEFT(r.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
GO
