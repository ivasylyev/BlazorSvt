USE [mdm];
GO

/*
    Проекция snapshot Currency из legacy-вью.
    Используется только первичной заливкой (02.Currency_Insert.sql).

    Currency — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2016 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.Currency_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_Currency_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS CurrencyId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        LEFT(c.Code, 50)                                                         AS Code,
        LEFT(c.Name, 4000)                                                       AS Name,
        LEFT(c.Symbolic, 4000)                                                   AS Symbolic,
        CAST(c.Digital AS INT)                                                   AS Digital,
        TRY_CAST(c.Code_NSI AS INT)                                              AS Code_NSI,
        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Currency c
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO
