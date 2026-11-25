USE [mdm];
GO

/*
    Проекция snapshot Country из dbo.vw_Country.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo Country = 2067. Примитив = 1009,
    таблица dbo.PrimitiveEntityData_1009.

    StatusElement (1010) в каскад не входит. RowVer на 1009 уже есть.
    Своих размножающих соединений нет.
*/

CREATE OR ALTER VIEW v2.vw_Country_SnapshotSource
AS
    SELECT
        TRY_CAST(c.Code AS INT)                                                 AS Code,
        LEFT(c.Name_ru, 4000)                                                    AS NameRu,
        CAST(c.ISOCode AS INT)                                                   AS ISOCode,
        LEFT(c.Name_en, 4000)                                                    AS NameEn,
        LEFT(c.PlanCode, 4000)                                                   AS PlanCode,
        LEFT(c.Market, 4000)                                                     AS Market,
        LEFT(c.ISOCode3, 4000)                                                   AS ISOCode3,
        CAST(c.Status AS BIGINT)                                                 AS StatusId,
        CAST(c.Id AS BIGINT)                                                     AS CountryId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        c.LastChangeDate                                                         AS LastChangeDate
    FROM dbo.vw_Country c (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO
