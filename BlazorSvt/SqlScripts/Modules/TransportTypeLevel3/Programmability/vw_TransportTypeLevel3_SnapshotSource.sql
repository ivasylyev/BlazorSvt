USE [mdm];
GO

/*
    Проекция snapshot TransportTypeLevel3 из legacy-вью dbo.vw_TransportType_level_3.
    Используется только первичной заливкой (02.TransportTypeLevel3_Insert.sql).

    TransportTypeLevel3 — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2023 не добавляется.

    TransportKindId (ParentCode_1) и TransportTypeId (ParentCode_2) — Id enum
    TransportKind и TransportType; в гриде показываются через enum, без JOIN.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportTypeLevel3_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_TransportTypeLevel3_SnapshotSource
AS
    SELECT
        CAST(t.Id AS BIGINT)                                                     AS TransportTypeLevel3Id,
        CASE WHEN ISNULL(t.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        t.IsBasic_forCBD                                                         AS IsBasic_forCBD,
        t.Code                                                                   AS Code,
        CAST(t.ParentCode_1 AS INT)                                              AS TransportKindId,
        CAST(t.ParentCode_2 AS INT)                                              AS TransportTypeId,
        LEFT(t.Name, 4000)                                                       AS Name,
        LEFT(t.NameEnRu, 4000)                                                   AS NameEnRu,
        LEFT(t.TypeCode, 4000)                                                   AS TypeCode,
        LEFT(t.SAPCode, 4000)                                                    AS SAPCode,
        LEFT(t.OrionCode, 4000)                                                  AS OrionCode,
        t.CreationDate                                                           AS CreationDate,
        ISNULL(t.LastChangeDate, t.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportType_level_3 t;
GO
