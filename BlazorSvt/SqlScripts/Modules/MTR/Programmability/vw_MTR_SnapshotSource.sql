USE [mdm];
GO

/*
    Проекция snapshot MTR из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.MTR_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.MTR_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    TransportName — ссылка на ProductGroup (whitelist: код + имена).
    ETSNGCode — ссылка на длинный словарь: в снимке имя. У ETSNG нет английского
    имени, EtsngNameEn повторяет Name.
    Unit и DangerClass — короткие словари, в гриде enum по Id.
    В каскад sync входят ProductGroup и ETSNG.

    Локальный PrimitiveEntityInfo.Id = 1015, таблица dbo.PrimitiveEntityData_1015.
    В реестре скоупа URL masterdata/2005.
*/

CREATE OR ALTER VIEW v2.vw_MTR_SnapshotSource
AS
    SELECT
        CAST(m.Id AS BIGINT)                                                     AS MTRId,
        LEFT(m.Code, 50)                                                         AS Code,
        CASE WHEN ISNULL(m.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        LEFT(m.NameFull_ru, 4000)                                                AS NameFullRu,
        LEFT(m.NameShort_en, 4000)                                               AS NameShortEn,

        CAST(m.TransportName AS BIGINT)                                          AS ProductGroupId,
        LEFT(pg.Code, 50)                                                        AS ProductGroupCode,
        LEFT(pg.Name, 4000)                                                      AS ProductGroupNameRu,
        LEFT(pg.NameEn, 4000)                                                    AS ProductGroupNameEn,

        CAST(m.ETSNGCode AS BIGINT)                                              AS EtsngId,
        LEFT(e.Name, 4000)                                                       AS EtsngNameRu,
        LEFT(e.Name, 4000)                                                       AS EtsngNameEn,

        LEFT(m.GNGCode, 4000)                                                    AS GngCode,

        CAST(m.Unit AS BIGINT)                                                   AS UnitId,
        LEFT(u.Code, 50)                                                         AS UnitCode,
        LEFT(u.Name_ru, 4000)                                                    AS UnitNameRu,
        LEFT(u.Name_en, 4000)                                                    AS UnitNameEn,

        LEFT(m.DPOCode, 4000)                                                    AS DpoCode,
        LEFT(m.MTRType, 4000)                                                    AS MtrType,

        CAST(m.DangerClass AS BIGINT)                                            AS DangerClassId,
        LEFT(dc.Code, 50)                                                        AS DangerClassCode,
        LEFT(dc.Name, 4000)                                                      AS DangerClassNameRu,

        m.CreationDate                                                           AS CreationDate,
        ISNULL(m.LastChangeDate, m.CreationDate)                                 AS LastChangeDate

    FROM dbo.vw_MTR m (NOLOCK)
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK) ON m.TransportName = pg.Id
    LEFT JOIN dbo.vw_ETSNG e (NOLOCK) ON m.ETSNGCode = e.Id
    LEFT JOIN dbo.vw_Unit u (NOLOCK) ON m.Unit = u.Id
    LEFT JOIN dbo.vw_DangerClass dc (NOLOCK) ON m.DangerClass = dc.Id;
GO
