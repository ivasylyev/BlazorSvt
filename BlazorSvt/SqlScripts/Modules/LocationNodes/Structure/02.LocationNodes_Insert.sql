USE [mdm];
GO

INSERT INTO v2.LocationNodesSnapshot (
     NodeId
    ,IsArchive
    ,NodeTypeId
    ,LocationTypeId
    ,RegionId
    ,CountryId
    ,CreationDate
    ,LastChangeDate

    ,Code
    ,NameEn
    ,NameRu

    ,LocationTypeNameRu
    ,LocationTypeNameEn
    ,NodeTypeNameRu
    ,NodeTypeNameEn
    ,RegionNameRu
    ,RegionNameEn
    ,CountryNameRu
    ,CountryNameEn
)
SELECT
        CAST(n.Id AS BIGINT)                    AS NodeId,
        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        CAST(n.TypeNode AS BIGINT)              AS NodeTypeId,
        CAST(n.LocationType AS BIGINT)          AS LocationTypeId,
        CAST(n.Region AS BIGINT)                AS RegionId,
        CAST(n.Country AS BIGINT)               AS CountryId,
        n.CreationDate                          AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate,

        LEFT(n.Code, 10)                        AS Code,
        LEFT(n.Name_en, 100)                    AS NameEn,
        LEFT(n.Name_ru, 100)                    AS NameRu,

        LEFT(tp.Name, 20)                       AS LocationTypeNameRu,
        LEFT(tp.NameEnRu, 20)                   AS LocationTypeNameEn,
        LEFT(tn.NameRu, 20)                     AS NodeTypeNameRu,
        LEFT(tn.NameEn, 20)                     AS NodeTypeNameEn,
        LEFT(r.Name_ru, 100)                    AS RegionNameRu,
        LEFT(r.Name_en, 100)                    AS RegionNameEn,
        LEFT(c.Name_ru, 100)                    AS CountryNameRu,
        LEFT(c.Name_en, 100)                    AS CountryNameEn

    FROM vw_LocationsNodes n (NOLOCK)
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id

