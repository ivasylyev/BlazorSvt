USE [mdm];
GO

/*
    Карточка TransportTypeLevel3. Поля совпадают с длинным списком атрибутов MDM
    плюс системные. Источник — та же проекция, что и снимок; Status (только в карточке)
    берётся из legacy-вью по ключу.
*/

CREATE OR ALTER VIEW v2.vw_TransportTypeLevel3_Detail
AS
    SELECT
        s.TransportTypeLevel3Id,
        s.IsArchive,
        s.IsBasic_forCBD,
        s.Code,
        s.TransportKindId           AS TransportKindIdRu,
        s.TransportKindId           AS TransportKindIdEn,
        s.TransportTypeId           AS TransportTypeIdRu,
        s.TransportTypeId           AS TransportTypeIdEn,
        s.Name,
        s.NameEnRu,
        s.TypeCode,
        s.SAPCode,
        s.OrionCode,
        CAST(t.Status AS INT)       AS StatusIdRu,
        CAST(t.Status AS INT)       AS StatusIdEn,
        s.CreationDate,
        s.LastChangeDate
    FROM v2.vw_TransportTypeLevel3_SnapshotSource s
    JOIN dbo.vw_TransportType_level_3 t ON t.Id = s.TransportTypeLevel3Id;
GO
