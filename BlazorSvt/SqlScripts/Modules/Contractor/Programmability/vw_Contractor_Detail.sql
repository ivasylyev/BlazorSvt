USE [mdm];
GO

/*
    Карточка и полный Excel Contractor.
    Источник — dbo.vw_LegalType. vw_Contractor устарело.
    Address, Status, LegalForm и LevelOrg не выводятся.
*/

CREATE OR ALTER VIEW v2.vw_Contractor_Detail
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS ContractorId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        c.CreationDate                                                           AS CreationDate,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate,
        LEFT(c.Code, 50)                                                         AS Code,
        LEFT(c.NameSearch, 4000)                                                 AS NameSearch,
        ISNULL(c.Consignee, 0)                                                   AS Consignee,
        ISNULL(c.IsTrader, 0)                                                    AS IsTrader,
        ISNULL(c.IsProcessing, 0)                                                AS IsProcessing,
        ISNULL(c.IsSender, 0)                                                    AS IsSender,
        ISNULL(c.IsDebtor, 0)                                                    AS IsDebtor,
        ISNULL(c.IsCreditor, 0)                                                  AS IsCreditor,
        LEFT(c.INN, 4000)                                                        AS INN,
        LEFT(c.ShortNameEGRUL, 4000)                                             AS ShortNameEGRUL,
        LEFT(c.FullNameEGRUL, 4000)                                              AS FullNameEGRUL,
        LEFT(c.Customs, 4000)                                                    AS Customs,
        LEFT(c.CustomCode, 4000)                                                 AS CustomCode,
        LEFT(c.OrgAffiliation, 4000)                                             AS OrgAffiliation
    FROM dbo.vw_LegalType c;
GO
