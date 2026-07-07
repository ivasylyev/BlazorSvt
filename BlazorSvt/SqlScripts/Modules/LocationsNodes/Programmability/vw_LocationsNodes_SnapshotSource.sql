USE [mdm];
GO

/*
    Проекция snapshot LocationsNodes из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.LocationsNodes_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.LocationsNodes_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    FK-колонки (LocationTypeId / TypeNodeId / RegionId / CountryId) входят в
    snapshot и попадают в грид, но также используются синхронизацией для
    каскадной инвалидации (изменение справочника -> затронутые узлы).
*/

CREATE OR ALTER VIEW v2.vw_LocationsNodes_SnapshotSource
AS
    SELECT
        CAST(n.Id AS BIGINT) AS LocationsNodesId,
        LEFT(n.Code, 50) AS Code,
        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        LEFT(n.Name_ru, 100) AS NameRu,
        LEFT(n.Name_en, 100) AS NameEn,

        CAST(n.LocationType AS BIGINT) AS LocationTypeId,
        LEFT(tp.Code, 50) AS LocationTypeCode,
        LEFT(tp.Name, 100) AS LocationTypeNameRu,
        LEFT(tp.NameEnRu, 100) AS LocationTypeNameEn,

        CAST(n.TypeNode AS BIGINT) AS TypeNodeId,
        LEFT(tn.Code, 50) AS TypeNodeCode,
        LEFT(tn.NameRu, 100) AS TypeNodeNameRu,
        LEFT(tn.NameEn, 100) AS TypeNodeNameEn,

        CAST(n.Region AS BIGINT) AS RegionId,
        LEFT(r.Code, 50) AS RegionCode,
        LEFT(r.Name_ru, 100) AS RegionNameRu,
        LEFT(r.Name_en, 100) AS RegionNameEn,
        LEFT(n.RegionRU, 100) AS RegionRU,

        CAST(n.Country AS BIGINT) AS CountryId,
        LEFT(c.Name_ru, 100) AS CountryNameRu,
        LEFT(c.Name_en, 100) AS CountryNameEn,

        n.CreationDate AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id
GO
