USE [mdm];
GO

/*
    Проекция snapshot ProductGroup из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.ProductGroup_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.ProductGroup_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    RateSyncClass (dbo.PrimitiveEntityData_2127) — короткий словарь, в гриде enum
    по RateSyncClassId; в каскад синхронизации не входит.
*/

CREATE OR ALTER VIEW v2.vw_ProductGroup_SnapshotSource
AS
    SELECT
        CAST(pg.Id AS BIGINT)                                                    AS ProductGroupId,
        LEFT(pg.Code, 50)                                                        AS Code,
        CASE WHEN ISNULL(pg.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        LEFT(pg.Name, 4000)                                                      AS Name,
        LEFT(pg.ShortName, 4000)                                                 AS ShortName,
        LEFT(pg.GroupCode, 4000)                                                 AS GroupCode,
        LEFT(pg.ProductGroupCBD, 4000)                                           AS ProductGroupCBD,
        LEFT(pg.NameEnCode, 4000)                                                AS NameEnCode,
        LEFT(pg.NameEnRu, 4000)                                                  AS NameEnRu,
        LEFT(pg.Code_NSI, 50)                                                    AS Code_NSI,
        pg.SVTUseProductGroupWhenNoProductRateExists                             AS SVTUseProductGroupWhenNoProductRateExists,
        LEFT(pg.NameEn, 4000)                                                    AS NameEn,

        CAST(pg.RateSyncClass AS BIGINT)                                         AS RateSyncClassId,
        LEFT(rsc.Code, 50)                                                       AS RateSyncClassCode,
        LEFT(rsc.Name, 2000)                                                     AS RateSyncClassNameRu,

        pg.CreationDate                                                          AS CreationDate,
        ISNULL(pg.LastChangeDate, pg.CreationDate)                               AS LastChangeDate

    FROM dbo.vw_ProductGroup pg (NOLOCK)
    LEFT JOIN dbo.vw_RateSyncClass rsc (NOLOCK) ON pg.RateSyncClass = rsc.Id;
GO
