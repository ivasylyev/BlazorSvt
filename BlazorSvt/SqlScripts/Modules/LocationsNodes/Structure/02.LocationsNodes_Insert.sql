USE [mdm];
GO

INSERT INTO v2.LocationsNodes_Snapshot (
     LocationsNodesId
    ,Code
    ,IsArchive

    ,NameRu
    ,NameEn

    ,LocationTypeId
    ,LocationTypeCode
    ,LocationTypeNameRu
    ,LocationTypeNameEn

    ,TypeNodeId
    ,TypeNodeCode
    ,TypeNodeNameRu
    ,TypeNodeNameEn

    ,RegionId
    ,RegionCode
    ,RegionNameRu
    ,RegionNameEn
    ,RegionRU

    ,CountryId
    ,CountryCode
    ,CountryNameRu
    ,CountryNameEn

    ,CreationDate
    ,LastChangeDate
)
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
        LEFT(c.Code, 50) AS CountryCode,
        LEFT(c.Name_ru, 100) AS CountryNameRu,
        LEFT(c.Name_en, 100) AS CountryNameEn,

        n.CreationDate AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id;
