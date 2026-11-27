USE [mdm];
GO

/*
    Проекция snapshot RateCalcType из dbo.vw_RateCalcType.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2092 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateCalcType_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateCalcType_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateCalcTypeId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        LEFT(r.Code, 50)                                                         AS Code,
        LEFT(r.Name, 50)                                                         AS Name,
        LEFT(r.SearchCode, 50)                                                   AS SearchCode,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_RateCalcType r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO
