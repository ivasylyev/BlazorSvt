USE [mdm];
GO

/*
    Проекция snapshot RateSyncClass из dbo.vw_RateSyncClass.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2127 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateSyncClass_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateSyncClass_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateSyncClassId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        LEFT(r.Code, 50)                                                         AS Code,
        LEFT(r.Name, 2000)                                                       AS Name,
        r.CreationDate                                                           AS CreationDate,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_RateSyncClass r (NOLOCK);
GO
