USE [mdm];
GO

/*
    Проекция snapshot Contractor из dbo.vw_LegalType.
    vw_Contractor устарело и не используется.

    EntityInfo Contractor = 2054. Примитив LegalType = 2028,
    таблица dbo.PrimitiveEntityData_2028.

    Address, Status, LegalForm и LevelOrg в снимок не входят.
*/

CREATE OR ALTER VIEW v2.vw_Contractor_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS ContractorId,
        LEFT(c.Code, 50)                                                         AS Code,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        LEFT(c.NameSearch, 4000)                                                 AS NameSearch,
        ISNULL(c.Consignee, 0)                                                   AS Consignee,
        ISNULL(c.IsTrader, 0)                                                    AS IsTrader,
        ISNULL(c.IsProcessing, 0)                                                AS IsProcessing,
        ISNULL(c.IsSender, 0)                                                    AS IsSender,
        ISNULL(c.IsDebtor, 0)                                                    AS IsDebtor,
        ISNULL(c.IsCreditor, 0)                                                  AS IsCreditor,
        LEFT(c.INN, 4000)                                                        AS INN,

        c.CreationDate                                                           AS CreationDate,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_LegalType c;
GO
