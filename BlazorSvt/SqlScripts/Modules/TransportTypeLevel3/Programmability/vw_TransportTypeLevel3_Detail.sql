USE [mdm];
GO

/*
    Карточка TransportTypeLevel3. Поля совпадают с длинным списком атрибутов MDM
    плюс системные. Источник — та же проекция, что и снимок.
*/

CREATE OR ALTER VIEW v2.vw_TransportTypeLevel3_Detail
AS
    SELECT
        TransportTypeLevel3Id,
        IsArchive,
        IsBasic_forCBD,
        Code,
        TransportKindId AS TransportKindIdRu,
        TransportKindId AS TransportKindIdEn,
        TransportTypeId AS TransportTypeIdRu,
        TransportTypeId AS TransportTypeIdEn,
        Name,
        NameEnRu,
        TypeCode,
        SAPCode,
        OrionCode,
        StatusId        AS StatusIdRu,
        StatusId        AS StatusIdEn,
        CreationDate,
        LastChangeDate
    FROM v2.vw_TransportTypeLevel3_SnapshotSource;
GO
