USE [mdm];
GO

/*
    Проекция snapshot TransportKind из dbo.vw_TransportKind.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2008 не добавляется.

    StatusId — Id enum StatusElement; в каскад не входит.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportKind_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_TransportKind_SnapshotSource
AS
    SELECT
        LEFT(t.Code, 50)                                                         AS Code,
        LEFT(t.Name, 4000)                                                       AS Name,
        LEFT(t.NameEnRu, 4000)                                                   AS NameEnRu,
        CAST(t.Status AS BIGINT)                                                 AS StatusId,
        LEFT(t.Level, 4000)                                                      AS Level,
        LEFT(t.KindCode, 4000)                                                   AS KindCode,
        CAST(t.Id AS BIGINT)                                                     AS TransportKindId,
        CASE WHEN ISNULL(t.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        t.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(t.LastChangeDate, t.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportKind t (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = t.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(t.LastChangeUserId, t.CreationUserId);
GO
