USE [mdm];
GO

/*
    Проекция snapshot Relevance из dbo.vw_Relevance.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2108 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.Relevance_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_Relevance_SnapshotSource
AS
    SELECT
        LEFT(r.Code, 50)                                                         AS Code,
        LEFT(r.Name, 100)                                                        AS Name,
        CAST(r.Id AS BIGINT)                                                     AS RelevanceId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Relevance r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO
