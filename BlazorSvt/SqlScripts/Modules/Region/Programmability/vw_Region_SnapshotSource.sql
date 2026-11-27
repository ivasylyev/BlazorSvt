USE [mdm];
GO

/*
    Проекция snapshot Region из dbo.vw_Region.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo Region = 1003. Примитив = 1008,
    таблица dbo.PrimitiveEntityData_1008.

    Market — не колонка vw_Region: JOIN dbo.vw_Country.Market.
    StatusElement (1010) и Federal_Districts (2120) в каскад не входят.
    RowVer на dbo.PrimitiveEntityData_1008 уже есть.
*/

CREATE OR ALTER VIEW v2.vw_Region_SnapshotSource
AS
    SELECT
        LEFT(r.RegionCode, 4000)                                                 AS RegionCode,
        LEFT(r.Code_NSI, 255)                                                    AS Code_NSI,
        LEFT(r.RegionCodePlanning, 4000)                                         AS RegionCodePlanning,
        LEFT(r.RegionCodeRF, 4000)                                               AS RegionCodeRF,
        LEFT(r.Name_ru, 4000)                                                    AS NameRu,
        LEFT(r.Name_en, 4000)                                                    AS NameEn,
        LEFT(c.Name_ru, 4000)                                                    AS CountryNameRu,
        LEFT(c.Name_en, 4000)                                                    AS CountryNameEn,
        CAST(r.Country AS BIGINT)                                                AS CountryId,
        LEFT(c.Market, 4000)                                                     AS Market,
        CAST(r.Status AS INT)                                                    AS StatusId,
        CAST(r.Federal_Districts AS INT)                                         AS FederalDistrictsId,
        LEFT(fd.Code, 50)                                                        AS FederalDistrictsCode,
        LEFT(fd.FullName, 255)                                                   AS FederalDistrictsNameRu,
        LEFT(fd.FullNameEn, 255)                                                 AS FederalDistrictsNameEn,
        CAST(r.Id AS BIGINT)                                                     AS RegionId,
        LEFT(r.Code, 50)                                                         AS Code,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Region r (NOLOCK)
    LEFT JOIN dbo.vw_Country c (NOLOCK) ON r.Country = c.Id
    LEFT JOIN dbo.vw_Federal_Districts fd (NOLOCK) ON r.Federal_Districts = fd.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO
