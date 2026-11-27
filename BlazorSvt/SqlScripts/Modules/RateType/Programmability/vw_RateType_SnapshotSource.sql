USE [mdm];
GO

/*
    Проекция snapshot RateType из legacy-вью.
    Используется только первичной заливкой (02.RateType_Insert.sql).

    RateType — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2048 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateType_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateType_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                            AS RateTypeId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        TRY_CAST(r.Code AS INT)                                         AS Code,
        LEFT(r.Name, 4000)                                              AS Name,
        ISNULL(r.IsUseNomination, 0)                                    AS IsUseNomination,
        ISNULL(r.IsDeflated, 0)                                         AS IsDeflated,
        LEFT(r.Comment, 4000)                                           AS Comment,
        ISNULL(r.IsIntoSyncClass, 0)                                    AS IsIntoSyncClass,
        cu.DisplayName                                                  AS CreationUserName,
        r.CreationDate                                                  AS CreationDate,
        lu.DisplayName                                                  AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                        AS LastChangeDate
    FROM dbo.vw_RateType r
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId)
    WHERE r.Name IS NOT NULL;
GO
